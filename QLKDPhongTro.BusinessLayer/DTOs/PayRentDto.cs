using System;
using System.ComponentModel.DataAnnotations;

namespace QLKDPhongTro.BusinessLayer.DTOs
{
    public class PayRentDTO
    {
        [Required(ErrorMessage = "Mã thanh toán là bắt buộc")]
        public int MaThanhToan { get; set; }

        [Required(ErrorMessage = "Ngày thanh toán là bắt buộc")]
        public DateTime NgayThanhToan { get; set; } = DateTime.Now;

        public string PhuongThucThanhToan { get; set; } = "Tiền mặt";

        [StringLength(500, ErrorMessage = "Ghi chú không được vượt quá 500 ký tự")]
        public string GhiChu { get; set; }
    }
}