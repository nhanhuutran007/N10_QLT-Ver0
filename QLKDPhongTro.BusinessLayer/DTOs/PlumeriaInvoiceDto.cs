using System;
using System.Collections.Generic;

namespace QLKDPhongTro.BusinessLayer.DTOs
{
    /// <summary>
    /// DTO cho hóa đơn Plumeria - thiết kế PDF mới
    /// </summary>
    public class PlumeriaInvoiceDto
    {
        // Thông tin header
        public string ThangNam { get; set; } = string.Empty;
        public DateTime NgayPhatHanh { get; set; }
        public string SoThamChieu { get; set; } = string.Empty;

        // Thông tin khách hàng
        public string TenKhachHang { get; set; } = string.Empty;
        public string DienThoai { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string SoHopDong { get; set; } = string.Empty;
        public DateTime NgayBatDauHopDong { get; set; }

        // Thông tin phòng
        public string TenPhong { get; set; } = string.Empty;
        public int SoNguoiLuuTru { get; set; }

        // Chi tiết điện
        public decimal DonGiaDien { get; set; }
        public DateTime NgayGhiDienCu { get; set; }
        public int ChiSoDienCu { get; set; }
        public DateTime NgayGhiDienMoi { get; set; }
        public int ChiSoDienMoi { get; set; }
        public int MucTieuThuDien { get; set; }
        public decimal ThanhTienDien { get; set; }

        // Chi tiết nước
        public decimal DonGiaNuoc { get; set; }
        public DateTime NgayGhiNuocCu { get; set; }
        public int ChiSoNuocCu { get; set; }
        public DateTime NgayGhiNuocMoi { get; set; }
        public int ChiSoNuocMoi { get; set; }
        public int MucTieuThuNuoc { get; set; }
        public decimal ThanhTienNuoc { get; set; }

        // Các khoản mục chi tiết
        public List<KhoanMucDto> CacKhoanMuc { get; set; } = new List<KhoanMucDto>();
        public decimal TongCong { get; set; }

        // Thông tin ngân hàng
        public string TenTaiKhoanNH { get; set; } = string.Empty;
        public string SoTaiKhoanNH { get; set; } = string.Empty;
        public string NganHang { get; set; } = string.Empty;
        public string ChiNhanh { get; set; } = string.Empty;

        // Thời hạn thanh toán
        public DateTime ThoiHanThanhToan { get; set; }
    }

    /// <summary>
    /// DTO cho khoản mục chi tiết
    /// </summary>
    public class KhoanMucDto
    {
        public int STT { get; set; }
        public string NoiDung { get; set; } = string.Empty;
        public string DonVi { get; set; } = string.Empty;
        public decimal DonGia { get; set; }
        public decimal SoLuong { get; set; }
        public decimal ThanhTien { get; set; }
    }
}

