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
                var sql = @"
                    SELECT nt.*
                    FROM NguoiThue nt
                    ORDER BY nt.MaNguoiThue DESC";

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
                var sql = @"
                    SELECT DISTINCT nt.*
                    FROM NguoiThue nt
                    LEFT JOIN HopDong hd ON nt.MaNguoiThue = hd.MaNguoiThue
                    LEFT JOIN Phong p ON p.MaPhong = COALESCE(nt.MaPhong, hd.MaPhong)
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
                var sql = @"
                    SELECT nt.*
                    FROM NguoiThue nt
                    WHERE nt.MaNguoiThue = @MaNguoiThue";

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

        private Tenant MapReaderToTenant(DbDataReader reader)
        {
            return new Tenant
            {
                MaKhachThue = ReadInt(reader, "MaNguoiThue"),
                MaPhong = ReadNullableInt(reader, "MaPhong"),
                HoTen = ReadString(reader, "HoTen"),
                SoDienThoai = ReadString(reader, "SoDienThoai"),
                CCCD = ReadString(reader, "CCCD"),
                TrangThai = ReadString(reader, "TrangThai", "Đang ở"),
                GhiChu = ReadString(reader, "GhiChu"),
                NgaySinh = ReadNullableDateTime(reader, "NgaySinh"),
                NgayCap = ReadNullableDateTime(reader, "NgayCap"),
                NoiCap = ReadString(reader, "NoiCap"),
                DiaChi = ReadString(reader, "DiaChi"),
                Email = ReadString(reader, "Email"),
                GioiTinh = ReadString(reader, "GioiTinh"),
                NgheNghiep = ReadString(reader, "NgheNghiep"),
                NgayTao = DateTime.Now,
                NgayCapNhat = DateTime.Now
            };
        }

        public async Task<bool> CreateAsync(Tenant tenant)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                await conn.OpenAsync();
                // ✅ Cập nhật INSERT để bao gồm GioiTinh, NgheNghiep
                var sql = @"
                    INSERT INTO NguoiThue
                    (MaPhong, HoTen, SoDienThoai, CCCD, NgayBatDau, TrangThai, GhiChu, 
                     NgaySinh, NgayCap, NoiCap, DiaChi, Email, GioiTinh, NgheNghiep)
                    VALUES
                    (@MaPhong, @HoTen, @SoDienThoai, @CCCD, @NgayBatDau, @TrangThai, @GhiChu,
                     @NgaySinh, @NgayCap, @NoiCap, @DiaChi, @Email, @GioiTinh, @NgheNghiep)";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@MaPhong", (object?)tenant.MaPhong ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@HoTen", tenant.HoTen);
                    cmd.Parameters.AddWithValue("@SoDienThoai", (object?)tenant.SoDienThoai ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CCCD", (object?)tenant.CCCD ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@NgayBatDau", DateTime.Now);
                    var trangThai = string.IsNullOrWhiteSpace(tenant.TrangThai) ? "Đang ở" : tenant.TrangThai;
                    cmd.Parameters.AddWithValue("@TrangThai", trangThai);
                    cmd.Parameters.AddWithValue("@GhiChu", (object?)tenant.GhiChu ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@NgaySinh", (object?)tenant.NgaySinh ?? DBNull.Value);

                    cmd.Parameters.AddWithValue("@NgayCap", (object?)tenant.NgayCap ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@NoiCap", (object?)tenant.NoiCap ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@DiaChi", (object?)tenant.DiaChi ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", (object?)tenant.Email ?? DBNull.Value);

                    // Parameter GioiTinh, NgheNghiep
                    cmd.Parameters.AddWithValue("@GioiTinh", (object?)tenant.GioiTinh ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@NgheNghiep", (object?)tenant.NgheNghiep ?? DBNull.Value);

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
                        MaPhong = @MaPhong,
                        HoTen = @HoTen,
                        SoDienThoai = @SoDienThoai,
                        CCCD = @CCCD,
                        GioiTinh = @GioiTinh,
                        NgheNghiep = @NgheNghiep,
                        TrangThai = @TrangThai,
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
                    cmd.Parameters.AddWithValue("@MaPhong", (object?)tenant.MaPhong ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@HoTen", tenant.HoTen);
                    cmd.Parameters.AddWithValue("@SoDienThoai", (object?)tenant.SoDienThoai ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CCCD", (object?)tenant.CCCD ?? DBNull.Value);

                    cmd.Parameters.AddWithValue("@GioiTinh", (object?)tenant.GioiTinh ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@NgheNghiep", (object?)tenant.NgheNghiep ?? DBNull.Value);
                    var trangThai = string.IsNullOrWhiteSpace(tenant.TrangThai) ? "Đang ở" : tenant.TrangThai;
                    cmd.Parameters.AddWithValue("@TrangThai", trangThai);

                    cmd.Parameters.AddWithValue("@GhiChu", (object?)tenant.GhiChu ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@NgaySinh", (object?)tenant.NgaySinh ?? DBNull.Value);

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
                var sql = @"
                    SELECT nt.*
                    FROM NguoiThue nt
                    WHERE nt.HoTen LIKE @Name
                    ORDER BY nt.MaNguoiThue DESC";

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

        private static int GetOrdinalSafe(DbDataReader reader, string columnName)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (string.Equals(reader.GetName(i), columnName, StringComparison.OrdinalIgnoreCase))
                {
                    return i;
                }
            }
            return -1;
        }

        private static string ReadString(DbDataReader reader, string columnName, string defaultValue = "")
        {
            int ordinal = GetOrdinalSafe(reader, columnName);
            return ordinal >= 0 && !reader.IsDBNull(ordinal) ? reader.GetString(ordinal) : defaultValue;
        }

        private static int? ReadNullableInt(DbDataReader reader, string columnName)
        {
            int ordinal = GetOrdinalSafe(reader, columnName);
            return ordinal >= 0 && !reader.IsDBNull(ordinal) ? reader.GetInt32(ordinal) : (int?)null;
        }

        private static int ReadInt(DbDataReader reader, string columnName, int defaultValue = 0)
        {
            int ordinal = GetOrdinalSafe(reader, columnName);
            return ordinal >= 0 && !reader.IsDBNull(ordinal) ? reader.GetInt32(ordinal) : defaultValue;
        }

        private static DateTime? ReadNullableDateTime(DbDataReader reader, string columnName)
        {
            int ordinal = GetOrdinalSafe(reader, columnName);
            return ordinal >= 0 && !reader.IsDBNull(ordinal) ? reader.GetDateTime(ordinal) : null;
        }

        public async Task<List<TenantAsset>> GetAssetsAsync(int maNguoiThue)
        {
            var assets = new List<TenantAsset>();
            using var conn = new MySqlConnection(connectionString);
            await conn.OpenAsync();
            var sql = @"
                SELECT MaTaiSan, MaNguoiThue, LoaiTaiSan, MoTa, IFNULL(PhiPhuThu,0) AS PhiPhuThu
                FROM TaiSanNguoiThue
                WHERE MaNguoiThue = @MaNguoiThue
                ORDER BY MaTaiSan DESC";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@MaNguoiThue", maNguoiThue);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                assets.Add(new TenantAsset
                {
                    MaTaiSan = reader.GetInt32(0),
                    MaNguoiThue = reader.GetInt32(1),
                    LoaiTaiSan = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                    MoTa = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                    PhiPhuThu = reader.IsDBNull(4) ? 0 : reader.GetDecimal(4),
                    NgayTao = null
                });
            }
            return assets;
        }

        public async Task<bool> CreateAssetAsync(TenantAsset asset)
        {
            using var conn = new MySqlConnection(connectionString);
            await conn.OpenAsync();
            var sql = @"
                INSERT INTO TaiSanNguoiThue (MaNguoiThue, LoaiTaiSan, MoTa, PhiPhuThu)
                VALUES (@MaNguoiThue, @LoaiTaiSan, @MoTa, @PhiPhuThu)";
            
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@MaNguoiThue", asset.MaNguoiThue);
            cmd.Parameters.AddWithValue("@LoaiTaiSan", asset.LoaiTaiSan);
            cmd.Parameters.AddWithValue("@MoTa", asset.MoTa ?? string.Empty);
            cmd.Parameters.AddWithValue("@PhiPhuThu", asset.PhiPhuThu);
            
            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> UpdateAssetAsync(TenantAsset asset)
        {
            using var conn = new MySqlConnection(connectionString);
            await conn.OpenAsync();
            var sql = @"
                UPDATE TaiSanNguoiThue
                SET LoaiTaiSan = @LoaiTaiSan, MoTa = @MoTa, PhiPhuThu = @PhiPhuThu
                WHERE MaTaiSan = @MaTaiSan";
            
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@MaTaiSan", asset.MaTaiSan);
            cmd.Parameters.AddWithValue("@LoaiTaiSan", asset.LoaiTaiSan);
            cmd.Parameters.AddWithValue("@MoTa", asset.MoTa ?? string.Empty);
            cmd.Parameters.AddWithValue("@PhiPhuThu", asset.PhiPhuThu);
            
            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteAssetAsync(int maTaiSan)
        {
            using var conn = new MySqlConnection(connectionString);
            await conn.OpenAsync();
            var sql = "DELETE FROM TaiSanNguoiThue WHERE MaTaiSan = @MaTaiSan";
            
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@MaTaiSan", maTaiSan);
            
            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        public async Task<TenantStayInfo?> GetCurrentStayInfoAsync(int maNguoiThue)
        {
            using var conn = new MySqlConnection(connectionString);
            await conn.OpenAsync();
            var sql = @"
                SELECT 
                    hd.MaHopDong,
                    hd.MaPhong,
                    p.TenPhong,
                    p.TrangThai AS TrangThaiPhong,
                    hd.TrangThai AS TrangThaiHopDong,
                    hd.NgayBatDau,
                    hd.NgayKetThuc,
                    hd.TienCoc,
                    nt.TrangThai AS TrangThaiNguoiThue,
                    nt.SoDienThoai
                FROM NguoiThue nt
                LEFT JOIN HopDong hd ON hd.MaNguoiThue = nt.MaNguoiThue
                LEFT JOIN Phong p ON p.MaPhong = hd.MaPhong
                WHERE nt.MaNguoiThue = @MaNguoiThue
                ORDER BY 
                    CASE 
                        WHEN hd.TrangThai = 'Hiệu lực' THEN 0
                        WHEN hd.TrangThai = 'Sắp hết hạn' THEN 1
                        WHEN hd.TrangThai = 'Hết hạn' THEN 2
                        WHEN hd.TrangThai = 'Hủy' THEN 3
                        WHEN hd.MaHopDong IS NULL THEN 4
                        ELSE 5
                    END,
                    hd.NgayKetThuc DESC
                LIMIT 1";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@MaNguoiThue", maNguoiThue);
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new TenantStayInfo
                {
                    MaHopDong = reader.IsDBNull(0) ? (int?)null : reader.GetInt32(0),
                    MaPhong = reader.IsDBNull(1) ? (int?)null : reader.GetInt32(1),
                    TenPhong = reader.IsDBNull(2) ? null : reader.GetString(2),
                    TrangThaiPhong = reader.IsDBNull(3) ? null : reader.GetString(3),
                    TrangThaiHopDong = reader.IsDBNull(4) ? null : reader.GetString(4),
                    NgayBatDau = reader.IsDBNull(5) ? (DateTime?)null : reader.GetDateTime(5),
                    NgayKetThuc = reader.IsDBNull(6) ? (DateTime?)null : reader.GetDateTime(6),
                    TienCoc = reader.IsDBNull(7) ? (decimal?)null : reader.GetDecimal(7),
                    TrangThaiNguoiThue = reader.IsDBNull(8) ? null : reader.GetString(8),
                    SoDienThoai = reader.IsDBNull(9) ? null : reader.GetString(9)
                };
            }
            return null;
        }

        public async Task<List<RoomTenantInfo>> GetTenantsByRoomIdAsync(int maPhong)
        {
            var roomTenants = new List<RoomTenantInfo>();
            using var conn = new MySqlConnection(connectionString);
            await conn.OpenAsync();
            // ✅ SỬA: Chỉ lấy người thuê ĐANG Ở trong phòng (không lấy người đã trả phòng)
            // Filter theo TrangThaiNguoiThue = 'Đang ở' và MaPhong = @MaPhong
            var sql = @"
                SELECT 
                    nt.MaNguoiThue,
                    nt.HoTen,
                    nt.SoDienThoai,
                    nt.TrangThai AS TrangThaiNguoiThue,
                    hd.MaHopDong,
                    hd.TrangThai AS TrangThaiHopDong,
                    hd.NgayBatDau,
                    hd.NgayKetThuc
                FROM NguoiThue nt
                LEFT JOIN HopDong hd ON hd.MaHopDong = (
                    SELECT h.MaHopDong
                    FROM HopDong h
                    WHERE h.MaNguoiThue = nt.MaNguoiThue
                      AND h.MaPhong = @MaPhong
                      AND (h.TrangThai IS NULL OR h.TrangThai <> 'Hủy')
                    ORDER BY 
                        CASE 
                            WHEN h.TrangThai = 'Hiệu lực' THEN 0
                            WHEN h.TrangThai = 'Sắp hết hạn' THEN 1
                            WHEN h.TrangThai = 'Hết hạn' THEN 2
                            ELSE 3
                        END,
                        h.NgayKetThuc DESC
                    LIMIT 1
                )
                WHERE nt.MaPhong = @MaPhong
                  AND nt.TrangThai = 'Đang ở'
                ORDER BY 
                    CASE 
                        WHEN hd.TrangThai = 'Hiệu lực' THEN 0
                        WHEN hd.TrangThai = 'Sắp hết hạn' THEN 1
                        WHEN hd.TrangThai = 'Hết hạn' THEN 2
                        ELSE 3
                    END,
                    hd.NgayKetThuc DESC,
                    nt.MaNguoiThue";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@MaPhong", maPhong);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                roomTenants.Add(new RoomTenantInfo
                {
                    MaNguoiThue = reader.GetInt32(0),
                    HoTen = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                    SoDienThoai = reader.IsDBNull(2) ? null : reader.GetString(2),
                    TrangThaiNguoiThue = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                    MaHopDong = reader.IsDBNull(4) ? (int?)null : reader.GetInt32(4),
                    TrangThaiHopDong = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                    NgayBatDau = reader.IsDBNull(6) ? (DateTime?)null : reader.GetDateTime(6),
                    NgayKetThuc = reader.IsDBNull(7) ? (DateTime?)null : reader.GetDateTime(7)
                });
            }
            return roomTenants;
        }

        public async Task<bool> UpdateTenantStatusAsync(int maNguoiThue, string trangThai)
        {
            using var conn = new MySqlConnection(connectionString);
            await conn.OpenAsync();
            var cmd = new MySqlCommand("UPDATE NguoiThue SET TrangThai = @TrangThai WHERE MaNguoiThue = @MaNguoiThue", conn);
            cmd.Parameters.AddWithValue("@TrangThai", trangThai);
            cmd.Parameters.AddWithValue("@MaNguoiThue", maNguoiThue);
            return await cmd.ExecuteNonQueryAsync() > 0;
        }
    }
}