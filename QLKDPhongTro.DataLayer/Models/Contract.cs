using System;

namespace QLKDPhongTro.DataLayer.Models
{
    public class Contract
    {
        public int MaHopDong { get; set; }
        public int MaPhong { get; set; }
        public int MaNguoiThue { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }
        public decimal TienCoc { get; set; }
        public decimal GiaThue { get; set; }
        public string? GhiChu { get; set; }
        public string? TrangThai { get; set; }
        public string? FileHopDong { get; set; }

        // ---------------------------
        // 🔹 Thông tin liên kết Phòng (BÊN A)
        // ---------------------------
        public string? TenPhong { get; set; }
        public string? DiaChiPhong { get; set; }
        public double? DienTich { get; set; }
        public string? TrangThietBi { get; set; }

        // ---------------------------
        // 🔹 Thông tin BÊN A (chủ nhà)
        // ---------------------------
        public string? TenChuPhong { get; set; }
        public DateTime? NgaySinhChu { get; set; }
        public string? CCCDChu { get; set; }
        public DateTime? NgayCapChu { get; set; }
        public string? NoiCapChu { get; set; }
        public string? DiaChiChu { get; set; }
        public string? DienThoaiChu { get; set; }

        // ---------------------------
        // 🔹 Thông tin BÊN B (người thuê)
        // ---------------------------
        public string? TenNguoiThue { get; set; }
        public DateTime? NgaySinhNguoiThue { get; set; }
        public string? CCCDNguoiThue { get; set; }
        public DateTime? NgayCapNguoiThue { get; set; }
        public string? NoiCapNguoiThue { get; set; }
        public string? DiaChiNguoiThue { get; set; }
        public string? DienThoaiNguoiThue { get; set; }

        // ---------------------------
        // 🔹 Thông tin tạo hợp đồng
        // ---------------------------
        public string? NoiTaoHopDong { get; set; }
        public DateTime? NgayTaoHopDong { get; set; }
        public string? GiaBangChu { get; set; }
        public string? NgayTraTien { get; set; }
        public int? ThoiHanNam { get; set; }
        public DateTime? NgayGiaoNha { get; set; }
    }
}
