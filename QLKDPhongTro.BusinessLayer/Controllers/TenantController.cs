using QLKDPhongTro.BusinessLayer.DTOs;
using QLKDPhongTro.DataLayer.Models;
using QLKDPhongTro.DataLayer.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QLKDPhongTro.BusinessLayer.Controllers
{
    public class TenantController
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly IRentedRoomRepository _roomRepository;

        public TenantController(ITenantRepository tenantRepository, IRentedRoomRepository? roomRepository = null)
        {
            _tenantRepository = tenantRepository;
            _roomRepository = roomRepository ?? new RentedRoomRepository();
        }

        public async Task<List<TenantDto>> GetAllTenantsAsync()
        {
            List<Tenant> tenants;
            var current = AuthController.CurrentUser;
            if (current != null && current.MaNha > 0)
            {
                tenants = await _tenantRepository.GetAllByMaNhaAsync(current.MaNha);
            }
            else
            {
                tenants = await _tenantRepository.GetAllAsync();
            }
            return tenants.Select(MapTenant).ToList();
        }

        public async Task<TenantDto?> GetTenantByIdAsync(int maKhachThue)
        {
            var tenant = await _tenantRepository.GetByIdAsync(maKhachThue);
            return tenant == null ? null : MapTenant(tenant);
        }

        public async Task<TenantDetailDto?> GetTenantDetailAsync(int maKhachThue)
        {
            var tenant = await _tenantRepository.GetByIdAsync(maKhachThue);
            if (tenant == null) return null;

            var assets = await _tenantRepository.GetAssetsAsync(maKhachThue);
            var stayInfo = await _tenantRepository.GetCurrentStayInfoAsync(maKhachThue) ?? new TenantStayInfo
            {
                TrangThaiNguoiThue = tenant.TrangThai
            };

            var stayInfoDto = await BuildStayInfoAsync(tenant, stayInfo);

            return new TenantDetailDto
            {
                BasicInfo = MapTenant(tenant),
                Assets = assets.Select(MapAsset).ToList(),
                StayInfo = stayInfoDto
            };
        }

        public async Task<List<RoomTenantDto>> GetTenantsByRoomIdAsync(int maPhong)
        {
            var tenants = await _tenantRepository.GetTenantsByRoomIdAsync(maPhong);
            return tenants.Select(MapRoomTenant).ToList();
        }

        public async Task<ValidationResult> CreateTenantAsync(TenantDto dto)
        {
            if (await _tenantRepository.IsCCCDExistsAsync(dto.CCCD))
            {
                return new ValidationResult { IsValid = false, Message = "CCCD đã tồn tại!" };
            }

            var tenant = new Tenant
            {
                MaPhong = dto.MaPhong,
                HoTen = dto.HoTen,
                CCCD = dto.CCCD,
                SoDienThoai = dto.SoDienThoai,
                Email = dto.Email,
                GioiTinh = dto.GioiTinh,
                NgheNghiep = dto.NgheNghiep,
                GhiChu = dto.GhiChu,
                NgaySinh = dto.NgaySinh,
                NgayCap = dto.NgayCap,
                NoiCap = dto.NoiCap,
                DiaChi = dto.DiaChi,
                NgayTao = DateTime.Now,
                NgayCapNhat = DateTime.Now
            };

            var success = await _tenantRepository.CreateAsync(tenant);
            return new ValidationResult
            {
                IsValid = success,
                Message = success ? "✅ Thêm khách thuê thành công!" : "❌ Thêm khách thuê thất bại!"
            };
        }

        public async Task<ValidationResult> UpdateTenantAsync(TenantDto dto)
        {
            if (await _tenantRepository.IsCCCDExistsAsync(dto.CCCD, dto.MaKhachThue))
            {
                return new ValidationResult { IsValid = false, Message = "CCCD đã tồn tại!" };
            }

            var existingTenant = await _tenantRepository.GetByIdAsync(dto.MaKhachThue);
            if (existingTenant == null)
            {
                return new ValidationResult { IsValid = false, Message = "❌ Không tìm thấy khách thuê để cập nhật!" };
            }

            existingTenant.HoTen = dto.HoTen;
            existingTenant.CCCD = dto.CCCD;
            existingTenant.SoDienThoai = dto.SoDienThoai;
            existingTenant.Email = dto.Email;
            existingTenant.GioiTinh = dto.GioiTinh;
            existingTenant.NgheNghiep = dto.NgheNghiep;
            existingTenant.GhiChu = dto.GhiChu;
            existingTenant.MaPhong = dto.MaPhong;
            existingTenant.NgaySinh = dto.NgaySinh;
            existingTenant.NgayCap = dto.NgayCap;
            existingTenant.NoiCap = dto.NoiCap;
            existingTenant.DiaChi = dto.DiaChi;
            existingTenant.NgayCapNhat = DateTime.Now;

            var success = await _tenantRepository.UpdateAsync(existingTenant);

            return new ValidationResult
            {
                IsValid = success,
                Message = success ? "✅ Cập nhật khách thuê thành công!" : "❌ Cập nhật thất bại!"
            };
        }

        public async Task<ValidationResult> DeleteTenantAsync(int maKhachThue)
        {
            var success = await _tenantRepository.DeleteAsync(maKhachThue);
            return new ValidationResult
            {
                IsValid = success,
                Message = success ? "🗑️ Đã xóa khách thuê thành công!" : "❌ Xóa khách thuê thất bại!"
            };
        }

        public async Task<List<TenantDto>> SearchTenantsByNameAsync(string keyword)
        {
            var tenants = await _tenantRepository.SearchByNameAsync(keyword);
            return tenants.Select(MapTenant).ToList();
        }

        private static TenantDto MapTenant(Tenant t)
        {
            return new TenantDto
            {
                MaKhachThue = t.MaKhachThue,
                MaPhong = t.MaPhong,
                HoTen = t.HoTen,
                CCCD = t.CCCD,
                SoDienThoai = t.SoDienThoai,
                Email = t.Email,
                GioiTinh = t.GioiTinh,
                NgheNghiep = t.NgheNghiep,
                GhiChu = t.GhiChu,
                NgaySinh = t.NgaySinh,
                NgayCap = t.NgayCap,
                NoiCap = t.NoiCap,
                DiaChi = t.DiaChi,
                NgayTao = t.NgayTao,
                NgayCapNhat = t.NgayCapNhat,
                TrangThai = t.TrangThai
            };
        }

        private static TenantAssetDto MapAsset(TenantAsset asset)
            => new TenantAssetDto
            {
                MaTaiSan = asset.MaTaiSan,
                LoaiTaiSan = asset.LoaiTaiSan,
                MoTa = asset.MoTa,
                PhiPhuThu = asset.PhiPhuThu
            };

        private static RoomTenantDto MapRoomTenant(RoomTenantInfo info)
        {
            var isActiveContract = string.Equals(info.TrangThaiHopDong, "Hiệu lực", StringComparison.OrdinalIgnoreCase)
                || string.Equals(info.TrangThaiHopDong, "Sắp hết hạn", StringComparison.OrdinalIgnoreCase);

            if (info.NgayKetThuc.HasValue && info.NgayKetThuc.Value.Date < DateTime.Today)
            {
                isActiveContract = false;
            }

            return new RoomTenantDto
            {
                MaKhachThue = info.MaNguoiThue,
                HoTen = info.HoTen,
                SoDienThoai = info.SoDienThoai,
                TrangThaiNguoiThue = info.TrangThaiNguoiThue,
                MaHopDong = info.MaHopDong,
                TrangThaiHopDong = info.TrangThaiHopDong,
                NgayBatDau = info.NgayBatDau,
                NgayKetThuc = info.NgayKetThuc,
                IsContractHolder = isActiveContract
            };
        }

        private async Task<TenantStayInfoDto> BuildStayInfoAsync(Tenant tenant, TenantStayInfo stayInfo)
        {
            var consistency = await EnsureStatusConsistencyAsync(tenant, stayInfo);

            return new TenantStayInfoDto
            {
                MaHopDong = stayInfo.MaHopDong,
                MaPhong = stayInfo.MaPhong,
                TenPhong = stayInfo.TenPhong,
                TrangThaiPhong = stayInfo.TrangThaiPhong,
                TrangThaiHopDong = stayInfo.TrangThaiHopDong,
                NgayBatDau = stayInfo.NgayBatDau,
                NgayKetThuc = stayInfo.NgayKetThuc,
                TienCoc = stayInfo.TienCoc,
                TrangThaiNguoiThue = tenant.TrangThai,
                SoDienThoai = stayInfo.SoDienThoai ?? tenant.SoDienThoai,
                ExpectedTenantStatus = consistency.ExpectedTenantStatus,
                ExpectedRoomStatus = consistency.ExpectedRoomStatus,
                IsSynchronized = !consistency.TenantUpdated && !consistency.RoomUpdated,
                ConsistencyMessage = consistency.Message
            };
        }

        private async Task<StatusConsistencyResult> EnsureStatusConsistencyAsync(Tenant tenant, TenantStayInfo? stayInfo)
        {
            var hasContract = stayInfo?.MaHopDong != null &&
                              !string.Equals(stayInfo.TrangThaiHopDong, "Hủy", StringComparison.OrdinalIgnoreCase);

            var contractStillActive = hasContract &&
                ((string.Equals(stayInfo!.TrangThaiHopDong, "Hiệu lực", StringComparison.OrdinalIgnoreCase)) ||
                 (stayInfo.NgayKetThuc.HasValue && stayInfo.NgayKetThuc.Value.Date >= DateTime.Today));

            string expectedTenantStatus;
            string expectedRoomStatus;

            if (contractStillActive)
            {
                var daysLeft = stayInfo!.NgayKetThuc.HasValue
                    ? (stayInfo.NgayKetThuc.Value.Date - DateTime.Today).TotalDays
                    : 999;

                expectedTenantStatus = daysLeft <= 7 ? "Sắp trả phòng" : "Đang ở";
                expectedRoomStatus = "Đang thuê";
            }
            else if (hasContract && stayInfo!.NgayBatDau.HasValue && stayInfo.NgayBatDau.Value.Date > DateTime.Today)
            {
                expectedTenantStatus = "Sắp trả phòng";
                expectedRoomStatus = "Dự kiến";
            }
            else
            {
                expectedTenantStatus = "Đã trả phòng";
                expectedRoomStatus = "Trống";
            }

            var tenantUpdated = false;
            if (!string.Equals(tenant.TrangThai, expectedTenantStatus, StringComparison.OrdinalIgnoreCase))
            {
                await _tenantRepository.UpdateTenantStatusAsync(tenant.MaKhachThue, expectedTenantStatus);
                tenant.TrangThai = expectedTenantStatus;
                tenantUpdated = true;
            }

            var roomUpdated = false;
            if (stayInfo?.MaPhong != null && !string.IsNullOrEmpty(expectedRoomStatus))
            {
                if (string.IsNullOrWhiteSpace(stayInfo.TrangThaiPhong) ||
                    !string.Equals(stayInfo.TrangThaiPhong, expectedRoomStatus, StringComparison.OrdinalIgnoreCase))
                {
                    await _roomRepository.UpdateStatusAsync(stayInfo.MaPhong.Value, expectedRoomStatus);
                    stayInfo.TrangThaiPhong = expectedRoomStatus;
                    roomUpdated = true;
                }
            }

            var messageParts = new List<string>();
            if (tenantUpdated)
            {
                messageParts.Add($"Đồng bộ trạng thái khách thuê thành \"{expectedTenantStatus}\".");
            }
            if (roomUpdated && stayInfo?.TenPhong != null)
            {
                messageParts.Add($"Cập nhật phòng {stayInfo.TenPhong} thành \"{expectedRoomStatus}\".");
            }
            if (!messageParts.Any())
            {
                messageParts.Add("Trạng thái người thuê và phòng đã được đồng bộ.");
            }

            return new StatusConsistencyResult
            {
                ExpectedTenantStatus = expectedTenantStatus,
                ExpectedRoomStatus = expectedRoomStatus,
                TenantUpdated = tenantUpdated,
                RoomUpdated = roomUpdated,
                Message = string.Join(" ", messageParts)
            };
        }

        private sealed class StatusConsistencyResult
        {
            public string ExpectedTenantStatus { get; set; } = string.Empty;
            public string ExpectedRoomStatus { get; set; } = string.Empty;
            public bool TenantUpdated { get; set; }
            public bool RoomUpdated { get; set; }
            public string Message { get; set; } = string.Empty;
        }
    }
}