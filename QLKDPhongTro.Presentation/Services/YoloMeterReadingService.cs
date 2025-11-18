using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using QLKDPhongTro.BusinessLayer.DTOs;
// Thư viện ImageSharp
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Drawing;
using SixLabors.Fonts;
using System.Diagnostics;
using System.Globalization; // Cho CultureInfo
using QLKDPhongTro.BusinessLayer.DTOs;

namespace QLKDPhongTro.Presentation.Services
{
    /// <summary>
    /// Service YOLOv9 (Đã sửa logic theo data.yaml)
    /// </summary>
    public class YoloMeterReadingService : IDisposable
    {
        private InferenceSession? _session;
        private readonly string _modelPath;
        private const int InputSize = 640;

        // Ngưỡng từ Python
        private readonly float _confidenceThreshold = 0.45f;
        private readonly float _nmsThreshold = 0.85f;

        // === FIX 1: Sửa lại mảng _classNames để khớp data.yaml (12 class) ===
        private readonly string[] _classNames = {
            ".", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "Kwh"
        };

        // 4 (xywh) + 12 (classes) = 16
        private readonly int _numValuesPerBox = 16;
        private readonly int _numClasses = 12;

        public YoloMeterReadingService(string? modelPath = null)
        {
            if (string.IsNullOrEmpty(modelPath))
            {
                var baseDir = AppDomain.CurrentDomain.BaseDirectory;
                var possiblePaths = new[]
                {
                    @"C:\Users\User\Desktop\CNPM\N10_QLT-Ver0\QLKDPhongTro.Presentation\bin\Debug\net8.0-windows\models\yolov9n_meter_reading.onnx",
                    System.IO.Path.Combine(baseDir, "models", "yolov9n_meter_reading.onnx"),
                    System.IO.Path.Combine(baseDir, "yolov9n_meter_reading.onnx"),
                };
                _modelPath = possiblePaths.FirstOrDefault(System.IO.File.Exists) ?? possiblePaths[0];
            }
            else { _modelPath = modelPath; }

            InitializeSession();
        }

        private void InitializeSession()
        {
            if (_session != null) return;
            if (System.IO.File.Exists(_modelPath))
            {
                try
                {
                    var options = new SessionOptions { GraphOptimizationLevel = GraphOptimizationLevel.ORT_ENABLE_ALL };
                    _session = new InferenceSession(_modelPath, options);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Không thể load YOLOv9 model tại {_modelPath}: {ex.Message}", ex);
                }
            }
            else
            {
                throw new FileNotFoundException($"Model không tìm thấy tại {_modelPath}");
            }
        }

        // Helper class
        private class PreprocessingResult
        {
            public DenseTensor<float> InputTensor { get; set; }
            public (float W, float H) Ratio { get; set; }
            public (float PadW, float PadH) Padding { get; set; }
        }

        public async Task<MeterReadingResult> AnalyzeImageAsync(string imagePath, MeterType type)
        {
            if (_session == null) InitializeSession();

            return await Task.Run(() =>
            {
                try
                {
                    var prepResult = LoadAndPreprocessImage_Letterbox(imagePath);
                    if (prepResult == null)
                        return new MeterReadingResult { ErrorMessage = "Lỗi khi tiền xử lý ảnh." };

                    var inputs = new List<NamedOnnxValue> { NamedOnnxValue.CreateFromTensor("images", prepResult.InputTensor) };
                    using var results = _session.Run(inputs);
                    var output = results.First().AsTensor<float>();

                    var detections = PostProcess(output, prepResult.Ratio, prepResult.Padding);

                    var result = ParseMeterReading(detections, type);
                    result.Detections = detections;
                    result.VisualizedImageBase64 = VisualizeDetections(imagePath, detections);
                    return result;
                }
                catch (Exception ex)
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    return new MeterReadingResult { ErrorMessage = $"Lỗi xử lý ảnh: {ex.Message}" };
                }
            });
        }

