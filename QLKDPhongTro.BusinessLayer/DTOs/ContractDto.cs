// QLKDPhongTro.BusinessLayer/DTOs/HopDongDTO.cs
using System;

namespace QLKDPhongTro.BusinessLayer.DTOs
{
    public class ContractDto
    {
        public int MaHopDong { get; set; }
        public int MaNguoiThue { get; set; }
        public int MaPhong { get; set; }
        public DateTime NgayBatDau { get; set; } = DateTime.Now;
        public DateTime NgayKetThuc { get; set; } = DateTime.Now.AddYears(1);
        public decimal TienCoc { get; set; }
        public string FileHopDong { get; set; }
        public string TrangThai { get; set; }

        // Các thuộc tính bổ sung để hiển thị
        public string TenNguoiThue { get; set; }
        public string TenPhong { get; set; }
    }
}