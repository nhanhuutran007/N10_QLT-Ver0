using System;

namespace QLKDPhongTro.BusinessLayer.DTOs
{
    public class DebtReportDTO
    {
        public int MaThanhToan { get; set; }
        public int MaHopDong { get; set; }
        public string TenPhong { get; set; }
        public string TenKhachHang { get; set; }
        public string SoDienThoai { get; set; }
        public string ThangNam { get; set; }
        public decimal TongTien { get; set; }
        public string TrangThaiThanhToan { get; set; }
        public int SoThangNo { get; set; }
        public DateTime? NgayThanhToan { get; set; }
        public string DiaChi { get; set; }
    }
}