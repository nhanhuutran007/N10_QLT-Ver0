using System;

namespace QLKDPhongTro.BusinessLayer.DTOs
{
    /// <summary>
    /// DTO cho bản ghi tài chính hiển thị trong FinancialWindow
    /// </summary>
    public class FinancialRecordDto
    {
        public int MaThanhToan { get; set; }
        public int? MaHopDong { get; set; }
        public string TenPhong { get; set; } = string.Empty;
        public DateTime KyHan { get; set; }
        public string LoaiGiaoDich { get; set; } = string.Empty; // "Tiền Thuê", "Chi Phí", "Chỉ Số Điện/Nước"
        public decimal TongTien { get; set; }
        public string ChiTietGiaoDich { get; set; } = string.Empty;
        public string TrangThaiText { get; set; } = string.Empty;
        public string TrangThaiColor { get; set; } = "#7D8FA9";
        public double ProgressValue { get; set; }
        public string TrangThai { get; set; } = "BinhThuong"; // "BinhThuong", "HoanThanh", "CanhBao", "QuaHan"
        public string ThangNam { get; set; } = string.Empty;
        public string TrangThaiThanhToan { get; set; } = "Chưa trả";
        public DateTime? NgayThanhToan { get; set; }
    }
}

