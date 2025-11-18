using System;
namespace QLKDPhongTro.BusinessLayer.DTOs
{
    public class ProcessingResult
    {
        /// <summary>
        /// Xử lý thành công hay thất bại.
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Thông báo lỗi (nếu có).
        /// </summary>
        public string ErrorMessage { get; set; } = string.Empty;
    }
}
