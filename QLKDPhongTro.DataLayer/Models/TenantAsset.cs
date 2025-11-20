using System;

namespace QLKDPhongTro.DataLayer.Models
{
    /// <summary>
    /// Đại diện bản ghi bảng TaiSanNguoiThue
    /// </summary>
    public class TenantAsset
    {
        public int MaTaiSan { get; set; }
        public int MaNguoiThue { get; set; }
        public string LoaiTaiSan { get; set; } = string.Empty;
        public string MoTa { get; set; } = string.Empty;
        public decimal PhiPhuThu { get; set; }
        public DateTime? NgayTao { get; set; }
    }
}

