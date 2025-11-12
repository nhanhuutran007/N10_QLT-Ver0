using QLKDPhongTro.BusinessLayer.DTOs;
using QLKDPhongTro.DataLayer.Models;
using QLKDPhongTro.DataLayer.Repositories;
using QLKDPhongTro.Presentation.Services;
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
        private readonly GoogleFormService _googleFormService;

        // Constants for electricity and water calculations
        private const decimal DON_GIA_DIEN = 3500; // 3.500 VND/kWh
        private const decimal DON_GIA_NUOC = 100000; // 100.000 VND/tháng

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
            _googleFormService = new GoogleFormService();
        }

        // ==== Lookups for UI ====
        public async Task<List<DataLayer.Models.Tenant>> GetAllTenantsAsync()
        {
            return await _tenantRepository.GetAllAsync();
        }

        public async Task<List<DataLayer.Models.RentedRoom>> GetAllRoomsAsync()
        {
            return await _roomRepository.GetAllAsync();
        }

        public async Task<List<DataLayer.Models.Contract>> GetActiveContractsAsync()
        {
            return await _contractRepository.GetActiveContractsAsync();
        }

        public async Task<List<DataLayer.Models.Contract>> GetActiveContractsByTenantAsync(int maNguoiThue)
        {
            return await _contractRepository.GetActiveContractsByTenantAsync(maNguoiThue);
        }

        public async Task<DataLayer.Models.Contract?> GetContractByIdAsync(int maHopDong)
        {
            return await _contractRepository.GetByIdAsync(maHopDong);
        }

        public async Task<DataLayer.Models.RentedRoom?> GetRoomByIdAsync(int maPhong)
        {
            return await _roomRepository.GetByIdAsync(maPhong);
        }

        /// <summary>
        /// Tạo controller với các repository mặc định
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
        /// Đọc dữ liệu từ Google Form và tạo công nợ tự động
        /// </summary>
        public async Task<ValidationResult> CreateDebtsFromGoogleFormAsync(string spreadsheetId, string range = "A:E")
        {
            try
            {
                var debtData = await _googleFormService.ReadDebtDataFromGoogleSheetAsync(spreadsheetId, range);
                var validDebts = debtData.Where(d => d.IsValid).ToList();
                var errors = debtData.Where(d => !d.IsValid).ToList();

                int successCount = 0;

                foreach (var debt in validDebts)
                {
                    try
                    {
                        // Tìm hợp đồng theo tên phòng
                        var contract = await FindContractByRoomNameAsync(debt.TenPhong);
                        if (contract == null)
                        {
                            errors.Add(new DebtCreationDto
                            {
                                ErrorMessage = $"Không tìm thấy hợp đồng cho phòng {debt.TenPhong}"
                            });
                            continue;
                        }

                        // Lấy chỉ số điện cũ từ payment gần nhất
                        var previousPayment = await GetLastPaymentByContractIdAsync(contract.MaHopDong);
                        var chiSoDienCu = previousPayment?.ChiSoDienMoi ?? 0;

                        // Tính toán tiền điện theo logic mới
                        var tienDien = CalculateElectricityCost(chiSoDienCu, debt.ChiSoDienMoi);

                        // Tạo payment mới
                        var payment = new Payment
                        {
                            MaHopDong = contract.MaHopDong,
                            ThangNam = debt.ThangNam,
                            ChiSoDienCu = chiSoDienCu,
                            ChiSoDienMoi = debt.ChiSoDienMoi,
                            TienDien = tienDien,
                            TienNuoc = DON_GIA_NUOC, // Nước cố định 100k
                            TienThue = contract.GiaThue,
                            TongTien = contract.GiaThue + tienDien + DON_GIA_NUOC,
                            TrangThaiThanhToan = "Chưa trả",
                            NgayTao = DateTime.Now,
                            DonGiaDien = DON_GIA_DIEN,
                            DonGiaNuoc = DON_GIA_NUOC,
                            SoDien = (decimal)debt.ChiSoDienMoi,
                            SoNuoc = 1,
                            GhiChu = $"Tạo tự động từ Google Form. Confidence: {debt.Confidence:P1}. {debt.GhiChu}"
                        };

                        var success = await _paymentRepository.CreateAsync(payment);
                        if (success)
                        {
                            successCount++;
                        }
                        else
                        {
                            errors.Add(new DebtCreationDto
                            {
                                ErrorMessage = $"Lỗi khi lưu công nợ cho phòng {debt.TenPhong}"
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        errors.Add(new DebtCreationDto
                        {
                            ErrorMessage = $"Lỗi xử lý công nợ {debt.TenPhong}: {ex.Message}"
                        });
                    }
                }

                var message = $"Đã tạo {successCount} công nợ thành công từ Google Form.";
                if (errors.Count > 0)
                {
                    message += $" {errors.Count} lỗi: " + string.Join("; ", errors.Select(e => e.ErrorMessage));
                }

                return new ValidationResult { IsValid = true, Message = message };
            }
            catch (Exception ex)
            {
                return new ValidationResult
                {
                    IsValid = false,
                    Message = $"Lỗi khi tạo công nợ từ Google Form: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Tính tiền điện theo công thức: (chỉ số mới - chỉ số cũ) * 3.500 VND
        /// </summary>
        public decimal CalculateElectricityCost(double chiSoCu, double chiSoMoi)
        {
            var mucTieuThu = Math.Max(0, chiSoMoi - chiSoCu);
            return (decimal)mucTieuThu * DON_GIA_DIEN;
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
                NgayThanhToan = p.NgayThanhToan,
                TenPhong = p.TenPhong
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
        /// Ghi nhận tiền thuê hàng tháng với logic tính điện nước mới
        /// </summary>
        public async Task<ValidationResult> CreatePaymentAsync(CreatePaymentDto dto)
        {
            // Kiểm tra hợp đồng tồn tại
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

            // Tìm số điện tháng trước từ thanh toán gần nhất
            var previousPayment = await GetLastPaymentByContractIdAsync(dto.MaHopDong);
            decimal? soDienThangTruoc = previousPayment?.ChiSoDienMoi;

            // === TÍNH TOÁN TIỀN ĐIỆN THEO LOGIC MỚI ===
            decimal tienDien = 0;
            if (dto.SoDien > 0 && soDienThangTruoc.HasValue)
            {
                decimal soDienTieuThu = dto.SoDien - soDienThangTruoc.Value;
                if (soDienTieuThu < 0)
                {
                    return new ValidationResult(false, "Số điện tháng này không thể nhỏ hơn số điện tháng trước");
                }

                tienDien = soDienTieuThu * DON_GIA_DIEN;
            }

            // === TIỀN NƯỚC CỐ ĐỊNH ===
            decimal tienNuoc = DON_GIA_NUOC;

            var payment = new Payment
            {
                MaHopDong = dto.MaHopDong,
                ThangNam = dto.ThangNam,
                TienThue = dto.TienThue,
                TienDien = tienDien,
                TienNuoc = tienNuoc,
                TienInternet = dto.TienInternet,
                TienVeSinh = dto.TienVeSinh,
                TienGiuXe = dto.TienGiuXe,
                ChiPhiKhac = dto.ChiPhiKhac,
                SoDien = dto.SoDien,
                ChiSoDienCu = soDienThangTruoc,
                ChiSoDienMoi = dto.SoDien,
                SoNuoc = 1, // Đánh dấu đã tính tiền nước
                DonGiaDien = DON_GIA_DIEN,
                DonGiaNuoc = DON_GIA_NUOC,
                TrangThaiThanhToan = "Chưa trả",
                NgayThanhToan = null,
                TongTien = CalculateTotalAmount(dto, tienDien, tienNuoc)
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

            if (payment.TrangThaiThanhToan == "Đã trả")
            {
                return new ValidationResult(false, "Thanh toán đã được thực hiện trước đó");
            }

            payment.TrangThaiThanhToan = "Đã trả";
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
        /// Tổng hợp công nợ tự động với logic tính điện nước mới
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

                        // Tìm số điện tháng trước từ thanh toán gần nhất
                        var previousPayment = await GetLastPaymentByContractIdAsync(contract.MaHopDong);
                        var chiSoDienCu = previousPayment?.ChiSoDienMoi ?? 0;

                        var payment = new Payment
                        {
                            MaHopDong = contract.MaHopDong,
                            ThangNam = currentMonth,
                            TienThue = contract.GiaThue,
                            TienDien = 0, // Sẽ tính khi có số điện
                            TienNuoc = DON_GIA_NUOC, // Nước cố định 100k
                            TienInternet = 0,
                            TienVeSinh = 0,
                            TienGiuXe = 0,
                            ChiPhiKhac = 0,
                            ChiSoDienCu = chiSoDienCu,
                            DonGiaDien = DON_GIA_DIEN,
                            DonGiaNuoc = DON_GIA_NUOC,
                            SoNuoc = 1,
                            TrangThaiThanhToan = "Chưa trả",
                            NgayThanhToan = null,
                            TongTien = contract.GiaThue + DON_GIA_NUOC // Tiền thuê + tiền nước
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
                TongThuNhap = yearlyPayments.Where(p => p.TrangThaiThanhToan == "Đã trả")
                                            .Sum(p => p.TongTien),
                TongChiPhi = yearlyPayments.Sum(p => (p.TienDien ?? 0) + (p.TienNuoc ?? 0) +
                                           (p.TienInternet ?? 0) + (p.TienVeSinh ?? 0) +
                                           (p.TienGiuXe ?? 0) + (p.ChiPhiKhac ?? 0)),
                TongCongNo = yearlyPayments.Where(p => p.TrangThaiThanhToan == "Chưa trả")
                                          .Sum(p => p.TongTien),
                SoPhongNo = yearlyPayments.Where(p => p.TrangThaiThanhToan == "Chưa trả")
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
        /// Cập nhật trạng thái thanh toán
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
                success ? "Cập nhật trạng thái thanh toán thành công" : "Cập nhật trạng thái thanh toán thất bại");
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
        /// Lấy thông tin chi tiết hóa đơn thanh toán
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
                ChiPhiKhac = payment.ChiPhiKhac ?? 0,
                DonGiaDien = payment.DonGiaDien ?? DON_GIA_DIEN,
                DonGiaNuoc = payment.DonGiaNuoc ?? DON_GIA_NUOC,
                SoDien = payment.SoDien,
                SoNuoc = payment.SoNuoc
            };
        }

        /// <summary>
        /// Cập nhật đơn giá/các khoản phí với logic tính điện nước mới
        /// </summary>
        public async Task<bool> UpdateInvoiceUnitPricesAsync(
            int maThanhToan,
            decimal? soDienThangTruoc,
            decimal? soDienThangNay,
            decimal? tienThue,
            decimal? tienInternet,
            decimal? tienVeSinh,
            decimal? tienGiuXe,
            decimal? chiPhiKhac,
            decimal khauTru)
        {
            var payment = await _paymentRepository.GetByIdAsync(maThanhToan);
            if (payment == null) return false;

            // === TÍNH TOÁN TIỀN ĐIỆN THEO LOGIC MỚI ===
            decimal tienDien = 0;
            if (soDienThangNay.HasValue && soDienThangTruoc.HasValue)
            {
                decimal soDienTieuThu = soDienThangNay.Value - soDienThangTruoc.Value;
                if (soDienTieuThu < 0)
                {
                    throw new ArgumentException("Số điện tháng này không thể nhỏ hơn số điện tháng trước");
                }

                tienDien = soDienTieuThu * DON_GIA_DIEN;
                payment.DonGiaDien = DON_GIA_DIEN;
                payment.SoDien = soDienThangNay;
                payment.ChiSoDienCu = soDienThangTruoc;
                payment.ChiSoDienMoi = soDienThangNay;
            }

            // === TIỀN NƯỚC CỐ ĐỊNH ===
            decimal tienNuoc = DON_GIA_NUOC;
            payment.DonGiaNuoc = DON_GIA_NUOC;
            payment.SoNuoc = 1;

            payment.TienThue = tienThue ?? payment.TienThue;
            payment.TienInternet = tienInternet ?? payment.TienInternet;
            payment.TienVeSinh = tienVeSinh ?? payment.TienVeSinh;
            payment.TienGiuXe = tienGiuXe ?? payment.TienGiuXe;
            payment.ChiPhiKhac = chiPhiKhac ?? payment.ChiPhiKhac;

            // Cập nhật tiền điện và nước đã tính toán
            payment.TienDien = tienDien;
            payment.TienNuoc = tienNuoc;

            var tamTinh = (payment.TienThue ?? 0)
                          + tienDien
                          + tienNuoc
                          + (payment.TienInternet ?? 0)
                          + (payment.TienVeSinh ?? 0)
                          + (payment.TienGiuXe ?? 0)
                          + (payment.ChiPhiKhac ?? 0);

            payment.TongTien = tamTinh - khauTru;

            return await _paymentRepository.UpdateAsync(payment);
        }

        /// <summary>
        /// Lấy lịch sử giao dịch
        /// </summary>
        public async Task<List<TransactionHistoryDto>> GetTransactionHistoryAsync(
            DateTime? tuNgay = null, DateTime? denNgay = null)
        {
            var allPayments = await _paymentRepository.GetAllAsync();
            var transactions = allPayments.Where(p => p.TrangThaiThanhToan == "Đã trả" &&
                p.NgayThanhToan.HasValue &&
                (tuNgay == null || p.NgayThanhToan >= tuNgay) &&
                (denNgay == null || p.NgayThanhToan <= denNgay));

            var result = new List<TransactionHistoryDto>();
            foreach (var transaction in transactions)
            {
                var contract = await _contractRepository.GetByIdAsync(transaction.MaHopDong ?? 0);
                if (contract == null) continue;

                var tenant = await _tenantRepository.GetByIdAsync(contract.MaNguoiThue);
                result.Add(new TransactionHistoryDto
                {
                    MaThanhToan = transaction.MaThanhToan,
                    TenPhong = contract.MaPhong.ToString(),
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

        /// <summary>
        /// Lấy danh sách hợp đồng active dạng DTO
        /// </summary>
        public async Task<List<ContractDto>> GetActiveContractDtosAsync()
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
                TenNguoiThue = c.MaNguoiThue.ToString(),
                TenPhong = c.MaPhong.ToString()
            }).ToList();
        }

        #region Private Methods

        private async Task<Contract?> FindContractByRoomNameAsync(string tenPhong)
        {
            var rooms = await _roomRepository.GetAllAsync();
            var room = rooms.FirstOrDefault(r =>
                r.TenPhong?.Equals(tenPhong, StringComparison.OrdinalIgnoreCase) == true);

            if (room == null) return null;

            var contracts = await _contractRepository.GetActiveContractsAsync();
            return contracts.FirstOrDefault(c => c.MaPhong == room.MaPhong);
        }

        private async Task<Payment?> GetLastPaymentByContractIdAsync(int maHopDong)
        {
            var payments = await _paymentRepository.GetAllAsync();
            return payments
                .Where(p => p.MaHopDong == maHopDong)
                .OrderByDescending(p => p.ThangNam)
                .FirstOrDefault();
        }

        // Overload method để tính tổng tiền với tiền điện và nước đã tính toán
        private static decimal CalculateTotalAmount(CreatePaymentDto dto, decimal tienDien, decimal tienNuoc)
        {
            return dto.TienThue + tienDien + tienNuoc + dto.TienInternet +
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
}