using QLKDPhongTro.BusinessLayer.DTOs;
using QLKDPhongTro.DataLayer.Models;
using QLKDPhongTro.DataLayer.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;

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
            
            // Lấy hợp đồng hiện tại của phòng để xác định người đứng tên
            var contractController = new ContractController(new DataLayer.Repositories.ContractRepository());
            var activeContract = await contractController.GetActiveContractByRoomIdAsync(maPhong);
            int? contractHolderMaNguoiThue = activeContract?.MaNguoiThue;
            
            return tenants.Select(info => MapRoomTenant(info, contractHolderMaNguoiThue)).ToList();
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
                TrangThai = string.IsNullOrWhiteSpace(dto.TrangThai) ? "Đang ở" : dto.TrangThai,
                NgaySinh = dto.NgaySinh,
                NgayCap = dto.NgayCap,
                NoiCap = dto.NoiCap,
                DiaChi = dto.DiaChi,
                NgayTao = DateTime.Now,
                NgayCapNhat = DateTime.Now
            };

            var success = await _tenantRepository.CreateAsync(tenant);
            
            // Sau khi tạo người thuê thành công
            if (success && dto.MaPhong.HasValue)
            {
                var maPhong = dto.MaPhong.Value;
                
                // Kiểm tra xem phòng đã có hợp đồng chưa
                var contractController = new ContractController(new DataLayer.Repositories.ContractRepository());
                var activeContract = await contractController.GetActiveContractByRoomIdAsync(maPhong);
                
                // Nếu người thuê ở trạng thái "Đặt cọc" thì phòng chuyển sang "Dự kiến"
                if (string.Equals(dto.TrangThai, "Đặt cọc", StringComparison.OrdinalIgnoreCase))
                {
                    await _roomRepository.UpdateStatusAsync(maPhong, "Dự kiến");
                }
                else if (activeContract != null)
                {
                    // Nếu đã có hợp đồng, đảm bảo trạng thái là "Đang thuê"
                    await _roomRepository.UpdateStatusAsync(maPhong, "Đang thuê");
                }
                else
                {
                    // Nếu chưa có hợp đồng và không phải "Đặt cọc", đổi trạng thái phòng sang "Đang thuê" (nếu đang "Trống")
                    var room = await _roomRepository.GetByIdAsync(maPhong);
                    if (room != null && string.Equals(room.TrangThai, "Trống", StringComparison.OrdinalIgnoreCase))
                    {
                        await _roomRepository.UpdateStatusAsync(maPhong, "Đang thuê");
                    }
                }
            }
            
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

            // Lấy thông tin lưu trú hiện tại để kiểm tra hợp đồng/phòng
            var stayInfo = await _tenantRepository.GetCurrentStayInfoAsync(dto.MaKhachThue);
            var hasContract = stayInfo?.MaHopDong != null &&
                              !string.Equals(stayInfo.TrangThaiHopDong, "Hủy", StringComparison.OrdinalIgnoreCase);
            var contractStillActive = hasContract &&
                ((string.Equals(stayInfo!.TrangThaiHopDong, "Hiệu lực", StringComparison.OrdinalIgnoreCase)) ||
                 (stayInfo.NgayKetThuc.HasValue && stayInfo.NgayKetThuc.Value.Date >= DateTime.Today));

            // Nếu người thuê đang đứng tên hợp đồng còn hiệu lực thì không cho phép chuyển sang "Đã trả phòng"
            if (contractStillActive &&
                !string.IsNullOrWhiteSpace(dto.TrangThai) &&
                string.Equals(dto.TrangThai, "Đã trả phòng", StringComparison.OrdinalIgnoreCase))
            {
                return new ValidationResult
                {
                    IsValid = false,
                    Message = "❌ Không thể chuyển trạng thái sang 'Đã trả phòng' khi hợp đồng vẫn còn hiệu lực. Vui lòng kết thúc/hủy hợp đồng trước."
                };
            }

            existingTenant.HoTen = dto.HoTen;
            existingTenant.CCCD = dto.CCCD;
            existingTenant.SoDienThoai = dto.SoDienThoai;
            existingTenant.Email = dto.Email;
            existingTenant.GioiTinh = dto.GioiTinh;
            existingTenant.NgheNghiep = dto.NgheNghiep;
            existingTenant.GhiChu = dto.GhiChu;
            if (!string.IsNullOrWhiteSpace(dto.TrangThai))
            {
                existingTenant.TrangThai = dto.TrangThai;
            }
            existingTenant.MaPhong = dto.MaPhong;
            existingTenant.NgaySinh = dto.NgaySinh;
            existingTenant.NgayCap = dto.NgayCap;
            existingTenant.NoiCap = dto.NoiCap;
            existingTenant.DiaChi = dto.DiaChi;
            existingTenant.NgayCapNhat = DateTime.Now;

            var success = await _tenantRepository.UpdateAsync(existingTenant);

            // Sau khi cập nhật trạng thái người thuê, đồng bộ trạng thái phòng (không ép hợp đồng)
            if (success)
            {
                await UpdateRoomStatusAfterTenantChangeAsync(existingTenant, stayInfo);
            }

            return new ValidationResult
            {
                IsValid = success,
                Message = success ? "✅ Cập nhật khách thuê thành công!" : "❌ Cập nhật thất bại!"
            };
        }

        public async Task<DeleteTenantResult> DeleteTenantAsync(int maKhachThue)
        {
            // Lấy thông tin người thuê trước khi xóa
            var tenant = await _tenantRepository.GetByIdAsync(maKhachThue);
            if (tenant == null)
            {
                return new DeleteTenantResult
                {
                    IsValid = false,
                    Message = "❌ Không tìm thấy khách thuê để xóa!"
                };
            }

            var maPhong = tenant.MaPhong;
            var result = new DeleteTenantResult
            {
                MaPhong = maPhong
            };

            // Kiểm tra xem người thuê có phải là người đứng tên hợp đồng không
            ContractDto? activeContract = null;
            if (maPhong.HasValue)
            {
                var contractController = new ContractController(new DataLayer.Repositories.ContractRepository());
                activeContract = await contractController.GetActiveContractByRoomIdAsync(maPhong.Value);
                
                // Kiểm tra xem người thuê có phải là người đứng tên hợp đồng không
                if (activeContract != null && activeContract.MaNguoiThue == maKhachThue)
                {
                    // Lấy danh sách người thuê còn lại trong phòng (trước khi xóa)
                    var allRoomTenants = await _tenantRepository.GetTenantsByRoomIdAsync(maPhong.Value);
                    int? contractHolderMaNguoiThue = activeContract.MaNguoiThue;
                    var remainingTenants = allRoomTenants
                        .Where(t => t.MaNguoiThue != maKhachThue && 
                                   string.Equals(t.TrangThaiNguoiThue, "Đang ở", StringComparison.OrdinalIgnoreCase))
                        .Select(info => MapRoomTenant(info, contractHolderMaNguoiThue))
                        .ToList();

                    // Nếu còn người thuê khác, cần tạo hợp đồng mới
                    if (remainingTenants.Any())
                    {
                        result.RequiresNewContract = true;
                        result.RemainingTenants = remainingTenants;
                        result.OldContract = activeContract;
                    }
                }
            }

            // Thực hiện xóa người thuê
            var success = await _tenantRepository.DeleteAsync(maKhachThue);
            
            if (!success)
            {
                result.IsValid = false;
                result.Message = "❌ Xóa khách thuê thất bại!";
                return result;
            }

            // Sau khi xóa thành công, kiểm tra và cập nhật trạng thái phòng
            if (maPhong.HasValue)
            {
                // Kiểm tra phòng còn người thuê không
                var remainingTenantsAfterDelete = await _tenantRepository.GetTenantsByRoomIdAsync(maPhong.Value);
                var hasActiveTenants = remainingTenantsAfterDelete.Any(t =>
                    !string.Equals(t.TrangThaiNguoiThue, "Đã trả phòng", StringComparison.OrdinalIgnoreCase));

                if (!hasActiveTenants)
                {
                    // Không còn người thuê, chuyển phòng sang trạng thái "Trống"
                    await _roomRepository.UpdateStatusAsync(maPhong.Value, "Trống");
                    result.Message = "🗑️ Đã xóa khách thuê thành công! Phòng đã được chuyển sang trạng thái Trống.";
                }
                else
                {
                    result.Message = "🗑️ Đã xóa khách thuê thành công!";
                }
            }
            else
            {
                result.Message = "🗑️ Đã xóa khách thuê thành công!";
            }

            result.IsValid = true;
            return result;
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

        public async Task<ValidationResult> CreateAssetAsync(TenantAssetDto dto, int maNguoiThue)
        {
            var asset = new TenantAsset
            {
                MaNguoiThue = maNguoiThue,
                LoaiTaiSan = dto.LoaiTaiSan,
                MoTa = dto.MoTa,
                PhiPhuThu = dto.PhiPhuThu
            };

            var success = await _tenantRepository.CreateAssetAsync(asset);
            return new ValidationResult
            {
                IsValid = success,
                Message = success ? "✅ Thêm tài sản thành công!" : "❌ Thêm tài sản thất bại!"
            };
        }

        public async Task<ValidationResult> UpdateAssetAsync(TenantAssetDto dto)
        {
            var asset = new TenantAsset
            {
                MaTaiSan = dto.MaTaiSan,
                LoaiTaiSan = dto.LoaiTaiSan,
                MoTa = dto.MoTa,
                PhiPhuThu = dto.PhiPhuThu
            };

            var success = await _tenantRepository.UpdateAssetAsync(asset);
            return new ValidationResult
            {
                IsValid = success,
                Message = success ? "✅ Cập nhật tài sản thành công!" : "❌ Cập nhật tài sản thất bại!"
            };
        }

        public async Task<ValidationResult> DeleteAssetAsync(int maTaiSan)
        {
            var success = await _tenantRepository.DeleteAssetAsync(maTaiSan);
            return new ValidationResult
            {
                IsValid = success,
                Message = success ? "🗑️ Đã xóa tài sản thành công!" : "❌ Xóa tài sản thất bại!"
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

        private static RoomTenantDto MapRoomTenant(RoomTenantInfo info, int? contractHolderMaNguoiThue = null)
        {
            // Kiểm tra xem người thuê này có phải là người đứng tên hợp đồng không
            // Nếu có contractHolderMaNguoiThue, so sánh với MaNguoiThue của người thuê
            bool isContractHolder = false;
            if (contractHolderMaNguoiThue.HasValue)
            {
                isContractHolder = info.MaNguoiThue == contractHolderMaNguoiThue.Value;
            }
            else
            {
                // Fallback: Nếu không có thông tin hợp đồng, kiểm tra xem người thuê có hợp đồng hiệu lực không
                var isActiveContract = string.Equals(info.TrangThaiHopDong, "Hiệu lực", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(info.TrangThaiHopDong, "Sắp hết hạn", StringComparison.OrdinalIgnoreCase);

                if (info.NgayKetThuc.HasValue && info.NgayKetThuc.Value.Date < DateTime.Today)
                {
                    isActiveContract = false;
                }
                
                isContractHolder = isActiveContract && info.MaHopDong.HasValue;
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
                IsContractHolder = isContractHolder
            };
        }

        private async Task<TenantStayInfoDto> BuildStayInfoAsync(Tenant tenant, TenantStayInfo stayInfo)
        {
            if (tenant.MaPhong.HasValue)
            {
                stayInfo.MaPhong = tenant.MaPhong;
                var room = await _roomRepository.GetByIdAsync(tenant.MaPhong.Value);
                stayInfo.TenPhong = room?.TenPhong ?? stayInfo.TenPhong;
                stayInfo.TrangThaiPhong = room?.TrangThai ?? stayInfo.TrangThaiPhong;
            }

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

        /// <summary>
        /// Đồng bộ trạng thái phòng sau khi trạng thái người thuê thay đổi.
        /// Đảm bảo: 
        /// - Khi người thuê ở trạng thái "Đặt cọc" thì phòng chuyển sang "Dự kiến"
        /// - Không set phòng Trống nếu vẫn còn hợp đồng còn hiệu lực.
        /// - Khi tất cả khách thuê trong phòng đã trả phòng và không còn hợp đồng active thì set phòng Trống.
        /// </summary>
        private async Task UpdateRoomStatusAfterTenantChangeAsync(Tenant tenant, TenantStayInfo? stayInfo)
        {
            if (!tenant.MaPhong.HasValue)
            {
                return;
            }

            var maPhong = tenant.MaPhong.Value;

            // Nếu còn hợp đồng còn hiệu lực cho người này thì giữ phòng ở trạng thái đang thuê
            var hasContract = stayInfo?.MaHopDong != null &&
                              !string.Equals(stayInfo.TrangThaiHopDong, "Hủy", StringComparison.OrdinalIgnoreCase);
            var contractStillActive = hasContract &&
                ((string.Equals(stayInfo!.TrangThaiHopDong, "Hiệu lực", StringComparison.OrdinalIgnoreCase)) ||
                 (string.Equals(stayInfo.TrangThaiHopDong, "Sắp hết hạn", StringComparison.OrdinalIgnoreCase)) ||
                 (stayInfo.NgayKetThuc.HasValue && stayInfo.NgayKetThuc.Value.Date >= DateTime.Today));

            if (contractStillActive)
            {
                // Phòng có hợp đồng còn hiệu lực => luôn là Đang thuê
                await _roomRepository.UpdateStatusAsync(maPhong, "Đang thuê");
                return;
            }

            // Kiểm tra xem có người thuê nào trong phòng ở trạng thái "Đặt cọc" không
            var roomTenants = await _tenantRepository.GetTenantsByRoomIdAsync(maPhong);
            var hasDepositTenant = roomTenants.Any(t =>
                string.Equals(t.TrangThaiNguoiThue, "Đặt cọc", StringComparison.OrdinalIgnoreCase));

            if (hasDepositTenant)
            {
                // Nếu có người thuê "Đặt cọc" thì phòng chuyển sang "Dự kiến"
                await _roomRepository.UpdateStatusAsync(maPhong, "Dự kiến");
                return;
            }

            // Không còn hợp đồng active và không có người "Đặt cọc":
            // Kiểm tra còn khách thuê "Đang ở" trong phòng không
            var hasActiveTenant = roomTenants.Any(t =>
                !string.Equals(t.TrangThaiNguoiThue, "Đã trả phòng", StringComparison.OrdinalIgnoreCase));

            if (hasActiveTenant)
            {
                await _roomRepository.UpdateStatusAsync(maPhong, "Đang thuê");
            }
            else
            {
                await _roomRepository.UpdateStatusAsync(maPhong, "Trống");
            }
        }

        private Task<StatusConsistencyResult> EnsureStatusConsistencyAsync(Tenant tenant, TenantStayInfo? stayInfo)
        {
            var hasContract = stayInfo?.MaHopDong != null &&
                              !string.Equals(stayInfo.TrangThaiHopDong, "Hủy", StringComparison.OrdinalIgnoreCase);

            var contractStillActive = hasContract &&
                ((string.Equals(stayInfo!.TrangThaiHopDong, "Hiệu lực", StringComparison.OrdinalIgnoreCase)) ||
                 (string.Equals(stayInfo.TrangThaiHopDong, "Sắp hết hạn", StringComparison.OrdinalIgnoreCase)) ||
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
                expectedTenantStatus = tenant.TrangThai;
                expectedRoomStatus = "Trống";
            }

            // Không tự động overwrite trạng thái người thuê/phòng ở đây – chỉ tính toán và gợi ý
            var tenantUpdated = !string.Equals(tenant.TrangThai, expectedTenantStatus, StringComparison.OrdinalIgnoreCase);

            bool roomUpdated = false;
            if (stayInfo?.MaPhong != null && !string.IsNullOrEmpty(expectedRoomStatus))
            {
                roomUpdated = !string.Equals(stayInfo.TrangThaiPhong, expectedRoomStatus, StringComparison.OrdinalIgnoreCase);
            }

            var messageParts = new List<string>();
            if (tenantUpdated)
            {
                messageParts.Add($"Đề xuất trạng thái khách thuê nên là \"{expectedTenantStatus}\".");
            }
            if (roomUpdated && stayInfo?.TenPhong != null)
            {
                messageParts.Add($"Đề xuất phòng {stayInfo.TenPhong} nên ở trạng thái \"{expectedRoomStatus}\".");
            }
            if (!messageParts.Any())
            {
                messageParts.Add("Trạng thái người thuê và phòng đang đồng bộ.");
            }

            var result = new StatusConsistencyResult
            {
                ExpectedTenantStatus = expectedTenantStatus,
                ExpectedRoomStatus = expectedRoomStatus,
                TenantUpdated = tenantUpdated,
                RoomUpdated = roomUpdated,
                Message = string.Join(" ", messageParts)
            };

            return Task.FromResult(result);
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