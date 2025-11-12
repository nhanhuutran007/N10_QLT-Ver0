using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;

namespace QLKDPhongTro.Presentation.Services
{
    /// <summary>
    /// Service sử dụng YOLOv9 để detect và đọc chỉ số đồng hồ điện/nước
    /// Thay thế OCR bằng Object Detection để chính xác hơn
    /// </summary>
    public class YoloMeterReadingService
    {
        private InferenceSession? _session;
        private readonly string _modelPath;
        private const int InputSize = 640; // YOLO input size
        private readonly float _confidenceThreshold = 0.25f;
        private readonly float _nmsThreshold = 0.45f;

        // Class names cho các chữ số (0-9) và các ký tự đặc biệt
        private readonly string[] _classNames = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };

        public YoloMeterReadingService(string? modelPath = null)
        {
            // Tìm model file trong thư mục models
            if (string.IsNullOrEmpty(modelPath))
            {
                var baseDir = AppDomain.CurrentDomain.BaseDirectory;
                var possiblePaths = new[]
                {
                    Path.Combine(baseDir, "models", "yolov9n_meter_reading.onnx"),
                    Path.Combine(baseDir, "yolov9n_meter_reading.onnx"),
                    Path.Combine(Directory.GetCurrentDirectory(), "models", "yolov9n_meter_reading.onnx")
                };

                _modelPath = possiblePaths.FirstOrDefault(File.Exists) ?? possiblePaths[0];
            }
            else
            {
                _modelPath = modelPath;
            }

            // Khởi tạo ONNX Runtime session nếu có model
            if (File.Exists(_modelPath))
            {
                try
                {
                    var options = new SessionOptions();
                    // Sử dụng CPU (có thể thay bằng GPU nếu có)
                    // options.AppendExecutionProvider_CUDA(0);
                    _session = new InferenceSession(_modelPath, options);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Không thể load YOLOv9 model: {ex.Message}", ex);
                }
            }
        }

