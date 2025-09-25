using System;

namespace QLKDPhongTro.Models
{
    /// <summary>
    /// Model cho bảng tblChiTietHoaDon
    /// </summary>
    public class ChiTietHoaDon : BaseModel
    {
        public int STT { get; set; }
        public string MaHoaDonCT { get; set; }
        public string TenDichVu { get; set; }
        public int ChiSoCu { get; set; }
        public int ChiSoMoi { get; set; }
        public int SoLuong => ChiSoMoi - ChiSoCu;
        public decimal DonGia { get; set; }
        public decimal ThanhTien => SoLuong * DonGia;
        
        // Navigation properties
        public HoaDon HoaDon { get; set; }
    }
}
