using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using QLKDPhongTro.DataLayer.Models;

namespace QLKDPhongTro.DataLayer.Repositories
{
    public class RentedRoomRepository : IRentedRoomRepository
    {
        private readonly string connectionString;

        public RentedRoomRepository()
        {
            connectionString = "Data Source=.;Initial Catalog=QLKDPhongTro;Integrated Security=True;TrustServerCertificate=True;Encrypt=False";
        }

        public async Task<List<RentedRoom>> GetAllAsync()
        {
            var rooms = new List<RentedRoom>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("SELECT MaPhong, TenPhong, DienTich, GiaCoBan, TrangThai, GhiChu FROM RentedRooms", conn);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        rooms.Add(new RentedRoom
                        {
                            MaPhong = reader.GetInt32(0),
                            TenPhong = reader.GetString(1),
                            DienTich = reader.GetDouble(2),
                            GiaCoBan = reader.GetDouble(3),
                            TrangThai = reader.GetString(4),
                            GhiChu = reader.GetString(5)
                        });
                    }
                }
            }
            return rooms;
        }

        public async Task<RentedRoom?> GetByIdAsync(int maPhong)
        {
            RentedRoom? room = null;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("SELECT MaPhong, TenPhong, DienTich, GiaCoBan, TrangThai, GhiChu FROM RentedRooms WHERE MaPhong=@MaPhong", conn);
                cmd.Parameters.AddWithValue("@MaPhong", maPhong);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        room = new RentedRoom
                        {
                            MaPhong = reader.GetInt32(0),
                            TenPhong = reader.GetString(1),
                            DienTich = reader.GetDouble(2),
                            GiaCoBan = reader.GetDouble(3),
                            TrangThai = reader.GetString(4),
                            GhiChu = reader.GetString(5)
                        };
                    }
                }
            }
            return room;
        }

        public async Task<bool> IsRoomExistsAsync(int maPhong)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("SELECT COUNT(*) FROM RentedRooms WHERE MaPhong=@MaPhong", conn);
                cmd.Parameters.AddWithValue("@MaPhong", maPhong);
                var count = (int)await cmd.ExecuteScalarAsync();
                return count > 0;
            }
        }

        public async Task<bool> CreateAsync(RentedRoom room)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("INSERT INTO RentedRooms(MaPhong, TenPhong, DienTich, GiaCoBan, TrangThai, GhiChu) VALUES(@MaPhong, @TenPhong, @DienTich, @GiaCoBan, @TrangThai, @GhiChu)", conn);
                cmd.Parameters.AddWithValue("@MaPhong", room.MaPhong);
                cmd.Parameters.AddWithValue("@TenPhong", room.TenPhong);
                cmd.Parameters.AddWithValue("@DienTich", room.DienTich);
                cmd.Parameters.AddWithValue("@GiaCoBan", room.GiaCoBan);
                cmd.Parameters.AddWithValue("@TrangThai", room.TrangThai);
                cmd.Parameters.AddWithValue("@GhiChu", room.GhiChu);
                return await cmd.ExecuteNonQueryAsync() > 0;
            }
        }

        public async Task<bool> UpdateAsync(RentedRoom room)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("UPDATE RentedRooms SET TenPhong=@TenPhong, DienTich=@DienTich, GiaCoBan=@GiaCoBan, TrangThai=@TrangThai, GhiChu=@GhiChu WHERE MaPhong=@MaPhong", conn);
                cmd.Parameters.AddWithValue("@TenPhong", room.TenPhong);
                cmd.Parameters.AddWithValue("@DienTich", room.DienTich);
                cmd.Parameters.AddWithValue("@GiaCoBan", room.GiaCoBan);
                cmd.Parameters.AddWithValue("@TrangThai", room.TrangThai);
                cmd.Parameters.AddWithValue("@GhiChu", room.GhiChu);
                cmd.Parameters.AddWithValue("@MaPhong", room.MaPhong);
                return await cmd.ExecuteNonQueryAsync() > 0;
            }
        }

        public async Task<bool> DeleteAsync(int maPhong)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("DELETE FROM RentedRooms WHERE MaPhong=@MaPhong", conn);
                cmd.Parameters.AddWithValue("@MaPhong", maPhong);
                return await cmd.ExecuteNonQueryAsync() > 0;
            }
        }

        public async Task<bool> UpdateStatusAsync(int maPhong, string trangThai)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("UPDATE RentedRooms SET TrangThai=@TrangThai WHERE MaPhong=@MaPhong", conn);
                cmd.Parameters.AddWithValue("@TrangThai", trangThai);
                cmd.Parameters.AddWithValue("@MaPhong", maPhong);
                return await cmd.ExecuteNonQueryAsync() > 0;
            }
        }
    }
}
