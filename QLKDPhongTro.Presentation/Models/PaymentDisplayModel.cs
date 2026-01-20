using System;

namespace QLKDPhongTro.Presentation.Models
{
    /// <summary>
    /// Model for displaying payment information in the UI
    /// </summary>
    public class PaymentDisplayModel
    {
        // Basic Information
        public int MaThanhToan { get; set; }
        public int? MaHopDong { get; set; }
        public string MaThanhToanDisplay { get; set; } = string.Empty;
        public string ThangNam { get; set; } = string.Empty;
        public string TrangThai { get; set; } = string.Empty;
        public string TrangThaiColor { get; set; } = "#2E7D32"; // Default green
        public string NgayThanhToan { get; set; } = string.Empty;
        
        // Cost Breakdown - Formatted for display
        public string TienThue { get; set; } = string.Empty;
        public string TienDien { get; set; } = string.Empty;
        public string TienNuoc { get; set; } = string.Empty;
        public string TienInternet { get; set; } = string.Empty;
        public string TienVeSinh { get; set; } = string.Empty;
        public string TienGiuXe { get; set; } = string.Empty;
        public string ChiPhiKhac { get; set; } = string.Empty;
        
        // Electricity and Water Details
        public string ChiSoDienCu { get; set; } = string.Empty;
        public string ChiSoDienMoi { get; set; } = string.Empty;
        public string SoDien { get; set; } = string.Empty;
        public string DonGiaDien { get; set; } = string.Empty;
        public string SoNuoc { get; set; } = string.Empty;
        public string DonGiaNuoc { get; set; } = string.Empty;
        
        // Summary
        public string TongTien { get; set; } = string.Empty;
        public string SoTienDaTra { get; set; } = string.Empty;
        public string SoTienConLai { get; set; } = string.Empty;
        public string GhiChu { get; set; } = string.Empty;
        
        // Additional Info
        public string TenPhong { get; set; } = string.Empty;
        public string TenKhachHang { get; set; } = string.Empty;
        
        // Raw values for calculations
        public decimal TongTienRaw { get; set; }
        public decimal SoTienDaTraRaw { get; set; }
        public DateTime? NgayThanhToanRaw { get; set; }
        
        // For DataGrid display - Short format
        public string ThangNamShort => ThangNam;
        public string TongTienShort => TongTien;
        public string TrangThaiShort => TrangThai;
    }
}
