using System;

namespace QLKDPhongTro.Models
{
    /// <summary>
    /// Model cho bảng tblDangNhap
    /// </summary>
    public class User : BaseModel
    {
        public string Acc { get; set; }
        public string TenDangNhap { get; set; }
        public string MatKhau { get; set; }
        public string Email { get; set; }
    }
}
