using System; // Thêm namespace này để dùng DateTime
using System.ComponentModel.DataAnnotations;

namespace QLKDPhongTro.BusinessLayer.DTOs
{
    public class DebtCreationDto
    {
        [Required(ErrorMessage = "Tên phòng là bắt buộc")]
        public string RoomName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Timestamp { get; set; } = string.Empty;

        [Required(ErrorMessage = "URL ảnh công tơ điện là bắt buộc")]
        public string ElectricImageUrl { get; set; } = string.Empty;

        public decimal OldElectricValue { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Chỉ số điện mới phải lớn hơn 0")]
        public decimal CurrentElectricValue { get; set; }

        // === LOGIC VALIDATION ===
        public bool IsValid => !string.IsNullOrEmpty(RoomName) && CurrentElectricValue > 0;

        // === CÁC TRƯỜNG ALIAS (BỔ SUNG ĐỂ KHỚP VỚI CONTROLLER) ===
        public string TenPhong => RoomName;
        public double ChiSoDienMoi => (double)CurrentElectricValue;
        public string ThangNam => DateTime.Now.ToString("MM/yyyy");

        // Sửa lỗi: Thêm thuộc tính Confidence (Mặc định 1.0 vì người nhập tay)
        public double Confidence => 1.0;

        public string GhiChu => $"Số liệu từ Google Form - Có ảnh minh chứng";

        public string? ErrorMessage { get; set; }
    }
}