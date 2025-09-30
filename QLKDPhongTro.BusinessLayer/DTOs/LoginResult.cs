using QLKDPhongTro.DataLayer.Models;

namespace QLKDPhongTro.BusinessLayer.DTOs
{
    /// <summary>
    /// Kết quả đăng nhập
    /// </summary>
    public class LoginResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public User? User { get; set; }
    }
}
