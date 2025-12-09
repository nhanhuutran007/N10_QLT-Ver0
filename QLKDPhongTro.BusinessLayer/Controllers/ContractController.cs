using QLKDPhongTro.BusinessLayer.DTOs;
using QLKDPhongTro.BusinessLayer.Services;
using QLKDPhongTro.DataLayer.Models;
using QLKDPhongTro.DataLayer.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QLKDPhongTro.Presentation.Utils;
using System.Windows;


namespace QLKDPhongTro.BusinessLayer.Controllers
{
    public class ContractController
    {
        private readonly IContractRepository _repository;
        private readonly ITenantRepository _tenantRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRentedRoomRepository _roomRepository;


        public ContractController(IContractRepository repository)
        {
            _repository = repository;
            _tenantRepository = new TenantRepository();
            _userRepository = new UserRepository();
            _roomRepository = new RentedRoomRepository();
        }

        public ContractController(IContractRepository repository, ITenantRepository tenantRepository)
        {
            _repository = repository;
            _tenantRepository = tenantRepository;
            _userRepository = new UserRepository();
            _roomRepository = new RentedRoomRepository();
        }

        public ContractController(IContractRepository repository, ITenantRepository tenantRepository, IUserRepository userRepository)
        {
            _repository = repository;
            _tenantRepository = tenantRepository;
            _userRepository = userRepository;
            _roomRepository = new RentedRoomRepository();
        }

        private static string NormalizeStatus(DateTime endDate, string? currentStatus)
        {
            if (string.Equals(currentStatus, "Hủy", StringComparison.OrdinalIgnoreCase))
            {
                return "Hủy";
            }

            var daysLeft = (endDate.Date - DateTime.Today).TotalDays;

            if (daysLeft < 0)
            {
                return "Hết hạn";
            }

            if (daysLeft <= 30)
            {
                return "Sắp hết hạn";
            }

            return string.IsNullOrWhiteSpace(currentStatus) ? "Hiệu lực" : currentStatus!;
        }

        private static ContractDto WithDerivedStatus(ContractDto dto)
        {
            dto.TrangThai = NormalizeStatus(dto.NgayKetThuc, dto.TrangThai);
            return dto;
        }

        public static ContractController CreateDefault()
        {
            var repository = new QLKDPhongTro.DataLayer.Repositories.ContractRepository();
            return new ContractController(repository);
        }

        public async Task<List<ContractDto>> GetAllHopDongAsync()
        {
            // Nếu admin đang đăng nhập có MaNha, chỉ lấy hợp đồng thuộc nhà đó
            var current = AuthController.CurrentUser;
            var entities = (current != null && current.MaNha > 0)
                ? await _repository.GetAllByMaNhaAsync(current.MaNha)
                : await _repository.GetAllHopDongAsync();
            return entities.Select(e => new ContractDto
            {
                MaHopDong = e.MaHopDong,
                MaNguoiThue = e.MaNguoiThue,
                MaPhong = e.MaPhong,
                NgayBatDau = e.NgayBatDau,
                NgayKetThuc = e.NgayKetThuc,
                TienCoc = e.TienCoc,
                FileHopDong = e.FileHopDong,
                TrangThai = NormalizeStatus(e.NgayKetThuc, e.TrangThai),
                GhiChu = e.GhiChu,
                // Lấy từ JOIN trong repository
                TenNguoiThue = e.TenNguoiThue,
                TenPhong = e.TenPhong
            }).ToList();
        }

        public async Task<ContractDto?> GetByIdAsync(int maHopDong)
        {
            var entity = await _repository.GetByIdAsync(maHopDong);
            if (entity == null) return null;

            return WithDerivedStatus(new ContractDto
            {
                MaHopDong = entity.MaHopDong,
                MaNguoiThue = entity.MaNguoiThue,
                MaPhong = entity.MaPhong,
                NgayBatDau = entity.NgayBatDau,
                NgayKetThuc = entity.NgayKetThuc,
                TienCoc = entity.TienCoc,
                FileHopDong = entity.FileHopDong,
                TrangThai = entity.TrangThai,
                GhiChu = entity.GhiChu,
                TenNguoiThue = entity.TenNguoiThue,
                TenPhong = entity.TenPhong
            });
        }

        public async Task<List<ContractDto>> GetActiveContractsAsync()
        {
            var entities = await _repository.GetActiveContractsAsync();
            return entities.Select(e => WithDerivedStatus(new ContractDto
            {
                MaHopDong = e.MaHopDong,
                MaNguoiThue = e.MaNguoiThue,
                MaPhong = e.MaPhong,
                NgayBatDau = e.NgayBatDau,
                NgayKetThuc = e.NgayKetThuc,
                TienCoc = e.TienCoc,
                FileHopDong = e.FileHopDong,
                TrangThai = e.TrangThai,
                GhiChu = e.GhiChu,
                TenNguoiThue = e.TenNguoiThue,
                TenPhong = e.TenPhong
            })).ToList();
        }

