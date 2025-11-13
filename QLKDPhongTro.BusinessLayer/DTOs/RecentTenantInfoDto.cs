namespace QLKDPhongTro.BusinessLayer.DTOs
{
    public class RecentTenantInfoDto
    {
        public int MaNguoiThue { get; set; }
        public string HoTen { get; set; } = string.Empty;
        public decimal TienCoc { get; set; }
        public string TrangThai { get; set; } = string.Empty;
    }
}
