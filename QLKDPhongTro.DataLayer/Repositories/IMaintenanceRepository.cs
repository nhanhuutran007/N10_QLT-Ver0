using QLKDPhongTro.DataLayer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QLKDPhongTro.DataLayer.Repositories
{
    public interface IMaintenanceRepository
    {
        Task<List<MaintenanceIncident>> GetAllAsync();
        Task<MaintenanceIncident?> GetByIdAsync(int id);
        Task AddAsync(MaintenanceIncident incident);
        Task UpdateAsync(MaintenanceIncident incident);
        Task DeleteAsync(int id);
        Task MarkAsDeletedFromSyncAsync(int maPhong, string moTaSuCo, DateTime ngayBaoCao);
        Task<bool> IsDeletedFromSyncAsync(int maPhong, string moTaSuCo, DateTime ngayBaoCao);
    }
}
