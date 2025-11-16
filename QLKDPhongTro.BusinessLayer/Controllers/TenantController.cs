using QLKDPhongTro.BusinessLayer.DTOs;
using QLKDPhongTro.DataLayer.Models;
using QLKDPhongTro.DataLayer.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QLKDPhongTro.BusinessLayer.Controllers
{
    public class TenantController
    {
        private readonly ITenantRepository _tenantRepository;

        public TenantController(ITenantRepository tenantRepository)
        {
            _tenantRepository = tenantRepository;
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
            return tenants.Select(t => new TenantDto
            {
                MaKhachThue = t.MaKhachThue,
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
                NgayCapNhat = t.NgayCapNhat
            }).ToList();
        }

        public async Task<ValidationResult> CreateTenantAsync(TenantDto dto)
        {
            if (await _tenantRepository.IsCCCDExistsAsync(dto.CCCD))
            {
                return new ValidationResult { IsValid = false, Message = "CCCD đã tồn tại!" };
            }

            var tenant = new Tenant
            {
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
                NgayTao = System.DateTime.Now,
                NgayCapNhat = System.DateTime.Now
            };

            var success = await _tenantRepository.CreateAsync(tenant);
            return new ValidationResult
            {
                IsValid = success,
                Message = success ? "✅ Thêm khách thuê thành công!" : "❌ Thêm khách thuê thất bại!"
            };
        }

        // ⭐⭐⭐ BẮT ĐẦU SỬA LỖI (Logic Update) ⭐⭐⭐
        public async Task<ValidationResult> UpdateTenantAsync(TenantDto dto)
        {
            // 1. Kiểm tra CCCD
            if (await _tenantRepository.IsCCCDExistsAsync(dto.CCCD, dto.MaKhachThue))
            {
                return new ValidationResult { IsValid = false, Message = "CCCD đã tồn tại!" };
            }

            // 2. LẤY đối tượng cũ từ Database (Giả sử Repository có GetByIdAsync)
            var existingTenant = await _tenantRepository.GetByIdAsync(dto.MaKhachThue);
            if (existingTenant == null)
            {
                return new ValidationResult { IsValid = false, Message = "❌ Không tìm thấy khách thuê để cập nhật!" };
            }

            // 3. CẬP NHẬT các trường từ DTO (form) lên đối tượng cũ
            existingTenant.HoTen = dto.HoTen;
            existingTenant.CCCD = dto.CCCD;
            existingTenant.SoDienThoai = dto.SoDienThoai;
            existingTenant.Email = dto.Email;
            existingTenant.GioiTinh = dto.GioiTinh;
            existingTenant.NgheNghiep = dto.NgheNghiep;
            existingTenant.GhiChu = dto.GhiChu;
            existingTenant.NgaySinh = dto.NgaySinh;
            existingTenant.NgayCap = dto.NgayCap;
            existingTenant.NoiCap = dto.NoiCap;
            existingTenant.DiaChi = dto.DiaChi;
            existingTenant.NgayCapNhat = System.DateTime.Now;

            // Lưu ý: existingTenant.NgayTao và các trường khác không có trên form
            // sẽ được bảo toàn.

            // 4. LƯU LẠI đối tượng đã cập nhật
            var success = await _tenantRepository.UpdateAsync(existingTenant);

            return new ValidationResult
            {
                IsValid = success,
                Message = success ? "✅ Cập nhật khách thuê thành công!" : "❌ Cập nhật thất bại!"
            };
        }
        // ⭐⭐⭐ KẾT THÚC SỬA LỖI ⭐⭐⭐

        // 🗑️ Xóa khách thuê theo mã
        public async Task<ValidationResult> DeleteTenantAsync(int maKhachThue)
        {
            var success = await _tenantRepository.DeleteAsync(maKhachThue);
            return new ValidationResult
            {
                IsValid = success,
                Message = success ? "🗑️ Đã xóa khách thuê thành công!" : "❌ Xóa khách thuê thất bại!"
            };
        }

        // 🔍 Tìm kiếm khách thuê theo tên
        public async Task<List<TenantDto>> SearchTenantsByNameAsync(string keyword)
        {
            var tenants = await _tenantRepository.SearchByNameAsync(keyword);
            return tenants.Select(t => new TenantDto
            {
                MaKhachThue = t.MaKhachThue,
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
                NgayCapNhat = t.NgayCapNhat
            }).ToList();
        }
    }
}