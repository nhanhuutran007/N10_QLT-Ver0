using MySql.Data.MySqlClient;
using QLKDPhongTro.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QLKDPhongTro.DataLayer.Repositories
{
    /// <summary>
    /// Repository xử lý dữ liệu Tenant
    /// </summary>
    public class TenantRepository : ITenantRepository
    {
        // Sử dụng ConnectDB chung để quản lý connection string
        private string connectionString => ConnectDB.GetConnectionString();

        public async Task<List<Tenant>> GetAllAsync()
        {
            var tenants = new List<Tenant>();
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                await conn.OpenAsync();

                // === FIX 1: Cập nhật SQL SELECT ===
                // - Đã XÓA NgayBatDau, TrangThai
                // - Đã SỬA TÊN CỘT (NgayCap, NoiCap, DiaChi)
                // - Đã THÊM Email, GioiTinh, NgheNghiep
                var cmd = new MySqlCommand(@"
                    SELECT MaNguoiThue, HoTen, SoDienThoai, CCCD, Email, GioiTinh, NgheNghiep, 
                           GhiChu, NgaySinh, NgayCap, NoiCap, DiaChi 
                    FROM NguoiThue 
                    ORDER BY MaNguoiThue DESC", conn);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        // === FIX 2: Cập nhật mapping theo thứ tự cột SELECT mới ===
                        tenants.Add(new Tenant
                        {
                            MaKhachThue = reader.GetInt32(0),
                            HoTen = reader.GetString(1),
                            SoDienThoai = reader.IsDBNull(2) ? "" : reader.GetString(2),
                            CCCD = reader.IsDBNull(3) ? "" : reader.GetString(3),
                            Email = reader.IsDBNull(4) ? "" : reader.GetString(4),         // Mới
                            GioiTinh = reader.IsDBNull(5) ? "" : reader.GetString(5),     // Mới
                            NgheNghiep = reader.IsDBNull(6) ? "" : reader.GetString(6),   // Mới
                            GhiChu = reader.IsDBNull(7) ? "" : reader.GetString(7),       // Đổi index
                            NgaySinh = reader.IsDBNull(8) ? null : reader.GetDateTime(8), // Đổi index
                            NgayCap = reader.IsDBNull(9) ? null : reader.GetDateTime(9),  // Đổi index
                            NoiCap = reader.IsDBNull(10) ? "" : reader.GetString(10),     // Đổi index
                            DiaChi = reader.IsDBNull(11) ? "" : reader.GetString(11)      // Đổi index
                        });
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

                // === FIX 1: Cập nhật SQL SELECT ===
                var cmd = new MySqlCommand(@"
                    SELECT MaNguoiThue, HoTen, SoDienThoai, CCCD, Email, GioiTinh, NgheNghiep, 
                           GhiChu, NgaySinh, NgayCap, NoiCap, DiaChi 
                    FROM NguoiThue 
                    WHERE MaNguoiThue = @MaNguoiThue", conn);

                cmd.Parameters.AddWithValue("@MaNguoiThue", maKhachThue);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        // === FIX 2: Cập nhật mapping theo thứ tự cột SELECT mới ===
                        return new Tenant
                        {
                            MaKhachThue = reader.GetInt32(0),
                            HoTen = reader.GetString(1),
                            SoDienThoai = reader.IsDBNull(2) ? "" : reader.GetString(2),
                            CCCD = reader.IsDBNull(3) ? "" : reader.GetString(3),
                            Email = reader.IsDBNull(4) ? "" : reader.GetString(4),
                            GioiTinh = reader.IsDBNull(5) ? "" : reader.GetString(5),
                            NgheNghiep = reader.IsDBNull(6) ? "" : reader.GetString(6),
                            GhiChu = reader.IsDBNull(7) ? "" : reader.GetString(7),
                            NgaySinh = reader.IsDBNull(8) ? null : reader.GetDateTime(8),
                            NgayCap = reader.IsDBNull(9) ? null : reader.GetDateTime(9),
                            NoiCap = reader.IsDBNull(10) ? "" : reader.GetString(10),
                            DiaChi = reader.IsDBNull(11) ? "" : reader.GetString(11)
                        };
                    }
                }
            }
            return null;
        }

        public async Task<bool> CreateAsync(Tenant tenant)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                await conn.OpenAsync();

                // === FIX 3: Cập nhật SQL INSERT ===
                var cmd = new MySqlCommand(@"
                        INSERT INTO NguoiThue
                        (HoTen, SoDienThoai, CCCD, Email, GioiTinh, NgheNghiep, 
                         GhiChu, NgaySinh, NgayCap, NoiCap, DiaChi)
                        VALUES
                        (@HoTen, @SoDienThoai, @CCCD, @Email, @GioiTinh, @NgheNghiep, 
                         @GhiChu, @NgaySinh, @NgayCap, @NoiCap, @DiaChi)", conn);

                cmd.Parameters.AddWithValue("@HoTen", tenant.HoTen);
                cmd.Parameters.AddWithValue("@SoDienThoai", (object?)tenant.SoDienThoai ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CCCD", (object?)tenant.CCCD ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Email", (object?)tenant.Email ?? DBNull.Value);           // Mới
                cmd.Parameters.AddWithValue("@GioiTinh", (object?)tenant.GioiTinh ?? DBNull.Value);     // Mới
                cmd.Parameters.AddWithValue("@NgheNghiep", (object?)tenant.NgheNghiep ?? DBNull.Value); // Mới
                cmd.Parameters.AddWithValue("@GhiChu", (object?)tenant.GhiChu ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@NgaySinh", (object?)tenant.NgaySinh ?? DBNull.Value);

                // === FIX 4: Sửa tên Parameters ===
                cmd.Parameters.AddWithValue("@NgayCap", (object?)tenant.NgayCap ?? DBNull.Value);     // Sửa tên
                cmd.Parameters.AddWithValue("@NoiCap", (object?)tenant.NoiCap ?? DBNull.Value);       // Sửa tên
                cmd.Parameters.AddWithValue("@DiaChi", (object?)tenant.DiaChi ?? DBNull.Value);       // Sửa tên

                // === FIX 5: Xóa Parameters không tồn tại ===
                // cmd.Parameters.AddWithValue("@NgayBatDau", DateTime.Now);  // XÓA
                // cmd.Parameters.AddWithValue("@TrangThai", "Đang ở");        // XÓA

                return await cmd.ExecuteNonQueryAsync() > 0;
            }
        }


        public async Task<bool> UpdateAsync(Tenant tenant)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                await conn.OpenAsync();

                // === FIX 6: Cập nhật SQL UPDATE ===
                var cmd = new MySqlCommand(@"
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
                            DiaChi = @DiaChi
                        WHERE MaNguoiThue = @MaNguoiThue", conn);

                cmd.Parameters.AddWithValue("@MaNguoiThue", tenant.MaKhachThue);
                cmd.Parameters.AddWithValue("@HoTen", tenant.HoTen);
                cmd.Parameters.AddWithValue("@SoDienThoai", (object?)tenant.SoDienThoai ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CCCD", (object?)tenant.CCCD ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Email", (object?)tenant.Email ?? DBNull.Value);           // Mới
                cmd.Parameters.AddWithValue("@GioiTinh", (object?)tenant.GioiTinh ?? DBNull.Value);     // Mới
                cmd.Parameters.AddWithValue("@NgheNghiep", (object?)tenant.NgheNghiep ?? DBNull.Value); // Mới
                cmd.Parameters.AddWithValue("@GhiChu", (object?)tenant.GhiChu ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@NgaySinh", (object?)tenant.NgaySinh ?? DBNull.Value);

                // === FIX 7: Sửa tên Parameters ===
                cmd.Parameters.AddWithValue("@NgayCap", (object?)tenant.NgayCap ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@NoiCap", (object?)tenant.NoiCap ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@DiaChi", (object?)tenant.DiaChi ?? DBNull.Value);

                return await cmd.ExecuteNonQueryAsync() > 0;
            }
        }

        // 🗑️ Xóa khách thuê (Hàm này đã đúng, không cần sửa)
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

        // 🔎 Tìm kiếm theo tên
        public async Task<List<Tenant>> SearchByNameAsync(string name)
        {
            var tenants = new List<Tenant>();
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                await conn.OpenAsync();

                // === FIX 1: Cập nhật SQL SELECT ===
                var cmd = new MySqlCommand(@"
                        SELECT 
                            MaNguoiThue, HoTen, SoDienThoai, CCCD, Email, GioiTinh, NgheNghiep, 
                            GhiChu, NgaySinh, NgayCap, NoiCap, DiaChi
                        FROM NguoiThue
                        WHERE HoTen LIKE @Name
                        ORDER BY MaNguoiThue DESC", conn);

                cmd.Parameters.AddWithValue("@Name", $"%{name}%");

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        // === FIX 2: Cập nhật mapping theo thứ tự cột SELECT mới ===
                        tenants.Add(new Tenant
                        {
                            MaKhachThue = reader.GetInt32(0),
                            HoTen = reader.GetString(1),
                            SoDienThoai = reader.IsDBNull(2) ? "" : reader.GetString(2),
                            CCCD = reader.IsDBNull(3) ? "" : reader.GetString(3),
                            Email = reader.IsDBNull(4) ? "" : reader.GetString(4),
                            GioiTinh = reader.IsDBNull(5) ? "" : reader.GetString(5),
                            NgheNghiep = reader.IsDBNull(6) ? "" : reader.GetString(6),
                            GhiChu = reader.IsDBNull(7) ? "" : reader.GetString(7),
                            NgaySinh = reader.IsDBNull(8) ? null : reader.GetDateTime(8),
                            NgayCap = reader.IsDBNull(9) ? null : reader.GetDateTime(9),
                            NoiCap = reader.IsDBNull(10) ? "" : reader.GetString(10),
                            DiaChi = reader.IsDBNull(11) ? "" : reader.GetString(11)
                        });
                    }
                }
            }
            return tenants;
        }

        // 🧩 Kiểm tra trùng CCCD (Hàm này đã đúng, không cần sửa)
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