        public async Task<int> CreateHopDongAsync(ContractDto dto)
        {
            if (dto.MaPhong <= 0)
            {
                throw new ArgumentException("MaPhong is required when creating a contract.", nameof(dto));
            }

            // Kiểm tra hợp đồng đang hiệu lực hiện tại của phòng
            var currentActiveContract = await _repository.GetActiveByRoomIdAsync(dto.MaPhong);

            var entity = new Contract
            {
                MaNguoiThue = dto.MaNguoiThue,
                MaPhong = dto.MaPhong,
                NgayBatDau = dto.NgayBatDau,
                NgayKetThuc = dto.NgayKetThuc,
                TienCoc = dto.TienCoc,
                FileHopDong = dto.FileHopDong,
                TrangThai = dto.TrangThai,
                GhiChu = dto.GhiChu
            };

            int newContractId = await _repository.AddHopDongAsync(entity);

            // ❗ Nếu phòng này trước đó CHƯA có hợp đồng hiệu lực
            //    => cập nhật tất cả người thuê của phòng sang "Đang ở"
            //    => và cập nhật trạng thái phòng sang "Đang thuê"
            if (currentActiveContract == null)
            {
                try
                {
                    // Lấy tất cả người thuê của phòng (trừ đã trả phòng)
                    var roomTenants = await _tenantRepository.GetTenantsByRoomIdAsync(dto.MaPhong);
                    foreach (var tenantInfo in roomTenants)
                    {
                        // Cập nhật trạng thái người thuê nếu khác "Đang ở"
                        if (!string.Equals(tenantInfo.TrangThaiNguoiThue, "Đang ở", StringComparison.OrdinalIgnoreCase))
                        {
                            await _tenantRepository.UpdateTenantStatusAsync(tenantInfo.MaNguoiThue, "Đang ở");
                        }
                    }

                    // Cập nhật trạng thái phòng
                    await _roomRepository.UpdateStatusAsync(dto.MaPhong, "Đang thuê");
                }
                catch (Exception ex)
                {
                    // Ghi log lỗi nếu cần, nhưng không chặn việc tạo hợp đồng
                    System.Diagnostics.Debug.WriteLine($"Lỗi khi đồng bộ trạng thái phòng/người thuê sau khi tạo hợp đồng: {ex.Message}");
                }
            }

            return newContractId;
        }

        public async Task UpdateHopDongAsync(ContractDto dto)
        {
            var entity = new Contract
            {
                MaHopDong = dto.MaHopDong,
                MaNguoiThue = dto.MaNguoiThue,
                MaPhong = dto.MaPhong,
                NgayBatDau = dto.NgayBatDau,
                NgayKetThuc = dto.NgayKetThuc,
                TienCoc = dto.TienCoc,
                FileHopDong = dto.FileHopDong,
                TrangThai = dto.TrangThai,
                GhiChu = dto.GhiChu
            };
            await _repository.UpdateHopDongAsync(entity);
        }

        public async Task DeleteHopDongAsync(int id)
        {
            await _repository.DeleteHopDongAsync(id);
        }

