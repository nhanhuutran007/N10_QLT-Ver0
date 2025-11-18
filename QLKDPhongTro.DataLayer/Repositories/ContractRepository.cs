using MySql.Data.MySqlClient;
using QLKDPhongTro.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq; // Cần thêm để dùng .FirstOrDefault()
using System.Threading.Tasks;

namespace QLKDPhongTro.DataLayer.Repositories
{
    public class ContractRepository : IContractRepository
    {
        private string _connectionString => ConnectDB.GetConnectionString();

        // ============================================================
        // CÁC HÀM CƠ BẢN (CRUD)
        // ============================================================

        public async Task<List<Contract>> GetAllHopDongAsync()
        {
            var contracts = new List<Contract>();
            using var connection = new MySqlConnection(_connectionString);
            var sql = @"
                SELECT 
                    hd.MaHopDong, hd.MaNguoiThue, hd.MaPhong, 
                    hd.NgayBatDau, hd.NgayKetThuc, hd.TienCoc, 
                    hd.FileHopDong, hd.TrangThai,
                    p.TenPhong, p.GiaCoBan, 
                    nt.HoTen
                FROM HopDong hd
                LEFT JOIN Phong p ON hd.MaPhong = p.MaPhong
                LEFT JOIN NguoiThue nt ON hd.MaNguoiThue = nt.MaNguoiThue
                ORDER BY hd.MaHopDong DESC";

            using var command = new MySqlCommand(sql, connection);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                contracts.Add(ReadContractWithJoin(reader));
            }
            return contracts;
        }

        public async Task<List<Contract>> GetAllByMaNhaAsync(int maNha)
        {
            var contracts = new List<Contract>();
            using var connection = new MySqlConnection(_connectionString);
            var command = new MySqlCommand(
                @"SELECT hd.MaHopDong, hd.MaNguoiThue, hd.MaPhong, hd.NgayBatDau, hd.NgayKetThuc, 
                         hd.TienCoc, hd.FileHopDong, hd.TrangThai,
                         p.TenPhong, p.GiaCoBan, nt.HoTen
                  FROM HopDong hd
                  LEFT JOIN Phong p ON hd.MaPhong = p.MaPhong
                  LEFT JOIN NguoiThue nt ON hd.MaNguoiThue = nt.MaNguoiThue
                  WHERE p.MaNha = @MaNha
                  ORDER BY hd.MaHopDong DESC", connection);

            command.Parameters.AddWithValue("@MaNha", maNha);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                contracts.Add(ReadContractWithJoin(reader));
            }
            return contracts;
        }

