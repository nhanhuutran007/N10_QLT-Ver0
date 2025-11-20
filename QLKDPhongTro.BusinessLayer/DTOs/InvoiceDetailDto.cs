using System;

namespace QLKDPhongTro.BusinessLayer.DTOs
{
    /// <summary>
    /// DTO cho chi tiết hóa đơn thanh toán
    /// </summary>
    public class InvoiceDetailDto
    {
        // Thông tin thanh toán
        public int MaThanhToan { get; set; }
        public string ThangNam { get; set; } = string.Empty;
        public DateTime? NgayThanhToan { get; set; }
        public string TrangThaiThanhToan { get; set; } = string.Empty;
        public decimal TongTien { get; set; }

        // Thông tin người thuê
        public string HoTen { get; set; } = string.Empty;
        public string CCCD { get; set; } = string.Empty;
        public string SoDienThoai { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        // Thông tin hợp đồng
        public int MaHopDong { get; set; }
        public decimal SoTienDaTra { get; set; }

        // Chi tiết các khoản phí
        public decimal TienThue { get; set; }
        public decimal TienDien { get; set; }
        public decimal TienNuoc { get; set; }
        public decimal TienInternet { get; set; }
        public decimal TienVeSinh { get; set; }
        public decimal TienGiuXe { get; set; }
        public decimal ChiPhiKhac { get; set; }

        // Điện nước chi tiết
        /// <summary>
        /// Chỉ số điện tháng trước (ChiSoDienCu).
        /// </summary>

        public decimal? DonGiaDien { get; set; }
        public decimal? DonGiaNuoc { get; set; }
        // Chỉ số điện tháng trước (để hiển thị và chỉnh đơn giá)
        public decimal? SoDienThangTruoc { get; set; }
        public decimal? SoDien { get; set; }
        public decimal? SoNuoc { get; set; }

        // Các giá trị tính toán (không lấy từ DB)
        public decimal TamTinhDien => (DonGiaDien ?? 0) * (SoDien ?? 0);
        public decimal TamTinhNuoc => (DonGiaNuoc ?? 0) * (SoNuoc ?? 0);
        public decimal TamTinhThue => TienThue;
        public decimal TamTinhInternet => TienInternet;
        public decimal TamTinhVeSinh => TienVeSinh;
        public decimal TamTinhGiuXe => TienGiuXe;
        public decimal TamTinhKhac => ChiPhiKhac;
        
        // TamTinhOverride: Nếu có giá trị, sử dụng nó; nếu không, tính từ các thành phần
        public decimal? TamTinhOverride { get; set; }
        public decimal TamTinh => TamTinhOverride ?? (TamTinhThue + TamTinhDien + TamTinhNuoc + TamTinhInternet + TamTinhVeSinh + TamTinhGiuXe + TamTinhKhac);
        public decimal TongTienTinhToan => Math.Max(0, TamTinh - SoTienDaTra);

        /// <summary>
        /// Chuỗi hiển thị tổng tiền: nếu đã trả thì hiển thị "Đã thanh toán",
        /// nếu chưa thì hiển thị số tiền sau khi khấu trừ tiền cọc.
        /// </summary>
        public string TongTienHienThi
        {
            get
            {
                if (string.Equals(TrangThaiThanhToan, "Đã trả", StringComparison.OrdinalIgnoreCase))
                {
                    return "Đã thanh toán";
                }

                return string.Format("{0:N0} đ", TamTinh);
            }
        }
    }
}

