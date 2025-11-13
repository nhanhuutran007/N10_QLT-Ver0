using MySql.Data.MySqlClient;
using QLKDPhongTro.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Data.Common; // 👈 Cần thêm thư viện này để dùng DbDataReader
using System.Threading.Tasks;

namespace QLKDPhongTro.DataLayer.Repositories
{
    public class TenantRepository : ITenantRepository
    {
        private string connectionString => ConnectDB.GetConnectionString();

        public async Task<List<Tenant>> GetAllAsync()
        {
            var tenants = new List<Tenant>();
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                await conn.OpenAsync();
                var sql = @"
                    SELECT 
                        MaNguoiThue, HoTen, SoDienThoai, CCCD, Email, GioiTinh, NgheNghiep, 
                        GhiChu, NgaySinh, NgayCap, NoiCap, DiaChi, NgayTao, NgayCapNhat
                    FROM NguoiThue 
                    ORDER BY MaNguoiThue DESC";

                using (var cmd = new MySqlCommand(sql, conn))
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        tenants.Add(MapReaderToTenant(reader));
                    }
                }
            }
            return tenants;
        }

        public async Task<Tenant?> GetByIdAsync(int maKhachThue)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                await conn.OpenAsync();
                var sql = @"
                    SELECT 
                        MaNguoiThue, HoTen, SoDienThoai, CCCD, Email, GioiTinh, NgheNghiep, 
                        GhiChu, NgaySinh, NgayCap, NoiCap, DiaChi, NgayTao, NgayCapNhat
                    FROM NguoiThue 
                    WHERE MaNguoiThue = @MaNguoiThue";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@MaNguoiThue", maKhachThue);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return MapReaderToTenant(reader);
                        }
                    }
                }
            }
            return null;
        }

        // ✅ ĐÃ SỬA: Thay 'MySqlDataReader' bằng 'DbDataReader' để nhận mọi loại reader
        private Tenant MapReaderToTenant(DbDataReader reader)
        {
            return new Tenant
            {
                MaKhachThue = reader.GetInt32(0),
                HoTen = reader.IsDBNull(1) ? "" : reader.GetString(1),
                SoDienThoai = reader.IsDBNull(2) ? "" : reader.GetString(2),
                CCCD = reader.IsDBNull(3) ? "" : reader.GetString(3),
                Email = reader.IsDBNull(4) ? "" : reader.GetString(4),
                GioiTinh = reader.IsDBNull(5) ? "" : reader.GetString(5),
                NgheNghiep = reader.IsDBNull(6) ? "" : reader.GetString(6),
                GhiChu = reader.IsDBNull(7) ? "" : reader.GetString(7),
                NgaySinh = reader.IsDBNull(8) ? null : reader.GetDateTime(8),
                NgayCap = reader.IsDBNull(9) ? null : reader.GetDateTime(9),
                NoiCap = reader.IsDBNull(10) ? "" : reader.GetString(10),
                DiaChi = reader.IsDBNull(11) ? "" : reader.GetString(11),
                NgayTao = reader.IsDBNull(12) ? DateTime.MinValue : reader.GetDateTime(12),
                NgayCapNhat = reader.IsDBNull(13) ? DateTime.MinValue : reader.GetDateTime(13),
                TrangThai = "Đang thuê" // Giá trị mặc định cho UI
            };
        }

        public async Task<bool> CreateAsync(Tenant tenant)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                await conn.OpenAsync();
                var sql = @"
                    INSERT INTO NguoiThue
                    (HoTen, SoDienThoai, CCCD, Email, GioiTinh, NgheNghiep, GhiChu, 
                     NgaySinh, NgayCap, NoiCap, DiaChi, NgayTao, NgayCapNhat)
                    VALUES
                    (@HoTen, @SoDienThoai, @CCCD, @Email, @GioiTinh, @NgheNghiep, @GhiChu,
                     @NgaySinh, @NgayCap, @NoiCap, @DiaChi, @NgayTao, @NgayCapNhat)";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@HoTen", tenant.HoTen);
                    cmd.Parameters.AddWithValue("@SoDienThoai", tenant.SoDienThoai ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CCCD", tenant.CCCD ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", tenant.Email ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@GioiTinh", tenant.GioiTinh ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@NgheNghiep", tenant.NgheNghiep ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@GhiChu", tenant.GhiChu ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@NgaySinh", tenant.NgaySinh ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@NgayCap", tenant.NgayCap ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@NoiCap", tenant.NoiCap ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@DiaChi", tenant.DiaChi ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@NgayTao", DateTime.Now);
                    cmd.Parameters.AddWithValue("@NgayCapNhat", DateTime.Now);

                    return await cmd.ExecuteNonQueryAsync() > 0;
                }
            }
        }

        public async Task<bool> UpdateAsync(Tenant tenant)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                await conn.OpenAsync();
                var sql = @"
                    UPDATE NguoiThue SET
                        HoTen = @HoTen,
                        SoDienThoai = @SoDienThoai,
                        CCCD = @CCCD,
                        Email = @Email,
                        GioiTinh = @GioiTinh,
                        NgheNghiep = @NgheNghiep,
                        GhiChu = @GhiChu,
                        NgaySinh = @NgaySinh,
                        NgayCap = @NgayCap,
                        NoiCap = @NoiCap,
                        DiaChi = @DiaChi,
                        NgayCapNhat = @NgayCapNhat
                    WHERE MaNguoiThue = @MaNguoiThue";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@MaNguoiThue", tenant.MaKhachThue);
                    cmd.Parameters.AddWithValue("@HoTen", tenant.HoTen);
                    cmd.Parameters.AddWithValue("@SoDienThoai", tenant.SoDienThoai ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CCCD", tenant.CCCD ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", tenant.Email ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@GioiTinh", tenant.GioiTinh ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@NgheNghiep", tenant.NgheNghiep ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@GhiChu", tenant.GhiChu ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@NgaySinh", tenant.NgaySinh ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@NgayCap", tenant.NgayCap ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@NoiCap", tenant.NoiCap ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@DiaChi", tenant.DiaChi ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@NgayCapNhat", DateTime.Now);

                    return await cmd.ExecuteNonQueryAsync() > 0;
                }
            }
        }

        public async Task<bool> DeleteAsync(int maKhachThue)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                await conn.OpenAsync();
                var cmd = new MySqlCommand("DELETE FROM NguoiThue WHERE MaNguoiThue = @MaNguoiThue", conn);
                cmd.Parameters.AddWithValue("@MaNguoiThue", maKhachThue);
                return await cmd.ExecuteNonQueryAsync() > 0;
            }
        }

        public async Task<List<Tenant>> SearchByNameAsync(string name)
        {
            var tenants = new List<Tenant>();
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                await conn.OpenAsync();
                var sql = @"
                    SELECT 
                        MaNguoiThue, HoTen, SoDienThoai, CCCD, Email, GioiTinh, NgheNghiep, 
                        GhiChu, NgaySinh, NgayCap, NoiCap, DiaChi, NgayTao, NgayCapNhat
                    FROM NguoiThue
                    WHERE HoTen LIKE @Name
                    ORDER BY MaNguoiThue DESC";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", $"%{name}%");
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            tenants.Add(MapReaderToTenant(reader));
                        }
                    }
                }
            }
            return tenants;
        }

        public async Task<bool> IsCCCDExistsAsync(string cccd, int excludeId = 0)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                await conn.OpenAsync();
                var cmd = new MySqlCommand("SELECT COUNT(*) FROM NguoiThue WHERE CCCD = @CCCD AND MaNguoiThue != @ExcludeId", conn);
                cmd.Parameters.AddWithValue("@CCCD", cccd);
                cmd.Parameters.AddWithValue("@ExcludeId", excludeId);

                long count = Convert.ToInt64(await cmd.ExecuteScalarAsync());
                return count > 0;
            }
        }
    }
}