        /// <summary>
        /// Phân tích ảnh và trích xuất chỉ số từ đồng hồ
        /// </summary>
        public async Task<MeterReadingResult> AnalyzeImageAsync(string imagePath, MeterType type)
        {
            if (_session == null)
            {
                return new MeterReadingResult
                {
                    Type = type,
                    Value = 0,
                    Confidence = 0f,
                    ErrorMessage = $"YOLOv9 model không tìm thấy tại: {_modelPath}. Vui lòng tải model và đặt vào thư mục models/",
                    RawText = "Model không khả dụng"
                };
            }

            try
            {
                return await Task.Run(() =>
                {
                    // 1. Load và preprocess ảnh
                    using (var image = Image.FromFile(imagePath))
                    {
                        var inputTensor = PreprocessImage(image);

                        // 2. Run inference
                        var inputs = new List<NamedOnnxValue>
                        {
                            NamedOnnxValue.CreateFromTensor("images", inputTensor)
                        };

                        using (var results = _session.Run(inputs))
                        {
                            var output = results.First().AsTensor<float>();

                            // 3. Post-process: NMS và parse kết quả
                            var detections = PostProcess(output, image.Width, image.Height);

                            // 4. Sắp xếp và ghép các chữ số thành số hoàn chỉnh
                            var meterValue = ParseMeterReading(detections, type);

                            return meterValue;
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                return new MeterReadingResult
                {
                    Type = type,
                    Value = 0,
                    Confidence = 0f,
                    ErrorMessage = $"Lỗi khi xử lý ảnh với YOLOv9: {ex.Message}",
                    RawText = ex.Message
                };
            }
        }

        /// <summary>
        /// Preprocess ảnh để phù hợp với YOLO input
        /// </summary>
        private DenseTensor<float> PreprocessImage(Image image)
        {
            // Resize ảnh về 640x640 và normalize
            using (var resized = new Bitmap(InputSize, InputSize))
            using (var graphics = Graphics.FromImage(resized))
            {
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.DrawImage(image, 0, 0, InputSize, InputSize);
                
                var tensor = new DenseTensor<float>(new[] { 1, 3, InputSize, InputSize });
                
                // Convert RGB và normalize về [0, 1]
                for (int y = 0; y < InputSize; y++)
                {
                    for (int x = 0; x < InputSize; x++)
                    {
                        var pixel = resized.GetPixel(x, y);
                        tensor[0, 0, y, x] = pixel.R / 255.0f; // R
                        tensor[0, 1, y, x] = pixel.G / 255.0f; // G
                        tensor[0, 2, y, x] = pixel.B / 255.0f; // B
                    }
                }
                
                return tensor;
            }
        }

        /// <summary>
        /// Post-process output từ YOLO: NMS và filter
        /// </summary>
        private List<Detection> PostProcess(Tensor<float> output, int originalWidth, int originalHeight)
        {
            var detections = new List<Detection>();
            
            // YOLO output format: [batch, num_detections, 6] (x, y, w, h, confidence, class)
            // Hoặc [batch, num_detections, 85] nếu có nhiều classes
            
            var shape = output.Dimensions.ToArray();
            var numDetections = shape[1];
            var numValues = shape[2];

            for (int i = 0; i < numDetections; i++)
            {
                var confidence = output[0, i, 4];
                
                if (confidence < _confidenceThreshold)
                    continue;

                // Tìm class có confidence cao nhất
                float maxClassScore = 0;
                int bestClass = 0;
                
                for (int c = 0; c < Math.Min(_classNames.Length, numValues - 5); c++)
                {
                    var classScore = output[0, i, 5 + c];
                    if (classScore > maxClassScore)
                    {
                        maxClassScore = classScore;
                        bestClass = c;
                    }
                }

                var finalConfidence = confidence * maxClassScore;
                if (finalConfidence < _confidenceThreshold)
                    continue;

                // Parse bounding box (normalized coordinates)
                var centerX = output[0, i, 0];
                var centerY = output[0, i, 1];
                var width = output[0, i, 2];
                var height = output[0, i, 3];

                // Convert về pixel coordinates
                var x1 = (centerX - width / 2) * originalWidth;
                var y1 = (centerY - height / 2) * originalHeight;
                var x2 = (centerX + width / 2) * originalWidth;
                var y2 = (centerY + height / 2) * originalHeight;

                detections.Add(new Detection
                {
                    X = (float)x1,
                    Y = (float)y1,
                    Width = (float)(x2 - x1),
                    Height = (float)(y2 - y1),
                    Confidence = finalConfidence,
                    ClassId = bestClass,
                    ClassName = bestClass < _classNames.Length ? _classNames[bestClass] : "?"
                });
            }

            // Apply Non-Maximum Suppression (NMS)
            return ApplyNMS(detections);
        }

        /// <summary>
        /// Non-Maximum Suppression để loại bỏ duplicate detections
        /// </summary>
        private List<Detection> ApplyNMS(List<Detection> detections)
        {
            if (detections.Count == 0)
                return detections;

            // Sắp xếp theo confidence
            detections = detections.OrderByDescending(d => d.Confidence).ToList();
            
            var result = new List<Detection>();
            var suppressed = new bool[detections.Count];

            for (int i = 0; i < detections.Count; i++)
            {
                if (suppressed[i])
                    continue;

                result.Add(detections[i]);

                // Suppress các detections có IoU cao với detection hiện tại
                for (int j = i + 1; j < detections.Count; j++)
                {
                    if (suppressed[j])
                        continue;

                    var iou = CalculateIoU(detections[i], detections[j]);
                    if (iou > _nmsThreshold)
                    {
                        suppressed[j] = true;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Tính Intersection over Union (IoU)
        /// </summary>
        private float CalculateIoU(Detection box1, Detection box2)
        {
            var x1 = Math.Max(box1.X, box2.X);
            var y1 = Math.Max(box1.Y, box2.Y);
            var x2 = Math.Min(box1.X + box1.Width, box2.X + box2.Width);
            var y2 = Math.Min(box1.Y + box1.Height, box2.Y + box2.Height);

            if (x2 <= x1 || y2 <= y1)
                return 0;

            var intersection = (x2 - x1) * (y2 - y1);
            var area1 = box1.Width * box1.Height;
            var area2 = box2.Width * box2.Height;
            var union = area1 + area2 - intersection;

            return union > 0 ? intersection / union : 0;
        }

        /// <summary>
        /// Parse các detections thành số đọc được từ đồng hồ
        /// </summary>
        private MeterReadingResult ParseMeterReading(List<Detection> detections, MeterType type)
        {
            if (detections.Count == 0)
            {
                return new MeterReadingResult
                {
                    Type = type,
                    Value = 0,
                    Confidence = 0f,
                    ErrorMessage = "Không tìm thấy chữ số nào trong ảnh",
                    RawText = "No detections"
                };
            }

            // Sắp xếp các chữ số từ trái sang phải (theo tọa độ X)
            var sortedDigits = detections
                .Where(d => int.TryParse(d.ClassName, out _))
                .OrderBy(d => d.X)
                .ToList();

            if (sortedDigits.Count == 0)
            {
                return new MeterReadingResult
                {
                    Type = type,
                    Value = 0,
                    Confidence = 0f,
                    ErrorMessage = "Không tìm thấy chữ số hợp lệ",
                    RawText = string.Join(", ", detections.Select(d => d.ClassName))
                };
            }

            // Ghép các chữ số thành số hoàn chỉnh
            var numberString = string.Join("", sortedDigits.Select(d => d.ClassName));
            var avgConfidence = sortedDigits.Average(d => d.Confidence);

            if (decimal.TryParse(numberString, out decimal value))
            {
                return new MeterReadingResult
                {
                    Type = type,
                    Value = value,
                    Confidence = avgConfidence,
                    RawText = $"Detected digits: {numberString}",
                    ErrorMessage = null
                };
            }

            return new MeterReadingResult
            {
                Type = type,
                Value = 0,
                Confidence = avgConfidence,
                ErrorMessage = $"Không thể parse số từ: {numberString}",
                RawText = numberString
            };
        }

        /// <summary>
        /// Phân tích nhiều ảnh
        /// </summary>
        public async Task<MeterReadingResult[]> AnalyzeImagesAsync(string[] imagePaths, MeterType type)
        {
            var results = new MeterReadingResult[imagePaths.Length];
            
            for (int i = 0; i < imagePaths.Length; i++)
            {
                try
                {
                    results[i] = await AnalyzeImageAsync(imagePaths[i], type);
                }
                catch (Exception ex)
                {
                    results[i] = new MeterReadingResult
                    {
                        Type = type,
                        Value = 0,
                        Confidence = 0f,
                        ErrorMessage = $"Lỗi khi xử lý ảnh {Path.GetFileName(imagePaths[i])}: {ex.Message}",
                        RawText = ex.Message
                    };
                }
            }

            return results;
        }

        /// <summary>
        /// Kiểm tra xem model có sẵn không
        /// </summary>
        public bool IsModelAvailable()
        {
            return _session != null && File.Exists(_modelPath);
        }

        /// <summary>
        /// Lấy đường dẫn model
        /// </summary>
        public string GetModelPath()
        {
            return _modelPath;
        }
    }

    /// <summary>
    /// Detection result từ YOLO
    /// </summary>
    internal class Detection
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public float Confidence { get; set; }
        public int ClassId { get; set; }
        public string ClassName { get; set; } = string.Empty;
    }
}

