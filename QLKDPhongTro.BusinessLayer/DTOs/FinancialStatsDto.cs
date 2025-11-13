using System;
using System.Collections.Generic;

namespace QLKDPhongTro.BusinessLayer.DTOs
{
    public class FinancialStatsDto
    {
        public decimal TongThuNhap { get; set; }
        public decimal TongChiPhi { get; set; }
        public decimal LoiNhuan { get; set; }
        public decimal TongCongNo { get; set; }
        public int SoPhongNo { get; set; }
        public int SoKhachDangThue { get; set; }
        public decimal TangTruongThuNhap { get; set; }
        public decimal TangTruongChiPhi { get; set; }
        public decimal TyLeLoiNhuan { get; set; }
        public int SoPhongDangThue { get; set; }

        // Thống kê theo thời gian
        public List<MonthlyStatsDto> ThongKeTheoThang { get; set; } = new List<MonthlyStatsDto>();
        public List<ExpenseCategoryDto> PhanLoaiChiPhi { get; set; } = new List<ExpenseCategoryDto>();
    }

    public class MonthlyStatsDto
    {
        public string ThangNam { get; set; }
        public decimal ThuNhap { get; set; }
        public decimal ChiPhi { get; set; }
        public decimal LoiNhuan { get; set; }
    }

    public class ExpenseCategoryDto
    {
        public string TenLoai { get; set; }
        public decimal SoTien { get; set; }
        public decimal TyLe { get; set; }
        public string Color { get; set; }
    }
}