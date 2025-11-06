using QLKDPhongTro.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using MySql.Data.MySqlClient; // Đã chuyển sang MySQL

namespace QLKDPhongTro.DataLayer.Repositories
{
    public class ContractRepository : IContractRepository
    {
        // Sử dụng ConnectDB chung để quản lý connection string
        private string _connectionString => ConnectDB.GetConnectionString();

        public async Task<List<Contract>> GetAllHopDongAsync()
        {
            var contracts = new List<Contract>();
            using var connection = new MySqlConnection(_connectionString);
            var command = new MySqlCommand("SELECT * FROM HopDong", connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                contracts.Add(ReadContract(reader));
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
                @"SELECT * FROM HopDong 
                  WHERE BINARY TrangThai = 'Hiệu lực' 
                  AND DATEDIFF(NgayKetThuc, NOW()) BETWEEN 0 AND @Days", connection);

            command.Parameters.AddWithValue("@Days", days);
            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                contracts.Add(ReadContract(reader));
            }
            return contracts;
        }

        public async Task<Contract?> GetByIdAsync(int maHopDong)
        {
            using var connection = new MySqlConnection(_connectionString);
            var command = new MySqlCommand("SELECT * FROM HopDong WHERE MaHopDong = @MaHopDong", connection);
            command.Parameters.AddWithValue("@MaHopDong", maHopDong);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return ReadContract(reader);
            }
            return null;
        }

        public async Task<List<Contract>> GetActiveContractsAsync()
        {
            var contracts = new List<Contract>();
            // Sử dụng ConnectDB.CreateConnectionAsync() để tự động set charset utf8mb4
            using var connection = await ConnectDB.CreateConnectionAsync();
            
            // ENUM('Hiệu lực','Hết hạn','Hủy') - giá trị phải khớp chính xác
            var command = new MySqlCommand("SELECT * FROM HopDong WHERE TrangThai = 'Hiệu lực'", connection);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                contracts.Add(ReadContract(reader));
            }
            return contracts;
        }

        public async Task<List<Contract>> GetActiveContractsByTenantAsync(int maNguoiThue)
        {
            var contracts = new List<Contract>();
            // Sử dụng ConnectDB.CreateConnectionAsync() để tự động set charset utf8mb4
            using var connection = await ConnectDB.CreateConnectionAsync();
            
            // ENUM('Hiệu lực','Hết hạn','Hủy') - giá trị phải khớp chính xác
            var command = new MySqlCommand("SELECT * FROM HopDong WHERE TrangThai = 'Hiệu lực' AND MaNguoiThue = @MaNguoiThue", connection);
            command.Parameters.AddWithValue("@MaNguoiThue", maNguoiThue);

            // Đảm bảo reader được đóng hoàn toàn trước khi sử dụng connection cho query khác
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    contracts.Add(ReadContract(reader));
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

    }
}
