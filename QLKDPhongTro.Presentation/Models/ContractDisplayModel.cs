using System;

namespace QLKDPhongTro.Presentation.Models
{
    /// <summary>
    /// Model for displaying contract information in the UI
    /// </summary>
    public class ContractDisplayModel
    {
        public int MaHopDong { get; set; }
        public string MaHopDongDisplay { get; set; } = string.Empty;
        public string TenPhong { get; set; } = string.Empty;
        public string TenNguoiThue { get; set; } = string.Empty;
        public string NgayBatDau { get; set; } = string.Empty;
        public string NgayKetThuc { get; set; } = string.Empty;
        public string TienCoc { get; set; } = string.Empty;
        public string GiaThue { get; set; } = string.Empty;
        public string TrangThai { get; set; } = string.Empty;
        public string TrangThaiColor { get; set; } = "#2E7D32"; // Default green
        public string SoNgayConLai { get; set; } = string.Empty;
        public string GhiChu { get; set; } = string.Empty;
        public string FileHopDong { get; set; } = string.Empty;
        public string FileHopDongPath { get; set; } = string.Empty; // Full path to the contract file
        public bool HasFile { get; set; } = false; // Indicates if contract has a file
        public bool IsActive { get; set; } = false;
        
        // Raw values for calculations
        public DateTime NgayBatDauRaw { get; set; }
        public DateTime NgayKetThucRaw { get; set; }
    }
}
