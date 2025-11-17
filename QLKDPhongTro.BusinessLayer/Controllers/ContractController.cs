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


        public ContractController(IContractRepository repository)
        {
            _repository = repository;
            _tenantRepository = new TenantRepository();
            _userRepository = new UserRepository();
        }

        public ContractController(IContractRepository repository, ITenantRepository tenantRepository)
        {
            _repository = repository;
            _tenantRepository = tenantRepository;
            _userRepository = new UserRepository();
        }

        public ContractController(IContractRepository repository, ITenantRepository tenantRepository, IUserRepository userRepository)
        {
            _repository = repository;
            _tenantRepository = tenantRepository;
            _userRepository = userRepository;
        }

        public static ContractController CreateDefault()
        {
            var repository = new QLKDPhongTro.DataLayer.Repositories.ContractRepository();
            return new ContractController(repository);
        }

        public async Task<List<ContractDto>> GetAllHopDongAsync()
        {
            var entities = await _repository.GetAllHopDongAsync();
            return entities.Select(e => new ContractDto
            {
                MaHopDong = e.MaHopDong,
                MaNguoiThue = e.MaNguoiThue,
                MaPhong = e.MaPhong,
                NgayBatDau = e.NgayBatDau,
                NgayKetThuc = e.NgayKetThuc,
                TienCoc = e.TienCoc,
                FileHopDong = e.FileHopDong,
                TrangThai = e.TrangThai,
                // Lấy từ JOIN trong repository
                TenNguoiThue = e.TenNguoiThue,
                TenPhong = e.TenPhong
            }).ToList();
        }

        public async Task<ContractDto?> GetByIdAsync(int maHopDong)
        {
            var entity = await _repository.GetByIdAsync(maHopDong);
            if (entity == null) return null;

            return new ContractDto
            {
                MaHopDong = entity.MaHopDong,
                MaNguoiThue = entity.MaNguoiThue,
                MaPhong = entity.MaPhong,
                NgayBatDau = entity.NgayBatDau,
                NgayKetThuc = entity.NgayKetThuc,
                TienCoc = entity.TienCoc,
                FileHopDong = entity.FileHopDong,
                TrangThai = entity.TrangThai,
                TenNguoiThue = entity.TenNguoiThue,
                TenPhong = entity.TenPhong
            };
        }

        public async Task<List<ContractDto>> GetActiveContractsAsync()
        {
            var entities = await _repository.GetActiveContractsAsync();
            return entities.Select(e => new ContractDto
            {
                MaHopDong = e.MaHopDong,
                MaNguoiThue = e.MaNguoiThue,
                MaPhong = e.MaPhong,
                NgayBatDau = e.NgayBatDau,
                NgayKetThuc = e.NgayKetThuc,
                TienCoc = e.TienCoc,
                FileHopDong = e.FileHopDong,
                TrangThai = e.TrangThai,
                TenNguoiThue = e.TenNguoiThue,
                TenPhong = e.TenPhong
            }).ToList();
        }

        public async Task<int> CreateHopDongAsync(ContractDto dto)
        {
            var entity = new Contract
            {
                MaNguoiThue = dto.MaNguoiThue,
                MaPhong = dto.MaPhong,
                NgayBatDau = dto.NgayBatDau,
                NgayKetThuc = dto.NgayKetThuc,
                TienCoc = dto.TienCoc,
                FileHopDong = dto.FileHopDong,
                TrangThai = dto.TrangThai
            };
            return await _repository.AddHopDongAsync(entity);
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
                TrangThai = dto.TrangThai
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
            return entities.Select(e => new ContractDto
            {
                MaHopDong = e.MaHopDong,
                MaNguoiThue = e.MaNguoiThue,
                MaPhong = e.MaPhong,
                NgayBatDau = e.NgayBatDau,
                NgayKetThuc = e.NgayKetThuc,
                TienCoc = e.TienCoc,
                FileHopDong = e.FileHopDong,
                TrangThai = e.TrangThai,
                TenNguoiThue = e.TenNguoiThue,
                TenPhong = e.TenPhong
            }).ToList();
        }
        // 🔹 Gửi email cảnh báo cho hợp đồng sắp hết hạn kèm file hợp đồng (gửi cho cả người thuê và admin)

        public async Task<(int Success, int Failed, List<string> Errors)> SendExpiryWarningEmailsAsync(int days)
        {
            var expiringContracts = await GetExpiringContractsAsync(days);

            if (expiringContracts == null || expiringContracts.Count == 0)
                return (0, 0, new List<string> { "Không có hợp đồng nào sắp hết hạn trong khoảng thời gian này." });

            int success = 0, failed = 0;
            var errors = new List<string>();

            // Lấy danh sách tất cả admin để gửi email
            var allAdmins = await _userRepository.GetAllAsync();
            var adminEmails = allAdmins
                .Where(a => !string.IsNullOrWhiteSpace(a.Email))
                .Select(a => a.Email)
                .ToList();

            // Nếu không có admin nào có email, sử dụng email mặc định
            if (adminEmails.Count == 0)
            {
                adminEmails.Add("ngochai1521@gmail.com");
            }

            foreach (var contract in expiringContracts)
            {
                try
                {
                    // === GỬI EMAIL CHO NGƯỜI THUÊ ===
                    var tenant = await _tenantRepository.GetByIdAsync(contract.MaNguoiThue);
                    string tenantEmail = tenant?.Email;

                    // Nếu tenant không có email, sử dụng email mặc định
                    if (string.IsNullOrWhiteSpace(tenantEmail))
                    {
                        tenantEmail = "ngochai1521@gmail.com";
                    }

                    // Tính số ngày còn lại
                    int daysRemaining = (contract.NgayKetThuc - DateTime.Now).Days;

                    // Tạo nội dung email cho người thuê
                    string tenantEmailBody = $@"Kính gửi {contract.TenNguoiThue ?? "Quý khách hàng"},

Thông báo quan trọng về hợp đồng thuê phòng của bạn:

- Mã hợp đồng: {contract.MaHopDong}
- Phòng: {contract.TenPhong ?? "N/A"}
- Ngày bắt đầu: {contract.NgayBatDau:dd/MM/yyyy}
- Ngày kết thúc: {contract.NgayKetThuc:dd/MM/yyyy}
- Số ngày còn lại: {daysRemaining} ngày

Hợp đồng của bạn sẽ hết hạn trong {daysRemaining} ngày tới. Vui lòng liên hệ với chúng tôi để gia hạn hợp đồng hoặc thảo luận về việc tiếp tục thuê phòng.

Trân trọng,
Quản lý Phòng Trọ";

                    // Gửi email cho người thuê với file hợp đồng đính kèm (nếu có)
                    string attachmentPath = contract.FileHopDong;
                    if (!string.IsNullOrWhiteSpace(attachmentPath) && System.IO.File.Exists(attachmentPath))
                    {
                        await EmailService.SendEmailWithAttachmentAsync(
                            tenantEmail,
                            $"Thông báo sắp hết hạn hợp đồng - Còn {daysRemaining} ngày",
                            tenantEmailBody,
                            attachmentPath
                        );
                    }
                    else
                    {
                        await EmailService.SendEmailAsync(
                            tenantEmail,
                            $"Thông báo sắp hết hạn hợp đồng - Còn {daysRemaining} ngày",
                            tenantEmailBody
                        );
                    }
                    success++;

                    // === GỬI EMAIL CHO TẤT CẢ ADMIN ===
                    string adminEmailBody = $@"Kính gửi Quản trị viên,

Thông báo về hợp đồng sắp hết hạn:

- Mã hợp đồng: {contract.MaHopDong}
- Người thuê: {contract.TenNguoiThue ?? "N/A"}
- Phòng: {contract.TenPhong ?? "N/A"}
- Ngày bắt đầu: {contract.NgayBatDau:dd/MM/yyyy}
- Ngày kết thúc: {contract.NgayKetThuc:dd/MM/yyyy}
- Số ngày còn lại: {daysRemaining} ngày

Hợp đồng này sẽ hết hạn trong {daysRemaining} ngày tới. Vui lòng liên hệ với người thuê để thảo luận về việc gia hạn hợp đồng.

Trân trọng,
Hệ thống Quản lý Phòng Trọ";

                    // Gửi email cho tất cả admin
                    foreach (var adminEmail in adminEmails)
                    {
                        try
                        {
                            if (!string.IsNullOrWhiteSpace(adminEmail))
                            {
                                // Gửi email cho admin với file hợp đồng đính kèm (nếu có)
                                if (!string.IsNullOrWhiteSpace(attachmentPath) && System.IO.File.Exists(attachmentPath))
                                {
                                    await EmailService.SendEmailWithAttachmentAsync(
                                        adminEmail,
                                        $"Cảnh báo: Hợp đồng sắp hết hạn - Còn {daysRemaining} ngày (HD#{contract.MaHopDong})",
                                        adminEmailBody,
                                        attachmentPath
                                    );
                                }
                                else
                                {
                                    await EmailService.SendEmailAsync(
                                        adminEmail,
                                        $"Cảnh báo: Hợp đồng sắp hết hạn - Còn {daysRemaining} ngày (HD#{contract.MaHopDong})",
                                        adminEmailBody
                                    );
                                }
                                success++;
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

            // Lấy danh sách tất cả admin để gửi email
            var allAdmins = await _userRepository.GetAllAsync();
            var adminEmails = allAdmins
                .Where(a => !string.IsNullOrWhiteSpace(a.Email))
                .Select(a => a.Email)
                .ToList();

            // Nếu không có admin nào có email, sử dụng email mặc định
            if (adminEmails.Count == 0)
            {
                adminEmails.Add("ngochai1521@gmail.com");
            }

            foreach (var contract in expiringContracts)
            {
                try
                {
                    // === GỬI EMAIL CHO NGƯỜI THUÊ ===
                    var tenant = await _tenantRepository.GetByIdAsync(contract.MaNguoiThue);
                    string tenantEmail = tenant?.Email;

                    // Nếu tenant không có email, sử dụng email mặc định
                    if (string.IsNullOrWhiteSpace(tenantEmail))
                    {
                        tenantEmail = "ngochai1521@gmail.com";
                    }

                    // Tạo nội dung email cho người thuê
                    string tenantEmailBody = $@"Kính gửi {contract.TenNguoiThue ?? "Quý khách hàng"},

Thông báo quan trọng về hợp đồng thuê phòng của bạn:

- Mã hợp đồng: {contract.MaHopDong}
- Phòng: {contract.TenPhong ?? "N/A"}
- Ngày bắt đầu: {contract.NgayBatDau:dd/MM/yyyy}
- Ngày kết thúc: {contract.NgayKetThuc:dd/MM/yyyy}
- Số ngày còn lại: {exactDays} ngày

Hợp đồng của bạn sẽ hết hạn trong {exactDays} ngày tới. Vui lòng liên hệ với chúng tôi để gia hạn hợp đồng hoặc thảo luận về việc tiếp tục thuê phòng.

Trân trọng,
Quản lý Phòng Trọ";

                    // Gửi email cho người thuê với file hợp đồng đính kèm (nếu có)
                    string attachmentPath = contract.FileHopDong;
                    if (!string.IsNullOrWhiteSpace(attachmentPath) && System.IO.File.Exists(attachmentPath))
                    {
                        await EmailService.SendEmailWithAttachmentAsync(
                            tenantEmail,
                            $"Thông báo sắp hết hạn hợp đồng - Còn {exactDays} ngày",
                            tenantEmailBody,
                            attachmentPath
                        );
                    }
                    else
                    {
                        await EmailService.SendEmailAsync(
                            tenantEmail,
                            $"Thông báo sắp hết hạn hợp đồng - Còn {exactDays} ngày",
                            tenantEmailBody
                        );
                    }
                    success++;

                    // === GỬI EMAIL CHO TẤT CẢ ADMIN ===
                    string adminEmailBody = $@"Kính gửi Quản trị viên,

Thông báo về hợp đồng sắp hết hạn:

- Mã hợp đồng: {contract.MaHopDong}
- Người thuê: {contract.TenNguoiThue ?? "N/A"}
- Phòng: {contract.TenPhong ?? "N/A"}
- Ngày bắt đầu: {contract.NgayBatDau:dd/MM/yyyy}
- Ngày kết thúc: {contract.NgayKetThuc:dd/MM/yyyy}
- Số ngày còn lại: {exactDays} ngày

Hợp đồng này sẽ hết hạn trong {exactDays} ngày tới. Vui lòng liên hệ với người thuê để thảo luận về việc gia hạn hợp đồng.

Trân trọng,
Hệ thống Quản lý Phòng Trọ";

                    // Gửi email cho tất cả admin
                    foreach (var adminEmail in adminEmails)
                    {
                        try
                        {
                            if (!string.IsNullOrWhiteSpace(adminEmail))
                            {
                                // Gửi email cho admin với file hợp đồng đính kèm (nếu có)
                                if (!string.IsNullOrWhiteSpace(attachmentPath) && System.IO.File.Exists(attachmentPath))
                                {
                                    await EmailService.SendEmailWithAttachmentAsync(
                                        adminEmail,
                                        $"Cảnh báo: Hợp đồng sắp hết hạn - Còn {exactDays} ngày (HD#{contract.MaHopDong})",
                                        adminEmailBody,
                                        attachmentPath
                                    );
                                }
                                else
                                {
                                    await EmailService.SendEmailAsync(
                                        adminEmail,
                                        $"Cảnh báo: Hợp đồng sắp hết hạn - Còn {exactDays} ngày (HD#{contract.MaHopDong})",
                                        adminEmailBody
                                    );
                                }
                                success++;
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
                    errors.Add($"Lỗi khi gửi email cho hợp đồng {contract.MaHopDong} ({contract.TenNguoiThue}): {ex.Message}");
                }
            }

            return (success, failed, errors);
        }



    }
}