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
        public int? MaPhong { get; set; }

        [Required(ErrorMessage = "Họ tên không được bỏ trống")]
        public string HoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "CCCD không được bỏ trống")]
        [StringLength(12, MinimumLength = 9)]
        public string CCCD { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string SoDienThoai { get; set; } = string.Empty;

        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string GioiTinh { get; set; } = string.Empty;
        public string NgheNghiep { get; set; } = string.Empty;
        public string TrangThai { get; set; } = string.Empty;

        [StringLength(500)]
        public string GhiChu { get; set; } = string.Empty;

        // 🆕 Các trường mới
        public DateTime? NgaySinh { get; set; }
        public DateTime? NgayCap { get; set; }
        public string NoiCap { get; set; } = string.Empty;
        public string DiaChi { get; set; } = string.Empty;

        public DateTime NgayTao { get; set; } = DateTime.Now;
        public DateTime NgayCapNhat { get; set; } = DateTime.Now;
    }
}