        // ✅ Đã sửa lỗi khai báo trùng biến command và sql
        public async Task<int> AddHopDongAsync(Contract contract)
        {
            using var connection = new MySqlConnection(_connectionString);
            var sql = @"INSERT INTO HopDong 
                  (MaNguoiThue, MaPhong, NgayBatDau, NgayKetThuc, TienCoc, FileHopDong, TrangThai) 
                  VALUES (@MaNguoiThue, @MaPhong, @NgayBatDau, @NgayKetThuc, @TienCoc, @FileHopDong, @TrangThai);
                  SELECT LAST_INSERT_ID();";

            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@MaNguoiThue", contract.MaNguoiThue);
            command.Parameters.AddWithValue("@MaPhong", contract.MaPhong);
            command.Parameters.AddWithValue("@NgayBatDau", contract.NgayBatDau);
            command.Parameters.AddWithValue("@NgayKetThuc", contract.NgayKetThuc);
            command.Parameters.AddWithValue("@TienCoc", contract.TienCoc);
            command.Parameters.AddWithValue("@FileHopDong", contract.FileHopDong ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@TrangThai", contract.TrangThai ?? "Hiệu lực");

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task UpdateHopDongAsync(Contract contract)
        {
            using var connection = new MySqlConnection(_connectionString);
            var sql = @"
                UPDATE HopDong SET 
                    MaNguoiThue = @MaNguoiThue,
                    MaPhong = @MaPhong,
                    NgayBatDau = @NgayBatDau,
                    NgayKetThuc = @NgayKetThuc,
                    TienCoc = @TienCoc,
                    FileHopDong = @FileHopDong,
                    TrangThai = @TrangThai
                WHERE MaHopDong = @MaHopDong";

            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@MaHopDong", contract.MaHopDong);
            command.Parameters.AddWithValue("@MaNguoiThue", contract.MaNguoiThue);
            command.Parameters.AddWithValue("@MaPhong", contract.MaPhong);
            command.Parameters.AddWithValue("@NgayBatDau", contract.NgayBatDau);
            command.Parameters.AddWithValue("@NgayKetThuc", contract.NgayKetThuc);
            command.Parameters.AddWithValue("@TienCoc", contract.TienCoc);
            command.Parameters.AddWithValue("@FileHopDong", contract.FileHopDong ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@TrangThai", contract.TrangThai);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task DeleteHopDongAsync(int maHopDong)
        {
            using var connection = new MySqlConnection(_connectionString);
            var sql = "DELETE FROM HopDong WHERE MaHopDong = @MaHopDong";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@MaHopDong", maHopDong);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        // ============================================================
        // CÁC HÀM BỔ SUNG & IMPLEMENTATION
        // ============================================================

        public async Task<List<Contract>> GetActiveContractsAsync()
        {
            var contracts = new List<Contract>();
            using var connection = new MySqlConnection(_connectionString);
            var sql = @"
                SELECT 
                    hd.MaHopDong, hd.MaNguoiThue, hd.MaPhong, 
                    hd.NgayBatDau, hd.NgayKetThuc, hd.TienCoc, 
                    hd.FileHopDong, hd.TrangThai,
                    p.TenPhong, p.GiaCoBan,
                    nt.HoTen
                FROM HopDong hd
                LEFT JOIN Phong p ON hd.MaPhong = p.MaPhong
                LEFT JOIN NguoiThue nt ON hd.MaNguoiThue = nt.MaNguoiThue
                WHERE hd.TrangThai = 'Hiệu lực'
                ORDER BY hd.MaHopDong DESC";

            using var command = new MySqlCommand(sql, connection);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                contracts.Add(ReadContractWithJoin(reader));
            }
            return contracts;
        }

        // ✅ Đã implement hàm thiếu: Lấy hợp đồng theo khách thuê
        public async Task<List<Contract>> GetActiveContractsByTenantAsync(int maNguoiThue)
        {
            var contracts = new List<Contract>();
            using var connection = new MySqlConnection(_connectionString);
            var sql = @"
                SELECT 
                    hd.MaHopDong, hd.MaNguoiThue, hd.MaPhong, 
                    hd.NgayBatDau, hd.NgayKetThuc, hd.TienCoc, 
                    hd.FileHopDong, hd.TrangThai,
                    p.TenPhong, p.GiaCoBan,
                    nt.HoTen
                FROM HopDong hd
                LEFT JOIN Phong p ON hd.MaPhong = p.MaPhong
                LEFT JOIN NguoiThue nt ON hd.MaNguoiThue = nt.MaNguoiThue
                WHERE hd.MaNguoiThue = @MaNguoiThue AND hd.TrangThai = 'Hiệu lực'
                ORDER BY hd.NgayBatDau DESC";

            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@MaNguoiThue", maNguoiThue);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                contracts.Add(ReadContractWithJoin(reader));
            }
            return contracts;
        }

        public async Task<List<Contract>> GetExpiringContractsAsync(int days)
        {
            var contracts = new List<Contract>();
            using var connection = new MySqlConnection(_connectionString);
            var sql = @"
                SELECT 
                    hd.MaHopDong, hd.MaNguoiThue, hd.MaPhong, 
                    hd.NgayBatDau, hd.NgayKetThuc, hd.TienCoc, 
                    hd.FileHopDong, hd.TrangThai,
                    p.TenPhong, p.GiaCoBan,
                    nt.HoTen
                FROM HopDong hd
                LEFT JOIN Phong p ON hd.MaPhong = p.MaPhong
                LEFT JOIN NguoiThue nt ON hd.MaNguoiThue = nt.MaNguoiThue
                WHERE hd.TrangThai = 'Hiệu lực' 
                  AND DATEDIFF(hd.NgayKetThuc, NOW()) BETWEEN 0 AND @Days
                ORDER BY hd.NgayKetThuc ASC";

            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Days", days);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                contracts.Add(ReadContractWithJoin(reader));
            }
            return contracts;
        }

        public async Task<List<Contract>> GetContractsExpiringInExactDaysAsync(int exactDays)
        {
            var contracts = new List<Contract>();
            using var connection = new MySqlConnection(_connectionString);
            var command = new MySqlCommand(
                @"SELECT hd.MaHopDong, hd.MaNguoiThue, hd.MaPhong, hd.NgayBatDau, hd.NgayKetThuc, 
                         hd.TienCoc, hd.FileHopDong, hd.TrangThai,
                         p.TenPhong, p.GiaCoBan, nt.HoTen
                  FROM HopDong hd
                  LEFT JOIN Phong p ON hd.MaPhong = p.MaPhong
                  LEFT JOIN NguoiThue nt ON hd.MaNguoiThue = nt.MaNguoiThue
                  WHERE BINARY hd.TrangThai = 'Hiệu lực' 
                  AND DATEDIFF(hd.NgayKetThuc, NOW()) = @ExactDays
                  ORDER BY hd.NgayKetThuc ASC", connection);

            command.Parameters.AddWithValue("@ExactDays", exactDays);
            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                contracts.Add(ReadContractWithJoin(reader));
            }

            return contracts;
        }

        // ✅ Đã sửa logic: Trả về Contract (dùng helper ReadContractWithJoin) thay vì Tuple
        public async Task<Contract?> GetByIdAsync(int maHopDong)
        {
            using var connection = new MySqlConnection(_connectionString);
            var sql = @"
                SELECT 
                    hd.MaHopDong, hd.MaNguoiThue, hd.MaPhong, 
                    hd.NgayBatDau, hd.NgayKetThuc, hd.TienCoc, 
                    hd.FileHopDong, hd.TrangThai,
                    p.TenPhong, p.GiaCoBan,
                    nt.HoTen
                FROM HopDong hd
                LEFT JOIN Phong p ON hd.MaPhong = p.MaPhong
                LEFT JOIN NguoiThue nt ON hd.MaNguoiThue = nt.MaNguoiThue
                WHERE hd.MaHopDong = @MaHopDong";

            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@MaHopDong", maHopDong);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return ReadContractWithJoin(reader);
            }
            return null;
        }

        // ✅ Đã implement hàm thiếu: Lấy 1 khách đặt cọc mới nhất
        public async Task<(int MaNguoiThue, string HoTen, decimal TienCoc, string TrangThai)?> GetMostRecentTenantWithDepositAsync()
        {
            var list = await GetMostRecentTenantsWithDepositAsync(1);
            return list.FirstOrDefault();
        }

        public async Task<List<(int MaNguoiThue, string HoTen, decimal TienCoc, string TrangThai)>> GetMostRecentTenantsWithDepositAsync(int count)
        {
            var list = new List<(int, string, decimal, string)>();
            using var connection = new MySqlConnection(_connectionString);
            var sql = @"
                SELECT hd.MaNguoiThue, nt.HoTen, hd.TienCoc, 'Đang thuê' AS TrangThai
                FROM HopDong hd
                INNER JOIN NguoiThue nt ON nt.MaNguoiThue = hd.MaNguoiThue
                WHERE hd.TrangThai = 'Hiệu lực'
                ORDER BY hd.NgayBatDau DESC
                LIMIT @Count";

            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Count", count);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add((
                    reader.GetInt32(0),
                    reader.IsDBNull(1) ? "" : reader.GetString(1),
                    reader.GetDecimal(2),
                    reader.GetString(3)
                ));
            }
            return list;
        }
        public async Task<Contract?> GetActiveByRoomIdAsync(int maPhong)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                // FIX: Thêm 'hd.GiaThue' vào câu lệnh SELECT
                var sql = @"
                    SELECT hd.MaHopDong, hd.MaNguoiThue, hd.MaPhong, hd.NgayBatDau, hd.NgayKetThuc, 
                           hd.TienCoc, hd.GiaThue, hd.FileHopDong, hd.TrangThai,
                           nt.HoTen AS TenNguoiThue, p.TenPhong
                    FROM HopDong hd
                    LEFT JOIN NguoiThue nt ON hd.MaNguoiThue = nt.MaNguoiThue
                    LEFT JOIN Phong p ON hd.MaPhong = p.MaPhong
                    WHERE hd.MaPhong = @MaPhong 
                      AND hd.TrangThai = 'Hiệu lực'
                    LIMIT 1";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@MaPhong", maPhong);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new Contract
                            {
                                MaHopDong = reader.GetInt32("MaHopDong"),
                                MaNguoiThue = reader.GetInt32("MaNguoiThue"),
                                MaPhong = reader.GetInt32("MaPhong"),
                                NgayBatDau = reader.GetDateTime("NgayBatDau"),
                                NgayKetThuc = reader.GetDateTime("NgayKetThuc"),

                                // Bây giờ cột GiaThue đã có trong SELECT, lệnh này sẽ chạy đúng
                                TienCoc = reader.IsDBNull(reader.GetOrdinal("TienCoc")) ? 0 : reader.GetDecimal("TienCoc"),
                                GiaThue = reader.IsDBNull(reader.GetOrdinal("GiaThue")) ? 0 : reader.GetDecimal("GiaThue"),

                                FileHopDong = reader.IsDBNull(reader.GetOrdinal("FileHopDong")) ? null : reader.GetString("FileHopDong"),
                                TrangThai = reader.IsDBNull(reader.GetOrdinal("TrangThai")) ? null : reader.GetString("TrangThai"),

                                TenNguoiThue = reader.IsDBNull(reader.GetOrdinal("TenNguoiThue")) ? "" : reader.GetString("TenNguoiThue"),
                                TenPhong = reader.IsDBNull(reader.GetOrdinal("TenPhong")) ? "" : reader.GetString("TenPhong")
                            };
                        }
                    }
                }
            }
            return null;
        }

        // ============================================================
        // HELPERS
        // ============================================================

        private Contract ReadContractWithJoin(DbDataReader reader)
        {
            // Lưu ý: Index phải khớp với câu SELECT ở các hàm trên (GetAll, GetById...)
            // 0: MaHopDong, 1: MaNguoiThue, 2: MaPhong, 3: NgayBatDau, 4: NgayKetThuc, 
            // 5: TienCoc, 6: FileHopDong, 7: TrangThai, 8: TenPhong, 9: GiaCoBan, 10: HoTen
            return new Contract
            {
                MaHopDong = reader.GetInt32(0),
                MaNguoiThue = reader.GetInt32(1),
                MaPhong = reader.GetInt32(2),
                NgayBatDau = reader.GetDateTime(3),
                NgayKetThuc = reader.GetDateTime(4),
                TienCoc = reader.GetDecimal(5),
                FileHopDong = reader.IsDBNull(6) ? null : reader.GetString(6),
                TrangThai = reader.IsDBNull(7) ? "Hiệu lực" : reader.GetString(7),

                // Map các trường mở rộng từ JOIN
                TenPhong = reader.IsDBNull(8) ? "" : reader.GetString(8),
                GiaThue = reader.IsDBNull(9) ? 0 : reader.GetDecimal(9),
                TenNguoiThue = reader.IsDBNull(10) ? "" : reader.GetString(10)
            };
        }
    }
}