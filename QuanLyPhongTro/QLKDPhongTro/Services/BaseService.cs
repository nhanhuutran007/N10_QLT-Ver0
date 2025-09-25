using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QLKDPhongTro.Services
{
    /// <summary>
    /// Lớp cơ sở cho tất cả các Service
    /// </summary>
    /// <typeparam name="T">Kiểu Model</typeparam>
    public abstract class BaseService<T> where T : class
    {
        protected string connectionString;

        public BaseService()
        {
            // TODO: Đọc connection string từ config
            connectionString = "Data Source=.;Initial Catalog=QLKDPhongTro;Integrated Security=True;TrustServerCertificate=True;Encrypt=False";
        }

        public abstract Task<List<T>> GetAllAsync();
        public abstract Task<T> GetByIdAsync(string id);
        public abstract Task<bool> CreateAsync(T entity);
        public abstract Task<bool> UpdateAsync(T entity);
        public abstract Task<bool> DeleteAsync(string id);
    }
}
