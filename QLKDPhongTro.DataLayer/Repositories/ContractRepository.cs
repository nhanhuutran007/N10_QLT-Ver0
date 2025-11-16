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
        // Sử dụng ConnectDB chung để quản lý connection string
        private string _connectionString => ConnectDB.GetConnectionString();

        /// <summary>
        /// Lấy tất cả hop dong kèm thông tin phòng và thông tin người thuê (LEFT JOIN)
        /// </summary>
        public async Task<List<Contract>> GetAllHopDongAsync()
        {
            var contracts = new List<Contract>();
            using var connection = new MySqlConnection(_connectionString);
            var command = new MySqlCommand(
                @"SELECT hd.MaHopDong, hd.MaNguoiThue, hd.MaPhong, hd.NgayBatDau, hd.NgayKetThuc, 
                         hd.TienCoc, hd.FileHopDong, hd.TrangThai,
                         p.TenPhong, nt.HoTen
                  FROM HopDong hd
                  LEFT JOIN Phong p ON hd.MaPhong = p.MaPhong
                  LEFT JOIN NguoiThue nt ON hd.MaNguoiThue = nt.MaNguoiThue
                  ORDER BY hd.MaHopDong DESC", connection);

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
                         p.TenPhong, nt.HoTen
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

        public async Task AddHopDongAsync(Contract contract)
        {
            using var connection = new MySqlConnection(_connectionString);
            var command = new MySqlCommand(
                @"INSERT INTO HopDong 
                  (MaNguoiThue, MaPhong, NgayBatDau, NgayKetThuc, TienCoc, FileHopDong, TrangThai) 
                  VALUES (@MaNguoiThue, @MaPhong, @NgayBatDau, @NgayKetThuc, @TienCoc, @FileHopDong, @TrangThai)",
                connection);

            command.Parameters.AddWithValue("@MaNguoiThue", contract.MaNguoiThue);
            command.Parameters.AddWithValue("@MaPhong", contract.MaPhong);
            command.Parameters.AddWithValue("@NgayBatDau", contract.NgayBatDau);
            command.Parameters.AddWithValue("@NgayKetThuc", contract.NgayKetThuc);
            command.Parameters.AddWithValue("@TienCoc", contract.TienCoc);
            command.Parameters.AddWithValue("@FileHopDong", string.IsNullOrEmpty(contract.FileHopDong) ? (object)DBNull.Value : contract.FileHopDong);
            command.Parameters.AddWithValue("@TrangThai", contract.TrangThai);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task UpdateHopDongAsync(Contract contract)
        {
            using var connection = new MySqlConnection(_connectionString);
            var command = new MySqlCommand(
                @"UPDATE HopDong SET 
                    MaNguoiThue = @MaNguoiThue,
                    MaPhong = @MaPhong,
                    NgayBatDau = @NgayBatDau,
                    NgayKetThuc = @NgayKetThuc,
                    TienCoc = @TienCoc,
                    FileHopDong = @FileHopDong,
                    TrangThai = @TrangThai
                  WHERE MaHopDong = @MaHopDong", connection);

            command.Parameters.AddWithValue("@MaHopDong", contract.MaHopDong);
            command.Parameters.AddWithValue("@MaNguoiThue", contract.MaNguoiThue);
            command.Parameters.AddWithValue("@MaPhong", contract.MaPhong);
            command.Parameters.AddWithValue("@NgayBatDau", contract.NgayBatDau);
            command.Parameters.AddWithValue("@NgayKetThuc", contract.NgayKetThuc);
            command.Parameters.AddWithValue("@TienCoc", contract.TienCoc);
            command.Parameters.AddWithValue("@FileHopDong", string.IsNullOrEmpty(contract.FileHopDong) ? (object)DBNull.Value : contract.FileHopDong);
            command.Parameters.AddWithValue("@TrangThai", contract.TrangThai);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task DeleteHopDongAsync(int maHopDong)
        {
            using var connection = new MySqlConnection(_connectionString);
            var command = new MySqlCommand("DELETE FROM HopDong WHERE MaHopDong = @MaHopDong", connection);
            command.Parameters.AddWithValue("@MaHopDong", maHopDong);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task<List<Contract>> GetExpiringContractsAsync(int days)
        {
            var contracts = new List<Contract>();
            using var connection = new MySqlConnection(_connectionString);
            var command = new MySqlCommand(
                @"SELECT hd.MaHopDong, hd.MaNguoiThue, hd.MaPhong, hd.NgayBatDau, hd.NgayKetThuc, 
                         hd.TienCoc, hd.FileHopDong, hd.TrangThai,
                         p.TenPhong, nt.HoTen
                  FROM HopDong hd
                  LEFT JOIN Phong p ON hd.MaPhong = p.MaPhong
                  LEFT JOIN NguoiThue nt ON hd.MaNguoiThue = nt.MaNguoiThue
                  WHERE BINARY hd.TrangThai = 'Hiệu lực' 
                  AND DATEDIFF(hd.NgayKetThuc, NOW()) BETWEEN 0 AND @Days
                  ORDER BY hd.NgayKetThuc ASC", connection);

            command.Parameters.AddWithValue("@Days", days);
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
            var command = new MySqlCommand(
                @"SELECT hd.MaHopDong, hd.MaNguoiThue, hd.MaPhong, hd.NgayBatDau, hd.NgayKetThuc, 
                         hd.TienCoc, hd.FileHopDong, hd.TrangThai,
                         p.TenPhong, nt.HoTen
                  FROM HopDong hd
                  LEFT JOIN Phong p ON hd.MaPhong = p.MaPhong
                  LEFT JOIN NguoiThue nt ON hd.MaNguoiThue = nt.MaNguoiThue
                  WHERE hd.MaHopDong = @MaHopDong", connection);
            command.Parameters.AddWithValue("@MaHopDong", maHopDong);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return ReadContractWithJoin(reader);
            }

            return null;
        }

        public async Task<List<Contract>> GetActiveContractsAsync()
        {
            var contracts = new List<Contract>();
            // Sử dụng ConnectDB.CreateConnectionAsync() để tự động set charset utf8mb4
            using var connection = await ConnectDB.CreateConnectionAsync();

            // ENUM('Hiệu lực','Hết hạn','Hủy') - giá trị phải khớp chính xác
            var command = new MySqlCommand(
                @"SELECT hd.MaHopDong, hd.MaNguoiThue, hd.MaPhong, hd.NgayBatDau, hd.NgayKetThuc, 
                         hd.TienCoc, hd.FileHopDong, hd.TrangThai,
                         p.TenPhong, nt.HoTen
                  FROM HopDong hd
                  LEFT JOIN Phong p ON hd.MaPhong = p.MaPhong
                  LEFT JOIN NguoiThue nt ON hd.MaNguoiThue = nt.MaNguoiThue
                  WHERE hd.TrangThai = 'Hiệu lực'
                  ORDER BY hd.MaHopDong DESC", connection);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                contracts.Add(ReadContractWithJoin(reader));
            }

            return contracts;
        }

        public async Task<List<Contract>> GetActiveContractsByTenantAsync(int maNguoiThue)
        {
            var contracts = new List<Contract>();
            // Sử dụng ConnectDB.CreateConnectionAsync() để tự động set charset utf8mb4
            using var connection = await ConnectDB.CreateConnectionAsync();

            // ENUM('Hiệu lực','Hết hạn','Hủy') - giá trị phải khớp chính xác
            var command = new MySqlCommand(
                @"SELECT hd.MaHopDong, hd.MaNguoiThue, hd.MaPhong, hd.NgayBatDau, hd.NgayKetThuc, 
                         hd.TienCoc, hd.FileHopDong, hd.TrangThai,
                         p.TenPhong, nt.HoTen
                  FROM HopDong hd
                  LEFT JOIN Phong p ON hd.MaPhong = p.MaPhong
                  LEFT JOIN NguoiThue nt ON hd.MaNguoiThue = nt.MaNguoiThue
                  WHERE hd.TrangThai = 'Hiệu lực' AND hd.MaNguoiThue = @MaNguoiThue
                  ORDER BY hd.MaHopDong DESC", connection);
            command.Parameters.AddWithValue("@MaNguoiThue", maNguoiThue);

            // Đảm bảo reader được đóng hoàn toàn trước khi sử dụng connection cho query khác
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    contracts.Add(ReadContractWithJoin(reader));
                }
            } // Reader được dispose ở đây

            // Debug logging
            System.Diagnostics.Debug.WriteLine($"GetActiveContractsByTenantAsync: MaNguoiThue={maNguoiThue}, Found {contracts.Count} contracts");
            if (contracts.Count == 0)
            {
                // Debug: Kiểm tra xem có hợp đồng nào với MaNguoiThue này không (không phân biệt trạng thái)
                // Sử dụng ExecuteScalarAsync vì không cần reader
                var checkCmd = new MySqlCommand("SELECT COUNT(*) FROM HopDong WHERE MaNguoiThue = @MaNguoiThue", connection);
                checkCmd.Parameters.AddWithValue("@MaNguoiThue", maNguoiThue);
                var totalCount = Convert.ToInt32(await checkCmd.ExecuteScalarAsync());
                System.Diagnostics.Debug.WriteLine($"GetActiveContractsByTenantAsync: Total contracts for MaNguoiThue={maNguoiThue}: {totalCount}");

                // Kiểm tra trạng thái của các hợp đồng - sử dụng connection riêng để tránh conflict
                // Sử dụng ConnectDB.CreateConnectionAsync() để tự động set charset utf8mb4
                using (var debugConnection = await ConnectDB.CreateConnectionAsync())
                {

                    var statusCmd = new MySqlCommand("SELECT MaHopDong, TrangThai FROM HopDong WHERE MaNguoiThue = @MaNguoiThue", debugConnection);
                    statusCmd.Parameters.AddWithValue("@MaNguoiThue", maNguoiThue);
                    using (var statusReader = await statusCmd.ExecuteReaderAsync())
                    {
                        while (await statusReader.ReadAsync())
                        {
                            var maHD = statusReader.GetInt32(0);
                            var trangThai = statusReader.IsDBNull(1) ? "NULL" : statusReader.GetString(1);
                            System.Diagnostics.Debug.WriteLine($"GetActiveContractsByTenantAsync: Contract {maHD} has status: '{trangThai}'");
                        }
                    }
                }
            }

            return contracts;
        }

        /// <summary>
        /// Đọc một record từ DbDataReader và ánh xạ vào model Contract (bao gồm thông tin phòng và người thuê nếu có)
        /// </summary>
        private static Contract ReadContract(DbDataReader reader)
        {
            return new Contract
            {
                MaHopDong = reader.GetInt32(reader.GetOrdinal("MaHopDong")),
                MaNguoiThue = reader.GetInt32(reader.GetOrdinal("MaNguoiThue")),
                MaPhong = reader.GetInt32(reader.GetOrdinal("MaPhong")),
                NgayBatDau = reader.GetDateTime(reader.GetOrdinal("NgayBatDau")),
                NgayKetThuc = reader.GetDateTime(reader.GetOrdinal("NgayKetThuc")),
                TienCoc = reader.GetDecimal(reader.GetOrdinal("TienCoc")),
                FileHopDong = reader.IsDBNull(reader.GetOrdinal("FileHopDong")) ? string.Empty : reader.GetString(reader.GetOrdinal("FileHopDong")),
                TrangThai = reader.GetString(reader.GetOrdinal("TrangThai"))
            };
        }

        private static Contract ReadContractWithJoin(DbDataReader reader)
        {
            return new Contract
            {
                MaHopDong = reader.GetInt32(0),
                MaNguoiThue = reader.GetInt32(1),
                MaPhong = reader.GetInt32(2),
                NgayBatDau = reader.GetDateTime(3),
                NgayKetThuc = reader.GetDateTime(4),
                TienCoc = reader.GetDecimal(5),
                FileHopDong = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                TrangThai = reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
                TenPhong = reader.IsDBNull(8) ? string.Empty : reader.GetString(8),
                TenNguoiThue = reader.IsDBNull(9) ? string.Empty : reader.GetString(9)
            };
        }

        public async Task<(int MaNguoiThue, string HoTen, decimal TienCoc, string TrangThai)?> GetMostRecentTenantWithDepositAsync()
        {
            using var connection = await ConnectDB.CreateConnectionAsync();
            var cmd = new MySqlCommand(@"
                SELECT hd.MaNguoiThue, nt.HoTen, hd.TienCoc, nt.TrangThai
                FROM HopDong hd
                INNER JOIN NguoiThue nt ON nt.MaNguoiThue = hd.MaNguoiThue
                WHERE hd.TrangThai = 'Hiệu lực' AND nt.TrangThai = 'Đang ở'
                ORDER BY hd.NgayBatDau DESC, hd.MaHopDong DESC
                LIMIT 1;", connection);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var maNguoiThue = reader.GetInt32(0);
                var hoTen = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
                var tienCoc = reader.IsDBNull(2) ? 0m : reader.GetDecimal(2);
                var trangThai = reader.IsDBNull(3) ? string.Empty : reader.GetString(3);
                return (maNguoiThue, hoTen, tienCoc, trangThai);
            }
            return null;
        }

        public async Task<List<(int MaNguoiThue, string HoTen, decimal TienCoc, string TrangThai)>> GetMostRecentTenantsWithDepositAsync(int count)
        {
            var list = new List<(int, string, decimal, string)>();
            using var connection = await ConnectDB.CreateConnectionAsync();
            var limit = Math.Max(1, Math.Min(10, count));

            // 1) Ưu tiên: NT 'Đang ở' + HĐ 'Hiệu lực'
            var sqlPrimary = $@"
                SELECT x.MaNguoiThue, nt.HoTen, x.TienCoc, nt.TrangThai
                FROM (
                    SELECT hd.MaNguoiThue,
                           MAX(hd.NgayBatDau) AS NgayBatDauMoiNhat,
                           MAX(hd.MaHopDong) AS MaHDMoiNhat,
                           SUBSTRING_INDEX(GROUP_CONCAT(hd.TienCoc ORDER BY hd.NgayBatDau DESC, hd.MaHopDong DESC), ',', 1) AS TienCoc
                    FROM HopDong hd
                    WHERE hd.TrangThai = 'Hiệu lực'
                    GROUP BY hd.MaNguoiThue
                ) x
                INNER JOIN NguoiThue nt ON nt.MaNguoiThue = x.MaNguoiThue AND nt.TrangThai = 'Đang ở'
                ORDER BY x.NgayBatDauMoiNhat DESC, x.MaHDMoiNhat DESC
                LIMIT {limit};";
            using (var cmdPrimary = new MySqlCommand(sqlPrimary, connection))
            using (var reader = await cmdPrimary.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    var maNguoiThue = reader.GetInt32(0);
                    var hoTen = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
                    var tienCoc = reader.IsDBNull(2) ? 0m : reader.GetDecimal(2);
                    var trangThai = reader.IsDBNull(3) ? string.Empty : reader.GetString(3);
                    list.Add((maNguoiThue, hoTen, tienCoc, trangThai));
                }
            }

            if (list.Count >= limit)
                return list;

            // 2) Bổ sung: lấy khách gần nhất theo mọi trạng thái (cả HĐ và NT)
            var remain = limit - list.Count;
            var excludeIds = list.Select(x => x.Item1).ToList();
            var notInClause = excludeIds.Count > 0 ? $"WHERE nt.MaNguoiThue NOT IN ({string.Join(",", excludeIds)})" : string.Empty;

            var sqlFallback = $@"
                SELECT nt.MaNguoiThue, nt.HoTen, x.TienCoc, nt.TrangThai
                FROM (
                    SELECT hd.MaNguoiThue,
                           MAX(hd.NgayBatDau) AS NgayBatDauMoiNhat,
                           MAX(hd.MaHopDong) AS MaHDMoiNhat,
                           SUBSTRING_INDEX(GROUP_CONCAT(hd.TienCoc ORDER BY hd.NgayBatDau DESC, hd.MaHopDong DESC), ',', 1) AS TienCoc
                    FROM HopDong hd
                    GROUP BY hd.MaNguoiThue
                ) x
                INNER JOIN NguoiThue nt ON nt.MaNguoiThue = x.MaNguoiThue
                {notInClause}
                ORDER BY x.NgayBatDauMoiNhat DESC, x.MaHDMoiNhat DESC
                LIMIT {remain};";

            using (var cmdFallback = new MySqlCommand(sqlFallback, connection))
            using (var reader2 = await cmdFallback.ExecuteReaderAsync())
            {
                while (await reader2.ReadAsync())
                {
                    var maNguoiThue = reader2.GetInt32(0);
                    var hoTen = reader2.IsDBNull(1) ? string.Empty : reader2.GetString(1);
                    var tienCoc = reader2.IsDBNull(2) ? 0m : reader2.GetDecimal(2);
                    var trangThai = reader2.IsDBNull(3) ? string.Empty : reader2.GetString(3);
                    list.Add((maNguoiThue, hoTen, tienCoc, trangThai));
                }
            }

            return list;
        }

    }
}
