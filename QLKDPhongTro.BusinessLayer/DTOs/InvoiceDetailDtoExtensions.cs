using System;
using System.Collections.Generic;
using System.Linq;

namespace QLKDPhongTro.BusinessLayer.DTOs
{
    /// <summary>
    /// Extension methods để chuyển đổi InvoiceDetailDto sang PlumeriaInvoiceDto
    /// </summary>
    public static class InvoiceDetailDtoExtensions
    {
        /// <summary>
        /// Chuyển đổi InvoiceDetailDto sang PlumeriaInvoiceDto
        /// </summary>
        public static PlumeriaInvoiceDto ToPlumeriaInvoiceDto(this InvoiceDetailDto invoice, 
            ContractDto? contract = null, 
            string email = "", 
            string tenPhong = "", 
            int soNguoiLuuTru = 1)
        {
            var result = new PlumeriaInvoiceDto
            {
                // Thông tin header
                ThangNam = invoice.ThangNam,
                NgayPhatHanh = DateTime.Now,
                SoThamChieu = $"INV-{invoice.MaThanhToan:D6}",

                // Thông tin khách hàng
                TenKhachHang = invoice.HoTen,
                DienThoai = invoice.SoDienThoai,
                Email = email,
                SoHopDong = contract != null ? $"HD-{contract.MaHopDong:D6}" : $"HD-{invoice.MaHopDong:D6}",
                NgayBatDauHopDong = contract?.NgayBatDau ?? DateTime.Now,

                // Thông tin phòng
                TenPhong = tenPhong,
                SoNguoiLuuTru = soNguoiLuuTru,

                // Chi tiết điện
                DonGiaDien = invoice.DonGiaDien ?? 0,
                NgayGhiDienCu = DateTime.Now.AddMonths(-1), // Cần lấy từ DB thực tế
                ChiSoDienCu = 0, // Cần lấy từ DB thực tế
                NgayGhiDienMoi = DateTime.Now,
                ChiSoDienMoi = (int)(invoice.SoDien ?? 0),
                MucTieuThuDien = (int)(invoice.SoDien ?? 0),
                ThanhTienDien = invoice.TamTinhDien,

                // Chi tiết nước
                DonGiaNuoc = invoice.DonGiaNuoc ?? 0,
                NgayGhiNuocCu = DateTime.Now.AddMonths(-1), // Cần lấy từ DB thực tế
                ChiSoNuocCu = 0, // Cần lấy từ DB thực tế
                NgayGhiNuocMoi = DateTime.Now,
                ChiSoNuocMoi = (int)(invoice.SoNuoc ?? 0),
                MucTieuThuNuoc = (int)(invoice.SoNuoc ?? 0),
                ThanhTienNuoc = invoice.TamTinhNuoc,

                // Thông tin ngân hàng (mặc định)
                TenTaiKhoanNH = "TRAN HUU NHAN",
                SoTaiKhoanNH = "0869918250",
                NganHang = "Vietcombank",
                ChiNhanh = "TP.HCM",

                // Thời hạn thanh toán (mặc định 30 ngày)
                ThoiHanThanhToan = DateTime.Now.AddDays(30)
            };

            // Tạo danh sách khoản mục
            var khoanMuc = new List<KhoanMucDto>();
            int stt = 1;

            if (invoice.TienThue > 0)
            {
                khoanMuc.Add(new KhoanMucDto
                {
                    STT = stt++,
                    NoiDung = "Tiền thuê",
                    DonVi = "tháng",
                    DonGia = invoice.TienThue,
                    SoLuong = 1,
                    ThanhTien = invoice.TienThue
                });
            }

            if (invoice.TamTinhDien > 0)
            {
                khoanMuc.Add(new KhoanMucDto
                {
                    STT = stt++,
                    NoiDung = "Điện tiêu dùng",
                    DonVi = "Kwh",
                    DonGia = invoice.DonGiaDien ?? 0,
                    SoLuong = invoice.SoDien ?? 0,
                    ThanhTien = invoice.TamTinhDien
                });
            }

            if (invoice.TamTinhNuoc > 0)
            {
                khoanMuc.Add(new KhoanMucDto
                {
                    STT = stt++,
                    NoiDung = "Nước sinh hoạt",
                    DonVi = "m³",
                    DonGia = invoice.DonGiaNuoc ?? 0,
                    SoLuong = invoice.SoNuoc ?? 0,
                    ThanhTien = invoice.TamTinhNuoc
                });
            }

            if (invoice.TienInternet > 0)
            {
                khoanMuc.Add(new KhoanMucDto
                {
                    STT = stt++,
                    NoiDung = "Internet",
                    DonVi = "tháng",
                    DonGia = invoice.TienInternet,
                    SoLuong = 1,
                    ThanhTien = invoice.TienInternet
                });
            }

            if (invoice.TienVeSinh > 0)
            {
                khoanMuc.Add(new KhoanMucDto
                {
                    STT = stt++,
                    NoiDung = "Vệ sinh",
                    DonVi = "tháng",
                    DonGia = invoice.TienVeSinh,
                    SoLuong = 1,
                    ThanhTien = invoice.TienVeSinh
                });
            }

            if (invoice.TienGiuXe > 0)
            {
                khoanMuc.Add(new KhoanMucDto
                {
                    STT = stt++,
                    NoiDung = "Giữ xe",
                    DonVi = "tháng",
                    DonGia = invoice.TienGiuXe,
                    SoLuong = 1,
                    ThanhTien = invoice.TienGiuXe
                });
            }

            if (invoice.ChiPhiKhac > 0)
            {
                khoanMuc.Add(new KhoanMucDto
                {
                    STT = stt++,
                    NoiDung = "Chi phí khác",
                    DonVi = "lần",
                    DonGia = invoice.ChiPhiKhac,
                    SoLuong = 1,
                    ThanhTien = invoice.ChiPhiKhac
                });
            }

            result.CacKhoanMuc = khoanMuc;
            result.TongCong = invoice.TongTienTinhToan;

            return result;
        }
    }
}