        public string? VisualizeDetections(string imagePath, List<Detection> detections)
        {
            if (!System.IO.File.Exists(imagePath) || detections == null) return null;
            try
            {
                using var image = Image.Load(imagePath);

                FontCollection collection = new();
                collection.AddSystemFonts();
                FontFamily family = collection.Families.FirstOrDefault(f => f.Name.Equals("Arial", StringComparison.OrdinalIgnoreCase));
                if (family == null) { family = collection.Families.First(); }
                Font font = family.CreateFont(16, FontStyle.Bold);

                var pen = Pens.Solid(SixLabors.ImageSharp.Color.Red, 3);
                var textBrush = Brushes.Solid(SixLabors.ImageSharp.Color.Red);
                var bgBrush = Brushes.Solid(SixLabors.ImageSharp.Color.Black.WithAlpha(0.5f));

                image.Mutate(ctx =>
                {
                    foreach (var det in detections)
                    {
                        float x1 = Math.Max(0, det.X);
                        float y1 = Math.Max(0, det.Y);
                        float w = Math.Min(image.Width - x1, det.Width);
                        float h = Math.Min(image.Height - y1, det.Height);
                        var rect = new RectangleF(x1, y1, w, h);
                        ctx.Draw(pen, rect);

                        // Hiển thị tên class từ mảng 12 class
                        string label = $"{det.ClassName} ({det.Confidence:0.00})";
                        var textOptions = new RichTextOptions(font) { Origin = new PointF(x1 + 3, y1) };
                        FontRectangle size = TextMeasurer.MeasureBounds(label, textOptions);
                        float labelY = y1 - size.Height - 3;
                        if (labelY < 0) labelY = y1;
                        var textBgRect = new RectangleF(x1, labelY, size.Width + 6, size.Height + 6);
                        var textPoint = new PointF(x1 + 3, labelY + 3);
                        ctx.Fill(bgBrush, textBgRect);
                        ctx.DrawText(label, font, textBrush, textPoint);
                    }
                });
                using var ms = new MemoryStream();
                image.SaveAsJpeg(ms);
                return Convert.ToBase64String(ms.ToArray());
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[DEBUG] LỖI VisualizeDetections: {ex.Message}");
                return null;
            }
        }

