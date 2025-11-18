using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLKDPhongTro.BusinessLayer.DTOs
{
    // DTO kết quả tính công nợ, dùng chung giữa BusinessLayer và Presentation
    public class DebtCalculationResult
    {
        public string RoomName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
            = DateTime.Now;

        // Chỉ số điện
        public MeterType MeterType { get; set; } = MeterType.Electricity;
        public decimal OldElectricValue { get; set; }
        public decimal CurrentElectricValue { get; set; }
        public decimal ElectricityUsage { get; set; }
        public decimal ElectricityCost { get; set; }

        // Nước / chi phí khác
        public decimal WaterCost { get; set; }
        public decimal TotalDebt { get; set; }

        // Thông tin OCR & sự cố
        public string OcrStatus { get; set; } = string.Empty;
        public string ElectricImageUrl { get; set; } = string.Empty;
        public bool IsProcessed { get; set; }
        public string? ErrorMessage { get; set; }

        // Giá trị nhập tay và đọc từ ảnh (dùng cho so sánh)
        public decimal ManualValue { get; set; }
        public decimal OcrValue { get; set; }
        public bool IsDiscrepancy { get; set; }
        public string? WarningNote { get; set; }
        public string? OriginalImagePath { get; set; }
    }
}
