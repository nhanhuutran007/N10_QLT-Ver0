using System;

namespace QLKDPhongTro.Models
{
    /// <summary>
    /// Model cho bảng tblDienNuoc
    /// </summary>
    public class DienNuoc : BaseModel
    {
        public string MaDienNuoc { get; set; }
        public string MaPhongTro { get; set; }
        public string ThangNam { get; set; }
        public int SoDienCu { get; set; }
        public int SoDienMoi { get; set; }
        public int TieuThuDien => SoDienMoi - SoDienCu;
        public decimal TienDien { get; set; }
        public int SoNuocCu { get; set; }
        public int SoNuocMoi { get; set; }
        public int TieuThuNuoc => SoNuocMoi - SoNuocCu;
        public decimal TienNuoc { get; set; }
        public decimal DonGiaDien { get; set; }
        public decimal DonGiaNuoc { get; set; }
        
        // Navigation properties
        public PhongTro PhongTro { get; set; }
    }
}
