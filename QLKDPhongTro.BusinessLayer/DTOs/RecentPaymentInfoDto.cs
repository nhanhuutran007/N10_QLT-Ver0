namespace QLKDPhongTro.BusinessLayer.DTOs
{
    public class RecentPaymentInfoDto
    {
        public int MaThanhToan { get; set; }
        public string HoTen { get; set; } = string.Empty;
        public string TrangThai { get; set; } = "Đã trả";
        public decimal TongTien { get; set; }
    }
}

