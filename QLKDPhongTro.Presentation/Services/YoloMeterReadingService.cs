using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
// Ensure this namespace is accessible or classes are defined in a separate file
using QLKDPhongTro.Presentation.Services;

namespace QLKDPhongTro.Presentation.Services
{
    /// <summary>
    /// Service sử dụng YOLOv9 để detect và đọc chỉ số đồng hồ điện/nước.
    /// Đã tối ưu hóa bộ nhớ (LockBits + Unsafe) để tránh OutOfMemoryException.
    /// </summary>
    public class YoloMeterReadingService : IDisposable
    {
        private InferenceSession? _session;
        private readonly string _modelPath;
        private const int InputSize = 640; // Kích thước đầu vào chuẩn của YOLO
        private readonly float _confidenceThreshold = 0.25f;
        private readonly float _nmsThreshold = 0.45f;

        // Class names cho các chữ số (0-9)
        private readonly string[] _classNames = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };

        public YoloMeterReadingService(string? modelPath = null)
        {
            // 1. Tìm đường dẫn model
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

            // 2. Khởi tạo Session
            InitializeSession();
        }

        private void InitializeSession()
        {
            if (_session != null) return;

            if (File.Exists(_modelPath))
            {
                try
                {
                    var options = new SessionOptions();
                    // Tối ưu hóa graph để chạy nhanh hơn và ít tốn RAM hơn
                    options.GraphOptimizationLevel = GraphOptimizationLevel.ORT_ENABLE_ALL;
                    _session = new InferenceSession(_modelPath, options);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Không thể load YOLOv9 model: {ex.Message}", ex);
                }
            }
        }

        /// <summary>
        /// Phân tích ảnh và trích xuất chỉ số
        /// </summary>
        public async Task<MeterReadingResult> AnalyzeImageAsync(string imagePath, MeterType type)
        {
            if (_session == null) InitializeSession();

            if (_session == null)
            {
                return new MeterReadingResult
                {
                    Type = type,
                    Value = 0,
                    Confidence = 0f,
                    ErrorMessage = $"Model không tìm thấy tại: {_modelPath}",
                    RawText = "Model missing"
                };
            }

            return await Task.Run(() =>
            {
                try
                {
                    // BƯỚC 1: Load và Preprocess ảnh (Dùng LockBits để tối ưu bộ nhớ)
                    var inputTensor = LoadAndPreprocessImage(imagePath);

                    // BƯỚC 2: Chạy Inference
                    var inputs = new List<NamedOnnxValue>
                    {
                        NamedOnnxValue.CreateFromTensor("images", inputTensor)
                    };

                    using var results = _session.Run(inputs);
                    var output = results.First().AsTensor<float>();

                    // BƯỚC 3: Xử lý kết quả (Post-process & NMS)
                    // Dùng InputSize vì ta đã resize ảnh về kích thước này
                    var detections = PostProcess(output, InputSize, InputSize);

                    // BƯỚC 4: Đọc số
                    var result = ParseMeterReading(detections, type);

                    // BƯỚC 5: Bổ sung Visualize và Raw Detections
                    result.Detections = detections; // Lưu lại danh sách bounding box
                    result.VisualizedImageBase64 = VisualizeDetections(imagePath, detections);

                    return result;
                }
                catch (Exception ex)
                {
                    // Nếu gặp lỗi bộ nhớ, ép GC chạy ngay lập tức
                    if (ex is OutOfMemoryException)
                    {
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                    }

                    return new MeterReadingResult
                    {
                        Type = type,
                        Value = 0,
                        Confidence = 0f,
                        ErrorMessage = $"Lỗi xử lý ảnh: {ex.Message}",
                        RawText = ex.Message
                    };
                }
            });
        }

