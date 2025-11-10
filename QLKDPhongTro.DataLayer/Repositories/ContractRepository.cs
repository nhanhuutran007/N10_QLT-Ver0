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
        private readonly string _connectionString;

        public ContractRepository()
        {
            // ===== CẤU HÌNH KẾT NỐI MY SQL =====
            string server = "host80.vietnix.vn";
            string database = "githubio_QLT_Ver1";
            string username = "githubio_admin";
            string password = "nhanhuutran007";
            string port = "3306";

            _connectionString = $"Server={server};Port={port};Database={database};Uid={username};Pwd={password};SslMode=Preferred;";
        }

        /// <summary>
        /// Lấy tất cả hop dong kèm thông tin phòng và thông tin người thuê (LEFT JOIN)
        /// </summary>
        public async Task<List<Contract>> GetAllHopDongAsync()
        {
            var list = new List<Contract>();

            var sql = @"
                SELECT h.*,
                       p.TenPhong AS Ph_TenPhong,
                       p.DienTich AS Ph_DienTich,
                       p.GiaCoBan AS Ph_GiaCoBan,
                       p.GiaBangChu AS Ph_GiaBangChu,
                       p.TrangThietBi AS Ph_TrangThietBi,
                       -- tenant columns
                       t.HoTen AS T_HoTen,
                       t.NgaySinh AS T_NgaySinh,
                       t.CCCD AS T_CCCD,
                       t.NgayCapCCCD AS T_NgayCapCCCD,
                       t.NoiCapCCCD AS T_NoiCapCCCD,
                       t.DiaChiThuongTru AS T_DiaChiThuongTru,
                       t.SoDienThoai AS T_SoDienThoai
                FROM HopDong h
                LEFT JOIN Phong p ON h.MaPhong = p.MaPhong
                LEFT JOIN NguoiThue t ON h.MaNguoiThue = t.MaNguoiThue
                ORDER BY h.NgayBatDau DESC;
            ";

            using var conn = new MySqlConnection(_connectionString);
            using var cmd = new MySqlCommand(sql, conn);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(ReadContract(reader));
            }

            return list;
        }

        public async Task<Contract?> GetByIdAsync(int maHopDong)
        {
            var sql = @"
                SELECT h.*,
                       p.TenPhong AS Ph_TenPhong,
                       p.DienTich AS Ph_DienTich,
                       p.GiaCoBan AS Ph_GiaCoBan,
                       p.GiaBangChu AS Ph_GiaBangChu,
                       p.TrangThietBi AS Ph_TrangThietBi,
                       t.HoTen AS T_HoTen,
                       t.NgaySinh AS T_NgaySinh,
                       t.CCCD AS T_CCCD,
                       t.NgayCapCCCD AS T_NgayCapCCCD,
                       t.NoiCapCCCD AS T_NoiCapCCCD,
                       t.DiaChiThuongTru AS T_DiaChiThuongTru,
                       t.SoDienThoai AS T_SoDienThoai
                FROM HopDong h
                LEFT JOIN Phong p ON h.MaPhong = p.MaPhong
                LEFT JOIN NguoiThue t ON h.MaNguoiThue = t.MaNguoiThue
                WHERE h.MaHopDong = @MaHopDong
                LIMIT 1;
            ";

            using var conn = new MySqlConnection(_connectionString);
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@MaHopDong", maHopDong);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return ReadContract(reader);
            }

            return null;
        }

        public async Task<List<Contract>> GetActiveContractsAsync()
        {
            var list = new List<Contract>();

            var sql = @"
                SELECT h.*,
                       p.TenPhong AS Ph_TenPhong,
                       p.DienTich AS Ph_DienTich,
                       p.GiaCoBan AS Ph_GiaCoBan,
                       p.GiaBangChu AS Ph_GiaBangChu,
                       p.TrangThietBi AS Ph_TrangThietBi,
                       t.HoTen AS T_HoTen,
                       t.NgaySinh AS T_NgaySinh,
                       t.CCCD AS T_CCCD,
                       t.NgayCapCCCD AS T_NgayCapCCCD,
                       t.NoiCapCCCD AS T_NoiCapCCCD,
                       t.DiaChiThuongTru AS T_DiaChiThuongTru,
                       t.SoDienThoai AS T_SoDienThoai
                FROM HopDong h
                LEFT JOIN Phong p ON h.MaPhong = p.MaPhong
                LEFT JOIN NguoiThue t ON h.MaNguoiThue = t.MaNguoiThue
                WHERE BINARY h.TrangThai = 'Hiệu lực'
                ORDER BY h.NgayKetThuc ASC;
            ";

            using var conn = new MySqlConnection(_connectionString);
            using var cmd = new MySqlCommand(sql, conn);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(ReadContract(reader));
            }

            return list;
        }

        public async Task<List<Contract>> GetActiveContractsByTenantAsync(int maNguoiThue)
        {
            var list = new List<Contract>();

            var sql = @"
                SELECT h.*,
                       p.TenPhong AS Ph_TenPhong,
                       p.DienTich AS Ph_DienTich,
                       p.GiaCoBan AS Ph_GiaCoBan,
                       p.GiaBangChu AS Ph_GiaBangChu,
                       p.TrangThietBi AS Ph_TrangThietBi,
                       t.HoTen AS T_HoTen,
                       t.NgaySinh AS T_NgaySinh,
                       t.CCCD AS T_CCCD,
                       t.NgayCapCCCD AS T_NgayCapCCCD,
                       t.NoiCapCCCD AS T_NoiCapCCCD,
                       t.DiaChiThuongTru AS T_DiaChiThuongTru,
                       t.SoDienThoai AS T_SoDienThoai
                FROM HopDong h
                LEFT JOIN Phong p ON h.MaPhong = p.MaPhong
                LEFT JOIN NguoiThue t ON h.MaNguoiThue = t.MaNguoiThue
                WHERE BINARY h.TrangThai = 'Hiệu lực' AND h.MaNguoiThue = @MaNguoiThue
                ORDER BY h.NgayKetThuc ASC;
            ";

            using var conn = new MySqlConnection(_connectionString);
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@MaNguoiThue", maNguoiThue);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(ReadContract(reader));
            }

            System.Diagnostics.Debug.WriteLine($"GetActiveContractsByTenantAsync: MaNguoiThue={maNguoiThue}, Found {list.Count} contracts");

            return list;
        }

        public async Task<List<Contract>> GetExpiringContractsAsync(int days)
        {
            var list = new List<Contract>();

            var sql = @"
                SELECT h.*,
                       p.TenPhong AS Ph_TenPhong,
                       p.DienTich AS Ph_DienTich,
                       p.GiaCoBan AS Ph_GiaCoBan,
                       p.GiaBangChu AS Ph_GiaBangChu,
                       p.TrangThietBi AS Ph_TrangThietBi,
                       t.HoTen AS T_HoTen,
                       t.NgaySinh AS T_NgaySinh,
                       t.CCCD AS T_CCCD,
                       t.NgayCapCCCD AS T_NgayCapCCCD,
                       t.NoiCapCCCD AS T_NoiCapCCCD,
                       t.DiaChiThuongTru AS T_DiaChiThuongTru,
                       t.SoDienThoai AS T_SoDienThoai
                FROM HopDong h
                LEFT JOIN Phong p ON h.MaPhong = p.MaPhong
                LEFT JOIN NguoiThue t ON h.MaNguoiThue = t.MaNguoiThue
                WHERE BINARY h.TrangThai = 'Hiệu lực'
                  AND DATEDIFF(h.NgayKetThuc, NOW()) BETWEEN 0 AND @Days
                ORDER BY h.NgayKetThuc ASC;
            ";

            using var conn = new MySqlConnection(_connectionString);
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Days", days);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(ReadContract(reader));
            }

            return list;
        }

        public async Task AddHopDongAsync(Contract contract)
        {
            var sql = @"
                INSERT INTO HopDong
                (MaNguoiThue, MaPhong, NgayBatDau, NgayKetThuc, TienCoc, GiaThue, TrangThai, FileHopDong,
                 NoiTaoHopDong, NgayTaoHopDong, GiaBangChu, NgayTraTien, ThoiHanNam, NgayGiaoNha)
                VALUES
                (@MaNguoiThue, @MaPhong, @NgayBatDau, @NgayKetThuc, @TienCoc, @GiaThue, @TrangThai, @FileHopDong,
                 @NoiTaoHopDong, @NgayTaoHopDong, @GiaBangChu, @NgayTraTien, @ThoiHanNam, @NgayGiaoNha);
            ";

            using var conn = new MySqlConnection(_connectionString);
            using var cmd = new MySqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@MaNguoiThue", contract.MaNguoiThue);
            cmd.Parameters.AddWithValue("@MaPhong", contract.MaPhong);
            cmd.Parameters.AddWithValue("@NgayBatDau", contract.NgayBatDau);
            cmd.Parameters.AddWithValue("@NgayKetThuc", contract.NgayKetThuc);
            cmd.Parameters.AddWithValue("@TienCoc", contract.TienCoc);
            cmd.Parameters.AddWithValue("@GiaThue", contract.GiaThue);
            cmd.Parameters.AddWithValue("@TrangThai", (object?)contract.TrangThai ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@FileHopDong", string.IsNullOrEmpty(contract.FileHopDong) ? (object)DBNull.Value : contract.FileHopDong);

            cmd.Parameters.AddWithValue("@NoiTaoHopDong", (object?)contract.NoiTaoHopDong ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@NgayTaoHopDong", contract.NgayTaoHopDong.HasValue ? (object)contract.NgayTaoHopDong.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@GiaBangChu", (object?)contract.GiaBangChu ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@NgayTraTien", (object?)contract.NgayTraTien ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ThoiHanNam", contract.ThoiHanNam.HasValue ? (object)contract.ThoiHanNam.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@NgayGiaoNha", contract.NgayGiaoNha.HasValue ? (object)contract.NgayGiaoNha.Value : DBNull.Value);

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task UpdateHopDongAsync(Contract contract)
        {
            var sql = @"
                UPDATE HopDong SET
                    MaNguoiThue = @MaNguoiThue,
                    MaPhong = @MaPhong,
                    NgayBatDau = @NgayBatDau,
                    NgayKetThuc = @NgayKetThuc,
                    TienCoc = @TienCoc,
                    GiaThue = @GiaThue,
                    TrangThai = @TrangThai,
                    FileHopDong = @FileHopDong,
                    NoiTaoHopDong = @NoiTaoHopDong,
                    NgayTaoHopDong = @NgayTaoHopDong,
                    GiaBangChu = @GiaBangChu,
                    NgayTraTien = @NgayTraTien,
                    ThoiHanNam = @ThoiHanNam,
                    NgayGiaoNha = @NgayGiaoNha
                WHERE MaHopDong = @MaHopDong;
            ";

            using var conn = new MySqlConnection(_connectionString);
            using var cmd = new MySqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@MaHopDong", contract.MaHopDong);
            cmd.Parameters.AddWithValue("@MaNguoiThue", contract.MaNguoiThue);
            cmd.Parameters.AddWithValue("@MaPhong", contract.MaPhong);
            cmd.Parameters.AddWithValue("@NgayBatDau", contract.NgayBatDau);
            cmd.Parameters.AddWithValue("@NgayKetThuc", contract.NgayKetThuc);
            cmd.Parameters.AddWithValue("@TienCoc", contract.TienCoc);
            cmd.Parameters.AddWithValue("@GiaThue", contract.GiaThue);
            cmd.Parameters.AddWithValue("@TrangThai", (object?)contract.TrangThai ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@FileHopDong", string.IsNullOrEmpty(contract.FileHopDong) ? (object)DBNull.Value : contract.FileHopDong);

            cmd.Parameters.AddWithValue("@NoiTaoHopDong", (object?)contract.NoiTaoHopDong ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@NgayTaoHopDong", contract.NgayTaoHopDong.HasValue ? (object)contract.NgayTaoHopDong.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@GiaBangChu", (object?)contract.GiaBangChu ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@NgayTraTien", (object?)contract.NgayTraTien ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ThoiHanNam", contract.ThoiHanNam.HasValue ? (object)contract.ThoiHanNam.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@NgayGiaoNha", contract.NgayGiaoNha.HasValue ? (object)contract.NgayGiaoNha.Value : DBNull.Value);

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task DeleteHopDongAsync(int maHopDong)
        {
            var sql = "DELETE FROM HopDong WHERE MaHopDong = @MaHopDong;";

            using var conn = new MySqlConnection(_connectionString);
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@MaHopDong", maHopDong);

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Đọc một record từ DbDataReader và ánh xạ vào model Contract (bao gồm thông tin phòng và người thuê nếu có)
        /// </summary>
        private static Contract ReadContract(DbDataReader reader)
        {
            Contract c = new Contract();

            // Basic HopDong fields
            c.MaHopDong = reader.GetInt32(reader.GetOrdinal("MaHopDong"));
            c.MaNguoiThue = reader.GetInt32(reader.GetOrdinal("MaNguoiThue"));
            c.MaPhong = reader.GetInt32(reader.GetOrdinal("MaPhong"));

            c.NgayBatDau = reader.IsDBNull(reader.GetOrdinal("NgayBatDau")) ? default : reader.GetDateTime(reader.GetOrdinal("NgayBatDau"));
            c.NgayKetThuc = reader.IsDBNull(reader.GetOrdinal("NgayKetThuc")) ? default : reader.GetDateTime(reader.GetOrdinal("NgayKetThuc"));

            c.TienCoc = reader.IsDBNull(reader.GetOrdinal("TienCoc")) ? 0 : reader.GetDecimal(reader.GetOrdinal("TienCoc"));
            c.GiaThue = reader.FieldCount > 0 && ColumnExists(reader, "GiaThue") && !reader.IsDBNull(reader.GetOrdinal("GiaThue"))
                        ? reader.GetDecimal(reader.GetOrdinal("GiaThue"))
                        // fallback: nếu HopDong không có GiaThue, có thể dùng giá phòng (Ph_GiaCoBan)
                        : (ColumnExists(reader, "Ph_GiaCoBan") && !reader.IsDBNull(reader.GetOrdinal("Ph_GiaCoBan")) ? reader.GetDecimal(reader.GetOrdinal("Ph_GiaCoBan")) : 0);

          
            c.TrangThai = ColumnExists(reader, "TrangThai") && !reader.IsDBNull(reader.GetOrdinal("TrangThai")) ? reader.GetString(reader.GetOrdinal("TrangThai")) : null;
            c.FileHopDong = ColumnExists(reader, "FileHopDong") && !reader.IsDBNull(reader.GetOrdinal("FileHopDong")) ? reader.GetString(reader.GetOrdinal("FileHopDong")) : null;

            // Optional create info
            c.NoiTaoHopDong = ColumnExists(reader, "NoiTaoHopDong") && !reader.IsDBNull(reader.GetOrdinal("NoiTaoHopDong")) ? reader.GetString(reader.GetOrdinal("NoiTaoHopDong")) : null;
            c.NgayTaoHopDong = ColumnExists(reader, "NgayTaoHopDong") && !reader.IsDBNull(reader.GetOrdinal("NgayTaoHopDong")) ? reader.GetDateTime(reader.GetOrdinal("NgayTaoHopDong")) : (DateTime?)null;
            c.GiaBangChu = ColumnExists(reader, "GiaBangChu") && !reader.IsDBNull(reader.GetOrdinal("GiaBangChu")) ? reader.GetString(reader.GetOrdinal("GiaBangChu")) : null;
            c.NgayTraTien = ColumnExists(reader, "NgayTraTien") && !reader.IsDBNull(reader.GetOrdinal("NgayTraTien")) ? reader.GetString(reader.GetOrdinal("NgayTraTien")) : null;
            c.ThoiHanNam = ColumnExists(reader, "ThoiHanNam") && !reader.IsDBNull(reader.GetOrdinal("ThoiHanNam")) ? reader.GetInt32(reader.GetOrdinal("ThoiHanNam")) : (int?)null;
            c.NgayGiaoNha = ColumnExists(reader, "NgayGiaoNha") && !reader.IsDBNull(reader.GetOrdinal("NgayGiaoNha")) ? reader.GetDateTime(reader.GetOrdinal("NgayGiaoNha")) : (DateTime?)null;

            // -------------------------
            // Phòng (BÊN A) - from join aliases Ph_*
            // -------------------------
            c.TenPhong = ColumnExists(reader, "Ph_TenPhong") && !reader.IsDBNull(reader.GetOrdinal("Ph_TenPhong")) ? reader.GetString(reader.GetOrdinal("Ph_TenPhong")) : null;
            c.DienTich = ColumnExists(reader, "Ph_DienTich") && !reader.IsDBNull(reader.GetOrdinal("Ph_DienTich")) ? reader.GetDouble(reader.GetOrdinal("Ph_DienTich")) : (double?)null;
            c.TrangThietBi = ColumnExists(reader, "Ph_TrangThietBi") && !reader.IsDBNull(reader.GetOrdinal("Ph_TrangThietBi")) ? reader.GetString(reader.GetOrdinal("Ph_TrangThietBi")) : null;
            c.GiaBangChu = c.GiaBangChu ?? (ColumnExists(reader, "Ph_GiaBangChu") && !reader.IsDBNull(reader.GetOrdinal("Ph_GiaBangChu")) ? reader.GetString(reader.GetOrdinal("Ph_GiaBangChu")) : c.GiaBangChu);

            // -------------------------
            // Chủ phòng (BÊN A) - nếu lưu thông tin chủ trong Phong hoặc Nha, không có trong SQL hiện tại.
            // (Nếu cần, bổ sung JOIN với bảng Chủ/Owner)
            // -------------------------

            // -------------------------
            // Người thuê (BÊN B)
            // -------------------------
            c.TenNguoiThue = ColumnExists(reader, "T_HoTen") && !reader.IsDBNull(reader.GetOrdinal("T_HoTen")) ? reader.GetString(reader.GetOrdinal("T_HoTen")) : null;
            c.NgaySinhNguoiThue = ColumnExists(reader, "T_NgaySinh") && !reader.IsDBNull(reader.GetOrdinal("T_NgaySinh")) ? reader.GetDateTime(reader.GetOrdinal("T_NgaySinh")) : (DateTime?)null;
            c.CCCDNguoiThue = ColumnExists(reader, "T_CCCD") && !reader.IsDBNull(reader.GetOrdinal("T_CCCD")) ? reader.GetString(reader.GetOrdinal("T_CCCD")) : null;
            c.NgayCapNguoiThue = ColumnExists(reader, "T_NgayCapCCCD") && !reader.IsDBNull(reader.GetOrdinal("T_NgayCapCCCD")) ? reader.GetDateTime(reader.GetOrdinal("T_NgayCapCCCD")) : (DateTime?)null;
            c.NoiCapNguoiThue = ColumnExists(reader, "T_NoiCapCCCD") && !reader.IsDBNull(reader.GetOrdinal("T_NoiCapCCCD")) ? reader.GetString(reader.GetOrdinal("T_NoiCapCCCD")) : null;
            c.DiaChiNguoiThue = ColumnExists(reader, "T_DiaChiThuongTru") && !reader.IsDBNull(reader.GetOrdinal("T_DiaChiThuongTru")) ? reader.GetString(reader.GetOrdinal("T_DiaChiThuongTru")) : null;
            c.DienThoaiNguoiThue = ColumnExists(reader, "T_SoDienThoai") && !reader.IsDBNull(reader.GetOrdinal("T_SoDienThoai")) ? reader.GetString(reader.GetOrdinal("T_SoDienThoai")) : null;

            return c;
        }

        /// <summary>
        /// Helper kiểm tra column có tồn tại trong reader (tránh GetOrdinal lỗi khi không có)
        /// </summary>
        private static bool ColumnExists(DbDataReader reader, string columnName)
        {
            try
            {
                return reader.GetOrdinal(columnName) >= 0;
            }
            catch
            {
                return false;
            }
        }
    }
}
