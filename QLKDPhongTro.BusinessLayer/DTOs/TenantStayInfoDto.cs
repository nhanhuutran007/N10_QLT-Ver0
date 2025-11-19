using System;

namespace QLKDPhongTro.BusinessLayer.DTOs
{
    public class TenantStayInfoDto
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

        public string ExpectedTenantStatus { get; set; } = string.Empty;
        public string ExpectedRoomStatus { get; set; } = string.Empty;
        public bool IsSynchronized { get; set; }
        public string ConsistencyMessage { get; set; } = string.Empty;
    }
}

