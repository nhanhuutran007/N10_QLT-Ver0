using System.ComponentModel.DataAnnotations;

namespace QLKDPhongTro.BusinessLayer.DTOs
{
    /// <summary>
    /// DTO để tạo công nợ từ Google Form
    /// </summary>
    public class DebtCreationDto
    {
        [Required(ErrorMessage = "Tên phòng là bắt buộc")]
        public string RoomName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; } = string.Empty;

        public string Timestamp { get; set; } = string.Empty;

        [Required(ErrorMessage = "URL ảnh công tơ điện là bắt buộc")]
        public string ElectricImageUrl { get; set; } = string.Empty;

        [Range(0, double.MaxValue, ErrorMessage = "Chỉ số điện cũ không được âm")]
        public decimal OldElectricValue { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Chỉ số điện mới không được âm")]
        public decimal CurrentElectricValue { get; set; }

        // Các trường tính toán tự động
        public decimal ElectricityUsage => CurrentElectricValue > OldElectricValue ? CurrentElectricValue - OldElectricValue : 0;
        public decimal ElectricityCost => ElectricityUsage * 3500; // 3.500 VND/kWh
        public decimal WaterCost => 100000; // 100.000 VND/tháng cố định
        public decimal TotalDebt => ElectricityCost + WaterCost;
        public bool HasManualValues => OldElectricValue > 0 && CurrentElectricValue > 0;

        // Thông tin hợp đồng (sẽ được điền tự động)
        public int? ContractId { get; set; }
        public string? CustomerName { get; set; }
        public string? PhoneNumber { get; set; }

        // Trạng thái xử lý
        public bool IsProcessed { get; set; }
        public string? ProcessingStatus { get; set; }
        public string? ErrorMessage { get; set; }

        // === THÊM CÁC PROPERTY BỊ THIẾU ===
        public bool IsValid => !string.IsNullOrEmpty(RoomName) &&
                              !string.IsNullOrEmpty(Email) &&
                              CurrentElectricValue >= OldElectricValue;

        // Alias properties để tương thích với FinancialController
        public string TenPhong => RoomName;
        public double ChiSoDienMoi => (double)CurrentElectricValue;
        public string ThangNam => DateTime.Now.ToString("MM/yyyy");
        public string GhiChu => $"Tạo từ Google Form - Email: {Email}";
        public double Confidence => 1.0;
    }

    /// <summary>
    /// DTO cho batch processing từ Google Form
    /// </summary>
    public class GoogleFormBatchProcessingDto
    {
        public List<DebtCreationDto> Debts { get; set; } = new List<DebtCreationDto>();
        public int TotalRecords { get; set; }
        public int ProcessedCount { get; set; }
        public int ErrorCount { get; set; }
        public DateTime ProcessingTime { get; set; } = DateTime.Now;
        public string? SpreadsheetId { get; set; }
        public string? ProcessingSummary { get; set; }
    }
}