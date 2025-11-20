using System;

namespace QLKDPhongTro.BusinessLayer.DTOs
{
    /// <summary>
    /// Kết quả tính toán công nợ cho một phòng dựa trên dữ liệu Google Form + DB.
    /// </summary>
    public class DebtCalculationResult
    {
        // Thông tin chung
        public string RoomName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }

        // Chỉ số điện
        public decimal OldElectricValue { get; set; }
        public decimal CurrentElectricValue { get; set; }
        public decimal ElectricityUsage { get; set; }
        public decimal ElectricityCost { get; set; }

        // Các khoản phí cố định
        public decimal WaterCost { get; set; }
        public decimal InternetCost { get; set; }
        public decimal SanitationCost { get; set; }
        public decimal OtherCost { get; set; }

        public decimal TotalDebt { get; set; }

        // Thông tin OCR & trạng thái xử lý
        public string OcrStatus { get; set; } = string.Empty;
        public string ElectricImageUrl { get; set; } = string.Empty;
        public bool IsProcessed { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;

        // Dữ liệu phục vụ so sánh / cảnh báo
        public decimal ManualValue { get; set; }
        public decimal OcrValue { get; set; }
        public bool IsDiscrepancy { get; set; }
        public string WarningNote { get; set; } = string.Empty;
        public string OriginalImagePath { get; set; } = string.Empty;
    }
}
