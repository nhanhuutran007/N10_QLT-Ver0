using QLKDPhongTro.DataLayer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QLKDPhongTro.DataLayer.Repositories
{
    /// <summary>
    /// Interface cho repository xử lý dữ liệu Tenant
    /// </summary>
    public interface ITenantRepository
    {
        /// <summary>
        /// Lấy tất cả khách thuê
        /// </summary>
        Task<List<Tenant>> GetAllAsync();

        /// <summary>
        /// Lấy khách thuê theo ID
        /// </summary>
        Task<Tenant?> GetByIdAsync(int maKhachThue);

        /// <summary>
        /// Tạo khách thuê mới
        /// </summary>
        Task<bool> CreateAsync(Tenant tenant);

        /// <summary>
        /// Cập nhật thông tin khách thuê
        /// </summary>
        Task<bool> UpdateAsync(Tenant tenant);

        /// <summary>
        /// Xóa khách thuê
        /// </summary>
        Task<bool> DeleteAsync(int maKhachThue);

        /// <summary>
        /// Tìm kiếm khách thuê theo tên
        /// </summary>
        Task<List<Tenant>> SearchByNameAsync(string name);

        /// <summary>
        /// Kiểm tra CCCD đã tồn tại chưa
        /// </summary>
        Task<bool> IsCCCDExistsAsync(string cccd, int excludeId = 0);
    }
}
