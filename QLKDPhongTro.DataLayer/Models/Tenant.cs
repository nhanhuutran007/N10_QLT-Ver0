using System;

namespace QLKDPhongTro.DataLayer.Models
{
    /// <summary>
    /// Model đại diện cho người thuê phòng (mapping DB table [NguoiThue])
    /// </summary>
    public class Tenant
    {
        public int MaKhachThue { get; set; } = 0; // Mapping từ MaNguoiThue
        public string HoTen { get; set; } = string.Empty;
        public string CCCD { get; set; } = string.Empty;
        public string SoDienThoai { get; set; } = string.Empty;

        public DateTime? NgaySinh { get; set; }  // 🆕
        public DateTime? NgayCap { get; set; }  // 🆕
        public string NoiCap { get; set; } = string.Empty;  // 🆕
        public string DiaChi { get; set; } = string.Empty;  // 🆕

        public string GhiChu { get; set; } = string.Empty;
        public string TrangThai { get; set; } = "Đang thuê";

        // Các trường mở rộng phục vụ UI
        public string Email { get; set; } = string.Empty;
        public string GioiTinh { get; set; } = string.Empty;
        public string NgheNghiep { get; set; } = string.Empty;
        public DateTime NgayTao { get; set; } = DateTime.Now;
        public DateTime NgayCapNhat { get; set; } = DateTime.Now;
    }
}
