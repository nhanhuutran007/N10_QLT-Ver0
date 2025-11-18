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
                var cmd = new MySqlCommand("SELECT MaNguoiThue, HoTen, SoDienThoai, CCCD, NgayBatDau, TrangThai, GhiChu, NgaySinh, NgayCapCCCD, NoiCapCCCD, DiaChiThuongTru, Email FROM NguoiThue ORDER BY MaNguoiThue DESC", conn);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        tenants.Add(new Tenant
                        {
                            MaKhachThue = reader.GetInt32(0),
                            HoTen = reader.GetString(1),
                            SoDienThoai = reader.IsDBNull(2) ? "" : reader.GetString(2),
                            CCCD = reader.IsDBNull(3) ? "" : reader.GetString(3),
                            NgaySinh = reader.IsDBNull(7) ? null : reader.GetDateTime(7),
                            NgayCap = reader.IsDBNull(8) ? null : reader.GetDateTime(8),
                            NoiCap = reader.IsDBNull(9) ? "" : reader.GetString(9),
                            DiaChi = reader.IsDBNull(10) ? "" : reader.GetString(10),
                            GhiChu = reader.IsDBNull(6) ? "" : reader.GetString(6),
                            Email = reader.IsDBNull(11) ? "" : reader.GetString(11)
                        });
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
                var cmd = new MySqlCommand(@"
                    SELECT DISTINCT nt.MaNguoiThue, nt.HoTen, nt.SoDienThoai, nt.CCCD, nt.NgayBatDau, nt.TrangThai, nt.GhiChu,
                           nt.NgaySinh, nt.NgayCapCCCD, nt.NoiCapCCCD, nt.DiaChiThuongTru, nt.Email
                    FROM NguoiThue nt
                    LEFT JOIN HopDong hd ON nt.MaNguoiThue = hd.MaNguoiThue
                    LEFT JOIN Phong p ON hd.MaPhong = p.MaPhong
                    WHERE p.MaNha = @MaNha OR p.MaNha IS NULL
                    ORDER BY nt.MaNguoiThue DESC", conn);
                cmd.Parameters.AddWithValue("@MaNha", maNha);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        tenants.Add(new Tenant
                        {
                            MaKhachThue = reader.GetInt32(0),
                            HoTen = reader.GetString(1),
                            SoDienThoai = reader.IsDBNull(2) ? "" : reader.GetString(2),
                            CCCD = reader.IsDBNull(3) ? "" : reader.GetString(3),
                            NgaySinh = reader.IsDBNull(7) ? null : reader.GetDateTime(7),
                            NgayCap = reader.IsDBNull(8) ? null : reader.GetDateTime(8),
                            NoiCap = reader.IsDBNull(9) ? "" : reader.GetString(9),
                            DiaChi = reader.IsDBNull(10) ? "" : reader.GetString(10),
                            GhiChu = reader.IsDBNull(6) ? "" : reader.GetString(6),
                            Email = reader.IsDBNull(11) ? "" : reader.GetString(11)
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
                var sql = "SELECT MaNguoiThue, HoTen, SoDienThoai, CCCD, NgayBatDau, TrangThai, GhiChu, NgaySinh, NgayCapCCCD, NoiCapCCCD, DiaChiThuongTru, Email FROM NguoiThue WHERE MaNguoiThue = @MaNguoiThue";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@MaNguoiThue", maKhachThue);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new Tenant
                            {
                                MaKhachThue = reader.GetInt32(0),
                                HoTen = reader.GetString(1),
                                SoDienThoai = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                CCCD = reader.IsDBNull(3) ? "" : reader.GetString(3),
                                NgaySinh = reader.IsDBNull(7) ? null : reader.GetDateTime(7),
                                NgayCap = reader.IsDBNull(8) ? null : reader.GetDateTime(8),
                                NoiCap = reader.IsDBNull(9) ? "" : reader.GetString(9),
                                DiaChi = reader.IsDBNull(10) ? "" : reader.GetString(10),
                                GhiChu = reader.IsDBNull(6) ? "" : reader.GetString(6),
                                Email = reader.IsDBNull(11) ? "" : reader.GetString(11)
                            };
                        }
                    }
                }
            }
            return null;
        }

        // Helper method để map dữ liệu (lưu ý: thứ tự cột phải khớp với câu lệnh SELECT dùng method này)
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
                TrangThai = "Đang thuê"
            };
        }

        public async Task<bool> CreateAsync(Tenant tenant)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                await conn.OpenAsync();
                var sql = @"
                    INSERT INTO NguoiThue
                    (HoTen, SoDienThoai, CCCD, NgayBatDau, TrangThai, GhiChu, 
                     NgaySinh, NgayCapCCCD, NoiCapCCCD, DiaChiThuongTru, Email)
                    VALUES
                    (@HoTen, @SoDienThoai, @CCCD, @NgayBatDau, @TrangThai, @GhiChu,
                     @NgaySinh, @NgayCapCCCD, @NoiCapCCCD, @DiaChiThuongTru, @Email)";

                // Đã thêm khối using command bị thiếu
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@HoTen", tenant.HoTen);
                    cmd.Parameters.AddWithValue("@SoDienThoai", (object?)tenant.SoDienThoai ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CCCD", (object?)tenant.CCCD ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@NgayBatDau", DateTime.Now);
                    cmd.Parameters.AddWithValue("@TrangThai", "Đang ở");
                    cmd.Parameters.AddWithValue("@GhiChu", (object?)tenant.GhiChu ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@NgaySinh", (object?)tenant.NgaySinh ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@NgayCapCCCD", (object?)tenant.NgayCap ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@NoiCapCCCD", (object?)tenant.NoiCap ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@DiaChiThuongTru", (object?)tenant.DiaChi ?? DBNull.Value);
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
                var sql = @"
                    UPDATE NguoiThue SET
                        HoTen = @HoTen,
                        SoDienThoai = @SoDienThoai,
                        CCCD = @CCCD,
                        GioiTinh = @GioiTinh,
                        NgheNghiep = @NgheNghiep,
                        GhiChu = @GhiChu,
                        NgaySinh = @NgaySinh,
                        NgayCapCCCD = @NgayCapCCCD,
                        NoiCapCCCD = @NoiCapCCCD,
                        DiaChiThuongTru = @DiaChiThuongTru,
                        Email = @Email
                    WHERE MaNguoiThue = @MaNguoiThue";

                // Đã thêm khối using command bị thiếu
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@MaNguoiThue", tenant.MaKhachThue);
                    cmd.Parameters.AddWithValue("@HoTen", tenant.HoTen);
                    cmd.Parameters.AddWithValue("@SoDienThoai", (object?)tenant.SoDienThoai ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CCCD", (object?)tenant.CCCD ?? DBNull.Value);
                    // Lưu ý: Code gốc thiếu tham số @GioiTinh và @NgheNghiep trong Object Tenant của CreateAsync, 
                    // nhưng ở UpdateAsync lại có trong SQL. Tôi thêm tạm DBNull nếu property không có.
                    cmd.Parameters.AddWithValue("@GioiTinh", (object?)tenant.GioiTinh ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@NgheNghiep", (object?)tenant.NgheNghiep ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@GhiChu", (object?)tenant.GhiChu ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@NgaySinh", (object?)tenant.NgaySinh ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@NgayCapCCCD", (object?)tenant.NgayCap ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@NoiCapCCCD", (object?)tenant.NoiCap ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@DiaChiThuongTru", (object?)tenant.DiaChi ?? DBNull.Value);
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
                var sql = @"
                    SELECT 
                        MaNguoiThue, HoTen, SoDienThoai, CCCD, Email, GioiTinh, NgheNghiep, 
                        GhiChu, NgaySinh, NgayCapCCCD, NoiCapCCCD, DiaChiThuongTru, NgayTao, NgayCapNhat
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
                            // Dùng helper method ở đây vì câu SQL trả về đủ cột
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