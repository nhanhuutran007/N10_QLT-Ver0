using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using QLKDPhongTro.DataLayer.Models;
using MySql.Data.MySqlClient; // Đã chuyển sang MySQL

namespace QLKDPhongTro.DataLayer.Repositories
{
    public class RentedRoomRepository : IRentedRoomRepository
    {
        private readonly string connectionString;

        public RentedRoomRepository()
        {
            // ===== CÁCH KẾT NỐI CŨ: SQL SERVER =====
            // connectionString = "Data Source=.;Initial Catalog=QLThueNhaV1;Integrated Security=True;TrustServerCertificate=True;Encrypt=False";
            
            // ===== CÁCH KẾT NỐI MỚI: MYSQL =====
            string server = "host80.vietnix.vn";
            string database = "githubio_QLT_Ver1";
            string username = "githubio_admin";
            string password = "nhanhuutran007";
            string port = "3306";
            
            connectionString = $"Server={server};Port={port};Database={database};Uid={username};Pwd={password};SslMode=Preferred;Charset=utf8mb4;";
        }

        public async Task<List<RentedRoom>> GetAllAsync()
        {
            var rooms = new List<RentedRoom>();
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                await conn.OpenAsync();
                var cmd = new MySqlCommand("SELECT MaPhong, TenPhong, DienTich, GiaCoBan, TrangThai, GhiChu, GiaBangChu, TrangThietBi FROM Phong", conn);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        rooms.Add(new RentedRoom
                        {
                            MaPhong = reader.GetInt32(0),
                            TenPhong = reader.GetString(1),
                            DienTich = reader.GetDecimal(2),
                            GiaCoBan = reader.GetDecimal(3),
                            TrangThai = reader.GetString(4),
                            GhiChu = reader.GetString(5),
                            GiaBangChu = reader.GetString(6),
                            TrangThietBi = reader.GetString(7)
                        });
                    }
                }
            }
            return rooms;
        }

        public async Task<RentedRoom?> GetByIdAsync(int maPhong)
        {
            RentedRoom? room = null;
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                await conn.OpenAsync();
                var cmd = new MySqlCommand("SELECT MaPhong, TenPhong, DienTich, GiaCoBan, TrangThai, GhiChu, GiaBangChu, TrangThietBi FROM Phong WHERE MaPhong=@MaPhong", conn);
                cmd.Parameters.AddWithValue("@MaPhong", maPhong);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        room = new RentedRoom
                        {
                            MaPhong = reader.GetInt32(0),
                            TenPhong = reader.GetString(1),
                            DienTich = reader.GetDecimal(2),
                            GiaCoBan = reader.GetDecimal(3),
                            TrangThai = reader.GetString(4),
                            GhiChu = reader.GetString(5),
                            GiaBangChu = reader.GetString(6),
                            TrangThietBi = reader.GetString(7)
                        };
                    }
                }
            }
            return room;
        }

        public async Task<bool> IsRoomExistsAsync(int maPhong)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                await conn.OpenAsync();
                var cmd = new MySqlCommand("SELECT COUNT(*) FROM Phong WHERE MaPhong=@MaPhong", conn);
                cmd.Parameters.AddWithValue("@MaPhong", maPhong);
                var count = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                return count > 0;
            }
        }

        public async Task<bool> CreateAsync(RentedRoom room)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                await conn.OpenAsync();
                var cmd = new MySqlCommand("INSERT INTO Phong(TenPhong, DienTich, GiaCoBan, TrangThai, GhiChu, GiaBangChu, TrangThietBi) VALUES(@TenPhong, @DienTich, @GiaCoBan, @TrangThai, @GhiChu,  @GiaBangChu, @TrangThietBi)", conn);
                cmd.Parameters.AddWithValue("@TenPhong", room.TenPhong);
                cmd.Parameters.AddWithValue("@DienTich", room.DienTich);
                cmd.Parameters.AddWithValue("@GiaCoBan", room.GiaCoBan);
                cmd.Parameters.AddWithValue("@TrangThai", room.TrangThai);
                cmd.Parameters.AddWithValue("@GhiChu", room.GhiChu);
                cmd.Parameters.AddWithValue("@GiaBangChu", room.GiaBangChu);
                cmd.Parameters.AddWithValue("@TrangThietBi", room.TrangThietBi);
                return await cmd.ExecuteNonQueryAsync() > 0;
            }
        }

        public async Task<bool> UpdateAsync(RentedRoom room)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                await conn.OpenAsync();
                var cmd = new MySqlCommand("UPDATE Phong SET TenPhong=@TenPhong, DienTich=@DienTich, GiaCoBan=@GiaCoBan, TrangThai=@TrangThai, GhiChu=@GhiChu, GiaBangChu=@GiaBangChu, TrangThietBi=@TrangThietBi WHERE MaPhong=@MaPhong", conn);
                cmd.Parameters.AddWithValue("@TenPhong", room.TenPhong);
                cmd.Parameters.AddWithValue("@DienTich", room.DienTich);
                cmd.Parameters.AddWithValue("@GiaCoBan", room.GiaCoBan);
                cmd.Parameters.AddWithValue("@TrangThai", room.TrangThai);
                cmd.Parameters.AddWithValue("@GhiChu", room.GhiChu);
                cmd.Parameters.AddWithValue("@MaPhong", room.MaPhong);
                cmd.Parameters.AddWithValue("@GiaBangChu", room.GiaBangChu);
                cmd.Parameters.AddWithValue("@TrangThietBi", room.TrangThietBi);
                return await cmd.ExecuteNonQueryAsync() > 0;
            }
        }

        public async Task<bool> DeleteAsync(int maPhong)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                await conn.OpenAsync();
                var cmd = new MySqlCommand("DELETE FROM Phong WHERE MaPhong=@MaPhong", conn);
                cmd.Parameters.AddWithValue("@MaPhong", maPhong);
                return await cmd.ExecuteNonQueryAsync() > 0;
            }
        }

        public async Task<bool> UpdateStatusAsync(int maPhong, string trangThai)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                await conn.OpenAsync();
                var cmd = new MySqlCommand("UPDATE Phong SET TrangThai=@TrangThai WHERE MaPhong=@MaPhong", conn);
                cmd.Parameters.AddWithValue("@TrangThai", trangThai);
                cmd.Parameters.AddWithValue("@MaPhong", maPhong);
                return await cmd.ExecuteNonQueryAsync() > 0;
            }
        }
    }
}
