using System;
using System.ComponentModel.DataAnnotations;

namespace QLKDPhongTro.BusinessLayer.DTOs
{
    /// <summary>
    /// DTO cho khách thuê phòng
    /// </summary>
    public class TenantDto
    {
        public int MaKhachThue { get; set; }

        [Required(ErrorMessage = "Họ tên không được bỏ trống")]
        [StringLength(100, ErrorMessage = "Họ tên không được vượt quá 100 ký tự")]
        public string HoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "CCCD không được bỏ trống")]
        [StringLength(12, MinimumLength = 9, ErrorMessage = "CCCD phải có từ 9-12 ký tự")]
        public string CCCD { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số điện thoại không được bỏ trống")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string SoDienThoai { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Địa chỉ không được bỏ trống")]
        [StringLength(200, ErrorMessage = "Địa chỉ không được vượt quá 200 ký tự")]
        public string DiaChi { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ngày bắt đầu thuê không được bỏ trống")]
        public DateTime NgaySinh { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Giới tính không được bỏ trống")]
        public string GioiTinh { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Nghề nghiệp không được vượt quá 100 ký tự")]
        public string NgheNghiep { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Ghi chú không được vượt quá 500 ký tự")]
        public string GhiChu { get; set; } = string.Empty;

        public DateTime NgayTao { get; set; } = DateTime.Now;
        public DateTime NgayCapNhat { get; set; } = DateTime.Now;
    }
}
