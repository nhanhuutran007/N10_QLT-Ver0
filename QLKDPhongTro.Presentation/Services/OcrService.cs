using System;
using System.IO;
using System.Threading.Tasks;

namespace QLKDPhongTro.Presentation.Services
{
    /// <summary>
    /// Service sử dụng YOLOv9 để đọc chỉ số điện/nước từ ảnh
    /// Chỉ sử dụng YOLOv9 Object Detection model đã train
    /// </summary>
    public class OcrService
    {
        private readonly YoloMeterReadingService _yoloService;

        public OcrService()
        {
            // Chỉ sử dụng YOLOv9 model đã train
            _yoloService = new YoloMeterReadingService();
            
            if (!_yoloService.IsModelAvailable())
            {
                throw new InvalidOperationException(
                    $"YOLOv9 model không tìm thấy. Vui lòng đặt file 'yolov9n_meter_reading.onnx' vào thư mục:\n" +
                    $"- {Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "models")}\n" +
                    $"- {AppDomain.CurrentDomain.BaseDirectory}\n" +
                    $"- {Path.Combine(Directory.GetCurrentDirectory(), "models")}");
            }
        }

        /// <summary>
        /// Kiểm tra xem đang dùng phương thức nào (luôn là YOLO)
        /// </summary>
        public bool IsUsingYolo => _yoloService.IsModelAvailable();

        /// <summary>
        /// Phân tích ảnh và trích xuất cả chỉ số điện và nước
        /// Chỉ sử dụng YOLOv9 model đã train
        /// </summary>
        public async Task<MeterReadingResult> AnalyzeImageAsync(string imagePath, MeterType type)
        {
            if (!_yoloService.IsModelAvailable())
            {
                return new MeterReadingResult
                {
                    Type = type,
                    Value = 0,
                    Confidence = 0f,
                    ErrorMessage = "YOLOv9 model không khả dụng. Vui lòng kiểm tra lại file model.",
                    RawText = "Model not available"
                };
            }

            try
            {
                return await _yoloService.AnalyzeImageAsync(imagePath, type);
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
        /// Phân tích nhiều ảnh và trích xuất chỉ số
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
                        RawText = $"Lỗi: {ex.Message}",
                        ErrorMessage = $"Lỗi khi xử lý ảnh {Path.GetFileName(imagePaths[i])}: {ex.Message}"
                    };
                }
            }

            return results;
        }
    }

    /// <summary>
    /// Kết quả đọc chỉ số từ YOLOv9
    /// </summary>
    public class MeterReadingResult
    {
        public MeterType Type { get; set; }
        public decimal Value { get; set; }
        public float Confidence { get; set; }
        public string RawText { get; set; } = string.Empty;
        public string? ErrorMessage { get; set; }
        public bool IsValid => Confidence > 0.3f && Value > 0;
    }

    /// <summary>
    /// Loại chỉ số
    /// </summary>
    public enum MeterType
    {
        Electricity,
        Water
    }
}
