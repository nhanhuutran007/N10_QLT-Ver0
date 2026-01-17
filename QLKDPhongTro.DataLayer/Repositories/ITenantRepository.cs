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
        /// Lấy tất cả khách thuê theo mã nhà
        /// </summary>
        Task<List<Tenant>> GetAllByMaNhaAsync(int maNha);

        /// <summary>
        /// Tìm khách thuê theo Email
        /// </summary>
        Task<Tenant?> GetByEmailAsync(string email);

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

        /// <summary>
        /// Lấy danh sách tài sản người thuê đang sở hữu
        /// </summary>
        Task<List<TenantAsset>> GetAssetsAsync(int maNguoiThue);

        /// <summary>
        /// Tạo tài sản mới cho người thuê
        /// </summary>
        Task<bool> CreateAssetAsync(TenantAsset asset);

        /// <summary>
        /// Cập nhật thông tin tài sản
        /// </summary>
        Task<bool> UpdateAssetAsync(TenantAsset asset);

        /// <summary>
        /// Xóa tài sản
        /// </summary>
        Task<bool> DeleteAssetAsync(int maTaiSan);

        /// <summary>
        /// Lấy thông tin lưu trú hiện tại (phòng, hợp đồng, trạng thái)
        /// </summary>
        Task<TenantStayInfo?> GetCurrentStayInfoAsync(int maNguoiThue);

        /// <summary>
        /// Lấy danh sách người thuê đang/chuẩn bị ở một phòng cụ thể
        /// </summary>
        Task<List<RoomTenantInfo>> GetTenantsByRoomIdAsync(int maPhong);

        /// <summary>
        /// Cập nhật nhanh trạng thái người thuê
        /// </summary>
        Task<bool> UpdateTenantStatusAsync(int maNguoiThue, string trangThai);
    }
}
