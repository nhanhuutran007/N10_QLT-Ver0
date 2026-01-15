using System;

namespace QLKDPhongTro.BusinessLayer.DTOs
{
    public class ContractDto
    {
        public int MaHopDong { get; set; }
        public int MaNguoiThue { get; set; }
        public int MaPhong { get; set; }
        public DateTime NgayBatDau { get; set; } = DateTime.Now;
        public DateTime NgayKetThuc { get; set; } = DateTime.Now.AddYears(1);
        public decimal TienCoc { get; set; }
        public decimal GiaThue { get; set; }
        public string? FileHopDong { get; set; }
        public string? TrangThai { get; set; }
        public string? GhiChu { get; set; }

        // 🔹 Thông tin Phòng (Bên A)
        public string? TenPhong { get; set; }
        public string? DiaChiPhong { get; set; }
        public double? DienTich { get; set; }
        public string? TrangThietBi { get; set; }

        // 🔹 Thông tin Chủ nhà (Bên A)
        public string? TenChuPhong { get; set; }
        public DateTime? NgaySinhChu { get; set; }
        public string? CCCDChu { get; set; }
        public DateTime? NgayCapChu { get; set; }
        public string? NoiCapChu { get; set; }
        public string? DiaChiChu { get; set; }
        public string? DienThoaiChu { get; set; }

        // 🔹 Thông tin Người thuê (Bên B)
        public string? TenNguoiThue { get; set; }
        public DateTime? NgaySinhNguoiThue { get; set; }
        public string? CCCDNguoiThue { get; set; }
        public DateTime? NgayCapNguoiThue { get; set; }
        public string? NoiCapNguoiThue { get; set; }
        public string? DiaChiNguoiThue { get; set; }
        public string? DienThoaiNguoiThue { get; set; }

        // 🔹 Thông tin tạo hợp đồng
        public string? NoiTaoHopDong { get; set; }
        public DateTime? NgayTaoHopDong { get; set; }
        public string? GiaBangChu { get; set; }
        public string? NgayTraTien { get; set; }
        public int? ThoiHanNam { get; set; }
        public DateTime? NgayGiaoNha { get; set; }
    }
}