        private PreprocessingResult? LoadAndPreprocessImage_Letterbox(string imagePath)
        {
            try
            {
                using var image = Image.Load<Rgb24>(imagePath);
                int originalWidth = image.Width;
                int originalHeight = image.Height;

                float r = Math.Min((float)InputSize / originalHeight, (float)InputSize / originalWidth);
                (float r_w, float r_h) = (r, r);

                int newUnpadWidth = (int)Math.Round(originalWidth * r);
                int newUnpadHeight = (int)Math.Round(originalHeight * r);

                float dw = (InputSize - newUnpadWidth) / 2.0f;
                float dh = (InputSize - newUnpadHeight) / 2.0f;

                var options = new ResizeOptions
                {
                    Size = new Size(InputSize, InputSize),
                    Mode = ResizeMode.Pad,
                    PadColor = new Rgb24(114, 114, 114)
                };
                image.Mutate(x => x.Resize(options));

                var tensor = new DenseTensor<float>(new[] { 1, 3, InputSize, InputSize });
                image.ProcessPixelRows(accessor =>
                {
                    for (int y = 0; y < accessor.Height; y++)
                    {
                        var pixelRow = accessor.GetRowSpan(y);
                        for (int x = 0; x < accessor.Width; x++)
                        {
                            var pixel = pixelRow[x];
                            // Dùng RGB (giống Python cvtColor)
                            tensor[0, 0, y, x] = pixel.R / 255.0f;
                            tensor[0, 1, y, x] = pixel.G / 255.0f;
                            tensor[0, 2, y, x] = pixel.B / 255.0f;
                        }
                    }
                });

                return new PreprocessingResult
                {
                    InputTensor = tensor,
                    Ratio = (r_w, r_h),
                    Padding = (dw, dh)
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[DEBUG] LỖI Preprocessing: {ex.Message}");
                return null;
            }
        }

        // Logic PostProcess (YOLOv8) này đã đúng
        private List<Detection> PostProcess(Tensor<float> output, (float W, float H) ratio, (float PadW, float PadH) padding)
        {
            var detections = new List<Detection>();

            var shape = output.Dimensions.ToArray();
            if (shape[1] != _numValuesPerBox || shape[0] != 1)
            {
                Debug.WriteLine($"[DEBUG] LỖI SHAPE: Model trả về shape [{shape[0]}, {shape[1]}, {shape[2]}] nhưng code mong đợi [1, 16, 8400].");
                return detections;
            }

            int numDetections = shape[2]; // 8400

            var boxes = new List<float[]>();
            var scores = new List<float>();
            var classIds = new List<int>();

            for (int i = 0; i < numDetections; i++)
            {
                var data = Enumerable.Range(0, _numValuesPerBox).Select(index => output[0, index, i]).ToArray();

                // Logic YOLOv8 (scores = pred[:, 4:])
                var classScores = data.Skip(4).ToArray();
                float finalConf = classScores.Max();
                int bestClass = classScores.AsSpan().IndexOf(finalConf);

                if (finalConf < _confidenceThreshold) continue;

                float x_center = data[0];
                float y_center = data[1];
                float w = data[2];
                float h = data[3];

                float x1 = x_center - w / 2;
                float y1 = y_center - h / 2;
                float x2 = x_center + w / 2;
                float y2 = y_center + h / 2;

                // Scale về ảnh GỐC (giống Python)
                x1 = (x1 - padding.PadW) / ratio.W;
                y1 = (y1 - padding.PadH) / ratio.H;
                x2 = (x2 - padding.PadW) / ratio.W;
                y2 = (y2 - padding.PadH) / ratio.H;

                boxes.Add(new float[] { x1, y1, x2, y2 });
                scores.Add(finalConf);
                classIds.Add(bestClass);
            }

            if (boxes.Count == 0)
            {
                Debug.WriteLine("[DEBUG] Không tìm thấy box nào sau khi lọc bằng CONF_TH.");
                return detections;
            }

            var indices = NMS(boxes, scores, _nmsThreshold);

            foreach (var idx in indices)
            {
                var box = boxes[idx];
                var score = scores[idx];
                var classId = classIds[idx];

                detections.Add(new Detection
                {
                    X = box[0], // x1
                    Y = box[1], // y1
                    Width = box[2] - box[0], // w
                    Height = box[3] - box[1], // h
                    Confidence = score,
                    ClassId = classId,
                    // Gán class name từ mảng 12 class
                    ClassName = classId < _classNames.Length ? _classNames[classId] : "UKN"
                });
            }

            return detections;
        }

        private List<int> NMS(List<float[]> boxes, List<float> scores, float iouThreshold)
        {
            var indices = Enumerable.Range(0, boxes.Count).ToList();
            indices.Sort((a, b) => scores[b].CompareTo(scores[a]));
            var keep = new List<int>();
            var suppressed = new bool[boxes.Count];
            for (int i_idx = 0; i_idx < indices.Count; i_idx++)
            {
                int i = indices[i_idx];
                if (suppressed[i]) continue;
                keep.Add(i);
                for (int j_idx = i_idx + 1; j_idx < indices.Count; j_idx++)
                {
                    int j = indices[j_idx];
                    if (suppressed[j]) continue;
                    float iou = CalculateIoU_xyxy(boxes[i], boxes[j]);
                    if (iou > iouThreshold)
                    {
                        suppressed[j] = true;
                    }
                }
            }
            return keep;
        }

        private float CalculateIoU_xyxy(float[] box1, float[] box2)
        {
            float xA = Math.Max(box1[0], box2[0]);
            float yA = Math.Max(box1[1], box2[1]);
            float xB = Math.Min(box1[2], box2[2]);
            float yB = Math.Min(box1[3], box2[3]);
            float interArea = Math.Max(0, xB - xA) * Math.Max(0, yB - yA);
            float box1Area = (box1[2] - box1[0]) * (box1[3] - box1[1]);
            float box2Area = (box2[2] - box2[0]) * (box2[3] - box2[1]);
            float unionArea = box1Area + box2Area - interArea;
            return unionArea > 0 ? interArea / unionArea : 0;
        }


        // === FIX 2: Sửa logic Parse để lọc đúng (Height + X-Gap) ===
        // === FIX: Sửa logic Parse để áp dụng QUY TẮC 6 KÝ TỰ ===
        private MeterReadingResult ParseMeterReading(List<Detection> detections, MeterType type)
        {
            if (detections == null || detections.Count == 0)
            {
                return new MeterReadingResult { ErrorMessage = "Không tìm thấy đối tượng nào" };
            }

            // 1. Lọc TẤT CẢ các chữ số (0-9). 
            // (Chúng ta bỏ qua class "." vì quy tắc của bạn là "ký tự cuối cùng")
            var allDigits = detections
                .Where(d => int.TryParse(d.ClassName, out _))
                .ToList();

            if (allDigits.Count == 0)
            {
                return new MeterReadingResult { ErrorMessage = "Không tìm thấy CHỮ SỐ nào" };
            }

            // 2. === BỘ LỌC LỚP 1: LỌC THEO CHIỀU CAO ===
            float maxHeight = allDigits.Max(d => d.Height);
            var largeDigits = allDigits
                .Where(d => d.Height >= (maxHeight * 0.7f))
                .ToList();

            if (largeDigits.Count == 0) { largeDigits = allDigits; }

            // 3. === BỘ LỌC LỚP 2: LỌC THEO "X-GAP" (Khoảng cách ngang) ===
            var sortedByX = largeDigits.OrderBy(d => d.X).ToList();
            List<Detection> bestCluster = new List<Detection>();
            List<Detection> currentCluster = new List<Detection>();

            for (int i = 0; i < sortedByX.Count; i++)
            {
                if (currentCluster.Count == 0) { currentCluster.Add(sortedByX[i]); continue; }
                var prevDigit = currentCluster.Last();
                var currentDigit = sortedByX[i];
                float gap = currentDigit.X - (prevDigit.X + prevDigit.Width);
                float avgWidth = (prevDigit.Width + currentDigit.Width) / 2;

                if (gap < (avgWidth * 1.5f)) { currentCluster.Add(currentDigit); }
                else
                {
                    if (currentCluster.Count > bestCluster.Count) { bestCluster = new List<Detection>(currentCluster); }
                    currentCluster.Clear();
                    currentCluster.Add(currentDigit);
                }
            }
            if (currentCluster.Count > bestCluster.Count) { bestCluster = new List<Detection>(currentCluster); }
            // ==========================================

            if (bestCluster.Count == 0)
            {
                Debug.WriteLine("[DEBUG] Lọc X-Gap thất bại.");
                return new MeterReadingResult { ErrorMessage = "Lỗi khi lọc cụm X-Axis" };
            }

            // 4. === QUY TẮC NGHIỆP VỤ MỚI ===

            // Sắp xếp cụm chính từ trái qua phải (theo X)
            // VÀ CHỈ LẤY TỐI ĐA 6 KÝ TỰ ĐẦU TIÊN
            var mainDigits = bestCluster.OrderBy(d => d.X).Take(6).ToList();

            string numberStrRaw = string.Join("", mainDigits.Select(d => d.ClassName)); // Sẽ là "007590"
            float avgConf = mainDigits.Average(d => d.Confidence);

            // 5. Chèn dấu "." vào trước ký tự cuối cùng
            string numberStrParsed = numberStrRaw;
            if (numberStrParsed.Length > 1)
            {
                // "007590" -> "00759.0"
                numberStrParsed = numberStrParsed.Insert(numberStrParsed.Length - 1, ".");
            }

            // 6. Bỏ số 0 ở đầu
            numberStrParsed = numberStrParsed.TrimStart('0'); // "759.0"

            if (numberStrParsed.StartsWith(".")) { numberStrParsed = "0" + numberStrParsed; }
            if (string.IsNullOrEmpty(numberStrParsed)) { numberStrParsed = "0"; }

            // 7. Parse
            if (decimal.TryParse(numberStrParsed, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal val)) // val = 759.0m
            {
                // 8. LÀM TRÒN XUỐNG (theo yêu cầu)
                // "chuyển thành 759"
                val = Math.Floor(val); // val = 759m

                return new MeterReadingResult
                {
                    Type = type,
                    Value = val, // Sẽ là 759
                    Confidence = avgConf,
                    RawText = numberStrRaw, // Giữ chuỗi gốc "007590"
                };
            }

            return new MeterReadingResult { ErrorMessage = $"Lỗi parse số: {numberStrParsed}", RawText = numberStrRaw };
        }


        public async Task<MeterReadingResult[]> AnalyzeImagesAsync(string[] imagePaths, MeterType type)
        {
            var results = new MeterReadingResult[imagePaths.Length];
            for (int i = 0; i < imagePaths.Length; i++)
            {
                results[i] = await AnalyzeImageAsync(imagePaths[i], type);
                if (i > 0 && i % 5 == 0) { GC.Collect(); GC.WaitForPendingFinalizers(); }
            }
            return results;
        }

        public bool IsModelAvailable() => _session != null && System.IO.File.Exists(_modelPath);
        public string GetModelPath() => _modelPath;

        public void Dispose()
        {
            _session?.Dispose();
            _session = null;
            GC.SuppressFinalize(this);
        }
    }
}