using System;

namespace QLKDPhongTro.DataLayer.Models
{
    /// <summary>
    /// Model đại diện cho User
    /// </summary>
    public class User
    {
        public string Acc { get; set; } = string.Empty;
        public string TenDangNhap { get; set; } = string.Empty;
        public string MatKhau { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