        /// <summary>
        /// Vẽ bounding box lên ảnh gốc và trả về chuỗi Base64
        /// </summary>
        public string? VisualizeDetections(string imagePath, List<Detection> detections)
        {
            if (!File.Exists(imagePath)) return null;

            try
            {
                using var originalImage = Image.FromFile(imagePath);
                int originalWidth = originalImage.Width;
                int originalHeight = originalImage.Height;

                using var bitmap = new Bitmap(originalWidth, originalHeight);
                using (var graphics = Graphics.FromImage(bitmap))
                {
                    graphics.DrawImage(originalImage, 0, 0, originalWidth, originalHeight);
                    graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                    // Tính tỉ lệ scale từ 640x640 về kích thước thật
                    float scaleX = (float)originalWidth / InputSize;
                    float scaleY = (float)originalHeight / InputSize;

                    using var pen = new Pen(Color.Red, 3);
                    using var font = new Font("Arial", 16, FontStyle.Bold);
                    using var brush = new SolidBrush(Color.Red);
                    using var bgBrush = new SolidBrush(Color.FromArgb(150, Color.Black)); // Nền đen bán trong suốt

                    foreach (var det in detections)
                    {
                        // Scale tọa độ
                        float x = det.X * scaleX;
                        float y = det.Y * scaleY;
                        float w = det.Width * scaleX;
                        float h = det.Height * scaleY;

                        // Vẽ khung chữ nhật
                        graphics.DrawRectangle(pen, x, y, w, h);

                        // Vẽ nhãn
                        string label = $"{det.ClassName} ({det.Confidence:0.00})";
                        var size = graphics.MeasureString(label, font);

                        // Vẽ nền cho nhãn để dễ đọc hơn
                        float labelY = y - size.Height;
                        if (labelY < 0) labelY = y; // Nếu sát mép trên thì vẽ vào trong

                        graphics.FillRectangle(bgBrush, x, labelY, size.Width, size.Height);
                        graphics.DrawString(label, font, brush, x, labelY);
                    }
                }

                // Chuyển thành Base64
                using var ms = new MemoryStream();
                bitmap.Save(ms, ImageFormat.Jpeg);
                return Convert.ToBase64String(ms.ToArray());
            }
            catch
            {
                return null; // Trả về null nếu lỗi vẽ (không làm crash app chính)
            }
        }

        private DenseTensor<float> LoadAndPreprocessImage(string imagePath)
        {
            // Load ảnh gốc từ file
            using var originalImage = Image.FromFile(imagePath);

            // Tạo bitmap đích với kích thước chuẩn YOLO (640x640) và định dạng pixel cố định (24bppRgb)
            using var resized = new Bitmap(InputSize, InputSize, PixelFormat.Format24bppRgb);

            using (var graphics = Graphics.FromImage(resized))
            {
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

                // Vẽ ảnh gốc lên bitmap đích (Resize)
                graphics.DrawImage(originalImage, 0, 0, InputSize, InputSize);
            }

            var tensor = new DenseTensor<float>(new[] { 1, 3, InputSize, InputSize });

            // Kỹ thuật LockBits: Khóa vùng nhớ ảnh để truy cập trực tiếp
            BitmapData bmpData = resized.LockBits(
                new Rectangle(0, 0, resized.Width, resized.Height),
                ImageLockMode.ReadOnly,
                PixelFormat.Format24bppRgb);

            try
            {
                unsafe
                {
                    byte* ptr = (byte*)bmpData.Scan0;
                    int stride = bmpData.Stride;

                    Parallel.For(0, InputSize, y =>
                    {
                        byte* row = ptr + (y * stride);
                        for (int x = 0; x < InputSize; x++)
                        {
                            // PixelFormat.Format24bppRgb lưu theo thứ tự B-G-R
                            int i = x * 3;
                            float b = row[i] / 255.0f;
                            float g = row[i + 1] / 255.0f;
                            float r = row[i + 2] / 255.0f;

                            tensor[0, 0, y, x] = r;
                            tensor[0, 1, y, x] = g;
                            tensor[0, 2, y, x] = b;
                        }
                    });
                }
            }
            finally
            {
                resized.UnlockBits(bmpData);
            }

            return tensor;
        }

        private List<Detection> PostProcess(Tensor<float> output, int imgWidth, int imgHeight)
        {
            var detections = new List<Detection>();
            var shape = output.Dimensions.ToArray();
            var numDetections = shape[1];
            var numValues = shape[2];

            for (int i = 0; i < numDetections; i++)
            {
                float confidence = output[0, i, 4];
                if (confidence < _confidenceThreshold) continue;

                float maxClassScore = 0;
                int bestClass = 0;

                for (int c = 0; c < Math.Min(_classNames.Length, numValues - 5); c++)
                {
                    var score = output[0, i, 5 + c];
                    if (score > maxClassScore)
                    {
                        maxClassScore = score;
                        bestClass = c;
                    }
                }

                float finalConf = confidence * maxClassScore;
                if (finalConf < _confidenceThreshold) continue;

                float x = output[0, i, 0];
                float y = output[0, i, 1];
                float w = output[0, i, 2];
                float h = output[0, i, 3];

                float x1 = x - w / 2;
                float y1 = y - h / 2;

                detections.Add(new Detection
                {
                    X = x1,
                    Y = y1,
                    Width = w,
                    Height = h,
                    Confidence = finalConf,
                    ClassId = bestClass,
                    ClassName = bestClass < _classNames.Length ? _classNames[bestClass] : "?"
                });
            }

            return ApplyNMS(detections);
        }

