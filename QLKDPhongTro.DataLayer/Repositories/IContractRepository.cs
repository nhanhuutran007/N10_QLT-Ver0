using QLKDPhongTro.DataLayer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QLKDPhongTro.DataLayer.Repositories
{
    public interface IContractRepository
    {
        Task<List<Contract>> GetAllHopDongAsync();
        Task AddHopDongAsync(Contract contract);
        Task UpdateHopDongAsync(Contract contract);
        Task DeleteHopDongAsync(int maHopDong);
        Task<List<Contract>> GetExpiringContractsAsync(int days); // Lấy hợp đồng sắp hết hạn
        Task<Contract?> GetByIdAsync(int maHopDong); // Lấy theo ID
        Task<List<Contract>> GetActiveContractsAsync(); // Lấy các hợp đồng còn hiệu lực
        Task<List<Contract>> GetActiveContractsByTenantAsync(int maNguoiThue); // Lấy hợp đồng còn hiệu lực theo khách hàng
    }
}
