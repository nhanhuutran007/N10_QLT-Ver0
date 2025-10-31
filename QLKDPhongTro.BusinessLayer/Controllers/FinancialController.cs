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
        private readonly ITenantRepository _tenantRepository;
        public FinancialController(
            IPaymentRepository paymentRepository,
            IContractRepository contractRepository,
            IRentedRoomRepository roomRepository,
            ITenantRepository tenantRepository)
        {
            _paymentRepository = paymentRepository;
            _contractRepository = contractRepository;
            _roomRepository = roomRepository;
            _tenantRepository = tenantRepository;
        }

        /// <summary>
        /// Tạo controller với các repository mặc định (DataLayer) – tránh để Presentation tầng thao tác trực tiếp repo
        /// </summary>
        public static FinancialController CreateDefault()
        {
            return new FinancialController(
                new PaymentRepository(),
                new ContractRepository(),
                new RentedRoomRepository(),
                new TenantRepository()
            );
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
                TienThue = p.TienThue ?? 0,
                TienDien = p.TienDien ?? 0,
                TienNuoc = p.TienNuoc ?? 0,
                TienInternet = p.TienInternet ?? 0,
                TienVeSinh = p.TienVeSinh ?? 0,
                TienGiuXe = p.TienGiuXe ?? 0,
                ChiPhiKhac = p.ChiPhiKhac ?? 0,
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
                TienThue = payment.TienThue ?? 0,
                TienDien = payment.TienDien ?? 0,
                TienNuoc = payment.TienNuoc ?? 0,
                TienInternet = payment.TienInternet ?? 0,
                TienVeSinh = payment.TienVeSinh ?? 0,
                TienGiuXe = payment.TienGiuXe ?? 0,
                ChiPhiKhac = payment.ChiPhiKhac ?? 0,
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
            // Kiểm tra hợp đồng tồn tại - Sử dụng GetByIdAsync mới
            var contract = await _contractRepository.GetByIdAsync(dto.MaHopDong);
            if (contract == null)
            {
                return new ValidationResult(false, "Hợp đồng không tồn tại");
            }

            // Kiểm tra đã có thanh toán cho tháng này chưa
            var existingPayments = await _paymentRepository.GetAllAsync();
            if (existingPayments.Any(p => p.MaHopDong == dto.MaHopDong && p.ThangNam == dto.ThangNam))
            {
                return new ValidationResult(false, "Đã có thanh toán cho tháng này");
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
                NgayThanhToan = null,
                TongTien = CalculateTotalAmount(dto)
            };

            var success = await _paymentRepository.CreateAsync(payment);
            return new ValidationResult(success,
                success ? "Ghi nhận tiền thuê thành công" : "Ghi nhận tiền thuê thất bại");
        }

        /// <summary>
        /// Ghi nhận chi phí phát sinh
        /// </summary>
        public async Task<ValidationResult> CreateExpenseAsync(ExpenseDto dto)
        {
            var payment = await _paymentRepository.GetByIdAsync(dto.MaThanhToan);
            if (payment == null)
            {
                return new ValidationResult(false, "Thanh toán không tồn tại");
            }

            // Tạo chi phí mới (cần có bảng Expenses riêng)
            // Tạm thời cập nhật vào payment
            switch (dto.LoaiChiPhi?.ToLower())
            {
                case "điện":
                    payment.TienDien = (payment.TienDien ?? 0) + dto.SoTien;
                    break;
                case "nước":
                    payment.TienNuoc = (payment.TienNuoc ?? 0) + dto.SoTien;
                    break;
                case "internet":
                    payment.TienInternet = (payment.TienInternet ?? 0) + dto.SoTien;
                    break;
                case "vệ sinh":
                    payment.TienVeSinh = (payment.TienVeSinh ?? 0) + dto.SoTien;
                    break;
                case "giữ xe":
                    payment.TienGiuXe = (payment.TienGiuXe ?? 0) + dto.SoTien;
                    break;
                default:
                    payment.ChiPhiKhac = (payment.ChiPhiKhac ?? 0) + dto.SoTien;
                    break;
            }

            // Cập nhật tổng tiền
            payment.TongTien = CalculateTotalAmount(payment);

            var success = await _paymentRepository.UpdateAsync(payment);
            return new ValidationResult(success,
                success ? "Ghi nhận chi phí thành công" : "Ghi nhận chi phí thất bại");
        }

        /// <summary>
        /// Thanh toán tiền thuê
        /// </summary>
        public async Task<ValidationResult> PayRentAsync(PayRentDto dto)
        {
            var payment = await _paymentRepository.GetByIdAsync(dto.MaThanhToan);
            if (payment == null)
            {
                return new ValidationResult(false, "Thanh toán không tồn tại");
            }

            if (payment.TrangThaiThanhToan == "Đã thanh toán")
            {
                return new ValidationResult(false, "Thanh toán đã được thực hiện trước đó");
            }

            payment.TrangThaiThanhToan = "Đã thanh toán";
            payment.NgayThanhToan = dto.NgayThanhToan;

            var success = await _paymentRepository.UpdateAsync(payment);
            return new ValidationResult(success,
                success ? "Thanh toán thành công" : "Thanh toán thất bại");
        }

        /// <summary>
        /// Cập nhật chi phí phát sinh
        /// </summary>
        public async Task<ValidationResult> UpdateExpenseAsync(UpdatePaymentDto dto)
        {
            var payment = await _paymentRepository.GetByIdAsync(dto.MaThanhToan);
            if (payment == null)
            {
                return new ValidationResult(false, "Thanh toán không tồn tại");
            }

            if (dto.TienDien.HasValue) payment.TienDien = dto.TienDien.Value;
            if (dto.TienNuoc.HasValue) payment.TienNuoc = dto.TienNuoc.Value;
            if (dto.TienInternet.HasValue) payment.TienInternet = dto.TienInternet.Value;
            if (dto.TienVeSinh.HasValue) payment.TienVeSinh = dto.TienVeSinh.Value;
            if (dto.TienGiuXe.HasValue) payment.TienGiuXe = dto.TienGiuXe.Value;
            if (dto.ChiPhiKhac.HasValue) payment.ChiPhiKhac = dto.ChiPhiKhac.Value;

            // Cập nhật tổng tiền
            payment.TongTien = CalculateTotalAmount(payment);

            var success = await _paymentRepository.UpdateAsync(payment);
            return new ValidationResult(success,
                success ? "Cập nhật chi phí thành công" : "Cập nhật chi phí thất bại");
        }

        /// <summary>
        /// Lấy báo cáo công nợ
        /// </summary>
        public async Task<List<DebtReportDto>> GetDebtReportAsync(string? thangNam = null)
        {
            var allPayments = await _paymentRepository.GetAllAsync();
            var debts = allPayments.Where(p => p.TrangThaiThanhToan == "Chưa trả" &&
                (string.IsNullOrEmpty(thangNam) || p.ThangNam == thangNam));

            // Map từ model sang DTO
            var result = new List<DebtReportDto>();
            foreach (var debt in debts)
            {
                // Lấy thông tin hợp đồng
                var contract = await _contractRepository.GetByIdAsync(debt.MaHopDong ?? 0);
                if (contract == null) continue;

                // Lấy thông tin phòng
                var room = await _roomRepository.GetByIdAsync(contract.MaPhong);

                // Lấy thông tin người thuê
                var tenant = await _tenantRepository.GetByIdAsync(contract.MaNguoiThue);

                result.Add(new DebtReportDto
                {
                    MaThanhToan = debt.MaThanhToan,
                    MaHopDong = debt.MaHopDong ?? 0,
                    TenPhong = room?.TenPhong ?? "Không xác định",
                    TenKhachHang = tenant?.HoTen ?? "Không xác định",
                    SoDienThoai = tenant?.SoDienThoai ?? "Không xác định",
                    ThangNam = debt.ThangNam,
                    TongTien = debt.TongTien,
                    TrangThaiThanhToan = debt.TrangThaiThanhToan,
                    SoThangNo = CalculateMonthsOverdue(debt.ThangNam),
                    NgayThanhToan = debt.NgayThanhToan,
                    DiaChi = tenant?.DiaChi ?? "Không xác định"
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
                    var existingPayments = await _paymentRepository.GetAllAsync();
                    var existingPayment = existingPayments.FirstOrDefault(p =>
                        p.MaHopDong == contract.MaHopDong && p.ThangNam == currentMonth);

                    if (existingPayment == null)
                    {
                        var room = await _roomRepository.GetByIdAsync(contract.MaPhong);
                        var payment = new Payment
                        {
                            MaHopDong = contract.MaHopDong,
                            ThangNam = currentMonth,
                            TienThue = room?.GiaCoBan ?? 0,
                            TienDien = 0,
                            TienNuoc = 0,
                            TienInternet = 0,
                            TienVeSinh = 0,
                            TienGiuXe = 0,
                            ChiPhiKhac = 0,
                            TrangThaiThanhToan = "Chưa thanh toán",
                            NgayThanhToan = null,
                            TongTien = room?.GiaCoBan ?? 0
                        };

                        await _paymentRepository.CreateAsync(payment);
                        count++;
                    }
                }

                return new ValidationResult(true, $"Tổng hợp công nợ thành công cho {count} hợp đồng");
            }
            catch (Exception ex)
            {
                return new ValidationResult(false, $"Lỗi khi tổng hợp công nợ: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy thống kê tài chính tổng quan
        /// </summary>
        public async Task<FinancialStatsDto> GetFinancialStatsAsync(int? nam = null)
        {
            var currentYear = nam ?? DateTime.Now.Year;
            var allPayments = await _paymentRepository.GetAllAsync();

            var yearlyPayments = allPayments.Where(p =>
                !string.IsNullOrEmpty(p.ThangNam) && p.ThangNam.EndsWith($"/{currentYear}"));

            var stats = new FinancialStatsDto
            {
                TongThuNhap = yearlyPayments.Where(p => p.TrangThaiThanhToan == "Đã thanh toán")
                                            .Sum(p => p.TongTien),
                TongChiPhi = yearlyPayments.Sum(p => (p.TienDien ?? 0) + (p.TienNuoc ?? 0) +
                                           (p.TienInternet ?? 0) + (p.TienVeSinh ?? 0) +
                                           (p.TienGiuXe ?? 0) + (p.ChiPhiKhac ?? 0)),
                TongCongNo = yearlyPayments.Where(p => p.TrangThaiThanhToan == "Chưa thanh toán")
                                          .Sum(p => p.TongTien),
                SoPhongNo = yearlyPayments.Where(p => p.TrangThaiThanhToan == "Chưa thanh toán")
                                         .Select(p => p.MaHopDong).Distinct().Count()
            };

            stats.LoiNhuan = stats.TongThuNhap - stats.TongChiPhi;
            stats.TyLeLoiNhuan = stats.TongThuNhap > 0 ? (stats.LoiNhuan / stats.TongThuNhap) * 100 : 0;

            // Thống kê theo tháng
            for (int month = 1; month <= 12; month++)
            {
                var monthStr = month.ToString().PadLeft(2, '0');
                var monthlyPayments = yearlyPayments.Where(p => p.ThangNam.StartsWith($"{monthStr}/"));

                var monthlyStat = new MonthlyStatsDto
                {
                    ThangNam = $"{monthStr}/{currentYear}",
                    ThuNhap = monthlyPayments.Where(p => p.TrangThaiThanhToan == "Đã trả")
                                            .Sum(p => p.TongTien),
                    ChiPhi = monthlyPayments.Sum(p => (p.TienDien ?? 0) + (p.TienNuoc ?? 0) +
                                             (p.TienInternet ?? 0) + (p.TienVeSinh ?? 0) +
                                             (p.TienGiuXe ?? 0) + (p.ChiPhiKhac ?? 0))
                };
                monthlyStat.LoiNhuan = monthlyStat.ThuNhap - monthlyStat.ChiPhi;

                stats.ThongKeTheoThang.Add(monthlyStat);
            }

            return stats;
        }

        /// <summary>
        /// Cập nhật trạng thái thanh toán (Đã trả/Chưa trả) và ngày thanh toán theo chuẩn DB
        /// </summary>
        public async Task<ValidationResult> UpdatePaymentStatusAsync(int maThanhToan, string trangThaiChuan)
        {
            var normalized = (trangThaiChuan ?? string.Empty).Trim();
            if (!string.Equals(normalized, "Đã trả", StringComparison.OrdinalIgnoreCase)
                && !string.Equals(normalized, "Chưa trả", StringComparison.OrdinalIgnoreCase))
            {
                return new ValidationResult(false, "Trạng thái không hợp lệ. Chỉ chấp nhận 'Đã trả' hoặc 'Chưa trả'.");
            }

            var payment = await _paymentRepository.GetByIdAsync(maThanhToan);
            if (payment == null)
            {
                return new ValidationResult(false, "Thanh toán không tồn tại");
            }

            var isPaid = string.Equals(normalized, "Đã trả", StringComparison.OrdinalIgnoreCase);
            payment.TrangThaiThanhToan = isPaid ? "Đã trả" : "Chưa trả";
            payment.NgayThanhToan = isPaid ? DateTime.Now.Date : null;

            var success = await _paymentRepository.UpdateAsync(payment);
            return new ValidationResult(success,
                success ? "Cập nhật trạng thái thanh toán thành công" : "Cập nhật trạng thái thanh toán thất bại",
                new PaymentDto
                {
                    MaThanhToan = payment.MaThanhToan,
                    MaHopDong = payment.MaHopDong ?? 0,
                    ThangNam = payment.ThangNam,
                    TienThue = payment.TienThue ?? 0,
                    TienDien = payment.TienDien ?? 0,
                    TienNuoc = payment.TienNuoc ?? 0,
                    TienInternet = payment.TienInternet ?? 0,
                    TienVeSinh = payment.TienVeSinh ?? 0,
                    TienGiuXe = payment.TienGiuXe ?? 0,
                    ChiPhiKhac = payment.ChiPhiKhac ?? 0,
                    TongTien = payment.TongTien,
                    TrangThaiThanhToan = payment.TrangThaiThanhToan,
                    NgayThanhToan = payment.NgayThanhToan
                });
        }

        /// <summary>
        /// Xóa thanh toán theo ID
        /// </summary>
        public async Task<ValidationResult> DeletePaymentAsync(int maThanhToan)
        {
            var success = await _paymentRepository.DeleteAsync(maThanhToan);
            return new ValidationResult(success,
                success ? "Xóa thanh toán thành công" : "Xóa thanh toán thất bại");
        }

        /// <summary>
        /// Lấy thông tin chi tiết hóa đơn thanh toán theo mã thanh toán
        /// </summary>
        public async Task<InvoiceDetailDto?> GetInvoiceDetailAsync(int maThanhToan)
        {
            var payment = await _paymentRepository.GetByIdAsync(maThanhToan);
            if (payment == null) return null;

            var contract = payment.MaHopDong.HasValue
                ? await _contractRepository.GetByIdAsync(payment.MaHopDong.Value)
                : null;

            var tenant = contract != null
                ? await _tenantRepository.GetByIdAsync(contract.MaNguoiThue)
                : null;

            return new InvoiceDetailDto
            {
                MaThanhToan = payment.MaThanhToan,
                ThangNam = payment.ThangNam,
                NgayThanhToan = payment.NgayThanhToan,
                TrangThaiThanhToan = payment.TrangThaiThanhToan ?? "Chưa trả",
                TongTien = payment.TongTien,
                MaHopDong = payment.MaHopDong ?? 0,
                HoTen = tenant?.HoTen ?? "Không xác định",
                CCCD = tenant?.CCCD ?? "Không xác định",
                SoDienThoai = tenant?.SoDienThoai ?? "Không xác định",
                TienThue = payment.TienThue ?? 0,
                TienDien = payment.TienDien ?? 0,
                TienNuoc = payment.TienNuoc ?? 0,
                TienInternet = payment.TienInternet ?? 0,
                TienVeSinh = payment.TienVeSinh ?? 0,
                TienGiuXe = payment.TienGiuXe ?? 0,
                ChiPhiKhac = payment.ChiPhiKhac ?? 0
            };
        }

        /// <summary>
        /// Lấy lịch sử giao dịch
        /// </summary>
        public async Task<List<TransactionHistoryDto>> GetTransactionHistoryAsync(
            DateTime? tuNgay = null, DateTime? denNgay = null)
        {
            var allPayments = await _paymentRepository.GetAllAsync();
            var transactions = allPayments.Where(p => p.TrangThaiThanhToan == "Đã thanh toán" &&
                p.NgayThanhToan.HasValue &&
                (tuNgay == null || p.NgayThanhToan >= tuNgay) &&
                (denNgay == null || p.NgayThanhToan <= denNgay));

            // Map từ model sang DTO
            var result = new List<TransactionHistoryDto>();
            foreach (var transaction in transactions)
            {
                var contract = await _contractRepository.GetByIdAsync(transaction.MaHopDong ?? 0);
                var tenant = await _tenantRepository.GetByIdAsync(contract.MaNguoiThue);
                result.Add(new TransactionHistoryDto
                {
                    MaThanhToan = transaction.MaThanhToan,
                    TenPhong = contract?.MaPhong.ToString() ?? "Không xác định",
                    TenKhachHang = tenant?.HoTen ?? "Không xác định",
                    MoTa = $"Thanh toán tháng {transaction.ThangNam}",
                    SoTien = transaction.TongTien,
                    ThoiGian = transaction.NgayThanhToan ?? DateTime.Now,
                    LoaiGiaoDich = "Thuê phòng",
                    LoaiGiaoDichIcon = "💰",
                    TrangThai = "Hoàn thành",
                    LoaiGiaoDichColor = "#10D096",
                    TrangThaiColor = "#10D096"
                });
            }

            return result;
        }
        // Thêm vào class FinancialController
        public async Task<List<ContractDto>> GetActiveContractsAsync()
        {
            var contracts = await _contractRepository.GetActiveContractsAsync();
            return contracts.Select(c => new ContractDto
            {
                MaHopDong = c.MaHopDong,
                MaNguoiThue = c.MaNguoiThue,
                MaPhong = c.MaPhong,
                NgayBatDau = c.NgayBatDau,
                NgayKetThuc = c.NgayKetThuc,
                TienCoc = c.TienCoc,
                FileHopDong = c.FileHopDong,
                TrangThai = c.TrangThai,
                TenNguoiThue = c.MaNguoiThue.ToString(), // Có thể lấy từ tenant repository
                TenPhong = c.MaPhong.ToString() // Có thể lấy từ room repository
            }).ToList();
        }

        #region Private Methods

        private static decimal CalculateTotalAmount(CreatePaymentDto dto)
        {
            return dto.TienThue + dto.TienDien + dto.TienNuoc + dto.TienInternet +
                   dto.TienVeSinh + dto.TienGiuXe + dto.ChiPhiKhac;
        }

        private static decimal CalculateTotalAmount(Payment payment)
        {
            return (payment.TienThue ?? 0) + (payment.TienDien ?? 0) + (payment.TienNuoc ?? 0) +
                   (payment.TienInternet ?? 0) + (payment.TienVeSinh ?? 0) +
                   (payment.TienGiuXe ?? 0) + (payment.ChiPhiKhac ?? 0);
        }

        private static int CalculateMonthsOverdue(string thangNam)
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

    /// <summary>
    /// Class kết quả validation
    /// </summary>
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public string Message { get; set; } = string.Empty;
        public object? Data { get; set; }

        public ValidationResult() { }

        public ValidationResult(bool isValid, string message)
        {
            IsValid = isValid;
            Message = message;
        }

        public ValidationResult(bool isValid, string message, object data)
        {
            IsValid = isValid;
            Message = message;
            Data = data;
        }
    }
}