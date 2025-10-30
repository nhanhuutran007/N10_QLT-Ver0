using QLKDPhongTro.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace QLKDPhongTro.DataLayer.Repositories
{
    public class MaintenanceRepository : IMaintenanceRepository
    {
        private readonly string _connectionString =
            "Data Source=.;Initial Catalog=QLThueNhaV1;Integrated Security=True;TrustServerCertificate=True;Encrypt=False";

        public async Task<List<MaintenanceIncident>> GetAllAsync()
        {
            var result = new List<MaintenanceIncident>();
            using var connection = new SqlConnection(_connectionString);
            var cmd = new SqlCommand("SELECT * FROM BaoTri_SuCo", connection);
            await connection.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                result.Add(ReadIncident(reader));
            }
            return result;
        }

        public async Task<MaintenanceIncident?> GetByIdAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            var cmd = new SqlCommand("SELECT * FROM BaoTri_SuCo WHERE MaSuCo = @id", connection);
            cmd.Parameters.AddWithValue("@id", id);
            await connection.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
                return ReadIncident(reader);
            return null;
        }

        public async Task AddAsync(MaintenanceIncident incident)
        {
            using var connection = new SqlConnection(_connectionString);
            var cmd = new SqlCommand(
                "INSERT INTO BaoTri_SuCo (MaPhong, MoTaSuCo, NgayBaoCao, TrangThai, ChiPhi) VALUES (@MaPhong, @MoTaSuCo, @NgayBaoCao, @TrangThai, @ChiPhi)",
                connection);
            cmd.Parameters.AddWithValue("@MaPhong", incident.MaPhong);
            cmd.Parameters.AddWithValue("@MoTaSuCo", incident.MoTaSuCo);
            cmd.Parameters.AddWithValue("@NgayBaoCao", incident.NgayBaoCao);
            cmd.Parameters.AddWithValue("@TrangThai", incident.TrangThai);
            cmd.Parameters.AddWithValue("@ChiPhi", incident.ChiPhi);
            await connection.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task UpdateAsync(MaintenanceIncident incident)
        {
            using var connection = new SqlConnection(_connectionString);
            var cmd = new SqlCommand(
                "UPDATE BaoTri_SuCo SET MaPhong = @MaPhong, MoTaSuCo = @MoTaSuCo, NgayBaoCao = @NgayBaoCao, TrangThai = @TrangThai, ChiPhi = @ChiPhi WHERE MaSuCo = @MaSuCo",
                connection);
            cmd.Parameters.AddWithValue("@MaSuCo", incident.MaSuCo);
            cmd.Parameters.AddWithValue("@MaPhong", incident.MaPhong);
            cmd.Parameters.AddWithValue("@MoTaSuCo", incident.MoTaSuCo);
            cmd.Parameters.AddWithValue("@NgayBaoCao", incident.NgayBaoCao);
            cmd.Parameters.AddWithValue("@TrangThai", incident.TrangThai);
            cmd.Parameters.AddWithValue("@ChiPhi", incident.ChiPhi);
            await connection.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            var cmd = new SqlCommand("DELETE FROM BaoTri_SuCo WHERE MaSuCo = @id", connection);
            cmd.Parameters.AddWithValue("@id", id);
            await connection.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        private static MaintenanceIncident ReadIncident(SqlDataReader reader)
        {
            return new MaintenanceIncident
            {
                MaSuCo = reader.GetInt32(reader.GetOrdinal("MaSuCo")),
                MaPhong = reader.GetInt32(reader.GetOrdinal("MaPhong")),
                MoTaSuCo = reader.GetString(reader.GetOrdinal("MoTaSuCo")),
                NgayBaoCao = reader.GetDateTime(reader.GetOrdinal("NgayBaoCao")),
                TrangThai = reader.GetString(reader.GetOrdinal("TrangThai")),
                ChiPhi = reader.GetDecimal(reader.GetOrdinal("ChiPhi")),
            };
        }
    }
}
