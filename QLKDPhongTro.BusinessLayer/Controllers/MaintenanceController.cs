using QLKDPhongTro.DataLayer.Models;
using QLKDPhongTro.DataLayer.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QLKDPhongTro.BusinessLayer.Controllers
{
    public class MaintenanceController
    {
        private readonly IMaintenanceRepository _repo;

        public MaintenanceController(IMaintenanceRepository repo)
        {
            _repo = repo;
        }

        public Task<List<MaintenanceIncident>> GetAllAsync() => _repo.GetAllAsync();
        public Task<MaintenanceIncident?> GetByIdAsync(int id) => _repo.GetByIdAsync(id);
        public Task AddAsync(MaintenanceIncident incident) => _repo.AddAsync(incident);
        public Task UpdateAsync(MaintenanceIncident incident) => _repo.UpdateAsync(incident);
        public Task DeleteAsync(int id) => _repo.DeleteAsync(id);
    }
}
