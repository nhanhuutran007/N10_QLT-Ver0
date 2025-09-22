using System;
using System.Collections.Generic;

namespace QLKDPhongTro.Models
{
    /// <summary>
    /// Model cho bảng tblHoaDon
    /// </summary>
    public class HoaDon : BaseModel
    {
        public string MaHoaDon { get; set; }
        public string MaPhongTro { get; set; }
        public string ThangNam { get; set; }
        public decimal TienVeSinh { get; set; }
        public decimal Internet { get; set; }
        public decimal DichVuKhac { get; set; }
        public decimal KhuyenMai { get; set; }
        public string GhiChu { get; set; }
        public string TrangThai { get; set; } = "Chưa thanh toán";
        public DateTime? NgayThanhToan { get; set; }
        
        // Navigation properties
        public PhongTro PhongTro { get; set; }
        public List<ChiTietHoaDon> ChiTietHoaDons { get; set; } = new List<ChiTietHoaDon>();
    }
}
