using QLKDPhongTro.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

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

        public async Task AddHopDongAsync(Contract contract)
        {
            using var connection = new MySqlConnection(_connectionString);
            var sql = @"
                INSERT INTO HopDong 
                (MaNguoiThue, MaPhong, NgayBatDau, NgayKetThuc, TienCoc, FileHopDong, TrangThai) 
                VALUES 
                (@MaNguoiThue, @MaPhong, @NgayBatDau, @NgayKetThuc, @TienCoc, @FileHopDong, @TrangThai)";

            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@MaNguoiThue", contract.MaNguoiThue);
            command.Parameters.AddWithValue("@MaPhong", contract.MaPhong);
            command.Parameters.AddWithValue("@NgayBatDau", contract.NgayBatDau);
            command.Parameters.AddWithValue("@NgayKetThuc", contract.NgayKetThuc);
            command.Parameters.AddWithValue("@TienCoc", contract.TienCoc);
            command.Parameters.AddWithValue("@FileHopDong", contract.FileHopDong ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@TrangThai", contract.TrangThai ?? "Hiệu lực");

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
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
        // CÁC HÀM BỔ SUNG (IMPLEMENTING MISSING INTERFACE MEMBERS)
        // ============================================================

        // 1. Lấy hợp đồng còn hiệu lực
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

        // 2. Lấy hợp đồng sắp hết hạn trong X ngày
        public async Task<List<Contract>> GetExpiringContractsAsync(int days)
        {
            var contracts = new List<Contract>();
            using var connection = new MySqlConnection(_connectionString);
            // DATEDIFF(end_date, now) trả về số ngày còn lại
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

        // 3. Lấy hợp đồng còn hiệu lực của một khách thuê cụ thể
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
                WHERE hd.TrangThai = 'Hiệu lực' AND hd.MaNguoiThue = @MaNguoiThue
                ORDER BY hd.MaHopDong DESC";

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

        // 4. Lấy 1 khách thuê mới nhất có đặt cọc
        public async Task<(int MaNguoiThue, string HoTen, decimal TienCoc, string TrangThai)?> GetMostRecentTenantWithDepositAsync()
        {
            using var connection = new MySqlConnection(_connectionString);
            // Lấy hợp đồng hiệu lực mới nhất, join với người thuê
            var sql = @"
                SELECT hd.MaNguoiThue, nt.HoTen, hd.TienCoc, 'Đang thuê' AS TrangThai
                FROM HopDong hd
                INNER JOIN NguoiThue nt ON nt.MaNguoiThue = hd.MaNguoiThue
                WHERE hd.TrangThai = 'Hiệu lực'
                ORDER BY hd.NgayBatDau DESC, hd.MaHopDong DESC
                LIMIT 1";

            using var command = new MySqlCommand(sql, connection);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return (
                    reader.GetInt32(0),
                    reader.IsDBNull(1) ? "" : reader.GetString(1),
                    reader.GetDecimal(2),
                    reader.GetString(3)
                );
            }
            return null;
        }

        // 5. Lấy danh sách khách thuê mới nhất có đặt cọc (cho Dashboard)
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

        // ============================================================
        // HELPERS
        // ============================================================

        private Contract ReadContractWithJoin(DbDataReader reader)
        {
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