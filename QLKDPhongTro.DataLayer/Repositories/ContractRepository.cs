using QLKDPhongTro.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace QLKDPhongTro.DataLayer.Repositories
{
    public class ContractRepository : IContractRepository
    {
        private readonly string _connectionString =
            "Data Source=.;Initial Catalog=QLThueNhaV1;Integrated Security=True;TrustServerCertificate=True;Encrypt=False";

        public async Task<List<Contract>> GetAllHopDongAsync()
        {
            var contracts = new List<Contract>();
            using var connection = new SqlConnection(_connectionString);
            var command = new SqlCommand("SELECT * FROM HopDong", connection);

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
            using var connection = new SqlConnection(_connectionString);
            var command = new SqlCommand(
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
            using var connection = new SqlConnection(_connectionString);
            var command = new SqlCommand(
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
            using var connection = new SqlConnection(_connectionString);
            var command = new SqlCommand("DELETE FROM HopDong WHERE MaHopDong = @MaHopDong", connection);
            command.Parameters.AddWithValue("@MaHopDong", maHopDong);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task<List<Contract>> GetExpiringContractsAsync(int days)
        {
            var contracts = new List<Contract>();
            using var connection = new SqlConnection(_connectionString);
            var command = new SqlCommand(
                @"SELECT * FROM HopDong 
                  WHERE TrangThai = N'Hiệu lực' 
                  AND DATEDIFF(day, GETDATE(), NgayKetThuc) BETWEEN 0 AND @Days", connection);

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
            using var connection = new SqlConnection(_connectionString);
            var command = new SqlCommand("SELECT * FROM HopDong WHERE MaHopDong = @MaHopDong", connection);
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
            using var connection = new SqlConnection(_connectionString);
            var command = new SqlCommand("SELECT * FROM HopDong WHERE TrangThai = N'Hiệu lực'", connection);

            await connection.OpenAsync();
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
            using var connection = new SqlConnection(_connectionString);
            var command = new SqlCommand("SELECT * FROM HopDong WHERE TrangThai = N'Hiệu lực' AND MaNguoiThue = @MaNguoiThue", connection);
            command.Parameters.AddWithValue("@MaNguoiThue", maNguoiThue);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                contracts.Add(ReadContract(reader));
            }
            return contracts;
        }

        private static Contract ReadContract(SqlDataReader reader)
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
