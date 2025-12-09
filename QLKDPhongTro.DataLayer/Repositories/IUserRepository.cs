using System.Collections.Generic;
using System.Threading.Tasks;
using QLKDPhongTro.DataLayer.Models;

namespace QLKDPhongTro.DataLayer.Repositories
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllAsync();
        Task<User?> GetByIdAsync(string id);
        Task<User?> GetByMaAdminAsync(int maAdmin);
        Task<User?> GetByUsernameAsync(string username);
        Task<bool> CreateAsync(User user);
        Task<bool> UpdateAsync(User user);
        Task<bool> UpdateProfileAsync(User user);
        Task<bool> UpdatePasswordAsync(int maAdmin, string oldPassword, string newPassword);
        Task<bool> DeleteAsync(string id);
        Task<bool> IsUsernameExistsAsync(string username);
        Task<bool> IsEmailExistsAsync(string email);
        Task<bool> IsMaNhaExistsAsync(int maNha);
        Task<User?> LoginAsync(string username, string password);
        Task<bool> RegisterAsync(User user);
        Task<User?> GetByEmailAsync(string email);
        Task<List<User>> GetByHouseIdAsync(int maNha);
    }
}
