using QLKDPhongTro.BusinessLayer.DTOs;
using QLKDPhongTro.DataLayer.Models;
using QLKDPhongTro.DataLayer.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QLKDPhongTro.BusinessLayer.Controllers
{
    /// <summary>
    /// Controller xử lý logic nghiệp vụ cho Tenant
    /// </summary>
    public class TenantController
    {
        private readonly ITenantRepository _tenantRepository;

        public TenantController(ITenantRepository tenantRepository)
        {
            _tenantRepository = tenantRepository;
        }

        /// <summary>
        /// Lấy tất cả khách thuê
        /// </summary>
        public async Task<List<TenantDto>> GetAllTenantsAsync()
        {
            var tenants = await _tenantRepository.GetAllAsync();
            return tenants.Select(t => new TenantDto
            {
                MaKhachThue = t.MaKhachThue,
                HoTen = t.HoTen,
                CCCD = t.CCCD,
                SoDienThoai = t.SoDienThoai,
                Email = t.Email,
                DiaChi = t.DiaChi,
                NgaySinh = t.NgaySinh,
                GioiTinh = t.GioiTinh,
                NgheNghiep = t.NgheNghiep,
                GhiChu = t.GhiChu,
                NgayTao = t.NgayTao,
                NgayCapNhat = t.NgayCapNhat
            }).ToList();
        }

        /// <summary>
        /// Lấy khách thuê theo ID
        /// </summary>
        public async Task<TenantDto?> GetTenantByIdAsync(int maKhachThue)
        {
            var tenant = await _tenantRepository.GetByIdAsync(maKhachThue);
            if (tenant == null) return null;

            return new TenantDto
            {
                MaKhachThue = tenant.MaKhachThue,
                HoTen = tenant.HoTen,
                CCCD = tenant.CCCD,
                SoDienThoai = tenant.SoDienThoai,
                Email = tenant.Email,
                DiaChi = tenant.DiaChi,
                NgaySinh = tenant.NgaySinh,
                GioiTinh = tenant.GioiTinh,
                NgheNghiep = tenant.NgheNghiep,
                GhiChu = tenant.GhiChu,
                NgayTao = tenant.NgayTao,
                NgayCapNhat = tenant.NgayCapNhat
            };
        }

        /// <summary>
        /// Tạo khách thuê mới
        /// </summary>
        public async Task<ValidationResult> CreateTenantAsync(TenantDto dto)
        {
            // Kiểm tra CCCD đã tồn tại chưa
            if (await _tenantRepository.IsCCCDExistsAsync(dto.CCCD))
            {
                return new ValidationResult
                {
                    IsValid = false,
                    Message = "CCCD đã tồn tại trong hệ thống"
                };
            }

            var tenant = new Tenant
            {
                HoTen = dto.HoTen,
                CCCD = dto.CCCD,
                SoDienThoai = dto.SoDienThoai,
                Email = dto.Email,
                DiaChi = dto.DiaChi,
                NgaySinh = dto.NgaySinh,
                GioiTinh = dto.GioiTinh,
                NgheNghiep = dto.NgheNghiep,
                GhiChu = dto.GhiChu,
                NgayTao = DateTime.Now,
                NgayCapNhat = DateTime.Now
            };

            var success = await _tenantRepository.CreateAsync(tenant);
            return new ValidationResult
            {
                IsValid = success,
                Message = success ? "Thêm khách thuê thành công" : "Thêm khách thuê thất bại"
            };
        }

        /// <summary>
        /// Cập nhật thông tin khách thuê
        /// </summary>
        public async Task<ValidationResult> UpdateTenantAsync(TenantDto dto)
        {
            // Kiểm tra CCCD đã tồn tại chưa (trừ chính nó)
            if (await _tenantRepository.IsCCCDExistsAsync(dto.CCCD, dto.MaKhachThue))
            {
                return new ValidationResult
                {
                    IsValid = false,
                    Message = "CCCD đã tồn tại trong hệ thống"
                };
            }

            var tenant = new Tenant
            {
                MaKhachThue = dto.MaKhachThue,
                HoTen = dto.HoTen,
                CCCD = dto.CCCD,
                SoDienThoai = dto.SoDienThoai,
                Email = dto.Email,
                DiaChi = dto.DiaChi,
                NgaySinh = dto.NgaySinh,
                GioiTinh = dto.GioiTinh,
                NgheNghiep = dto.NgheNghiep,
                GhiChu = dto.GhiChu,
                NgayCapNhat = DateTime.Now
            };

            var success = await _tenantRepository.UpdateAsync(tenant);
            return new ValidationResult
            {
                IsValid = success,
                Message = success ? "Cập nhật khách thuê thành công" : "Cập nhật khách thuê thất bại"
            };
        }

        /// <summary>
        /// Xóa khách thuê
        /// </summary>
        public async Task<ValidationResult> DeleteTenantAsync(int maKhachThue)
        {
            var success = await _tenantRepository.DeleteAsync(maKhachThue);
            return new ValidationResult
            {
                IsValid = success,
                Message = success ? "Xóa khách thuê thành công" : "Xóa khách thuê thất bại"
            };
        }

        /// <summary>
        /// Tìm kiếm khách thuê theo tên
        /// </summary>
        public async Task<List<TenantDto>> SearchTenantsByNameAsync(string name)
        {
            var tenants = await _tenantRepository.SearchByNameAsync(name);
            return tenants.Select(t => new TenantDto
            {
                MaKhachThue = t.MaKhachThue,
                HoTen = t.HoTen,
                CCCD = t.CCCD,
                SoDienThoai = t.SoDienThoai,
                Email = t.Email,
                DiaChi = t.DiaChi,
                NgaySinh = t.NgaySinh,
                GioiTinh = t.GioiTinh,
                NgheNghiep = t.NgheNghiep,
                GhiChu = t.GhiChu,
                NgayTao = t.NgayTao,
                NgayCapNhat = t.NgayCapNhat
            }).ToList();
        }
    }
}
