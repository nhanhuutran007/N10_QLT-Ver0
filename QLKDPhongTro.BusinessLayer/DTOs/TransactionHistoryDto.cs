using System;

namespace QLKDPhongTro.BusinessLayer.DTOs
{
    public class TransactionHistoryDTO
    {
        public int MaThanhToan { get; set; }
        public string TenPhong { get; set; }
        public string TenKhachHang { get; set; }
        public string MoTa { get; set; }
        public decimal SoTien { get; set; }
        public DateTime ThoiGian { get; set; }
        public string LoaiGiaoDich { get; set; } // "Thuê", "Điện", "Nước", "Internet", "Khác"
        public string LoaiGiaoDichIcon { get; set; }
        public string TrangThai { get; set; }
        public string LoaiGiaoDichColor { get; set; }
        public string TrangThaiColor { get; set; }
    }
}