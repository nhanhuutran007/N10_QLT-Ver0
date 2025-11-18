using QLKDPhongTro.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using MySql.Data.MySqlClient; // Đã chuyển sang MySQL

namespace QLKDPhongTro.DataLayer.Repositories
{
    public class MaintenanceRepository : IMaintenanceRepository
    {
        // Sử dụng ConnectDB chung để quản lý connection string
        private string _connectionString => ConnectDB.GetConnectionString();

        public async Task<List<MaintenanceIncident>> GetAllAsync()
        {
            var result = new List<MaintenanceIncident>();
            // Sử dụng ConnectDB.CreateConnectionAsync() để tự động set charset utf8mb4
            using var connection = await ConnectDB.CreateConnectionAsync();
            var cmd = new MySqlCommand("SELECT * FROM BaoTri_SuCo", connection);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                result.Add(ReadIncident(reader));
            }
            return result;
        }

        public async Task<List<MaintenanceIncident>> GetAllByMaNhaAsync(int maNha)
        {
            var result = new List<MaintenanceIncident>();
            using var connection = await ConnectDB.CreateConnectionAsync();
            var cmd = new MySqlCommand(@"
                SELECT b.*
                FROM BaoTri_SuCo b
                INNER JOIN Phong p ON p.MaPhong = b.MaPhong
                WHERE p.MaNha = @MaNha", connection);
            cmd.Parameters.AddWithValue("@MaNha", maNha);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                result.Add(ReadIncident(reader));
            }
            return result;
        }

        public async Task<MaintenanceIncident?> GetByIdAsync(int id)
        {
            // Sử dụng ConnectDB.CreateConnectionAsync() để tự động set charset utf8mb4
            using var connection = await ConnectDB.CreateConnectionAsync();
            var cmd = new MySqlCommand("SELECT * FROM BaoTri_SuCo WHERE MaSuCo = @id", connection);
            cmd.Parameters.AddWithValue("@id", id);
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
                return ReadIncident(reader);
            return null;
        }

