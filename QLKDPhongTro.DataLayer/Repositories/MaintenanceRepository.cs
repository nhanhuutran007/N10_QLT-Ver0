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
            // Lấy thông tin bảo trì trước khi xóa để lưu signature
            var incident = await GetByIdAsync(id);
            if (incident != null)
            {
                // Đánh dấu là đã xóa để không sync lại từ Google Sheets
                await MarkAsDeletedFromSyncAsync(incident.MaPhong, incident.MoTaSuCo, incident.NgayBaoCao);
            }

            using var connection = new SqlConnection(_connectionString);
            var cmd = new SqlCommand("DELETE FROM BaoTri_SuCo WHERE MaSuCo = @id", connection);
            cmd.Parameters.AddWithValue("@id", id);
            await connection.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Đánh dấu một bảo trì đã bị xóa để không sync lại từ Google Sheets
        /// </summary>
        public async Task MarkAsDeletedFromSyncAsync(int maPhong, string moTaSuCo, DateTime ngayBaoCao)
        {
            // Tạo bảng nếu chưa tồn tại
            await EnsureDeletedSignaturesTableExistsAsync();

            using var connection = new SqlConnection(_connectionString);
            // Kiểm tra xem đã tồn tại chưa
            var checkCmd = new SqlCommand(
                "SELECT COUNT(*) FROM DeletedMaintenanceSignatures WHERE MaPhong = @MaPhong AND MoTaSuCo = @MoTaSuCo AND NgayBaoCao = @NgayBaoCao",
                connection);
            checkCmd.Parameters.AddWithValue("@MaPhong", maPhong);
            checkCmd.Parameters.AddWithValue("@MoTaSuCo", moTaSuCo);
            checkCmd.Parameters.AddWithValue("@NgayBaoCao", ngayBaoCao.Date);
            await connection.OpenAsync();
            var exists = (int)await checkCmd.ExecuteScalarAsync() > 0;

            if (!exists)
            {
                var cmd = new SqlCommand(
                    "INSERT INTO DeletedMaintenanceSignatures (MaPhong, MoTaSuCo, NgayBaoCao, NgayXoa) VALUES (@MaPhong, @MoTaSuCo, @NgayBaoCao, @NgayXoa)",
                    connection);
                cmd.Parameters.AddWithValue("@MaPhong", maPhong);
                cmd.Parameters.AddWithValue("@MoTaSuCo", moTaSuCo);
                cmd.Parameters.AddWithValue("@NgayBaoCao", ngayBaoCao.Date);
                cmd.Parameters.AddWithValue("@NgayXoa", DateTime.Now);
                await cmd.ExecuteNonQueryAsync();
            }
        }

        /// <summary>
        /// Kiểm tra xem một bảo trì có bị đánh dấu là đã xóa không
        /// </summary>
        public async Task<bool> IsDeletedFromSyncAsync(int maPhong, string moTaSuCo, DateTime ngayBaoCao)
        {
            await EnsureDeletedSignaturesTableExistsAsync();

            using var connection = new SqlConnection(_connectionString);
            var cmd = new SqlCommand(
                "SELECT COUNT(*) FROM DeletedMaintenanceSignatures WHERE MaPhong = @MaPhong AND MoTaSuCo = @MoTaSuCo AND NgayBaoCao = @NgayBaoCao",
                connection);
            cmd.Parameters.AddWithValue("@MaPhong", maPhong);
            cmd.Parameters.AddWithValue("@MoTaSuCo", moTaSuCo);
            cmd.Parameters.AddWithValue("@NgayBaoCao", ngayBaoCao.Date);
            await connection.OpenAsync();
            var count = (int)await cmd.ExecuteScalarAsync();
            return count > 0;
        }

        /// <summary>
        /// Đảm bảo bảng DeletedMaintenanceSignatures tồn tại
        /// </summary>
        private async Task EnsureDeletedSignaturesTableExistsAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            // Kiểm tra xem bảng có tồn tại không
            var checkTableCmd = new SqlCommand(
                "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'DeletedMaintenanceSignatures'",
                connection);
            var tableExists = (int)await checkTableCmd.ExecuteScalarAsync() > 0;

            if (!tableExists)
            {
                // Tạo bảng
                var createTableCmd = new SqlCommand(@"
                    CREATE TABLE [dbo].[DeletedMaintenanceSignatures](
                        [Id] [int] IDENTITY(1,1) NOT NULL,
                        [MaPhong] [int] NOT NULL,
                        [MoTaSuCo] [nvarchar](255) NOT NULL,
                        [NgayBaoCao] [date] NOT NULL,
                        [NgayXoa] [datetime] NOT NULL DEFAULT GETDATE(),
                        PRIMARY KEY CLUSTERED ([Id] ASC)
                    )", connection);
                await createTableCmd.ExecuteNonQueryAsync();
            }
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
