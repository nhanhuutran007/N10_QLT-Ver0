using System;

namespace QLKDPhongTro.DataLayer.Models
{
    /// <summary>
    /// Model đại diện cho Admin (User)
    /// </summary>
    public class User
    {
        public int MaAdmin { get; set; }
        public string TenDangNhap { get; set; } = string.Empty;
        public string MatKhau { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string SoDienThoai { get; set; } = string.Empty;
        public int MaNha { get; set; }
    }
}
