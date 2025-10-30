using System;

namespace QLKDPhongTro.DataLayer.Models
{
    public class Contract
    {
        public int MaHopDong { get; set; }
        public int MaPhong { get; set; }
        public int MaNguoiThue { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }
        public decimal TienCoc { get; set; }
        public decimal GiaThue { get; set; }
        public string? GhiChu { get; set; }
        public string? TrangThai { get; set; }
        public string? FileHopDong { get; set; }

        // Thông tin liên kết
        public string? TenPhong { get; set; }
        public string? TenNguoiThue { get; set; }
    }
}
