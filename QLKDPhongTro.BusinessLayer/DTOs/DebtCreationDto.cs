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
    }

    /// <summary>
    /// DTO cho kết quả xử lý công nợ
    /// </summary>
    public class DebtProcessingResultDto
    {
        public string RoomName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Timestamp { get; set; } = string.Empty;
        public decimal OldElectricValue { get; set; }
        public decimal CurrentElectricValue { get; set; }
        public decimal ElectricityUsage { get; set; }
        public decimal ElectricityCost { get; set; }
        public decimal WaterCost { get; set; }
        public decimal TotalDebt { get; set; }
        public string ProcessingStatus { get; set; } = string.Empty;
        public string ElectricImageUrl { get; set; } = string.Empty;
        public int? ContractId { get; set; }
        public bool IsProcessed { get; set; }
        public string? ErrorMessage { get; set; }
        public int? CreatedPaymentId { get; set; }
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