        private List<Detection> ApplyNMS(List<Detection> detections)
        {
            if (detections.Count == 0) return detections;

            var sorted = detections.OrderByDescending(d => d.Confidence).ToList();
            var result = new List<Detection>();
            var suppressed = new bool[sorted.Count];

            for (int i = 0; i < sorted.Count; i++)
            {
                if (suppressed[i]) continue;
                result.Add(sorted[i]);

                for (int j = i + 1; j < sorted.Count; j++)
                {
                    if (suppressed[j]) continue;
                    if (CalculateIoU(sorted[i], sorted[j]) > _nmsThreshold)
                    {
                        suppressed[j] = true;
                    }
                }
            }
            return result;
        }

        private float CalculateIoU(Detection box1, Detection box2)
        {
            float x1 = Math.Max(box1.X, box2.X);
            float y1 = Math.Max(box1.Y, box2.Y);
            float x2 = Math.Min(box1.X + box1.Width, box2.X + box2.Width);
            float y2 = Math.Min(box1.Y + box1.Height, box2.Y + box2.Height);

            if (x2 <= x1 || y2 <= y1) return 0;

            float intersection = (x2 - x1) * (y2 - y1);
            float area1 = box1.Width * box1.Height;
            float area2 = box2.Width * box2.Height;
            float union = area1 + area2 - intersection;

            return union > 0 ? intersection / union : 0;
        }

        private MeterReadingResult ParseMeterReading(List<Detection> detections, MeterType type)
        {
            if (detections.Count == 0)
            {
                return new MeterReadingResult
                {
                    Type = type,
                    Value = 0,
                    Confidence = 0,
                    ErrorMessage = "Không tìm thấy số",
                    RawText = "No data"
                };
            }

            // Sắp xếp từ trái qua phải
            var sortedDigits = detections
                .Where(d => int.TryParse(d.ClassName, out _))
                .OrderBy(d => d.X)
                .ToList();

            if (sortedDigits.Count == 0)
                return new MeterReadingResult { Type = type, ErrorMessage = "Không tìm thấy số hợp lệ" };

            string numberStr = string.Join("", sortedDigits.Select(d => d.ClassName));
            float avgConf = sortedDigits.Average(d => d.Confidence);

            if (decimal.TryParse(numberStr, out decimal val))
            {
                return new MeterReadingResult
                {
                    Type = type,
                    Value = val,
                    Confidence = avgConf,
                    RawText = numberStr, // Giữ nguyên chuỗi thô để xử lý bên ngoài (ví dụ cắt số 0)
                    ErrorMessage = null
                };
            }

            return new MeterReadingResult
            {
                Type = type,
                Value = 0,
                ErrorMessage = $"Lỗi parse số: {numberStr}",
                RawText = numberStr
            };
        }

        public async Task<MeterReadingResult[]> AnalyzeImagesAsync(string[] imagePaths, MeterType type)
        {
            var results = new MeterReadingResult[imagePaths.Length];
            for (int i = 0; i < imagePaths.Length; i++)
            {
                results[i] = await AnalyzeImageAsync(imagePaths[i], type);

                if (i > 0 && i % 5 == 0)
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
            }
            return results;
        }

        public bool IsModelAvailable() => _session != null && File.Exists(_modelPath);
        public string GetModelPath() => _modelPath;

        public void Dispose()
        {
            _session?.Dispose();
            _session = null;
            GC.SuppressFinalize(this);
        }
        // Trong class MeterReadingResult (thêm vào cuối class)
        public class MeterReadingResult
        {
            public MeterType Type { get; set; }
            public decimal Value { get; set; }
            public float Confidence { get; set; }
            public string RawText { get; set; } = string.Empty;
            public string? ErrorMessage { get; set; }
            public bool IsValid => Confidence > 0.3f && Value > 0;

            // Các property mới cho xử lý sự cố
            public List<Detection>? Detections { get; set; }
            public string? VisualizedImageBase64 { get; set; }
        }

        // Đảm bảo class Detection tồn tại
        public class Detection
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

    // Removed Duplicate class definitions. 
    // Make sure these are defined in a separate DTO file (e.g., MeterReadingResult.cs) 
    // or in OcrService.cs IF that is the primary place they should live.
}