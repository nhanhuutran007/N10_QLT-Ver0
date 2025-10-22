using System;
using System.ComponentModel.DataAnnotations;

namespace QLKDPhongTro.BusinessLayer.DTOs
{
    public class ExpenseDTO
    {
        public int MaChiPhi { get; set; }

        [Required(ErrorMessage = "Mã thanh toán là bắt buộc")]
        public int MaThanhToan { get; set; }

        [Required(ErrorMessage = "Loại chi phí là bắt buộc")]
        [StringLength(50, ErrorMessage = "Loại chi phí không được vượt quá 50 ký tự")]
        public string LoaiChiPhi { get; set; } // "Điện", "Nước", "Khác"

        [Required(ErrorMessage = "Số tiền là bắt buộc")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Số tiền phải lớn hơn 0")]
        public decimal SoTien { get; set; }

        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự")]
        public string MoTa { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;

        // Cho chi phí điện/nước
        public int? ChiSoCu { get; set; }
        public int? ChiSoMoi { get; set; }
        public string HinhAnhDongHo { get; set; }

        // Navigation
        public string TenPhong { get; set; }
        public string ThangNam { get; set; }
    }
}