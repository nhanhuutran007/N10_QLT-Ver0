using System;

namespace QLKDPhongTro.DataLayer.Models
{
    public class RentedRoom 
    {
        public int MaPhong { get; set; } = 0;
        public string TenPhong { get; set; } = string.Empty;
        public double DienTich { get; set; } = 0.0;
        public double GiaCoBan { get; set; } = 0.0;
        public string TrangThai { get; set; } = string.Empty;
        public string GhiChu { get; set; } = string.Empty;
    }
}
