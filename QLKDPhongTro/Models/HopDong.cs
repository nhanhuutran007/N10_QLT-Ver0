using System;

namespace QLKDPhongTro.Models
{
    /// <summary>
    /// Model cho bảng tblHopDong
    /// </summary>
    public class HopDong : BaseModel
    {
        public string MaHopDong { get; set; }
        public string MaPhongTro { get; set; }
        public string MaKhachThue { get; set; }
        public string HoTenNguoiKy { get; set; }
        public decimal TienCoc { get; set; }
        public DateTime NgayKy { get; set; }
        public DateTime NgayHetHan { get; set; }
        public string GhiChu { get; set; }
        public string TrangThai { get; set; } = "Có hiệu lực";
        
        // Navigation properties
        public PhongTro PhongTro { get; set; }
        public KhachThue KhachThue { get; set; }
    }
}
