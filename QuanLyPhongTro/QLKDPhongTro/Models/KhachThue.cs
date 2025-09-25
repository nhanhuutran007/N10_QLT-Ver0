using System;

namespace QLKDPhongTro.Models
{
    /// <summary>
    /// Model cho bảng tblKhachThue
    /// </summary>
    public class KhachThue : BaseModel
    {
        public string MaKhachThue { get; set; }
        public string MaPhongTro { get; set; }
        public string HoTen { get; set; }
        public string GioiTinh { get; set; }
        public DateTime NgaySinh { get; set; }
        public string QueQuan { get; set; }
        public string SoDienThoai { get; set; }
        public string CCCD { get; set; }
        
        // Navigation properties
        public PhongTro PhongTro { get; set; }
    }
}
