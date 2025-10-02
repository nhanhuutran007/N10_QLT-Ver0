using System.Collections.Generic;
using System.Threading.Tasks;
using QLKDPhongTro.DataLayer.Models;

namespace QLKDPhongTro.DataLayer.Repositories
{

    public interface IRentedRoomRepository
    {
        Task<List<RentedRoom>> GetAllAsync();
        Task<RentedRoom?> GetByIdAsync(int maPhong);
        Task<bool> IsRoomExistsAsync(int maPhong);
        Task<bool> CreateAsync(RentedRoom room);
        Task<bool> UpdateAsync(RentedRoom room);
        Task<bool> DeleteAsync(int maPhong);
        Task<bool> UpdateStatusAsync(int maPhong, string trangThai);
    }
}
