using System;

namespace QLKDPhongTro.DataLayer.Models
{
    /// <summary>
    /// Model đại diện cho người thuê phòng
    /// </summary>
    public class Tenant
    {
        public int MaKhachThue { get; set; } = 0; // Mapping từ MaNguoiThue
        public string HoTen { get; set; } = string.Empty;
        public string CCCD { get; set; } = string.Empty;
        public string SoDienThoai { get; set; } = string.Empty;
        public DateTime NgaySinh { get; set; } = DateTime.Now; // Mapping từ NgayBatDau
        public string GhiChu { get; set; } = string.Empty;
        
        // Các trường không có trong database nhưng cần cho UI
        public string Email { get; set; } = string.Empty;
        public string DiaChi { get; set; } = string.Empty;
        public string GioiTinh { get; set; } = string.Empty;
        public string NgheNghiep { get; set; } = string.Empty;
        public DateTime NgayTao { get; set; } = DateTime.Now;
        public DateTime NgayCapNhat { get; set; } = DateTime.Now;
    }
}
