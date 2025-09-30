namespace QLKDPhongTro.BusinessLayer.DTOs
{
    /// <summary>
    /// Kết quả validation
    /// </summary>
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
