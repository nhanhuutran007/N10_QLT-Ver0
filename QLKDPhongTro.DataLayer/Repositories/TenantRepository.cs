using QLKDPhongTro.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace QLKDPhongTro.DataLayer.Repositories
{
    /// <summary>
    /// Repository xử lý dữ liệu Tenant
    /// </summary>
    public class TenantRepository : ITenantRepository
    {
        private readonly string connectionString;

        public TenantRepository()
        {
            connectionString = "Data Source=.;Initial Catalog=QLThueNhaV1;Integrated Security=True;TrustServerCertificate=True;Encrypt=False";
        }

        public async Task<List<Tenant>> GetAllAsync()
        {
            var tenants = new List<Tenant>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("SELECT MaNguoiThue, HoTen, CCCD, SoDienThoai, NgayBatDau, TrangThai, GhiChu FROM NguoiThue ORDER BY MaNguoiThue DESC", conn);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        tenants.Add(new Tenant
                        {
                            MaKhachThue = reader.GetInt32(0), // MaNguoiThue
                            HoTen = reader.GetString(1),
                            CCCD = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                            SoDienThoai = reader.GetString(3),
                            NgaySinh = reader.IsDBNull(4) ? DateTime.Now : reader.GetDateTime(4), // NgayBatDau
                            GhiChu = reader.IsDBNull(6) ? string.Empty : reader.GetString(6)
                        });
                    }
                }
            }
            return tenants;
        }

        public async Task<Tenant?> GetByIdAsync(int maKhachThue)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("SELECT MaNguoiThue, HoTen, CCCD, SoDienThoai, NgayBatDau, TrangThai, GhiChu FROM NguoiThue WHERE MaNguoiThue = @MaNguoiThue", conn);
                cmd.Parameters.AddWithValue("@MaNguoiThue", maKhachThue);
                
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new Tenant
                        {
                            MaKhachThue = reader.GetInt32(0), // MaNguoiThue
                            HoTen = reader.GetString(1),
                            CCCD = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                            SoDienThoai = reader.GetString(3),
                            NgaySinh = reader.IsDBNull(4) ? DateTime.Now : reader.GetDateTime(4), // NgayBatDau
                            GhiChu = reader.IsDBNull(6) ? string.Empty : reader.GetString(6)
                        };
                    }
                }
            }
            return null;
        }

        public async Task<bool> CreateAsync(Tenant tenant)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("INSERT INTO NguoiThue (HoTen, CCCD, SoDienThoai, NgayBatDau, TrangThai, GhiChu) VALUES (@HoTen, @CCCD, @SoDienThoai, @NgayBatDau, @TrangThai, @GhiChu)", conn);
                
                cmd.Parameters.AddWithValue("@HoTen", tenant.HoTen);
                cmd.Parameters.AddWithValue("@CCCD", string.IsNullOrEmpty(tenant.CCCD) ? DBNull.Value : tenant.CCCD);
                cmd.Parameters.AddWithValue("@SoDienThoai", tenant.SoDienThoai);
                cmd.Parameters.AddWithValue("@NgayBatDau", tenant.NgaySinh);
                cmd.Parameters.AddWithValue("@TrangThai", "Đang ở");
                cmd.Parameters.AddWithValue("@GhiChu", string.IsNullOrEmpty(tenant.GhiChu) ? DBNull.Value : tenant.GhiChu);

                var result = await cmd.ExecuteNonQueryAsync();
                return result > 0;
            }
        }

        public async Task<bool> UpdateAsync(Tenant tenant)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("UPDATE NguoiThue SET HoTen = @HoTen, CCCD = @CCCD, SoDienThoai = @SoDienThoai, NgayBatDau = @NgayBatDau, GhiChu = @GhiChu WHERE MaNguoiThue = @MaNguoiThue", conn);
                
                cmd.Parameters.AddWithValue("@MaNguoiThue", tenant.MaKhachThue);
                cmd.Parameters.AddWithValue("@HoTen", tenant.HoTen);
                cmd.Parameters.AddWithValue("@CCCD", string.IsNullOrEmpty(tenant.CCCD) ? DBNull.Value : tenant.CCCD);
                cmd.Parameters.AddWithValue("@SoDienThoai", tenant.SoDienThoai);
                cmd.Parameters.AddWithValue("@NgayBatDau", tenant.NgaySinh);
                cmd.Parameters.AddWithValue("@GhiChu", string.IsNullOrEmpty(tenant.GhiChu) ? DBNull.Value : tenant.GhiChu);

                var result = await cmd.ExecuteNonQueryAsync();
                return result > 0;
            }
        }

        public async Task<bool> DeleteAsync(int maKhachThue)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("DELETE FROM NguoiThue WHERE MaNguoiThue = @MaNguoiThue", conn);
                cmd.Parameters.AddWithValue("@MaNguoiThue", maKhachThue);

                var result = await cmd.ExecuteNonQueryAsync();
                return result > 0;
            }
        }

        public async Task<List<Tenant>> SearchByNameAsync(string name)
        {
            var tenants = new List<Tenant>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("SELECT MaNguoiThue, HoTen, CCCD, SoDienThoai, NgayBatDau, TrangThai, GhiChu FROM NguoiThue WHERE HoTen LIKE @Name ORDER BY MaNguoiThue DESC", conn);
                cmd.Parameters.AddWithValue("@Name", $"%{name}%");
                
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        tenants.Add(new Tenant
                        {
                            MaKhachThue = reader.GetInt32(0), // MaNguoiThue
                            HoTen = reader.GetString(1),
                            CCCD = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                            SoDienThoai = reader.GetString(3),
                            NgaySinh = reader.IsDBNull(4) ? DateTime.Now : reader.GetDateTime(4), // NgayBatDau
                            GhiChu = reader.IsDBNull(6) ? string.Empty : reader.GetString(6)
                        });
                    }
                }
            }
            return tenants;
        }

        public async Task<bool> IsCCCDExistsAsync(string cccd, int excludeId = 0)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("SELECT COUNT(*) FROM NguoiThue WHERE CCCD = @CCCD AND MaNguoiThue != @ExcludeId", conn);
                cmd.Parameters.AddWithValue("@CCCD", cccd);
                cmd.Parameters.AddWithValue("@ExcludeId", excludeId);

                var count = (int)await cmd.ExecuteScalarAsync();
                return count > 0;
            }
        }
    }
}