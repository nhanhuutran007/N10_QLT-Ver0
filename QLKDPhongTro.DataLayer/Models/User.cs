using System;

namespace QLKDPhongTro.DataLayer.Models
{
    /// <summary>
    /// Model đại diện cho Admin (User)
    /// </summary>
    public class User
    {
        public int MaUser { get; set; }
        public int MaAdmin { get; set; }
        public string TenDangNhap { get; set; } = string.Empty;
        public string MatKhau { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string SoDienThoai { get; set; } = string.Empty;
        public string HoTen { get; set; } = string.Empty;
        public DateTime? NgaySinh { get; set; }
        public string CCCD { get; set; } = string.Empty;
        public DateTime? NgayCap { get; set; }
        public string NoiCap { get; set; } = string.Empty;
        public string DiaChi { get; set; } = string.Empty;
        public int MaNha { get; set; }
        public string TenTK { get; set; } = string.Empty;
        public string SoTK { get; set; } = string.Empty;
        public string LinkQr { get; set; } = string.Empty;
    }
}
