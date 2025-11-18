using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLKDPhongTro.BusinessLayer.DTOs
{
    // Thông tin chi tiết về sự cố sai lệch giữa số nhập tay và số đọc từ ảnh
    public class DebtDiscrepancyInfo
    {
        public string RoomName { get; set; } = string.Empty;
        public decimal ManualValue { get; set; }
        public decimal OcrValue { get; set; }
        public decimal ConfirmedValue { get; set; }
        public string? WarningNote { get; set; }
        public MeterReadingResult? MeterReadingResult { get; set; }
        public string? OriginalImagePath { get; set; }
        public DateTime DetectionTime { get; set; } = DateTime.Now;
    }
}
