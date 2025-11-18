using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLKDPhongTro.BusinessLayer.DTOs
{
    public class MeterReadingResult
    {
        // Đổi sang sử dụng enum ở Presentation để tránh trùng tên giữa hai layer
        public MeterType Type { get; set; }
        public decimal Value { get; set; }
        public float Confidence { get; set; }
        public string RawText { get; set; } = string.Empty;
        public string? ErrorMessage { get; set; }
        public bool IsValid => Confidence > 0.3f && Value > 0;

        // Dữ liệu hình ảnh trực quan hoá và danh sách detection (nếu cần hiển thị)
        public string? VisualizedImageBase64 { get; set; }
        public List<Detection>? Detections { get; set; }
    }

    public enum MeterType
    {
        Electricity,
        Water
    }
}
