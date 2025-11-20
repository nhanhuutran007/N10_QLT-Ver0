using System;

namespace QLKDPhongTro.DataLayer.Models
{
    public class RoomTenantInfo
    {
        public int MaNguoiThue { get; set; }
        public string HoTen { get; set; } = string.Empty;
        public string? SoDienThoai { get; set; }
        public string TrangThaiNguoiThue { get; set; } = string.Empty;
        public int? MaHopDong { get; set; }
        public string TrangThaiHopDong { get; set; } = string.Empty;
        public DateTime? NgayBatDau { get; set; }
        public DateTime? NgayKetThuc { get; set; }
    }
}