        public async Task<List<ContractDto>> GetExpiringContractsAsync(int days)
        {
            var entities = await _repository.GetExpiringContractsAsync(days);
            return entities.Select(e => WithDerivedStatus(new ContractDto
            {
                MaHopDong = e.MaHopDong,
                MaNguoiThue = e.MaNguoiThue,
                MaPhong = e.MaPhong,
                NgayBatDau = e.NgayBatDau,
                NgayKetThuc = e.NgayKetThuc,
                TienCoc = e.TienCoc,
                FileHopDong = e.FileHopDong,
                TrangThai = e.TrangThai,
                GhiChu = e.GhiChu,
                TenNguoiThue = e.TenNguoiThue,
                TenPhong = e.TenPhong
            })).ToList();
        }
        // 🔹 Gửi email cảnh báo cho hợp đồng sắp hết hạn kèm file hợp đồng (gửi cho cả người thuê và admin)
        /// <summary>
        /// Lấy hợp đồng đang có hiệu lực của một phòng cụ thể
        /// </summary>
        public async Task<ContractDto?> GetActiveContractByRoomIdAsync(int maPhong)
        {
            // Gọi Repository để lấy hợp đồng active theo MaPhong
            // (Bạn cần đảm bảo Repository đã có hàm GetActiveByRoomIdAsync, xem bước 2 bên dưới)
            var entity = await _repository.GetActiveByRoomIdAsync(maPhong);

            if (entity == null) return null;

            return WithDerivedStatus(new ContractDto
            {
                MaHopDong = entity.MaHopDong,
                MaNguoiThue = entity.MaNguoiThue,
                MaPhong = entity.MaPhong,
                NgayBatDau = entity.NgayBatDau,
                NgayKetThuc = entity.NgayKetThuc,

                // Quan trọng: Map các trường tiền để ViewModel tính toán
                TienCoc = entity.TienCoc,
                GiaThue = entity.GiaThue,

                FileHopDong = entity.FileHopDong,
                TrangThai = entity.TrangThai,
                GhiChu = entity.GhiChu,
                TenNguoiThue = entity.TenNguoiThue,
                TenPhong = entity.TenPhong
            });
        }
        public async Task<(int Success, int Failed, List<string> Errors)> SendExpiryWarningEmailsAsync(int days)
        {
            // Lấy hợp đồng còn trong khoảng nhắc nhở, loại bỏ đã hết hạn và đúng 30 ngày (gửi tự động)
            var expiringContracts = await GetExpiringContractsAsync(days);
            var contractsToSend = expiringContracts
                .Where(c =>
                {
                    var daysRemaining = (c.NgayKetThuc.Date - DateTime.Today).Days;
                    return daysRemaining >= 0 && daysRemaining < days; // <30 gửi thủ công
                })
                .ToList();

            if (contractsToSend.Count == 0)
                return (0, 0, new List<string> { $"Không có hợp đồng nào còn dưới {days} ngày để gửi thủ công." });

            int success = 0, failed = 0;
            var errors = new List<string>();
            var sentEmailTracker = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var contract in contractsToSend)
            {
                try
                {
                    var room = await _roomRepository.GetByIdAsync(contract.MaPhong);
                    int maNha = room?.MaNha ?? 0;

                    // Lấy danh sách admin của căn nhà
                    var houseAdmins = maNha > 0
                        ? await _userRepository.GetByHouseIdAsync(maNha)
                        : new List<DataLayer.Models.User>();

                    var adminEmails = houseAdmins
                        .Where(a => !string.IsNullOrWhiteSpace(a.Email))
                        .Select(a => a.Email)
                        .ToList();

                    // Nếu chưa có admin cho nhà, fallback toàn bộ admin hệ thống
                    if (adminEmails.Count == 0)
                    {
                        var allAdmins = await _userRepository.GetAllAsync();
                        adminEmails = allAdmins
                            .Where(a => !string.IsNullOrWhiteSpace(a.Email))
                            .Select(a => a.Email)
                            .ToList();
                    }

                    int daysRemaining = (contract.NgayKetThuc.Date - DateTime.Today).Days;

                    // === GỬI EMAIL CHO NGƯỜI THUÊ ===
                    var tenant = await _tenantRepository.GetByIdAsync(contract.MaNguoiThue);
                    string tenantEmail = tenant?.Email;

                    if (string.IsNullOrWhiteSpace(tenantEmail))
                    {
                        tenantEmail = "ngochai1521@gmail.com";
                    }

                    string tenantEmailBody = GenerateExpiringTenantEmailTemplate(contract, daysRemaining);

                    string attachmentPath = contract.FileHopDong;
                    if (await TrySendEmailOnceAsync(
                            sentEmailTracker,
                            contract.MaHopDong,
                            tenantEmail,
                            $"⚠️ Thông báo: Hợp đồng sắp hết hạn - Còn {daysRemaining} ngày",
                            tenantEmailBody,
                            attachmentPath))
                    {
                        success++;
                    }

                    // === GỬI EMAIL CHO ADMIN ===
                    if (adminEmails.Count > 0)
                    {
                        string adminEmailBody = GenerateExpiringAdminEmailTemplate(contract, daysRemaining, maNha);

                        foreach (var adminEmail in adminEmails)
                        {
                            try
                            {
                                if (!string.IsNullOrWhiteSpace(adminEmail))
                                {
                                    if (await TrySendEmailOnceAsync(
                                            sentEmailTracker,
                                            contract.MaHopDong,
                                            adminEmail,
                                            $"🚨 Cảnh báo: Hợp đồng HD-{contract.MaHopDong} sắp hết hạn - Còn {daysRemaining} ngày",
                                            adminEmailBody,
                                            attachmentPath))
                                    {
                                        success++;
                                    }
                                }
                            }
                            catch (Exception adminEx)
                            {
                                failed++;
                                errors.Add($"Lỗi khi gửi email cho admin {adminEmail} (Hợp đồng {contract.MaHopDong}): {adminEx.Message}");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    failed++;
                    errors.Add($"Lỗi khi gửi email cho hợp đồng {contract.MaHopDong} ({contract.TenNguoiThue}): {ex.Message}");
                }
            }

            return (success, failed, errors);
        }

        // 🔹 Tự động gửi email cảnh báo cho hợp đồng còn đúng 30 ngày (gửi cho cả người thuê và admin)
        public async Task<(int Success, int Failed, List<string> Errors)> SendExpiryWarningEmailsForExactDaysAsync(int exactDays)
        {
            var expiringContracts = await _repository.GetContractsExpiringInExactDaysAsync(exactDays);

            if (expiringContracts == null || expiringContracts.Count == 0)
                return (0, 0, new List<string>());

            int success = 0, failed = 0;
            var errors = new List<string>();
            var sentEmailTracker = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var contractEntity in expiringContracts)
            {
                try
                {
                    var contract = new ContractDto
                    {
                        MaHopDong = contractEntity.MaHopDong,
                        MaNguoiThue = contractEntity.MaNguoiThue,
                        MaPhong = contractEntity.MaPhong,
                        NgayBatDau = contractEntity.NgayBatDau,
                        NgayKetThuc = contractEntity.NgayKetThuc,
                        TienCoc = contractEntity.TienCoc,
                        FileHopDong = contractEntity.FileHopDong,
                        TrangThai = contractEntity.TrangThai,
                        GhiChu = contractEntity.GhiChu,
                        TenNguoiThue = contractEntity.TenNguoiThue,
                        TenPhong = contractEntity.TenPhong
                    };

                    var room = await _roomRepository.GetByIdAsync(contract.MaPhong);
                    int maNha = room?.MaNha ?? 0;

                    var houseAdmins = maNha > 0
                        ? await _userRepository.GetByHouseIdAsync(maNha)
                        : new List<DataLayer.Models.User>();

                    var adminEmails = houseAdmins
                        .Where(a => !string.IsNullOrWhiteSpace(a.Email))
                        .Select(a => a.Email)
                        .ToList();

                    if (adminEmails.Count == 0)
                    {
                        var allAdmins = await _userRepository.GetAllAsync();
                        adminEmails = allAdmins
                            .Where(a => !string.IsNullOrWhiteSpace(a.Email))
                            .Select(a => a.Email)
                            .ToList();
                    }

                    var tenant = await _tenantRepository.GetByIdAsync(contract.MaNguoiThue);
                    string tenantEmail = tenant?.Email;
                    if (string.IsNullOrWhiteSpace(tenantEmail))
                    {
                        tenantEmail = "ngochai1521@gmail.com";
                    }

                    string tenantEmailBody = GenerateExpiringTenantEmailTemplate(contract, exactDays);

                    string attachmentPath = contract.FileHopDong;
                    if (await TrySendEmailOnceAsync(
                            sentEmailTracker,
                            contract.MaHopDong,
                            tenantEmail,
                            $"⚠️ Thông báo: Hợp đồng sắp hết hạn - Còn {exactDays} ngày",
                            tenantEmailBody,
                            attachmentPath))
                    {
                        success++;
                    }

                    string adminEmailBody = GenerateExpiringAdminEmailTemplate(contract, exactDays, maNha);

                    foreach (var adminEmail in adminEmails)
                    {
                        try
                        {
                            if (!string.IsNullOrWhiteSpace(adminEmail))
                            {
                                if (await TrySendEmailOnceAsync(
                                        sentEmailTracker,
                                        contract.MaHopDong,
                                        adminEmail,
                                        $"🚨 Cảnh báo: Hợp đồng HD-{contract.MaHopDong} sắp hết hạn - Còn {exactDays} ngày",
                                        adminEmailBody,
                                        attachmentPath))
                                {
                                    success++;
                                }
                            }
                        }
                        catch (Exception adminEx)
                        {
                            failed++;
                            errors.Add($"Lỗi khi gửi email cho admin {adminEmail} (Hợp đồng {contract.MaHopDong}): {adminEx.Message}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    failed++;
                    errors.Add($"Lỗi khi gửi email cho hợp đồng {contractEntity.MaHopDong} ({contractEntity.TenNguoiThue ?? "N/A"}): {ex.Message}");
                }
            }

            return (success, failed, errors);
        }



        /// <summary>
        /// Tạo template HTML thông báo hợp đồng đã hết hạn (cho người thuê)
        /// </summary>
        private static string GenerateExpiringTenantEmailTemplate(ContractDto contract, int daysRemaining)
        {
            string tenantName = contract.TenNguoiThue ?? "Quý khách hàng";
            string roomName = contract.TenPhong ?? "N/A";
            string contractId = contract.MaHopDong.ToString();
            string startDate = contract.NgayBatDau.ToString("dd/MM/yyyy");
            string endDate = contract.NgayKetThuc.ToString("dd/MM/yyyy");
            string statusColor = daysRemaining <= 7 ? "#EF4444" : daysRemaining <= 15 ? "#F59E0B" : "#10B981";

            return $@"<!DOCTYPE html>
<html lang=""vi"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Thông báo hết hạn hợp đồng</title>
</head>
<body style=""margin: 0; padding: 0; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #f5f5f5;"">
    <table role=""presentation"" style=""width: 100%; border-collapse: collapse; background-color: #f5f5f5; padding: 20px;"">
        <tr>
            <td align=""center"">
                <table role=""presentation"" style=""max-width: 600px; width: 100%; background-color: #ffffff; border-radius: 8px; overflow: hidden; box-shadow: 0 2px 8px rgba(0,0,0,0.1);"">
                    <!-- Header -->
                    <tr>
                        <td style=""background: linear-gradient(135deg, #10B981 0%, #059669 100%); padding: 30px 20px; text-align: center;"">
                            <h1 style=""margin: 0; color: #ffffff; font-size: 24px; font-weight: 600;"">🏠 Quản Lý Phòng Trọ</h1>
                            <p style=""margin: 8px 0 0 0; color: #d1fae5; font-size: 14px;"">Hệ thống quản lý chuyên nghiệp</p>
                        </td>
                    </tr>
                    
                    <!-- Title -->
                    <tr>
                        <td style=""padding: 30px 20px 20px 20px; text-align: center; border-bottom: 2px solid #f3f4f6;"">
                            <h2 style=""margin: 0; color: #1F2937; font-size: 20px; font-weight: 600;"">⚠️ Thông Báo Quan Trọng</h2>
                            <p style=""margin: 10px 0 0 0; color: #6B7280; font-size: 16px;"">Hợp đồng của bạn sắp hết hạn</p>
                        </td>
                    </tr>
                    
                    <!-- Content -->
                    <tr>
                        <td style=""padding: 30px 20px;"">
                            <p style=""margin: 0 0 20px 0; color: #374151; font-size: 15px; line-height: 1.6;"">
                                Kính gửi <strong style=""color: #1F2937;"">{tenantName}</strong>,
                            </p>
                            <p style=""margin: 0 0 25px 0; color: #374151; font-size: 15px; line-height: 1.6;"">
                                Chúng tôi xin thông báo rằng hợp đồng thuê phòng của bạn sẽ hết hạn trong <strong style=""color: {statusColor}; font-size: 16px;"">{daysRemaining} ngày</strong> tới. 
                                Vui lòng liên hệ với chúng tôi để gia hạn hợp đồng hoặc thảo luận về việc tiếp tục thuê phòng.
                            </p>
                            
                            <!-- Contract Info Table -->
                            <table role=""presentation"" style=""width: 100%; border-collapse: collapse; margin: 25px 0; background-color: #F9FAFB; border-radius: 8px; overflow: hidden;"">
                                <tr>
                                    <td style=""padding: 20px; background-color: #10B981; color: #ffffff; font-weight: 600; font-size: 16px; text-align: center;"">
                                        📋 Thông Tin Hợp Đồng
                                    </td>
                                </tr>
                                <tr>
                                    <td style=""padding: 0;"">
                                        <table role=""presentation"" style=""width: 100%; border-collapse: collapse;"">
                                            <tr>
                                                <td style=""padding: 12px 20px; border-bottom: 1px solid #E5E7EB; color: #374151; font-size: 14px;"">
                                                    <strong style=""color: #1F2937; min-width: 140px; display: inline-block;"">Mã hợp đồng:</strong>
                                                    <span style=""color: #6B7280;"">HD-{contractId}</span>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style=""padding: 12px 20px; border-bottom: 1px solid #E5E7EB; color: #374151; font-size: 14px;"">
                                                    <strong style=""color: #1F2937; min-width: 140px; display: inline-block;"">Phòng:</strong>
                                                    <span style=""color: #6B7280;"">{roomName}</span>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style=""padding: 12px 20px; border-bottom: 1px solid #E5E7EB; color: #374151; font-size: 14px;"">
                                                    <strong style=""color: #1F2937; min-width: 140px; display: inline-block;"">Ngày bắt đầu:</strong>
                                                    <span style=""color: #6B7280;"">{startDate}</span>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style=""padding: 12px 20px; border-bottom: 1px solid #E5E7EB; color: #374151; font-size: 14px;"">
                                                    <strong style=""color: #1F2937; min-width: 140px; display: inline-block;"">Ngày kết thúc:</strong>
                                                    <span style=""color: #6B7280;"">{endDate}</span>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style=""padding: 12px 20px; background-color: #FEF3C7; color: #374151; font-size: 14px;"">
                                                    <strong style=""color: #92400E; min-width: 140px; display: inline-block;"">⏰ Số ngày còn lại:</strong>
                                                    <span style=""color: {statusColor}; font-weight: 600; font-size: 15px;"">{daysRemaining} ngày</span>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            
                            <!-- Call to Action -->
                            <div style=""background-color: #EFF6FF; border-left: 4px solid #3B82F6; padding: 15px 20px; margin: 25px 0; border-radius: 4px;"">
                                <p style=""margin: 0; color: #1E40AF; font-size: 14px; line-height: 1.6;"">
                                    <strong>💡 Lưu ý:</strong> Để tránh gián đoạn, vui lòng liên hệ với chúng tôi sớm nhất có thể để thảo luận về việc gia hạn hợp đồng.
                                </p>
                            </div>
                        </td>
                    </tr>
                    
                    <!-- Footer -->
                    <tr>
                        <td style=""background-color: #F9FAFB; padding: 25px 20px; text-align: center; border-top: 1px solid #E5E7EB;"">
                            <p style=""margin: 0 0 10px 0; color: #6B7280; font-size: 14px;"">
                                <strong style=""color: #1F2937;"">Trân trọng,</strong><br>
                                <span style=""color: #10B981; font-weight: 600;"">Đội ngũ Quản lý Phòng Trọ</span>
                            </p>
                            <p style=""margin: 15px 0 0 0; color: #9CA3AF; font-size: 12px; line-height: 1.6;"">
                                Email này được gửi tự động từ hệ thống quản lý.<br>
                                Vui lòng không trả lời email này.
                            </p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
        }

        /// <summary>
        /// Tạo template HTML thông báo hợp đồng đã hết hạn (cho người thuê)
        /// </summary>
        private static string GenerateExpiredTenantEmailTemplate(ContractDto contract)
        {
            string tenantName = contract.TenNguoiThue ?? "Quý khách hàng";
            string roomName = contract.TenPhong ?? "N/A";
            string contractId = contract.MaHopDong.ToString();
            string startDate = contract.NgayBatDau.ToString("dd/MM/yyyy");
            string endDate = contract.NgayKetThuc.ToString("dd/MM/yyyy");
            int overdueDays = Math.Max(0, (DateTime.Today - contract.NgayKetThuc.Date).Days);
            string overdueText = overdueDays > 0 ? $"{overdueDays} ngày" : "hôm nay";
            string statusColor = "#EF4444";

            return $@"<!DOCTYPE html>
<html lang=""vi"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Thông báo hết hạn hợp đồng</title>
</head>
<body style=""margin: 0; padding: 0; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #f5f5f5;"">
    <table role=""presentation"" style=""width: 100%; border-collapse: collapse; background-color: #f5f5f5; padding: 20px;"">
        <tr>
            <td align=""center"">
                <table role=""presentation"" style=""max-width: 600px; width: 100%; background-color: #ffffff; border-radius: 8px; overflow: hidden; box-shadow: 0 2px 8px rgba(0,0,0,0.1);"">
                    <!-- Header -->
                    <tr>
                        <td style=""background: linear-gradient(135deg, #10B981 0%, #059669 100%); padding: 30px 20px; text-align: center;"">
                            <h1 style=""margin: 0; color: #ffffff; font-size: 24px; font-weight: 600;"">🏠 Quản Lý Phòng Trọ</h1>
                            <p style=""margin: 8px 0 0 0; color: #d1fae5; font-size: 14px;"">Hệ thống quản lý chuyên nghiệp</p>
                        </td>
                    </tr>
                    
                    <!-- Title -->
                    <tr>
                        <td style=""padding: 30px 20px 20px 20px; text-align: center; border-bottom: 2px solid #f3f4f6;"">
                            <h2 style=""margin: 0; color: #1F2937; font-size: 20px; font-weight: 600;"">⚠️ Thông Báo Quan Trọng</h2>
                            <p style=""margin: 10px 0 0 0; color: #6B7280; font-size: 16px;"">Hợp đồng của bạn đã hết hạn</p>
                        </td>
                    </tr>
                    
                    <!-- Content -->
                    <tr>
                        <td style=""padding: 30px 20px;"">
                            <p style=""margin: 0 0 20px 0; color: #374151; font-size: 15px; line-height: 1.6;"">
                                Kính gửi <strong style=""color: #1F2937;"">{tenantName}</strong>,
                            </p>
                            <p style=""margin: 0 0 25px 0; color: #374151; font-size: 15px; line-height: 1.6;"">
                                Hợp đồng thuê phòng của bạn đã hết hạn từ <strong style=""color: {statusColor}; font-size: 16px;"">{overdueText}</strong>. 
                                Vui lòng liên hệ với chúng tôi để gia hạn hoặc bàn giao phòng.
                            </p>
                            
                            <!-- Contract Info Table -->
                            <table role=""presentation"" style=""width: 100%; border-collapse: collapse; margin: 25px 0; background-color: #F9FAFB; border-radius: 8px; overflow: hidden;"">
                                <tr>
                                    <td style=""padding: 20px; background-color: #10B981; color: #ffffff; font-weight: 600; font-size: 16px; text-align: center;"">
                                        📋 Thông Tin Hợp Đồng
                                    </td>
                                </tr>
                                <tr>
                                    <td style=""padding: 0;"">
                                        <table role=""presentation"" style=""width: 100%; border-collapse: collapse;"">
                                            <tr>
                                                <td style=""padding: 12px 20px; border-bottom: 1px solid #E5E7EB; color: #374151; font-size: 14px;"">
                                                    <strong style=""color: #1F2937; min-width: 140px; display: inline-block;"">Mã hợp đồng:</strong>
                                                    <span style=""color: #6B7280;"">HD-{contractId}</span>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style=""padding: 12px 20px; border-bottom: 1px solid #E5E7EB; color: #374151; font-size: 14px;"">
                                                    <strong style=""color: #1F2937; min-width: 140px; display: inline-block;"">Phòng:</strong>
                                                    <span style=""color: #6B7280;"">{roomName}</span>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style=""padding: 12px 20px; border-bottom: 1px solid #E5E7EB; color: #374151; font-size: 14px;"">
                                                    <strong style=""color: #1F2937; min-width: 140px; display: inline-block;"">Ngày bắt đầu:</strong>
                                                    <span style=""color: #6B7280;"">{startDate}</span>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style=""padding: 12px 20px; border-bottom: 1px solid #E5E7EB; color: #374151; font-size: 14px;"">
                                                    <strong style=""color: #1F2937; min-width: 140px; display: inline-block;"">Ngày kết thúc:</strong>
                                                    <span style=""color: #6B7280;"">{endDate}</span>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style=""padding: 12px 20px; background-color: #FEF3C7; color: #374151; font-size: 14px;"">
                                                    <strong style=""color: #92400E; min-width: 140px; display: inline-block;"">⏰ Tình trạng:</strong>
                                                    <span style=""color: {statusColor}; font-weight: 600; font-size: 15px;"">Đã hết hạn</span>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            
                            <!-- Call to Action -->
                            <div style=""background-color: #EFF6FF; border-left: 4px solid #3B82F6; padding: 15px 20px; margin: 25px 0; border-radius: 4px;"">
                                <p style=""margin: 0; color: #1E40AF; font-size: 14px; line-height: 1.6;"">
                                    <strong>💡 Lưu ý:</strong> Để tránh gián đoạn, vui lòng liên hệ với chúng tôi sớm nhất có thể để thảo luận về việc gia hạn hợp đồng.
                                </p>
                            </div>
                        </td>
                    </tr>
                    
                    <!-- Footer -->
                    <tr>
                        <td style=""background-color: #F9FAFB; padding: 25px 20px; text-align: center; border-top: 1px solid #E5E7EB;"">
                            <p style=""margin: 0 0 10px 0; color: #6B7280; font-size: 14px;"">
                                <strong style=""color: #1F2937;"">Trân trọng,</strong><br>
                                <span style=""color: #10B981; font-weight: 600;"">Đội ngũ Quản lý Phòng Trọ</span>
                            </p>
                            <p style=""margin: 15px 0 0 0; color: #9CA3AF; font-size: 12px; line-height: 1.6;"">
                                Email này được gửi tự động từ hệ thống quản lý.<br>
                                Vui lòng không trả lời email này.
                            </p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
        }

        /// <summary>
        /// Tạo template HTML thông báo hợp đồng đã hết hạn (cho admin)
        /// </summary>
        private static string GenerateExpiringAdminEmailTemplate(ContractDto contract, int daysRemaining, int maNha)
        {
            string tenantName = contract.TenNguoiThue ?? "N/A";
            string roomName = contract.TenPhong ?? "N/A";
            string contractId = contract.MaHopDong.ToString();
            string startDate = contract.NgayBatDau.ToString("dd/MM/yyyy");
            string endDate = contract.NgayKetThuc.ToString("dd/MM/yyyy");
            string statusColor = daysRemaining <= 7 ? "#EF4444" : daysRemaining <= 15 ? "#F59E0B" : "#10B981";
            string houseText = maNha > 0 ? maNha.ToString() : "N/A";

            return $@"<!DOCTYPE html>
<html lang=""vi"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Cảnh báo hợp đồng sắp hết hạn</title>
</head>
<body style=""margin: 0; padding: 0; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #f5f5f5;"">
    <table role=""presentation"" style=""width: 100%; border-collapse: collapse; background-color: #f5f5f5; padding: 20px;"">
        <tr>
            <td align=""center"">
                <table role=""presentation"" style=""max-width: 600px; width: 100%; background-color: #ffffff; border-radius: 8px; overflow: hidden; box-shadow: 0 2px 8px rgba(0,0,0,0.1);"">
                    <!-- Header -->
                    <tr>
                        <td style=""background: linear-gradient(135deg, #EF4444 0%, #DC2626 100%); padding: 30px 20px; text-align: center;"">
                            <h1 style=""margin: 0; color: #ffffff; font-size: 24px; font-weight: 600;"">🚨 Cảnh Báo Hệ Thống</h1>
                            <p style=""margin: 8px 0 0 0; color: #FEE2E2; font-size: 14px;"">Hợp đồng sắp hết hạn cần xử lý</p>
                        </td>
                    </tr>
                    
                    <!-- Title -->
                    <tr>
                        <td style=""padding: 30px 20px 20px 20px; text-align: center; border-bottom: 2px solid #f3f4f6;"">
                            <h2 style=""margin: 0; color: #1F2937; font-size: 20px; font-weight: 600;"">⚠️ Hợp Đồng Sắp Hết Hạn</h2>
                            <p style=""margin: 10px 0 0 0; color: #6B7280; font-size: 16px;"">Cần liên hệ với người thuê để gia hạn</p>
                        </td>
                    </tr>
                    
                    <!-- Content -->
                    <tr>
                        <td style=""padding: 30px 20px;"">
                            <p style=""margin: 0 0 20px 0; color: #374151; font-size: 15px; line-height: 1.6;"">
                                Kính gửi <strong style=""color: #1F2937;"">Quản trị viên</strong>,
                            </p>
                            <p style=""margin: 0 0 25px 0; color: #374151; font-size: 15px; line-height: 1.6;"">
                                Hệ thống phát hiện một hợp đồng sẽ hết hạn trong <strong style=""color: {statusColor}; font-size: 16px;"">{daysRemaining} ngày</strong> tới. 
                                Vui lòng liên hệ với người thuê để thảo luận về việc gia hạn hợp đồng.
                            </p>
                            
                            <!-- Contract Info Table -->
                            <table role=""presentation"" style=""width: 100%; border-collapse: collapse; margin: 25px 0; background-color: #F9FAFB; border-radius: 8px; overflow: hidden;"">
                                <tr>
                                    <td style=""padding: 20px; background-color: #EF4444; color: #ffffff; font-weight: 600; font-size: 16px; text-align: center;"">
                                        📋 Chi Tiết Hợp Đồng
                                    </td>
                                </tr>
                                <tr>
                                    <td style=""padding: 0;"">
                                        <table role=""presentation"" style=""width: 100%; border-collapse: collapse;"">
                                            <tr>
                                                <td style=""padding: 12px 20px; border-bottom: 1px solid #E5E7EB; color: #374151; font-size: 14px;"">
                                                    <strong style=""color: #1F2937; min-width: 140px; display: inline-block;"">Mã hợp đồng:</strong>
                                                    <span style=""color: #6B7280;"">HD-{contractId}</span>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style=""padding: 12px 20px; border-bottom: 1px solid #E5E7EB; color: #374151; font-size: 14px;"">
                                                    <strong style=""color: #1F2937; min-width: 140px; display: inline-block;"">Người thuê:</strong>
                                                    <span style=""color: #6B7280;"">{tenantName}</span>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style=""padding: 12px 20px; border-bottom: 1px solid #E5E7EB; color: #374151; font-size: 14px;"">
                                                    <strong style=""color: #1F2937; min-width: 140px; display: inline-block;"">Phòng:</strong>
                                                    <span style=""color: #6B7280;"">{roomName}</span>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style=""padding: 12px 20px; border-bottom: 1px solid #E5E7EB; color: #374151; font-size: 14px;"">
                                                    <strong style=""color: #1F2937; min-width: 140px; display: inline-block;"">Mã nhà:</strong>
                                                    <span style=""color: #6B7280;"">{houseText}</span>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style=""padding: 12px 20px; border-bottom: 1px solid #E5E7EB; color: #374151; font-size: 14px;"">
                                                    <strong style=""color: #1F2937; min-width: 140px; display: inline-block;"">Ngày bắt đầu:</strong>
                                                    <span style=""color: #6B7280;"">{startDate}</span>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style=""padding: 12px 20px; border-bottom: 1px solid #E5E7EB; color: #374151; font-size: 14px;"">
                                                    <strong style=""color: #1F2937; min-width: 140px; display: inline-block;"">Ngày kết thúc:</strong>
                                                    <span style=""color: #6B7280;"">{endDate}</span>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style=""padding: 12px 20px; background-color: #FEE2E2; color: #374151; font-size: 14px;"">
                                                    <strong style=""color: #991B1B; min-width: 140px; display: inline-block;"">⏰ Số ngày còn lại:</strong>
                                                    <span style=""color: {statusColor}; font-weight: 600; font-size: 15px;"">{daysRemaining} ngày</span>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            
                            <!-- Call to Action -->
                            <div style=""background-color: #FEF3C7; border-left: 4px solid #F59E0B; padding: 15px 20px; margin: 25px 0; border-radius: 4px;"">
                                <p style=""margin: 0; color: #92400E; font-size: 14px; line-height: 1.6;"">
                                    <strong>📞 Hành động cần thiết:</strong> Vui lòng liên hệ với người thuê <strong>{tenantName}</strong> để thảo luận về việc gia hạn hợp đồng trước khi hết hạn.
                                </p>
                            </div>
                        </td>
                    </tr>
                    
                    <!-- Footer -->
                    <tr>
                        <td style=""background-color: #F9FAFB; padding: 25px 20px; text-align: center; border-top: 1px solid #E5E7EB;"">
                            <p style=""margin: 0 0 10px 0; color: #6B7280; font-size: 14px;"">
                                <strong style=""color: #1F2937;"">Trân trọng,</strong><br>
                                <span style=""color: #EF4444; font-weight: 600;"">Hệ thống Quản lý Phòng Trọ</span>
                            </p>
                            <p style=""margin: 15px 0 0 0; color: #9CA3AF; font-size: 12px; line-height: 1.6;"">
                                Email cảnh báo tự động từ hệ thống.<br>
                                Vui lòng xử lý thông báo này trong thời gian sớm nhất.
                            </p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
        }

        private static string GenerateExpiredAdminEmailTemplate(ContractDto contract, int maNha)
        {
            string tenantName = contract.TenNguoiThue ?? "N/A";
            string roomName = contract.TenPhong ?? "N/A";
            string contractId = contract.MaHopDong.ToString();
            string startDate = contract.NgayBatDau.ToString("dd/MM/yyyy");
            string endDate = contract.NgayKetThuc.ToString("dd/MM/yyyy");
            string statusColor = "#EF4444";
            string houseText = maNha > 0 ? maNha.ToString() : "N/A";

            return $@"<!DOCTYPE html>
<html lang=""vi"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Cảnh báo hợp đồng đã hết hạn</title>
</head>
<body style=""margin: 0; padding: 0; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #f5f5f5;"">
    <table role=""presentation"" style=""width: 100%; border-collapse: collapse; background-color: #f5f5f5; padding: 20px;"">
        <tr>
            <td align=""center"">
                <table role=""presentation"" style=""max-width: 600px; width: 100%; background-color: #ffffff; border-radius: 8px; overflow: hidden; box-shadow: 0 2px 8px rgba(0,0,0,0.1);"">
                    <!-- Header -->
                    <tr>
                        <td style=""background: linear-gradient(135deg, #EF4444 0%, #DC2626 100%); padding: 30px 20px; text-align: center;"">
                            <h1 style=""margin: 0; color: #ffffff; font-size: 24px; font-weight: 600;"">🚨 Cảnh Báo Hệ Thống</h1>
                            <p style=""margin: 8px 0 0 0; color: #FEE2E2; font-size: 14px;"">Hợp đồng đã hết hạn cần xử lý</p>
                        </td>
                    </tr>
                    
                    <!-- Title -->
                    <tr>
                        <td style=""padding: 30px 20px 20px 20px; text-align: center; border-bottom: 2px solid #f3f4f6;"">
                            <h2 style=""margin: 0; color: #1F2937; font-size: 20px; font-weight: 600;"">⚠️ Hợp Đồng ĐÃ Hết Hạn</h2>
                            <p style=""margin: 10px 0 0 0; color: #6B7280; font-size: 16px;"">Cần liên hệ với người thuê để xử lý ngay</p>
                        </td>
                    </tr>
                    
                    <!-- Content -->
                    <tr>
                        <td style=""padding: 30px 20px;"">
                            <p style=""margin: 0 0 20px 0; color: #374151; font-size: 15px; line-height: 1.6;"">
                                Kính gửi <strong style=""color: #1F2937;"">Quản trị viên</strong>,
                            </p>
                            <p style=""margin: 0 0 25px 0; color: #374151; font-size: 15px; line-height: 1.6;"">
                                Hệ thống phát hiện một hợp đồng đã hết hạn. Vui lòng liên hệ với người thuê để gia hạn hoặc bàn giao phòng.
                            </p>
                            
                            <!-- Contract Info Table -->
                            <table role=""presentation"" style=""width: 100%; border-collapse: collapse; margin: 25px 0; background-color: #F9FAFB; border-radius: 8px; overflow: hidden;"">
                                <tr>
                                    <td style=""padding: 20px; background-color: #EF4444; color: #ffffff; font-weight: 600; font-size: 16px; text-align: center;"">
                                        📋 Chi Tiết Hợp Đồng
                                    </td>
                                </tr>
                                <tr>
                                    <td style=""padding: 0;"">
                                        <table role=""presentation"" style=""width: 100%; border-collapse: collapse;"">
                                            <tr>
                                                <td style=""padding: 12px 20px; border-bottom: 1px solid #E5E7EB; color: #374151; font-size: 14px;"">
                                                    <strong style=""color: #1F2937; min-width: 140px; display: inline-block;"">Mã hợp đồng:</strong>
                                                    <span style=""color: #6B7280;"">HD-{contractId}</span>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style=""padding: 12px 20px; border-bottom: 1px solid #E5E7EB; color: #374151; font-size: 14px;"">
                                                    <strong style=""color: #1F2937; min-width: 140px; display: inline-block;"">Người thuê:</strong>
                                                    <span style=""color: #6B7280;"">{tenantName}</span>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style=""padding: 12px 20px; border-bottom: 1px solid #E5E7EB; color: #374151; font-size: 14px;"">
                                                    <strong style=""color: #1F2937; min-width: 140px; display: inline-block;"">Phòng:</strong>
                                                    <span style=""color: #6B7280;"">{roomName}</span>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style=""padding: 12px 20px; border-bottom: 1px solid #E5E7EB; color: #374151; font-size: 14px;"">
                                                    <strong style=""color: #1F2937; min-width: 140px; display: inline-block;"">Ngày bắt đầu:</strong>
                                                    <span style=""color: #6B7280;"">{startDate}</span>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style=""padding: 12px 20px; border-bottom: 1px solid #E5E7EB; color: #374151; font-size: 14px;"">
                                                    <strong style=""color: #1F2937; min-width: 140px; display: inline-block;"">Ngày kết thúc:</strong>
                                                    <span style=""color: #6B7280;"">{endDate}</span>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style=""padding: 12px 20px; background-color: #FEE2E2; color: #374151; font-size: 14px;"">
                                                    <strong style=""color: #991B1B; min-width: 140px; display: inline-block;"">⏰ Trạng thái:</strong>
                                                    <span style=""color: {statusColor}; font-weight: 600; font-size: 15px;"">Đã hết hạn</span>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            
                            <!-- Call to Action -->
                            <div style=""background-color: #FEF3C7; border-left: 4px solid #F59E0B; padding: 15px 20px; margin: 25px 0; border-radius: 4px;"">
                                <p style=""margin: 0; color: #92400E; font-size: 14px; line-height: 1.6;"">
                                    <strong>📞 Hành động cần thiết:</strong> Vui lòng liên hệ với người thuê <strong>{tenantName}</strong> để thảo luận về việc gia hạn hợp đồng trước khi hết hạn.
                                </p>
                            </div>
                        </td>
                    </tr>
                    
                    <!-- Footer -->
                    <tr>
                        <td style=""background-color: #F9FAFB; padding: 25px 20px; text-align: center; border-top: 1px solid #E5E7EB;"">
                            <p style=""margin: 0 0 10px 0; color: #6B7280; font-size: 14px;"">
                                <strong style=""color: #1F2937;"">Trân trọng,</strong><br>
                                <span style=""color: #EF4444; font-weight: 600;"">Hệ thống Quản lý Phòng Trọ</span>
                            </p>
                            <p style=""margin: 15px 0 0 0; color: #9CA3AF; font-size: 12px; line-height: 1.6;"">
                                Email cảnh báo tự động từ hệ thống.<br>
                                Vui lòng xử lý thông báo này trong thời gian sớm nhất.
                            </p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
        }

        private static async Task<bool> TrySendEmailOnceAsync(
            HashSet<string> sentEmailTracker,
            int contractId,
            string? recipientEmail,
            string subject,
            string body,
            string? attachmentPath)
        {
            if (string.IsNullOrWhiteSpace(recipientEmail))
            {
                return false;
            }

            string normalizedEmail = recipientEmail.Trim();
            string dedupKey = $"{contractId}:{normalizedEmail.ToLowerInvariant()}";

            if (!sentEmailTracker.Add(dedupKey))
            {
                return false;
            }

            if (!string.IsNullOrWhiteSpace(attachmentPath) && System.IO.File.Exists(attachmentPath))
            {
                await EmailService.SendEmailWithAttachmentAsync(
                    normalizedEmail,
                    subject,
                    body,
                    attachmentPath);
            }
            else
            {
                await EmailService.SendEmailAsync(
                    normalizedEmail,
                    subject,
                    body);
            }

            return true;
        }

    }
}