        public async Task AddAsync(MaintenanceIncident incident)
        {
            // Sử dụng ConnectDB.CreateConnectionAsync() để tự động set charset utf8mb4
            using var connection = await ConnectDB.CreateConnectionAsync();
            var cmd = new MySqlCommand(
                "INSERT INTO BaoTri_SuCo (MaPhong, MoTaSuCo, NgayBaoCao, TrangThai, ChiPhi) VALUES (@MaPhong, @MoTaSuCo, @NgayBaoCao, @TrangThai, @ChiPhi)",
                connection);
            cmd.Parameters.AddWithValue("@MaPhong", incident.MaPhong);
            cmd.Parameters.AddWithValue("@MoTaSuCo", incident.MoTaSuCo);
            cmd.Parameters.AddWithValue("@NgayBaoCao", incident.NgayBaoCao);
            cmd.Parameters.AddWithValue("@TrangThai", incident.TrangThai);
            cmd.Parameters.AddWithValue("@ChiPhi", incident.ChiPhi);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task UpdateAsync(MaintenanceIncident incident)
        {
            // Sử dụng ConnectDB.CreateConnectionAsync() để tự động set charset utf8mb4
            using var connection = await ConnectDB.CreateConnectionAsync();
            var cmd = new MySqlCommand(
                "UPDATE BaoTri_SuCo SET MaPhong = @MaPhong, MoTaSuCo = @MoTaSuCo, NgayBaoCao = @NgayBaoCao, TrangThai = @TrangThai, ChiPhi = @ChiPhi WHERE MaSuCo = @MaSuCo",
                connection);
            cmd.Parameters.AddWithValue("@MaSuCo", incident.MaSuCo);
            cmd.Parameters.AddWithValue("@MaPhong", incident.MaPhong);
            cmd.Parameters.AddWithValue("@MoTaSuCo", incident.MoTaSuCo);
            cmd.Parameters.AddWithValue("@NgayBaoCao", incident.NgayBaoCao);
            cmd.Parameters.AddWithValue("@TrangThai", incident.TrangThai);
            cmd.Parameters.AddWithValue("@ChiPhi", incident.ChiPhi);
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

            // Sử dụng ConnectDB.CreateConnectionAsync() để tự động set charset utf8mb4
            using var connection = await ConnectDB.CreateConnectionAsync();
            var cmd = new MySqlCommand("DELETE FROM BaoTri_SuCo WHERE MaSuCo = @id", connection);
            cmd.Parameters.AddWithValue("@id", id);
            await cmd.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Đánh dấu một bảo trì đã bị xóa để không sync lại từ Google Sheets
        /// </summary>
        public async Task MarkAsDeletedFromSyncAsync(int maPhong, string moTaSuCo, DateTime ngayBaoCao)
        {
            // Tạo bảng nếu chưa tồn tại
            await EnsureDeletedSignaturesTableExistsAsync();

            // Sử dụng ConnectDB.CreateConnectionAsync() để tự động set charset utf8mb4
            using var connection = await ConnectDB.CreateConnectionAsync();
            
            // Kiểm tra xem đã tồn tại chưa - sử dụng BINARY để so sánh byte-by-byte, tránh lỗi collation mismatch
            var checkCmd = new MySqlCommand(
                "SELECT COUNT(*) FROM DeletedMaintenanceSignatures WHERE MaPhong = @MaPhong AND BINARY MoTaSuCo = @MoTaSuCo AND NgayBaoCao = @NgayBaoCao",
                connection);
            checkCmd.Parameters.AddWithValue("@MaPhong", maPhong);
            checkCmd.Parameters.AddWithValue("@MoTaSuCo", moTaSuCo);
            checkCmd.Parameters.AddWithValue("@NgayBaoCao", ngayBaoCao.Date);
            var exists = Convert.ToInt32(await checkCmd.ExecuteScalarAsync()) > 0;

            if (!exists)
            {
                var cmd = new MySqlCommand(
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

            // Sử dụng ConnectDB.CreateConnectionAsync() để tự động set charset utf8mb4
            using var connection = await ConnectDB.CreateConnectionAsync();
            
            // Sử dụng BINARY để so sánh byte-by-byte, tránh lỗi collation mismatch
            var cmd = new MySqlCommand(
                "SELECT COUNT(*) FROM DeletedMaintenanceSignatures WHERE MaPhong = @MaPhong AND BINARY MoTaSuCo = @MoTaSuCo AND NgayBaoCao = @NgayBaoCao",
                connection);
            cmd.Parameters.AddWithValue("@MaPhong", maPhong);
            cmd.Parameters.AddWithValue("@MoTaSuCo", moTaSuCo);
            cmd.Parameters.AddWithValue("@NgayBaoCao", ngayBaoCao.Date);
            var count = Convert.ToInt32(await cmd.ExecuteScalarAsync());
            return count > 0;
        }

        /// <summary>
        /// Đảm bảo bảng DeletedMaintenanceSignatures tồn tại
        /// </summary>
        private async Task EnsureDeletedSignaturesTableExistsAsync()
        {
            // Sử dụng ConnectDB.CreateConnectionAsync() để tự động set charset utf8mb4
            using var connection = await ConnectDB.CreateConnectionAsync();

            // Kiểm tra xem bảng có tồn tại không
            var checkTableCmd = new MySqlCommand(
                "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'DeletedMaintenanceSignatures'",
                connection);
            var result = await checkTableCmd.ExecuteScalarAsync();
            var tableExists = result != null && Convert.ToInt64(result) > 0;

            if (!tableExists)
            {
                // Tạo bảng với charset utf8mb4 và collation utf8mb4_unicode_ci
                var createTableCmd = new MySqlCommand(@"
                    CREATE TABLE IF NOT EXISTS DeletedMaintenanceSignatures(
                        Id INT AUTO_INCREMENT NOT NULL,
                        MaPhong INT NOT NULL,
                        MoTaSuCo VARCHAR(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
                        NgayBaoCao DATE NOT NULL,
                        NgayXoa DATETIME NOT NULL DEFAULT NOW(),
                        PRIMARY KEY (Id)
                    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci", connection);
                await createTableCmd.ExecuteNonQueryAsync();
            }
        }

        private static MaintenanceIncident ReadIncident(DbDataReader reader)
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
