using QLKDPhongTro.DataLayer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QLKDPhongTro.DataLayer.Repositories
{
    public interface IContractRepository
    {
        Task<List<Contract>> GetAllHopDongAsync();
        Task<List<Contract>> GetAllByMaNhaAsync(int maNha);
        Task<int> AddHopDongAsync(Contract contract);
        Task UpdateHopDongAsync(Contract contract);
        Task DeleteHopDongAsync(int maHopDong);
        Task<List<Contract>> GetExpiringContractsAsync(int days); // Lấy hợp đồng sắp hết hạn

        Task<List<Contract>> GetContractsExpiringInExactDaysAsync(int exactDays); // Lấy hợp đồng còn đúng số ngày cụ thể
        Task<Contract?> GetByIdAsync(int maHopDong); // Lấy theo ID
        Task<List<Contract>> GetActiveContractsAsync(); // Lấy các hợp đồng còn hiệu lực
        Task<List<Contract>> GetActiveContractsByTenantAsync(int maNguoiThue); // Lấy hợp đồng còn hiệu lực theo khách hàng
        Task<(int MaNguoiThue, string HoTen, decimal TienCoc, string TrangThai)?> GetMostRecentTenantWithDepositAsync();
        Task<List<(int MaNguoiThue, string HoTen, decimal TienCoc, string TrangThai)>> GetMostRecentTenantsWithDepositAsync(int count);
    }
}
