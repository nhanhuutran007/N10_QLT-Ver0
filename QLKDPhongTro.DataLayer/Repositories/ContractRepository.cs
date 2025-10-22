using QLKDPhongTro.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace QLKDPhongTro.DataLayer.Repositories
{
    public class ContractRepository : IContractRepository
    {
        // THAY THẾ CHUỖI KẾT NỐI NÀY BẰNG CHUỖI CỦA BẠN
        private readonly string _connectionString = "Data Source=.;Initial Catalog=QLThueNhaV0;Integrated Security=True;TrustServerCertificate=True;Encrypt=False";

        // Sử dụng JOIN để lấy thêm Tên Người Thuê và Tên Phòng
        public List<HopDong> GetAllHopDong()
        {
            var hopDongs = new List<HopDong>();
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(
                    @"SELECT hd.*, nt.HoTen AS TenNguoiThue, p.TenPhong 
                      FROM HopDong hd
                      LEFT JOIN NguoiThue nt ON hd.MaNguoiThue = nt.MaNguoiThue
                      LEFT JOIN Phong p ON hd.MaPhong = p.MaPhong", connection);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var hopDong = new HopDong
                        {
                            MaHopDong = (int)reader["MaHopDong"],
                            MaNguoiThue = (int)reader["MaNguoiThue"],
                            MaPhong = (int)reader["MaPhong"],
                            NgayBatDau = (DateTime)reader["NgayBatDau"],
                            NgayKetThuc = (DateTime)reader["NgayKetThuc"],
                            TienCoc = (decimal)reader["TienCoc"],
                            FileHopDong = reader["FileHopDong"] as string,
                            TrangThai = (string)reader["TrangThai"],
                            // Thêm các thuộc tính JOIN
                            TenNguoiThue = reader["TenNguoiThue"] as string,
                            TenPhong = reader["TenPhong"] as string
                        };
                        hopDongs.Add(hopDong);
                    }
                }
            }
            return hopDongs;
        }

        public void AddHopDong(HopDong hopDong)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("INSERT INTO HopDong (MaNguoiThue, MaPhong, NgayBatDau, NgayKetThuc, TienCoc, FileHopDong, TrangThai) VALUES (@MaNguoiThue, @MaPhong, @NgayBatDau, @NgayKetThuc, @TienCoc, @FileHopDong, @TrangThai)", connection);
                command.Parameters.AddWithValue("@MaNguoiThue", hopDong.MaNguoiThue);
                command.Parameters.AddWithValue("@MaPhong", hopDong.MaPhong);
                command.Parameters.AddWithValue("@NgayBatDau", hopDong.NgayBatDau);
                command.Parameters.AddWithValue("@NgayKetThuc", hopDong.NgayKetThuc);
                command.Parameters.AddWithValue("@TienCoc", hopDong.TienCoc);
                command.Parameters.AddWithValue("@FileHopDong", (object)hopDong.FileHopDong ?? DBNull.Value);
                command.Parameters.AddWithValue("@TrangThai", hopDong.TrangThai);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public void UpdateHopDong(HopDong hopDong)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("UPDATE HopDong SET MaNguoiThue = @MaNguoiThue, MaPhong = @MaPhong, NgayBatDau = @NgayBatDau, NgayKetThuc = @NgayKetThuc, TienCoc = @TienCoc, FileHopDong = @FileHopDong, TrangThai = @TrangThai WHERE MaHopDong = @MaHopDong", connection);
                command.Parameters.AddWithValue("@MaHopDong", hopDong.MaHopDong);
                command.Parameters.AddWithValue("@MaNguoiThue", hopDong.MaNguoiThue);
                command.Parameters.AddWithValue("@MaPhong", hopDong.MaPhong);
                command.Parameters.AddWithValue("@NgayBatDau", hopDong.NgayBatDau);
                command.Parameters.AddWithValue("@NgayKetThuc", hopDong.NgayKetThuc);
                command.Parameters.AddWithValue("@TienCoc", hopDong.TienCoc);
                command.Parameters.AddWithValue("@FileHopDong", (object)hopDong.FileHopDong ?? DBNull.Value);
                command.Parameters.AddWithValue("@TrangThai", hopDong.TrangThai);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public void DeleteHopDong(int maHopDong)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("DELETE FROM HopDong WHERE MaHopDong = @MaHopDong", connection);
                command.Parameters.AddWithValue("@MaHopDong", maHopDong);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public List<HopDong> GetExpiringContracts(int days)
        {
            var hopDongs = new List<HopDong>();
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("SELECT * FROM HopDong WHERE TrangThai = N'Hiệu lực' AND DATEDIFF(day, GETDATE(), NgayKetThuc) BETWEEN 0 AND @Days", connection);
                command.Parameters.AddWithValue("@Days", days);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var hopDong = new HopDong { /* ... map data ... */ };
                        hopDongs.Add(hopDong);
                    }
                }
            }
            return hopDongs;
        }
    }

    // Cập nhật model để chứa thêm thông tin JOIN
    public class HopDong
    {
        public int MaHopDong { get; set; }
        public int MaNguoiThue { get; set; }
        public int MaPhong { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }
        public decimal TienCoc { get; set; }
        public string FileHopDong { get; set; }
        public string TrangThai { get; set; }

        // Thuộc tính chỉ dùng để hiển thị, không có trong bảng HopDong
        public string TenNguoiThue { get; set; }
        public string TenPhong { get; set; }
    }
}