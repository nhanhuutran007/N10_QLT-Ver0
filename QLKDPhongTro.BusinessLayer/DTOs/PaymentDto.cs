using System;
using System.ComponentModel.DataAnnotations;

namespace QLKDPhongTro.BusinessLayer.DTOs
{
    public class PaymentDto
    {
        public int MaThanhToan { get; set; }

        [Required(ErrorMessage = "Mã hợp đồng là bắt buộc")]
        public int? MaHopDong { get; set; }

        [Required(ErrorMessage = "Tháng năm là bắt buộc")]
        [StringLength(7, MinimumLength = 7, ErrorMessage = "Tháng năm phải có định dạng MM/yyyy")]
        public string ThangNam { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Tiền thuê không được âm")]
        public decimal? TienThue { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Tiền điện không được âm")]
        public decimal? TienDien { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Tiền nước không được âm")]
        public decimal? TienNuoc { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Tiền internet không được âm")]
        public decimal? TienInternet { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Tiền vệ sinh không được âm")]
        public decimal? TienVeSinh { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Tiền giữ xe không được âm")]
        public decimal? TienGiuXe { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Chi phí khác không được âm")]
        public decimal? ChiPhiKhac { get; set; }

        public decimal TongTien { get; set; }
        public string TrangThaiThanhToan { get; set; } = "Chưa thanh toán";
        public DateTime? NgayThanhToan { get; set; }

        // Navigation properties
        public string TenKhachHang { get; set; }
        public string TenPhong { get; set; }
        public string SoDienThoai { get; set; }
        public string DiaChi { get; set; }
    }
}