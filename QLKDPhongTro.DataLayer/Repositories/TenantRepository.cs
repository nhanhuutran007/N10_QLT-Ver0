using MySql.Data.MySqlClient;
using QLKDPhongTro.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
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
                // CẬP NHẬT: Dùng tên cột mới (NgayCap, NoiCap, DiaChi)
                var sql = @"
                    SELECT MaNguoiThue, HoTen, SoDienThoai, CCCD, NgayBatDau, TrangThai, GhiChu, 
                           NgaySinh, NgayCap, NoiCap, DiaChi, Email 
                    FROM NguoiThue 
                    ORDER BY MaNguoiThue DESC";

                using (var cmd = new MySqlCommand(sql, conn))
                {
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

        public async Task<List<Tenant>> GetAllByMaNhaAsync(int maNha)
        {
            var tenants = new List<Tenant>();
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                await conn.OpenAsync();
                // CẬP NHẬT: Dùng tên cột mới trong câu lệnh JOIN
                var sql = @"
                    SELECT DISTINCT nt.MaNguoiThue, nt.HoTen, nt.SoDienThoai, nt.CCCD, nt.NgayBatDau, nt.TrangThai, nt.GhiChu,
                           nt.NgaySinh, nt.NgayCap, nt.NoiCap, nt.DiaChi, nt.Email
                    FROM NguoiThue nt
                    LEFT JOIN HopDong hd ON nt.MaNguoiThue = hd.MaNguoiThue
                    LEFT JOIN Phong p ON hd.MaPhong = p.MaPhong
                    WHERE p.MaNha = @MaNha OR p.MaNha IS NULL
                    ORDER BY nt.MaNguoiThue DESC";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@MaNha", maNha);
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

        public async Task<Tenant?> GetByIdAsync(int maKhachThue)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                await conn.OpenAsync();
                // CẬP NHẬT: Dùng tên cột mới
                var sql = @"
                    SELECT MaNguoiThue, HoTen, SoDienThoai, CCCD, NgayBatDau, TrangThai, GhiChu, 
                           NgaySinh, NgayCap, NoiCap, DiaChi, Email 
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

        // Helper method map dữ liệu (Map theo đúng thứ tự SELECT ở trên)
        private Tenant MapReaderToTenant(DbDataReader reader)
        {
            return new Tenant
            {
                MaKhachThue = reader.GetInt32(0),
                HoTen = reader.IsDBNull(1) ? "" : reader.GetString(1),
                SoDienThoai = reader.IsDBNull(2) ? "" : reader.GetString(2),
                CCCD = reader.IsDBNull(3) ? "" : reader.GetString(3),
                // Index 4 (NgayBatDau) và 5 (TrangThai) - Có thể map thêm nếu Model có
                TrangThai = reader.IsDBNull(5) ? "Đang ở" : reader.GetString(5),

                GhiChu = reader.IsDBNull(6) ? "" : reader.GetString(6),
                NgaySinh = reader.IsDBNull(7) ? null : reader.GetDateTime(7),

                // === CẬP NHẬT MAPPING ===
                NgayCap = reader.IsDBNull(8) ? null : reader.GetDateTime(8), // Cột DB là NgayCap
                NoiCap = reader.IsDBNull(9) ? "" : reader.GetString(9),      // Cột DB là NoiCap
                DiaChi = reader.IsDBNull(10) ? "" : reader.GetString(10),    // Cột DB là DiaChi

                Email = reader.IsDBNull(11) ? "" : reader.GetString(11),

                NgayTao = DateTime.Now,
                NgayCapNhat = DateTime.Now
            };
        }

        public async Task<bool> CreateAsync(Tenant tenant)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                await conn.OpenAsync();
                // CẬP NHẬT: Insert vào NgayCap, NoiCap, DiaChi
                var sql = @"
                    INSERT INTO NguoiThue
                    (HoTen, SoDienThoai, CCCD, NgayBatDau, TrangThai, GhiChu, 
                     NgaySinh, NgayCap, NoiCap, DiaChi, Email)
                    VALUES
                    (@HoTen, @SoDienThoai, @CCCD, @NgayBatDau, @TrangThai, @GhiChu,
                     @NgaySinh, @NgayCap, @NoiCap, @DiaChi, @Email)";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@HoTen", tenant.HoTen);
                    cmd.Parameters.AddWithValue("@SoDienThoai", (object?)tenant.SoDienThoai ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CCCD", (object?)tenant.CCCD ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@NgayBatDau", DateTime.Now);
                    cmd.Parameters.AddWithValue("@TrangThai", "Đang ở");
                    cmd.Parameters.AddWithValue("@GhiChu", (object?)tenant.GhiChu ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@NgaySinh", (object?)tenant.NgaySinh ?? DBNull.Value);

                    // Map Parameter
                    cmd.Parameters.AddWithValue("@NgayCap", (object?)tenant.NgayCap ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@NoiCap", (object?)tenant.NoiCap ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@DiaChi", (object?)tenant.DiaChi ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", (object?)tenant.Email ?? DBNull.Value);

                    return await cmd.ExecuteNonQueryAsync() > 0;
                }
            }
        }

        public async Task<bool> UpdateAsync(Tenant tenant)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                await conn.OpenAsync();
                // CẬP NHẬT: Update NgayCap, NoiCap, DiaChi
                var sql = @"
                    UPDATE NguoiThue SET
                        HoTen = @HoTen,
                        SoDienThoai = @SoDienThoai,
                        CCCD = @CCCD,
                        GioiTinh = @GioiTinh,
                        NgheNghiep = @NgheNghiep,
                        GhiChu = @GhiChu,
                        NgaySinh = @NgaySinh,
                        NgayCap = @NgayCap,
                        NoiCap = @NoiCap,
                        DiaChi = @DiaChi,
                        Email = @Email
                    WHERE MaNguoiThue = @MaNguoiThue";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@MaNguoiThue", tenant.MaKhachThue);
                    cmd.Parameters.AddWithValue("@HoTen", tenant.HoTen);
                    cmd.Parameters.AddWithValue("@SoDienThoai", (object?)tenant.SoDienThoai ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CCCD", (object?)tenant.CCCD ?? DBNull.Value);

                    // Thêm parameter GioiTinh, NgheNghiep (nếu Model có)
                    cmd.Parameters.AddWithValue("@GioiTinh", (object?)tenant.GioiTinh ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@NgheNghiep", (object?)tenant.NgheNghiep ?? DBNull.Value);

                    cmd.Parameters.AddWithValue("@GhiChu", (object?)tenant.GhiChu ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@NgaySinh", (object?)tenant.NgaySinh ?? DBNull.Value);

                    // Map Parameter Update
                    cmd.Parameters.AddWithValue("@NgayCap", (object?)tenant.NgayCap ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@NoiCap", (object?)tenant.NoiCap ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@DiaChi", (object?)tenant.DiaChi ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", (object?)tenant.Email ?? DBNull.Value);

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
                // CẬP NHẬT: SQL cho Search
                var sql = @"
                    SELECT 
                        MaNguoiThue, HoTen, SoDienThoai, CCCD, NgayBatDau, TrangThai, GhiChu, 
                        NgaySinh, NgayCap, NoiCap, DiaChi, Email
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