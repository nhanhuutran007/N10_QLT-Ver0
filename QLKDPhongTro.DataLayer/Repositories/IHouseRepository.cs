using System.Collections.Generic;
using System.Threading.Tasks;
using QLKDPhongTro.DataLayer.Models;

namespace QLKDPhongTro.DataLayer.Repositories
{
    /// <summary>
    /// Interface cho House Repository
    /// </summary>
    public interface IHouseRepository
    {
        Task<List<House>> GetAllAsync();
        Task<House?> GetByIdAsync(int maNha);
        Task<bool> CreateAsync(House house);
        Task<bool> UpdateAsync(House house);
        Task<bool> DeleteAsync(int maNha);
    }
}
