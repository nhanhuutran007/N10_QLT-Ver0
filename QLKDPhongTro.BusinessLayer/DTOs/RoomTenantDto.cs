using System;

namespace QLKDPhongTro.BusinessLayer.DTOs
{
    public class RoomTenantDto
    {
        public int MaKhachThue { get; set; }
        public string HoTen { get; set; } = string.Empty;
        public string? SoDienThoai { get; set; }
        public string TrangThaiNguoiThue { get; set; } = string.Empty;
        public int? MaHopDong { get; set; }
        public string TrangThaiHopDong { get; set; } = string.Empty;
        public DateTime? NgayBatDau { get; set; }
        public DateTime? NgayKetThuc { get; set; }
        public bool IsContractHolder { get; set; }

        public string RoleLabel => IsContractHolder ? "Người đứng tên" : "Người ở cùng";

        public string ContractRangeDisplay =>
            NgayBatDau.HasValue && NgayKetThuc.HasValue
                ? $"{NgayBatDau:dd/MM/yyyy} - {NgayKetThuc:dd/MM/yyyy}"
                : "Chưa cập nhật thời hạn";
    }
}

