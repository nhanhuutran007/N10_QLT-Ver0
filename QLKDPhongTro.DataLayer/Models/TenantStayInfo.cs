using System;

namespace QLKDPhongTro.DataLayer.Models
{
    /// <summary>
    /// Mô tả trạng thái lưu trú hiện tại của người thuê (phòng, hợp đồng, v.v)
    /// </summary>
    public class TenantStayInfo
    {
        public int? MaHopDong { get; set; }
        public int? MaPhong { get; set; }
        public string? TenPhong { get; set; }
        public string? TrangThaiPhong { get; set; }
        public string? TrangThaiHopDong { get; set; }
        public DateTime? NgayBatDau { get; set; }
        public DateTime? NgayKetThuc { get; set; }
        public decimal? TienCoc { get; set; }
        public string? TrangThaiNguoiThue { get; set; }
        public string? SoDienThoai { get; set; }
    }
}

