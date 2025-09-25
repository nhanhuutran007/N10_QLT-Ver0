using System;

namespace QLKDPhongTro.Models
{
    /// <summary>
    /// Model cho bảng tblPhongTro
    /// </summary>
    public class PhongTro : BaseModel
    {
        public string MaPhongTro { get; set; }
        public string ToaNha { get; set; }
        public int Tang { get; set; }
        public string SoPhong { get; set; }
        public decimal GiaPhong { get; set; }
        public string MoTa { get; set; }
        public string TrangThai { get; set; } = "Trống";
    }
}
