using System;

namespace QLKDPhongTro.DataLayer.Models
{
    /// <summary>
    /// Model đại diện cho thanh toán
    /// </summary>
    public class Payment
    {
        public int MaThanhToan { get; set; } = 0;
        public int? MaHopDong { get; set; }
        public string ThangNam { get; set; } = string.Empty;
        public decimal? TienThue { get; set; }
        public decimal? TienDien { get; set; }
        public decimal? TienNuoc { get; set; }
        public decimal? TienInternet { get; set; }
        public decimal? TienVeSinh { get; set; }
        public decimal? TienGiuXe { get; set; }
        public decimal? ChiPhiKhac { get; set; }
        public decimal TongTien { get; set; }
        public string TrangThaiThanhToan { get; set; } = "Chưa thanh toán";
        public DateTime? NgayThanhToan { get; set; }

        // Các trường không có trong database nhưng cần cho UI
        public string TenKhachHang { get; set; } = string.Empty;
        public string TenPhong { get; set; } = string.Empty;
        public string SoDienThoai { get; set; } = string.Empty;
        public string DiaChi { get; set; } = string.Empty;
    }
}