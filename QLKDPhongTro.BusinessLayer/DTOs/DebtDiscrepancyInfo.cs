using System;
namespace QLKDPhongTro.BusinessLayer.DTOs
{
    public class DebtDiscrepancyInfo
    {
        /// <summary>
        /// Tên phòng có sự cố.
        /// </summary>
        public string RoomName { get; set; } = string.Empty;

        /// <summary>
        /// Giá trị khách nhập tay từ Google Form.
        /// </summary>
        public decimal ManualValue { get; set; }

        /// <summary>
        /// Giá trị đọc được từ OCR.
        /// </summary>
        public decimal OcrValue { get; set; }

        /// <summary>
        /// Giá trị đã được admin xác nhận (nếu có).
        /// </summary>
        public decimal ConfirmedValue { get; set; }

        /// <summary>
        /// Ghi chú / cảnh báo chi tiết.
        /// </summary>
        public string WarningNote { get; set; } = string.Empty;

        /// <summary>
        /// Đường dẫn ảnh gốc đã tải về.
        /// </summary>
        public string OriginalImagePath { get; set; } = string.Empty;

        /// <summary>
        /// Thời điểm phát hiện sự cố.
        /// </summary>
        public DateTime DetectionTime { get; set; }

        /// <summary>
        /// Kết quả đọc chỉ số chi tiết (kiểu thực tế được lưu từ Presentation layer).
        /// Dùng object để tránh phụ thuộc ngược giữa các layer.
        /// </summary>
        public object? MeterReadingResult { get; set; }

        /// <summary>
        /// Chênh lệch tuyệt đối giữa giá trị nhập tay và OCR.
        /// </summary>
        public decimal Difference => Math.Abs(ManualValue - OcrValue);

        /// <summary>
        /// Đã có giá trị xác nhận hay chưa.
        /// </summary>
        public bool HasConfirmedValue => ConfirmedValue > 0;
    }
}
