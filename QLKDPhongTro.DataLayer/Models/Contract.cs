using System;

namespace QLKDPhongTro.DataLayer.Models
{
    public class Contract
    {
        public int MaHopDong { get; set; }
        public int MaNguoiThue { get; set; }
        public int MaPhong { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }
        public decimal TienCoc { get; set; }
        public string FileHopDong { get; set; } = string.Empty; // Đường dẫn file
        public string TrangThai { get; set; } = string.Empty;
    }
}
