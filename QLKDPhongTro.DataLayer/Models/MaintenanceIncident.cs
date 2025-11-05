using System;

namespace QLKDPhongTro.DataLayer.Models
{
    public class MaintenanceIncident
    {
        public int MaSuCo { get; set; }
        public int MaPhong { get; set; }
        public string MoTaSuCo { get; set; } = string.Empty;
        public DateTime NgayBaoCao { get; set; }
        public string TrangThai { get; set; } = string.Empty;
        public decimal ChiPhi { get; set; }
    }
}
