using QLKDPhongTro.BusinessLayer.DTOs;
using QLKDPhongTro.DataLayer.Models;
using QLKDPhongTro.DataLayer.Repositories;
using QLKDPhongTro.BusinessLayer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

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

        // Constants for fixed costs
        private const decimal DON_GIA_DIEN = 3500;      // 3.500 VND/kWh
        private const decimal DON_GIA_NUOC = 100000;    // 100.000 VND/tháng (Cố định)
        private const decimal DON_GIA_INTERNET = 100000; // 100.000 VND/tháng (Cố định)
        private const decimal DON_GIA_VE_SINH = 60000;   // 60.000 VND/tháng (Cố định)
        private const decimal DON_GIA_GIU_XE = 120000;   // 120.000 VND/tháng (Cố định)

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
            var current = AuthController.CurrentUser;
            if (current != null && current.MaNha > 0)
            {
                return await _tenantRepository.GetAllByMaNhaAsync(current.MaNha);
            }

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

        public async Task<RecentTenantInfoDto?> GetMostRecentTenantInfoAsync()
        {
            var current = AuthController.CurrentUser;

            // Ưu tiên lấy khách theo nhà hiện tại và trạng thái "Đang ở"
            if (current != null && current.MaNha > 0)
            {
                var tenants = await _tenantRepository.GetAllByMaNhaAsync(current.MaNha);
                if (tenants == null || tenants.Count == 0)
                    return null;

                var activeTenants = tenants
                    .Where(t => string.Equals(t.TrangThai, "Đang ở", StringComparison.OrdinalIgnoreCase))
                    .OrderByDescending(t => t.NgayTao)
                    .ThenByDescending(t => t.MaKhachThue)
                    .ToList();

                var first = activeTenants.FirstOrDefault();
                if (first == null) return null;

                return new RecentTenantInfoDto
                {
                    MaNguoiThue = first.MaKhachThue,
                    HoTen = first.HoTen,
                    // Không bắt buộc phải có hợp đồng, nên TienCoc để 0 nếu không có thông tin
                    TienCoc = 0,
                    TrangThai = first.TrangThai ?? "Đang ở"
                };
            }

            // Fallback cũ nếu không có thông tin nhà hiện tại
            var result = await _contractRepository.GetMostRecentTenantWithDepositAsync();
            if (result == null) return null;
            return new RecentTenantInfoDto
            {
                MaNguoiThue = result.Value.MaNguoiThue,
                HoTen = result.Value.HoTen,
                TienCoc = result.Value.TienCoc,
                TrangThai = result.Value.TrangThai
            };
        }

        public async Task<List<RecentTenantInfoDto>> GetMostRecentTenantsInfoAsync(int count)
        {
            var current = AuthController.CurrentUser;

            if (current != null && current.MaNha > 0)
            {
                var tenants = await _tenantRepository.GetAllByMaNhaAsync(current.MaNha);
                if (tenants == null || tenants.Count == 0)
                    return new List<RecentTenantInfoDto>();

                var limit = Math.Max(1, Math.Min(10, count));

                var activeTenants = tenants
                    .Where(t => string.Equals(t.TrangThai, "Đang ở", StringComparison.OrdinalIgnoreCase))
                    .OrderByDescending(t => t.NgayTao)
                    .ThenByDescending(t => t.MaKhachThue)
                    .Take(limit)
                    .ToList();

                return activeTenants.Select(t => new RecentTenantInfoDto
                {
                    MaNguoiThue = t.MaKhachThue,
                    HoTen = t.HoTen,
                    TienCoc = 0,
                    TrangThai = t.TrangThai ?? "Đang ở"
                }).ToList();
            }

            var list = await _contractRepository.GetMostRecentTenantsWithDepositAsync(count);
            return list.Select(x => new RecentTenantInfoDto
            {
                MaNguoiThue = x.MaNguoiThue,
                HoTen = x.HoTen,
                TienCoc = x.TienCoc,
                TrangThai = x.TrangThai
            }).ToList();
        }

        public async Task<List<RecentPaymentInfoDto>> GetMostRecentPaymentsInfoAsync(int count)
        {
            var current = AuthController.CurrentUser;
            List<DataLayer.Models.Payment> payments;

            // Lấy thanh toán theo MaNha nếu có
            if (current != null && current.MaNha > 0)
            {
                payments = await _paymentRepository.GetAllByMaNhaAsync(current.MaNha);
            }
            else
            {
                payments = await _paymentRepository.GetAllAsync();
            }

            // Lọc chỉ lấy thanh toán "Đã trả" và sắp xếp theo ngày thanh toán giảm dần
            var paidPayments = payments
                .Where(p => string.Equals(p.TrangThaiThanhToan, "Đã trả", StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(p => p.NgayThanhToan ?? DateTime.MinValue)
                .ThenByDescending(p => p.MaThanhToan)
                .Take(count)
                .ToList();

            return paidPayments.Select(p => new RecentPaymentInfoDto
            {
                MaThanhToan = p.MaThanhToan,
                HoTen = !string.IsNullOrWhiteSpace(p.TenKhachHang) ? p.TenKhachHang : "Không xác định",
                TrangThai = "Đã trả",
                TongTien = p.TongTien
            }).ToList();
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

                // Lấy toàn bộ payments hiện có để tránh tạo trùng công nợ
                var allExistingPayments = await _paymentRepository.GetAllAsync();

                int successCount = 0;

                foreach (var debt in validDebts)
                {
                    try
                    {
                        var contract = await FindContractByRoomNameAsync(debt.TenPhong);
                        if (contract == null)
                        {
                            errors.Add(new DebtCreationDto
                            {
                                RoomName = debt.RoomName,
                                ErrorMessage = $"Không tìm thấy hợp đồng cho phòng {debt.TenPhong}"
                            });
                            continue;
                        }

                        // === CHẶN TRÙNG CÔNG NỢ: Nếu đã tồn tại payment cho cùng hợp đồng + tháng, thì bỏ qua ===
                        if (!string.IsNullOrWhiteSpace(debt.ThangNam))
                        {
                            var hasExistingForMonth = allExistingPayments.Any(p =>
                                p.MaHopDong == contract.MaHopDong &&
                                string.Equals(p.ThangNam, debt.ThangNam, StringComparison.OrdinalIgnoreCase));

                            if (hasExistingForMonth)
                            {
                                // Ghi chú lỗi mềm để báo trong summary nhưng không tạo thêm bản ghi
                                errors.Add(new DebtCreationDto
                                {
                                    RoomName = debt.RoomName,
                                    ErrorMessage = $"Đã tồn tại công nợ cho phòng {debt.TenPhong} tháng {debt.ThangNam}, bỏ qua dòng Google Form tương ứng."
                                });
                                continue;
                            }
                        }

                        var previousPayment = await GetLastPaymentByContractIdAsync(contract.MaHopDong);
                        var chiSoDienCu = previousPayment?.ChiSoDienMoi ?? 0;

                        var tienDien = CalculateElectricityCost((double)chiSoDienCu, debt.ChiSoDienMoi);

                        // === TÍNH TOÁN TIỀN NƯỚC THEO ĐẦU NGƯỜI ===
                        var roomTenants = await _tenantRepository.GetTenantsByRoomIdAsync(contract.MaPhong);
                        int soNguoiTrongPhong = roomTenants?.Count(t =>
                            string.Equals(t.TrangThaiNguoiThue, "Đang ở", StringComparison.OrdinalIgnoreCase)) ?? 1;
                        if (soNguoiTrongPhong < 1) soNguoiTrongPhong = 1;
                        decimal tienNuocDauNguoi = DON_GIA_NUOC;
                        decimal tienNuocTong = tienNuocDauNguoi * soNguoiTrongPhong;

                        // Tính tổng tiền với các khoản cố định (tiền nước đã nhân theo số người)
                        var tongTien = contract.GiaThue + tienDien + tienNuocTong + DON_GIA_INTERNET + DON_GIA_VE_SINH + DON_GIA_GIU_XE;

                        var payment = new Payment
                        {
                            MaHopDong = contract.MaHopDong,
                            ThangNam = debt.ThangNam,
                            ChiSoDienCu = chiSoDienCu,
                            ChiSoDienMoi = (decimal)debt.ChiSoDienMoi,
                            TienDien = tienDien,
                            TienNuoc = tienNuocTong,         // Tổng tiền nước = tiền nước/đầu người × số người
                            TienInternet = DON_GIA_INTERNET, // Cố định
                            TienVeSinh = DON_GIA_VE_SINH,    // Cố định
                            TienGiuXe = DON_GIA_GIU_XE,      // Cố định
                            ChiPhiKhac = 0,
                            TienThue = contract.GiaThue,
                            TongTien = tongTien,
                            TrangThaiThanhToan = "Chưa trả",
                            DonGiaDien = DON_GIA_DIEN,
                            DonGiaNuoc = tienNuocDauNguoi,   // Lưu đơn giá/đầu người
                            SoDien = (decimal)debt.ChiSoDienMoi,
                            SoNuoc = soNguoiTrongPhong,
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
                                RoomName = debt.RoomName,
                                ErrorMessage = $"Lỗi khi lưu công nợ cho phòng {debt.TenPhong}"
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        errors.Add(new DebtCreationDto
                        {
                            RoomName = debt.RoomName,
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

        public decimal CalculateElectricityCost(double chiSoCu, double chiSoMoi)
        {
            var mucTieuThu = Math.Max(0, chiSoMoi - chiSoCu);
            return (decimal)mucTieuThu * DON_GIA_DIEN;
        }

        public async Task<List<PaymentDto>> GetAllPaymentsAsync()
        {
            List<DataLayer.Models.Payment> payments;
            var current = AuthController.CurrentUser;
            if (current != null && current.MaNha > 0)
            {
                payments = await _paymentRepository.GetAllByMaNhaAsync(current.MaNha);
            }
            else
            {
                payments = await _paymentRepository.GetAllAsync();
            }
            return payments.Select(p => new PaymentDto
            {
                MaThanhToan = p.MaThanhToan,
                MaHopDong = p.MaHopDong,
                ThangNam = p.ThangNam,
                TienThue = p.TienThue ?? 0,
                TienDien = p.TienDien ?? 0,
                TienNuoc = p.TienNuoc ?? DON_GIA_NUOC,
                TienInternet = p.TienInternet ?? DON_GIA_INTERNET,
                TienVeSinh = p.TienVeSinh ?? DON_GIA_VE_SINH,
                TienGiuXe = p.TienGiuXe ?? DON_GIA_GIU_XE,
                ChiPhiKhac = p.ChiPhiKhac ?? 0,
                TongTien = p.TongTien,
                TrangThaiThanhToan = p.TrangThaiThanhToan,
                NgayThanhToan = p.NgayThanhToan,
                SoTienDaTra = p.SoTienDaTra,
                TenPhong = p.TenPhong,
                GhiChu = p.GhiChu
            }).ToList();
        }

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
                NgayThanhToan = payment.NgayThanhToan,
                SoTienDaTra = payment.SoTienDaTra
            };
        }

        /// <summary>
        /// Ghi nhận tiền thuê hàng tháng với logic tính điện nước và dịch vụ CỐ ĐỊNH
        /// Tiền nước sẽ được nhân với số người trong phòng (tiền nước/đầu người × số người)
        /// </summary>
        public async Task<ValidationResult> CreatePaymentAsync(CreatePaymentDto dto)
        {
            var contract = await _contractRepository.GetByIdAsync(dto.MaHopDong);
            if (contract == null)
            {
                return new ValidationResult(false, "Hợp đồng không tồn tại");
            }

            var existingPayments = await _paymentRepository.GetAllAsync();
            if (existingPayments.Any(p => p.MaHopDong == dto.MaHopDong && p.ThangNam == dto.ThangNam))
            {
                return new ValidationResult(false, "Đã có thanh toán cho tháng này");
            }

            var previousPayment = await GetLastPaymentByContractIdAsync(dto.MaHopDong);
            decimal? soDienThangTruoc = previousPayment?.ChiSoDienMoi;

            // === TÍNH TOÁN TIỀN ĐIỆN ===
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

            // === TÍNH TOÁN TIỀN NƯỚC: NHÂN TIỀN NƯỚC/ĐẦU NGƯỜI VỚI SỐ NGƯỜI TRONG PHÒNG ===
            // Lấy số người đang ở trong phòng
            var roomTenants = await _tenantRepository.GetTenantsByRoomIdAsync(contract.MaPhong);
            int soNguoiTrongPhong = roomTenants?.Count(t =>
                string.Equals(t.TrangThaiNguoiThue, "Đang ở", StringComparison.OrdinalIgnoreCase)) ?? 1;

            // Đảm bảo ít nhất 1 người để tránh lỗi
            if (soNguoiTrongPhong < 1) soNguoiTrongPhong = 1;

            // Tiền nước từ DTO là tiền nước/đầu người, nhân với số người để ra tổng tiền nước
            // Nếu dto.TienNuoc = 0, sử dụng giá mặc định
            decimal tienNuocDauNguoi = dto.TienNuoc > 0 ? dto.TienNuoc : DON_GIA_NUOC;
            decimal tienNuocTong = tienNuocDauNguoi * soNguoiTrongPhong;

            // === ÁP DỤNG CÁC KHOẢN PHÍ CỐ ĐỊNH ===
            decimal tienInternet = DON_GIA_INTERNET;
            decimal tienVeSinh = DON_GIA_VE_SINH;
            decimal tienGiuXe = DON_GIA_GIU_XE;

            var payment = new Payment
            {
                MaHopDong = dto.MaHopDong,
                ThangNam = dto.ThangNam,
                TienThue = dto.TienThue,
                TienDien = tienDien,
                TienNuoc = tienNuocTong, // Tổng tiền nước = tiền nước/đầu người × số người
                TienInternet = tienInternet, // Sử dụng giá cố định
                TienVeSinh = tienVeSinh,     // Sử dụng giá cố định
                TienGiuXe = tienGiuXe,       // Sử dụng giá cố định
                ChiPhiKhac = dto.ChiPhiKhac,
                SoDien = dto.SoDien,
                ChiSoDienCu = soDienThangTruoc,
                ChiSoDienMoi = dto.SoDien,
                SoNuoc = 1,
                DonGiaDien = DON_GIA_DIEN,
                DonGiaNuoc = tienNuocDauNguoi, // Lưu tiền nước/đầu người vào DonGiaNuoc để tham khảo
                TrangThaiThanhToan = "Chưa trả",
                NgayThanhToan = null
            };

            // Tính tổng tiền dựa trên đối tượng payment đã gán giá trị cố định
            payment.TongTien = CalculateTotalAmount(payment);

            var success = await _paymentRepository.CreateAsync(payment);
            return new ValidationResult(success,
                success ? "Ghi nhận tiền thuê thành công" : "Ghi nhận tiền thuê thất bại");
        }

        public async Task<ValidationResult> CreateExpenseAsync(ExpenseDto dto)
        {
            var payment = await _paymentRepository.GetByIdAsync(dto.MaThanhToan);
            if (payment == null)
            {
                return new ValidationResult(false, "Thanh toán không tồn tại");
            }

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

            payment.TongTien = CalculateTotalAmount(payment);

            var success = await _paymentRepository.UpdateAsync(payment);
            return new ValidationResult(success,
                success ? "Ghi nhận chi phí thành công" : "Ghi nhận chi phí thất bại");
        }

        public async Task<ValidationResult> PayRentAsync(PayRentDto dto)
        {
            var payment = await _paymentRepository.GetByIdAsync(dto.MaThanhToan);
            if (payment == null)
            {
                return new ValidationResult(false, "Thanh toán không tồn tại");
            }

            if (string.Equals(payment.TrangThaiThanhToan, "Đã trả", StringComparison.OrdinalIgnoreCase))
            {
                return new ValidationResult(false, "Thanh toán đã được thực hiện trước đó");
            }

            payment.TrangThaiThanhToan = "Đã trả";
            payment.NgayThanhToan = dto.NgayThanhToan;

            var success = await _paymentRepository.UpdateAsync(payment);
            return new ValidationResult(success,
                success ? "Thanh toán thành công" : "Thanh toán thất bại");
        }

        public async Task<ValidationResult> UpdatePaymentStatusAsync(int maThanhToan, string trangThaiChuan, decimal? soTienDaTra = null)
        {
            var normalized = (trangThaiChuan ?? string.Empty).Trim();
            var validStatuses = new[] { "Đã trả", "Chưa trả", "Trả một phần" };

            if (!validStatuses.Any(status => status.Equals(normalized, StringComparison.OrdinalIgnoreCase)))
            {
                return new ValidationResult(false, "Trạng thái không hợp lệ. Vui lòng chọn 'Chưa trả', 'Trả một phần' hoặc 'Đã trả'.");
            }

            var payment = await _paymentRepository.GetByIdAsync(maThanhToan);
            if (payment == null)
            {
                return new ValidationResult(false, "Thanh toán không tồn tại");
            }

            var totalDue = CalculateTotalAmount(payment);

            // Handle partial payment
            if (normalized.Equals("Trả một phần", StringComparison.OrdinalIgnoreCase))
            {
                if (!soTienDaTra.HasValue || soTienDaTra <= 0)
                {
                    return new ValidationResult(false, "Vui lòng nhập số tiền đã trả hợp lệ (> 0).");
                }

                if (soTienDaTra >= totalDue)
                {
                    normalized = "Đã trả";
                }
                else
                {
                    payment.TrangThaiThanhToan = "Trả một phần";
                    payment.SoTienDaTra = soTienDaTra;
                    payment.NgayThanhToan = DateTime.Now;
                    payment.TongTien = totalDue;

                    var successPartial = await _paymentRepository.UpdateAsync(payment);
                    return new ValidationResult(successPartial,
                        successPartial ? "Đã cập nhật trạng thái 'Trả một phần'" : "Cập nhật trạng thái thất bại");
                }
            }

            var isPaid = normalized.Equals("Đã trả", StringComparison.OrdinalIgnoreCase);
            payment.TrangThaiThanhToan = isPaid ? "Đã trả" : "Chưa trả";
            payment.TongTien = totalDue;
            payment.NgayThanhToan = isPaid ? DateTime.Now.Date : null;
            payment.SoTienDaTra = isPaid ? totalDue : 0;

            var success = await _paymentRepository.UpdateAsync(payment);

            return new ValidationResult(success,
                success ? "Cập nhật trạng thái thanh toán thành công" : "Cập nhật trạng thái thanh toán thất bại");
        }

        public async Task<ValidationResult> DeletePaymentAsync(int maThanhToan)
        {
            var success = await _paymentRepository.DeleteAsync(maThanhToan);
            return new ValidationResult(success,
                success ? "Xóa thanh toán thành công" : "Xóa thanh toán thất bại");
        }

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

            var dto = new InvoiceDetailDto
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
                Email = string.IsNullOrWhiteSpace(tenant?.Email) ? null : tenant!.Email,
                TienThue = payment.TienThue ?? 0,
                TienDien = payment.TienDien ?? 0,
                TienNuoc = payment.TienNuoc ?? 0,
                TienInternet = payment.TienInternet ?? DON_GIA_INTERNET,
                TienVeSinh = payment.TienVeSinh ?? DON_GIA_VE_SINH,
                TienGiuXe = payment.TienGiuXe ?? DON_GIA_GIU_XE,
                ChiPhiKhac = payment.ChiPhiKhac ?? 0,
                DonGiaDien = payment.DonGiaDien ?? DON_GIA_DIEN,
                DonGiaNuoc = payment.DonGiaNuoc ?? DON_GIA_NUOC,
                SoDienThangTruoc = payment.ChiSoDienCu,
                SoDien = payment.SoDien,
                SoNuoc = payment.SoNuoc,
                // Sử dụng TongTien từ payment làm TamTinh để đảm bảo tính nhất quán
                // Điều này đảm bảo TamTinh khớp với giá trị đã được tính và lưu trong database
                TamTinhOverride = payment.TongTien
            };

            dto.SoTienDaTra = payment.SoTienDaTra ?? 0;

            return dto;
        }

        public async Task<List<DebtReportDto>> GetDebtReportAsync(string? thangNam = null)
        {
            var current = AuthController.CurrentUser;
            var allPayments = (current != null && current.MaNha > 0)
                ? await _paymentRepository.GetAllByMaNhaAsync(current.MaNha)
                : await _paymentRepository.GetAllAsync();

            // Tính số tiền còn lại (ConLai) = TongTien - SoTienDaTra; chỉ lấy những khoản còn nợ > 0
            var debts = allPayments.Where(p =>
                (string.IsNullOrEmpty(thangNam) || p.ThangNam == thangNam) &&
                (p.TongTien - (p.SoTienDaTra ?? 0m)) > 0m);

            var result = new List<DebtReportDto>();
            foreach (var debt in debts)
            {
                var contract = await _contractRepository.GetByIdAsync(debt.MaHopDong ?? 0);
                if (contract == null) continue;

                var room = await _roomRepository.GetByIdAsync(contract.MaPhong);
                var tenant = await _tenantRepository.GetByIdAsync(contract.MaNguoiThue);

                var conLai = debt.TongTien - (debt.SoTienDaTra ?? 0m);

                result.Add(new DebtReportDto
                {
                    MaThanhToan = debt.MaThanhToan,
                    MaHopDong = debt.MaHopDong ?? 0,
                    TenPhong = room?.TenPhong ?? "Không xác định",
                    TenKhachHang = tenant?.HoTen ?? "Không xác định",
                    SoDienThoai = tenant?.SoDienThoai ?? "Không xác định",
                    ThangNam = debt.ThangNam,
                    TongTien = conLai,
                    TrangThaiThanhToan = debt.TrangThaiThanhToan,
                    SoThangNo = CalculateMonthsOverdue(debt.ThangNam),
                    NgayThanhToan = debt.NgayThanhToan,
                    DiaChi = tenant?.DiaChi ?? "Không xác định",
                    // ĐÃ SỬA: Sử dụng trực tiếp GhiChu từ payment và gán ComparisonInfo = null để tránh lỗi biên dịch
                    GhiChu = debt.GhiChu,
                    ComparisonInfo = null
                });
            }

            return result;
        }

        public async Task<ValidationResult> AutoGenerateDebtsAsync()
        {
            try
            {
                var currentMonth = DateTime.Now.ToString("MM/yyyy");
                var current = AuthController.CurrentUser;
                IEnumerable<DataLayer.Models.Contract> activeContracts;
                if (current != null && current.MaNha > 0)
                {
                    var contractsByHouse = await _contractRepository.GetAllByMaNhaAsync(current.MaNha);
                    activeContracts = contractsByHouse.Where(c => string.Equals(c.TrangThai, "Hiệu lực", StringComparison.OrdinalIgnoreCase));
                }
                else
                {
                    activeContracts = await _contractRepository.GetActiveContractsAsync();
                }

                int count = 0;
                foreach (var contract in activeContracts)
                {
                    var existingPayments = (current != null && current.MaNha > 0)
                        ? await _paymentRepository.GetAllByMaNhaAsync(current.MaNha)
                        : await _paymentRepository.GetAllAsync();
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
                            TienNuoc = DON_GIA_NUOC,       // Cố định
                            TienInternet = DON_GIA_INTERNET, // Cố định
                            TienVeSinh = DON_GIA_VE_SINH,    // Cố định
                            TienGiuXe = DON_GIA_GIU_XE,      // Cố định
                            ChiPhiKhac = 0,
                            TrangThaiThanhToan = "Chưa trả",
                            NgayThanhToan = null
                        };
                        payment.TongTien = CalculateTotalAmount(payment);

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

        public async Task<FinancialStatsDto> GetFinancialStatsAsync(int? nam = null)
        {
            var currentYear = nam ?? DateTime.Now.Year;
            var current = AuthController.CurrentUser;
            var allPayments = (current != null && current.MaNha > 0)
                ? await _paymentRepository.GetAllByMaNhaAsync(current.MaNha)
                : await _paymentRepository.GetAllAsync();

            IEnumerable<DataLayer.Models.Payment> scopePayments = allPayments;
            if (nam.HasValue)
            {
                scopePayments = allPayments.Where(p =>
                    !string.IsNullOrEmpty(p.ThangNam) && p.ThangNam.EndsWith($"/{currentYear}"));
            }

            var stats = new FinancialStatsDto();

            stats.TongThuNhap = scopePayments
                .Where(p => string.Equals(p.TrangThaiThanhToan, "Đã trả", StringComparison.OrdinalIgnoreCase))
                .Sum(p => p.TongTien);

            stats.TongChiPhi = scopePayments.Sum(p => (p.TienDien ?? 0) + (p.TienNuoc ?? 0) +
                                                      (p.TienInternet ?? 0) + (p.TienVeSinh ?? 0) +
                                                      (p.TienGiuXe ?? 0) + (p.ChiPhiKhac ?? 0));

            stats.TongCongNo = scopePayments
                .Where(p => string.Equals(p.TrangThaiThanhToan, "Chưa trả", StringComparison.OrdinalIgnoreCase))
                .Sum(p => p.TongTien);

            stats.SoPhongNo = scopePayments
                .Where(p => string.Equals(p.TrangThaiThanhToan, "Chưa trả", StringComparison.OrdinalIgnoreCase))
                .Select(p => p.MaHopDong)
                .Distinct()
                .Count();

            List<DataLayer.Models.Tenant> tenantsScope;
            if (current != null && current.MaNha > 0)
            {
                tenantsScope = await _tenantRepository.GetAllByMaNhaAsync(current.MaNha);
            }
            else
            {
                tenantsScope = await _tenantRepository.GetAllAsync();
            }

            var activeTenantCount = tenantsScope
                .Count(t => string.Equals(t.TrangThai, "Đang ở", StringComparison.OrdinalIgnoreCase));
            stats.SoKhachDangThue = activeTenantCount;

            var roomsAll = (current != null && current.MaNha > 0)
                ? await _roomRepository.GetAllByMaNhaAsync(current.MaNha)
                : await _roomRepository.GetAllAsync();
            stats.SoPhongDangThue = roomsAll
                .Count(r => string.Equals(r.TrangThai, "Đang thuê", StringComparison.OrdinalIgnoreCase));

            stats.LoiNhuan = stats.TongThuNhap - stats.TongChiPhi;
            stats.TyLeLoiNhuan = stats.TongThuNhap > 0 ? (stats.LoiNhuan / stats.TongThuNhap) * 100 : 0;

            if (nam.HasValue)
            {
                for (int month = 1; month <= 12; month++)
                {
                    var monthStr = month.ToString().PadLeft(2, '0');
                    var monthlyPayments = scopePayments.Where(p => p.ThangNam != null && p.ThangNam.StartsWith($"{monthStr}/"));

                    var monthlyStat = new MonthlyStatsDto
                    {
                        ThangNam = $"{monthStr}/{currentYear}",
                        ThuNhap = monthlyPayments
                            .Where(p => string.Equals(p.TrangThaiThanhToan, "Đã trả", StringComparison.OrdinalIgnoreCase))
                            .Sum(p => p.TongTien),
                        ChiPhi = monthlyPayments.Sum(p => (p.TienDien ?? 0) + (p.TienNuoc ?? 0) +
                                                     (p.TienInternet ?? 0) + (p.TienVeSinh ?? 0) +
                                                     (p.TienGiuXe ?? 0) + (p.ChiPhiKhac ?? 0))
                    };
                    monthlyStat.LoiNhuan = monthlyStat.ThuNhap - monthlyStat.ChiPhi;

                    stats.ThongKeTheoThang.Add(monthlyStat);
                }
            }

            return stats;
        }

        public async Task<bool> UpdateInvoiceUnitPricesAsync(
            int maThanhToan,
            decimal? soDienThangTruoc,
            decimal? soDienThangNay,
            decimal? tienThue,
            decimal? tienNuoc,
            decimal? tienInternet,
            decimal? tienVeSinh,
            decimal? tienGiuXe,
            decimal? chiPhiKhac)
        {
            var payment = await _paymentRepository.GetByIdAsync(maThanhToan);
            if (payment == null) return false;

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

            // Cập nhật tiền nước từ input (nếu có), nếu không thì giữ nguyên giá trị hiện tại
            if (tienNuoc.HasValue && tienNuoc.Value > 0)
            {
                // Tính lại đơn giá nước/đầu người dựa trên số người trong phòng
                var contract = payment.MaHopDong.HasValue
                    ? await _contractRepository.GetByIdAsync(payment.MaHopDong.Value)
                    : null;
                if (contract != null)
                {
                    var roomTenants = await _tenantRepository.GetTenantsByRoomIdAsync(contract.MaPhong);
                    int soNguoiTrongPhong = roomTenants?.Count(t =>
                        string.Equals(t.TrangThaiNguoiThue, "Đang ở", StringComparison.OrdinalIgnoreCase)) ?? 1;
                    if (soNguoiTrongPhong < 1) soNguoiTrongPhong = 1;
                    
                    decimal tienNuocDauNguoi = tienNuoc.Value / soNguoiTrongPhong;
                    payment.TienNuoc = tienNuoc.Value; // Tổng tiền nước
                    payment.DonGiaNuoc = tienNuocDauNguoi; // Đơn giá/đầu người
                    payment.SoNuoc = soNguoiTrongPhong;
                }
                else
                {
                    // Fallback nếu không tìm thấy contract
                    payment.TienNuoc = tienNuoc.Value;
                    payment.DonGiaNuoc = tienNuoc.Value;
                    payment.SoNuoc = 1;
                }
            }
            // Nếu không có input, giữ nguyên giá trị hiện tại (không cập nhật)

            payment.TienThue = tienThue ?? payment.TienThue;
            payment.TienInternet = tienInternet ?? payment.TienInternet;
            payment.TienVeSinh = tienVeSinh ?? payment.TienVeSinh;
            payment.TienGiuXe = tienGiuXe ?? payment.TienGiuXe;
            payment.ChiPhiKhac = chiPhiKhac ?? payment.ChiPhiKhac;

            payment.TienDien = tienDien;

            payment.TongTien = CalculateTotalAmount(payment);

            return await _paymentRepository.UpdateAsync(payment);
        }

        /// <summary>
        /// Cập nhật đơn giá điện/nước cho thanh toán hiện tại và tất cả các thanh toán "Chưa trả" trong hệ thống
        /// </summary>
        public async Task<bool> UpdateUnitPricesForCurrentAndUnpaidPaymentsAsync(
            int maThanhToan,
            decimal? donGiaDien,
            decimal? donGiaNuoc)
        {
            // Lấy thanh toán hiện tại để đảm bảo nó tồn tại
            var currentPayment = await _paymentRepository.GetByIdAsync(maThanhToan);
            if (currentPayment == null) 
            {
                System.Diagnostics.Debug.WriteLine($"[UpdateUnitPrices] Không tìm thấy thanh toán: {maThanhToan}");
                return false;
            }

            // Lấy tất cả thanh toán có trạng thái "Chưa trả"
            var unpaidPayments = await _paymentRepository.GetPaymentsByStatusAsync("Chưa trả");
            
            System.Diagnostics.Debug.WriteLine($"[UpdateUnitPrices] Tìm thấy {unpaidPayments.Count} thanh toán chưa trả");

            if (unpaidPayments == null || unpaidPayments.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine($"[UpdateUnitPrices] Không có thanh toán nào chưa trả");
                // Vẫn cập nhật thanh toán hiện tại nếu nó không phải "Chưa trả"
                unpaidPayments = new List<DataLayer.Models.Payment>();
            }

            // Thêm thanh toán hiện tại vào danh sách nếu chưa có (để đảm bảo nó được cập nhật)
            if (!unpaidPayments.Any(p => p.MaThanhToan == maThanhToan))
            {
                unpaidPayments.Add(currentPayment);
                System.Diagnostics.Debug.WriteLine($"[UpdateUnitPrices] Thêm thanh toán hiện tại {maThanhToan} vào danh sách cập nhật");
            }

            bool allSuccess = true;
            int updateCount = 0;

            // Cập nhật từng thanh toán
            foreach (var payment in unpaidPayments)
            {
                System.Diagnostics.Debug.WriteLine($"[UpdateUnitPrices] Đang cập nhật thanh toán {payment.MaThanhToan}, Trạng thái: {payment.TrangThaiThanhToan}");

                bool paymentUpdated = false;

                // Cập nhật đơn giá điện nếu có
                if (donGiaDien.HasValue && donGiaDien.Value > 0)
                {
                    decimal oldDonGiaDien = payment.DonGiaDien ?? 0;
                    payment.DonGiaDien = donGiaDien.Value;
                    
                    // Tính lại tiền điện dựa trên số điện và đơn giá mới
                    // Công thức: TienDien = SoDien * DonGiaDien
                    if (payment.SoDien.HasValue && payment.SoDien.Value > 0)
                    {
                        decimal oldTienDien = payment.TienDien ?? 0;
                        payment.TienDien = payment.SoDien.Value * donGiaDien.Value;
                        System.Diagnostics.Debug.WriteLine($"[UpdateUnitPrices] Thanh toán {payment.MaThanhToan}: Đơn giá điện {oldDonGiaDien} -> {donGiaDien.Value}, Tiền điện {oldTienDien} -> {payment.TienDien} (Số điện: {payment.SoDien.Value})");
                    }
                    else
                    {
                        // Nếu chưa có số điện, đặt tiền điện = 0
                        payment.TienDien = 0;
                        System.Diagnostics.Debug.WriteLine($"[UpdateUnitPrices] Thanh toán {payment.MaThanhToan}: Đơn giá điện {oldDonGiaDien} -> {donGiaDien.Value}, Tiền điện = 0 (chưa có số điện)");
                    }
                    paymentUpdated = true;
                }

                // Cập nhật đơn giá nước nếu có
                if (donGiaNuoc.HasValue && donGiaNuoc.Value > 0)
                {
                    payment.DonGiaNuoc = donGiaNuoc.Value;
                    // Tính lại tiền nước dựa trên số nước và đơn giá mới
                    // Lưu ý: SoNuoc có thể là số người, cần tính lại tiền nước
                    if (payment.SoNuoc.HasValue && payment.SoNuoc.Value > 0)
                    {
                        payment.TienNuoc = payment.SoNuoc.Value * donGiaNuoc.Value;
                    }
                    else
                    {
                        // Nếu chưa có số nước, đặt tiền nước = 0
                        payment.TienNuoc = 0;
                    }
                    paymentUpdated = true;
                    System.Diagnostics.Debug.WriteLine($"[UpdateUnitPrices] Cập nhật đơn giá nước: {donGiaNuoc.Value}, Tiền nước: {payment.TienNuoc}");
                }

                if (paymentUpdated)
                {
                    // Tính lại tổng tiền (KHÔNG trừ tiền cọc, chỉ tính tổng các khoản phí)
                    // TongTien = TienThue + TienDien + TienNuoc + TienInternet + TienVeSinh + TienGiuXe + ChiPhiKhac
                    payment.TongTien = CalculateTotalAmount(payment);
                    
                    System.Diagnostics.Debug.WriteLine($"[UpdateUnitPrices] Tính lại TongTien cho thanh toán {payment.MaThanhToan}:");
                    System.Diagnostics.Debug.WriteLine($"  - TienThue: {payment.TienThue ?? 0}");
                    System.Diagnostics.Debug.WriteLine($"  - TienDien: {payment.TienDien ?? 0}");
                    System.Diagnostics.Debug.WriteLine($"  - TienNuoc: {payment.TienNuoc ?? 0}");
                    System.Diagnostics.Debug.WriteLine($"  - TienInternet: {payment.TienInternet ?? 0}");
                    System.Diagnostics.Debug.WriteLine($"  - TienVeSinh: {payment.TienVeSinh ?? 0}");
                    System.Diagnostics.Debug.WriteLine($"  - TienGiuXe: {payment.TienGiuXe ?? 0}");
                    System.Diagnostics.Debug.WriteLine($"  - ChiPhiKhac: {payment.ChiPhiKhac ?? 0}");
                    System.Diagnostics.Debug.WriteLine($"  - TongTien (KHÔNG trừ tiền cọc): {payment.TongTien}");

                    // Cập nhật vào database
                    var success = await _paymentRepository.UpdateAsync(payment);
                    if (success)
                    {
                        updateCount++;
                        System.Diagnostics.Debug.WriteLine($"[UpdateUnitPrices] Đã cập nhật thành công thanh toán {payment.MaThanhToan} vào database, TongTien = {payment.TongTien}");
                    }
                    else
                    {
                        allSuccess = false;
                        System.Diagnostics.Debug.WriteLine($"[UpdateUnitPrices] Lỗi khi cập nhật thanh toán {payment.MaThanhToan} vào database");
                    }
                }
            }

            System.Diagnostics.Debug.WriteLine($"[UpdateUnitPrices] Hoàn thành: Đã cập nhật {updateCount}/{unpaidPayments.Count} thanh toán");
            return allSuccess;
        }

        public async Task<List<TransactionHistoryDto>> GetTransactionHistoryAsync(
            DateTime? tuNgay = null, DateTime? denNgay = null)
        {
            var current = AuthController.CurrentUser;
            List<DataLayer.Models.Payment> transactions;
            if (current != null && current.MaNha > 0)
            {
                transactions = await _paymentRepository.GetTransactionHistoryByMaNhaAsync(current.MaNha, tuNgay, denNgay);
            }
            else
            {
                transactions = await _paymentRepository.GetTransactionHistoryAsync(tuNgay, denNgay);
            }

            var result = new List<TransactionHistoryDto>();
            foreach (var p in transactions)
            {
                result.Add(new TransactionHistoryDto
                {
                    MaThanhToan = p.MaThanhToan,
                    TenPhong = p.TenPhong ?? "Không xác định",
                    TenKhachHang = p.TenKhachHang ?? "Không xác định",
                    MoTa = $"Thanh toán tháng {p.ThangNam}",
                    SoTien = p.TongTien,
                    ThoiGian = p.NgayThanhToan ?? DateTime.Now,
                    LoaiGiaoDich = "Thuê phòng",
                    LoaiGiaoDichIcon = "💰",
                    TrangThai = "Hoàn thành",
                    LoaiGiaoDichColor = "#10D096",
                    TrangThaiColor = "#10D096"
                });
            }

            return result;
        }

        public async Task<List<ContractDto>> GetActiveContractDtosAsync()
        {
            var current = AuthController.CurrentUser;
            IEnumerable<DataLayer.Models.Contract> activeContracts;
            if (current != null && current.MaNha > 0)
            {
                var contractsByHouse = await _contractRepository.GetAllByMaNhaAsync(current.MaNha);
                activeContracts = contractsByHouse.Where(c => string.Equals(c.TrangThai, "Hiệu lực", StringComparison.OrdinalIgnoreCase));
            }
            else
            {
                activeContracts = await _contractRepository.GetActiveContractsAsync();
            }

            return activeContracts.Select(c => new ContractDto
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