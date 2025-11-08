using System;

namespace QLKDPhongTro.DataLayer.Models
{
    /// <summary>
    /// Model đại diện cho thanh toán
    /// </summary>
    public class Payment
    {
        public int MaThanhToan { get; set; } = 0;
        public int? MaHopDong { get; set; }
        public string ThangNam { get; set; } = string.Empty;
        public decimal? TienThue { get; set; }
        public decimal? TienDien { get; set; }
        public decimal? TienNuoc { get; set; }
        public decimal? TienInternet { get; set; }
        public decimal? TienVeSinh { get; set; }
        public decimal? TienGiuXe { get; set; }
        public decimal? ChiPhiKhac { get; set; }
        // Đơn giá và chỉ số điện nước theo tháng (nếu có trong bảng)
        public decimal? DonGiaDien { get; set; }
        public decimal? DonGiaNuoc { get; set; }
        public decimal? SoDien { get; set; }
        public decimal? SoNuoc { get; set; }
        public decimal TongTien { get; set; }
        public string TrangThaiThanhToan { get; set; } = "Chưa thanh toán";
        public DateTime? NgayThanhToan { get; set; }

        // Các trường không có trong database nhưng cần cho UI
        public string TenKhachHang { get; set; } = string.Empty;
        public string TenPhong { get; set; } = string.Empty;
        public string SoDienThoai { get; set; } = string.Empty;
        public string DiaChi { get; set; } = string.Empty;
    }
    /// <summary>
    /// Model cho thống kê tài chính
    /// </summary>
    public class FinancialStats
    {
        public decimal TongThuNhap { get; set; }
        public decimal TongChiPhi { get; set; }
        public decimal LoiNhuan { get; set; }
        public decimal TongCongNo { get; set; }
        public int SoPhongNo { get; set; }
        public decimal TangTruongThuNhap { get; set; }
        public decimal TangTruongChiPhi { get; set; }
        public decimal TyLeLoiNhuan { get; set; }
        public List<MonthlyStats> ThongKeTheoThang { get; set; } = new();
        public List<ExpenseCategory> PhanLoaiChiPhi { get; set; } = new();
    }

    public class MonthlyStats
    {
        public string ThangNam { get; set; } = string.Empty;
        public decimal ThuNhap { get; set; }
        public decimal ChiPhi { get; set; }
        public decimal LoiNhuan { get; set; }
    }

    public class ExpenseCategory
    {
        public string TenLoai { get; set; } = string.Empty;
        public decimal SoTien { get; set; }
        public decimal TyLe { get; set; }
        public string Color { get; set; } = "#000000";
    }
}