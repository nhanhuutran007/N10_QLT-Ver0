namespace QLKDPhongTro.BusinessLayer.DTOs
{
    /// <summary>
    /// Kết quả đăng ký
    /// </summary>
    public class RegisterResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
