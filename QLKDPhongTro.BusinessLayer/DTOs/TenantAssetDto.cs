namespace QLKDPhongTro.BusinessLayer.DTOs
{
    public class TenantAssetDto
    {
        public int MaTaiSan { get; set; }
        public string LoaiTaiSan { get; set; } = string.Empty;
        public string MoTa { get; set; } = string.Empty;
        public decimal PhiPhuThu { get; set; }
    }
}

