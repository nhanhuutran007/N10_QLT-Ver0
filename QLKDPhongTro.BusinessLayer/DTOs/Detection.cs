using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLKDPhongTro.BusinessLayer.DTOs
{
    public class Detection
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public float Confidence { get; set; }
        public int ClassId { get; set; }
        public string ClassName { get; set; } = string.Empty;
    }
}
