using QLKDPhongTro.BusinessLayer.DTOs;
using QLKDPhongTro.DataLayer.Models;
using QLKDPhongTro.DataLayer.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QLKDPhongTro.BusinessLayer.Controllers
{
    /// <summary>
    /// Controller xử lý logic nghiệp vụ cho Quản lý Tài chính
    /// </summary>
    public class FinancialController
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IContractRepository _contractRepository;
        private readonly IRentedRoomRepository _roomRepository;

        public FinancialController(
            IPaymentRepository paymentRepository,
            IContractRepository contractRepository,
            IRentedRoomRepository roomRepository)
        {
            _paymentRepository = paymentRepository;
            _contractRepository = contractRepository;
            _roomRepository = roomRepository;
        }

        /// <summary>
        /// Lấy tất cả thanh toán
        /// </summary>
        public async Task<List<PaymentDto>> GetAllPaymentsAsync()
        {
            var payments = await _paymentRepository.GetAllAsync();
            return payments.Select(p => new PaymentDto
            {
                MaThanhToan = p.MaThanhToan,
                MaHopDong = p.MaHopDong,
                ThangNam = p.ThangNam,
                TienThue = p.TienThue,
                TienDien = p.TienDien,
                TienNuoc = p.TienNuoc,
                TienInternet = p.TienInternet,
                TienVeSinh = p.TienVeSinh,
                TienGiuXe = p.TienGiuXe,
                ChiPhiKhac = p.ChiPhiKhac,
                TongTien = p.TongTien,
                TrangThaiThanhToan = p.TrangThaiThanhToan,
                NgayThanhToan = p.NgayThanhToan
            }).ToList();
        }

        /// <summary>
        /// Lấy thanh toán theo ID
        /// </summary>
        public async Task<PaymentDto?> GetPaymentByIdAsync(int maThanhToan)
        {
            var payment = await _paymentRepository.GetByIdAsync(maThanhToan);
            if (payment == null) return null;

            return new PaymentDto
            {
                MaThanhToan = payment.MaThanhToan,
                MaHopDong = payment.MaHopDong,
                ThangNam = payment.ThangNam,
                TienThue = payment.TienThue,
                TienDien = payment.TienDien,
                TienNuoc = payment.TienNuoc,
                TienInternet = payment.TienInternet,
                TienVeSinh = payment.TienVeSinh,
                TienGiuXe = payment.TienGiuXe,
                ChiPhiKhac = payment.ChiPhiKhac,
                TongTien = payment.TongTien,
                TrangThaiThanhToan = payment.TrangThaiThanhToan,
                NgayThanhToan = payment.NgayThanhToan
            };
        }

        /// <summary>
        /// Ghi nhận tiền thuê hàng tháng
        /// </summary>
        public async Task<ValidationResult> CreatePaymentAsync(CreatePaymentDto dto)
        {
            // Kiểm tra hợp đồng tồn tại
            var contract = await _contractRepository.GetByIdAsync(dto.MaHopDong);
            if (contract == null)
            {
                return new ValidationResult
                {
                    IsValid = false,
                    Message = "Hợp đồng không tồn tại"
                };
            }

            // Kiểm tra đã có thanh toán cho tháng này chưa
            if (await _paymentRepository.IsPaymentExistsAsync(dto.MaHopDong, dto.ThangNam))
            {
                return new ValidationResult
                {
                    IsValid = false,
                    Message = "Đã có thanh toán cho tháng này"
                };
            }

            var payment = new Payment
            {
                MaHopDong = dto.MaHopDong,
                ThangNam = dto.ThangNam,
                TienThue = dto.TienThue,
                TienDien = dto.TienDien,
                TienNuoc = dto.TienNuoc,
                TienInternet = dto.TienInternet,
                TienVeSinh = dto.TienVeSinh,
                TienGiuXe = dto.TienGiuXe,
                ChiPhiKhac = dto.ChiPhiKhac,
                TrangThaiThanhToan = "Chưa thanh toán",
                NgayThanhToan = null
            };

            var success = await _paymentRepository.CreateAsync(payment);
            return new ValidationResult
            {
                IsValid = success,
                Message = success ? "Ghi nhận tiền thuê thành công" : "Ghi nhận tiền thuê thất bại"
            };
        }

        /// <summary>
        /// Ghi nhận chi phí phát sinh
        /// </summary>
        public async Task<ValidationResult> CreateExpenseAsync(ExpenseDto dto)
        {
            var payment = await _paymentRepository.GetByIdAsync(dto.MaThanhToan);
            if (payment == null)
            {
                return new ValidationResult
                {
                    IsValid = false,
                    Message = "Thanh toán không tồn tại"
                };
            }

            // Cập nhật chi phí vào thanh toán
            switch (dto.LoaiChiPhi.ToLower())
            {
                case "điện":
                    payment.TienDien = dto.SoTien;
                    break;
                case "nước":
                    payment.TienNuoc = dto.SoTien;
                    break;
                case "internet":
                    payment.TienInternet = dto.SoTien;
                    break;
                case "vệ sinh":
                    payment.TienVeSinh = dto.SoTien;
                    break;
                case "giữ xe":
                    payment.TienGiuXe = dto.SoTien;
                    break;
                default:
                    payment.ChiPhiKhac = (payment.ChiPhiKhac ?? 0) + dto.SoTien;
                    break;
            }

            var success = await _paymentRepository.UpdateAsync(payment);
            return new ValidationResult
            {
                IsValid = success,
                Message = success ? "Ghi nhận chi phí thành công" : "Ghi nhận chi phí thất bại"
            };
        }

        /// <summary>
        /// Thanh toán tiền thuê
        /// </summary>
        public async Task<ValidationResult> PayRentAsync(PayRentDto dto)
        {
            var payment = await _paymentRepository.GetByIdAsync(dto.MaThanhToan);
            if (payment == null)
            {
                return new ValidationResult
                {
                    IsValid = false,
                    Message = "Thanh toán không tồn tại"
                };
            }

            if (payment.TrangThaiThanhToan == "Đã thanh toán")
            {
                return new ValidationResult
                {
                    IsValid = false,
                    Message = "Thanh toán đã được thực hiện trước đó"
                };
            }

            payment.TrangThaiThanhToan = "Đã thanh toán";
            payment.NgayThanhToan = dto.NgayThanhToan;

            var success = await _paymentRepository.UpdateAsync(payment);
            return new ValidationResult
            {
                IsValid = success,
                Message = success ? "Thanh toán thành công" : "Thanh toán thất bại"
            };
        }

        /// <summary>
        /// Cập nhật chi phí phát sinh
        /// </summary>
        public async Task<ValidationResult> UpdateExpenseAsync(UpdatePaymentDto dto)
        {
            var payment = await _paymentRepository.GetByIdAsync(dto.MaThanhToan);
            if (payment == null)
            {
                return new ValidationResult
                {
                    IsValid = false,
                    Message = "Thanh toán không tồn tại"
                };
            }

            if (dto.TienDien.HasValue) payment.TienDien = dto.TienDien.Value;
            if (dto.TienNuoc.HasValue) payment.TienNuoc = dto.TienNuoc.Value;
            if (dto.TienInternet.HasValue) payment.TienInternet = dto.TienInternet.Value;
            if (dto.TienVeSinh.HasValue) payment.TienVeSinh = dto.TienVeSinh.Value;
            if (dto.TienGiuXe.HasValue) payment.TienGiuXe = dto.TienGiuXe.Value;
            if (dto.ChiPhiKhac.HasValue) payment.ChiPhiKhac = dto.ChiPhiKhac.Value;

            var success = await _paymentRepository.UpdateAsync(payment);
            return new ValidationResult
            {
                IsValid = success,
                Message = success ? "Cập nhật chi phí thành công" : "Cập nhật chi phí thất bại"
            };
        }

        /// <summary>
        /// Lấy báo cáo công nợ
        /// </summary>
        public async Task<List<DebtReportDto>> GetDebtReportAsync(string? thangNam = null)
        {
            var debts = await _paymentRepository.GetDebtsAsync(thangNam);

            // Map từ model sang DTO
            var result = new List<DebtReportDto>();
            foreach (var debt in debts)
            {
                result.Add(new DebtReportDto
                {
                    MaThanhToan = debt.MaThanhToan,
                    MaHopDong = debt.MaHopDong ?? 0,
                    TenPhong = "P" + (debt.MaHopDong ?? 0).ToString(), // Placeholder
                    TenKhachHang = "Khách hàng", // Placeholder
                    SoDienThoai = "0123456789", // Placeholder
                    ThangNam = debt.ThangNam,
                    TongTien = debt.TongTien,
                    TrangThaiThanhToan = debt.TrangThaiThanhToan,
                    SoThangNo = CalculateMonthsOverdue(debt.ThangNam),
                    NgayThanhToan = debt.NgayThanhToan,
                    DiaChi = "Địa chỉ" // Placeholder
                });
            }

            return result;
        }

        /// <summary>
        /// Tổng hợp công nợ tự động
        /// </summary>
        public async Task<ValidationResult> AutoGenerateDebtsAsync()
        {
            try
            {
                var currentMonth = DateTime.Now.ToString("MM/yyyy");
                var activeContracts = await _contractRepository.GetActiveContractsAsync();

                int count = 0;
                foreach (var contract in activeContracts)
                {
                    var existingPayment = await _paymentRepository.GetPaymentByContractAndMonthAsync(
                        contract.MaHopDong, currentMonth);

                    if (existingPayment == null)
                    {
                        var room = await _roomRepository.GetByIdAsync(contract.MaPhong ?? 0);
                        var payment = new Payment
                        {
                            MaHopDong = contract.MaHopDong,
                            ThangNam = currentMonth,
                            TienThue = room?.GiaThue ?? 0,
                            TienDien = 0,
                            TienNuoc = 0,
                            TienInternet = 0,
                            TienVeSinh = 0,
                            TienGiuXe = 0,
                            ChiPhiKhac = 0,
                            TrangThaiThanhToan = "Chưa thanh toán",
                            NgayThanhToan = null
                        };

                        await _paymentRepository.CreateAsync(payment);
                        count++;
                    }
                }

                return new ValidationResult
                {
                    IsValid = true,
                    Message = $"Tổng hợp công nợ thành công cho {count} hợp đồng"
                };
            }
            catch (Exception ex)
            {
                return new ValidationResult
                {
                    IsValid = false,
                    Message = $"Lỗi khi tổng hợp công nợ: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Lấy thống kê tài chính tổng quan
        /// </summary>
        public async Task<FinancialStatsDto> GetFinancialStatsAsync(int? nam = null)
        {
            var currentYear = nam ?? DateTime.Now.Year;
            var stats = await _paymentRepository.GetFinancialStatsAsync(currentYear);

            return new FinancialStatsDto
            {
                TongThuNhap = stats.TongThuNhap,
                TongChiPhi = stats.TongChiPhi,
                LoiNhuan = stats.LoiNhuan,
                TongCongNo = stats.TongCongNo,
                SoPhongNo = stats.SoPhongNo,
                TangTruongThuNhap = stats.TangTruongThuNhap,
                TangTruongChiPhi = stats.TangTruongChiPhi,
                TyLeLoiNhuan = stats.TyLeLoiNhuan,
                ThongKeTheoThang = stats.ThongKeTheoThang.Select(t => new MonthlyStatsDto
                {
                    ThangNam = t.ThangNam,
                    ThuNhap = t.ThuNhap,
                    ChiPhi = t.ChiPhi,
                    LoiNhuan = t.LoiNhuan
                }).ToList(),
                PhanLoaiChiPhi = stats.PhanLoaiChiPhi.Select(p => new ExpenseCategoryDto
                {
                    TenLoai = p.TenLoai,
                    SoTien = p.SoTien,
                    TyLe = p.TyLe,
                    Color = p.Color
                }).ToList()
            };
        }

        /// <summary>
        /// Lấy lịch sử giao dịch
        /// </summary>
        public async Task<List<TransactionHistoryDto>> GetTransactionHistoryAsync(
            DateTime? tuNgay = null, DateTime? denNgay = null)
        {
            var transactions = await _paymentRepository.GetTransactionHistoryAsync(tuNgay, denNgay);

            // Map từ model sang DTO
            var result = new List<TransactionHistoryDto>();
            foreach (var transaction in transactions)
            {
                result.Add(new TransactionHistoryDto
                {
                    MaThanhToan = transaction.MaThanhToan,
                    TenPhong = "P" + (transaction.MaHopDong ?? 0).ToString(),
                    TenKhachHang = "Khách hàng",
                    MoTa = $"Thanh toán tháng {transaction.ThangNam}",
                    SoTien = transaction.TongTien,
                    ThoiGian = transaction.NgayThanhToan ?? DateTime.Now,
                    LoaiGiaoDich = "Thuê phòng",
                    LoaiGiaoDichIcon = "💰",
                    TrangThai = transaction.TrangThaiThanhToan == "Đã thanh toán" ? "Hoàn thành" : "Chưa thanh toán",
                    LoaiGiaoDichColor = "#10D096",
                    TrangThaiColor = transaction.TrangThaiThanhToan == "Đã thanh toán" ? "#10D096" : "#FF6B6B"
                });
            }

            return result;
        }

        /// <summary>
        /// Tìm kiếm thanh toán theo tháng
        /// </summary>
        public async Task<List<PaymentDto>> SearchPaymentsByMonthAsync(string thangNam)
        {
            var payments = await _paymentRepository.GetPaymentsByMonthAsync(thangNam);
            return payments.Select(p => new PaymentDto
            {
                MaThanhToan = p.MaThanhToan,
                MaHopDong = p.MaHopDong,
                ThangNam = p.ThangNam,
                TienThue = p.TienThue,
                TienDien = p.TienDien,
                TienNuoc = p.TienNuoc,
                TienInternet = p.TienInternet,
                TienVeSinh = p.TienVeSinh,
                TienGiuXe = p.TienGiuXe,
                ChiPhiKhac = p.ChiPhiKhac,
                TongTien = p.TongTien,
                TrangThaiThanhToan = p.TrangThaiThanhToan,
                NgayThanhToan = p.NgayThanhToan
            }).ToList();
        }

        #region Private Methods

        private int CalculateMonthsOverdue(string thangNam)
        {
            if (string.IsNullOrEmpty(thangNam)) return 0;

            var parts = thangNam.Split('/');
            if (parts.Length != 2) return 0;

            if (int.TryParse(parts[0], out int month) && int.TryParse(parts[1], out int year))
            {
                var paymentDate = new DateTime(year, month, 1);
                var currentDate = DateTime.Now;
                var monthsDifference = (currentDate.Year - paymentDate.Year) * 12 + currentDate.Month - paymentDate.Month;
                return Math.Max(0, monthsDifference);
            }

            return 0;
        }

        #endregion
    }
}