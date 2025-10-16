using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLKDPhongTro.DataLayer.Models
{
    internal class Contract 
    {

        public int MaHopDong { get; set; }
        public int MaNguoiThue { get; set; }
        public int MaPhong { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }
        public decimal TienCoc { get; set; }
        public string FileHopDong { get; set; } // Đường dẫn file
        public string TrangThai { get; set; }

    }
}

