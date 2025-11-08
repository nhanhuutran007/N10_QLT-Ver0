using System;

namespace QLKDPhongTro.BusinessLayer.DTOs
{
    /// <summary>
    /// DTO cho chi tiết hóa đơn thanh toán
    /// </summary>
    public class InvoiceDetailDto
    {
        // Thông tin thanh toán
        public int MaThanhToan { get; set; }
        public string ThangNam { get; set; } = string.Empty;
        public DateTime? NgayThanhToan { get; set; }
        public string TrangThaiThanhToan { get; set; } = string.Empty;
        public decimal TongTien { get; set; }

        // Thông tin người thuê
        public string HoTen { get; set; } = string.Empty;
        public string CCCD { get; set; } = string.Empty;
        public string SoDienThoai { get; set; } = string.Empty;

        // Thông tin hợp đồng
        public int MaHopDong { get; set; }

        // Chi tiết các khoản phí
        public decimal TienThue { get; set; }
        public decimal TienDien { get; set; }
        public decimal TienNuoc { get; set; }
        public decimal TienInternet { get; set; }
        public decimal TienVeSinh { get; set; }
        public decimal TienGiuXe { get; set; }
        public decimal ChiPhiKhac { get; set; }

        // Điện nước chi tiết
        public decimal? DonGiaDien { get; set; }
        public decimal? DonGiaNuoc { get; set; }
        public decimal? SoDien { get; set; }
        public decimal? SoNuoc { get; set; }

        // Các giá trị tính toán (không lấy từ DB)
        public decimal TamTinhDien => (DonGiaDien ?? 0) * (SoDien ?? 0);
        public decimal TamTinhNuoc => (DonGiaNuoc ?? 0) * (SoNuoc ?? 0);
        public decimal TamTinhThue => TienThue;
        public decimal TamTinhInternet => TienInternet;
        public decimal TamTinhVeSinh => TienVeSinh;
        public decimal TamTinhGiuXe => TienGiuXe;
        public decimal TamTinhKhac => ChiPhiKhac;
        public decimal TamTinh => TamTinhThue + TamTinhDien + TamTinhNuoc + TamTinhInternet + TamTinhVeSinh + TamTinhGiuXe + TamTinhKhac;
        public decimal KhauTru { get; set; } = 0;
        public decimal TongTienTinhToan => TamTinh - KhauTru;
    }
}

