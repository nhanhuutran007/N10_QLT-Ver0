using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using QLKDPhongTro.DataLayer.Models;
using MySql.Data.MySqlClient; // Đã chuyển sang MySQL

namespace QLKDPhongTro.DataLayer.Repositories
{
    public class RentedRoomRepository : IRentedRoomRepository
    {
        // Sử dụng ConnectDB chung để quản lý connection string
        private string connectionString => ConnectDB.GetConnectionString();

        public async Task<List<RentedRoom>> GetAllAsync()
        {
            var rooms = new List<RentedRoom>();
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                await conn.OpenAsync();
                var cmd = new MySqlCommand("SELECT MaPhong, MaNha, TenPhong, DienTich, GiaCoBan, TrangThai, GhiChu, GiaBangChu, TrangThietBi FROM Phong", conn);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        rooms.Add(new RentedRoom
                        {
                            MaPhong = reader.GetInt32(0),
                            MaNha = reader.IsDBNull(1) ? 0 : reader.GetInt32(1),
                            TenPhong = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                            DienTich = reader.IsDBNull(3) ? 0 : reader.GetDecimal(3),
                            GiaCoBan = reader.IsDBNull(4) ? 0 : reader.GetDecimal(4),
                            TrangThai = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                            GhiChu = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                            GiaBangChu = reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
                            TrangThietBi = reader.IsDBNull(8) ? string.Empty : reader.GetString(8)
                        });
                    }
                }
            }
            return rooms;
        }

        public async Task<List<RentedRoom>> GetAllByMaNhaAsync(int maNha)
        {
            var rooms = new List<RentedRoom>();
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                await conn.OpenAsync();
                var cmd = new MySqlCommand("SELECT MaPhong, MaNha, TenPhong, DienTich, GiaCoBan, TrangThai, GhiChu, GiaBangChu, TrangThietBi FROM Phong WHERE MaNha = @MaNha", conn);
                cmd.Parameters.AddWithValue("@MaNha", maNha);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        rooms.Add(new RentedRoom
                        {
                            MaPhong = reader.GetInt32(0),
                            MaNha = reader.IsDBNull(1) ? 0 : reader.GetInt32(1),
                            TenPhong = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                            DienTich = reader.IsDBNull(3) ? 0 : reader.GetDecimal(3),
                            GiaCoBan = reader.IsDBNull(4) ? 0 : reader.GetDecimal(4),
                            TrangThai = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                            GhiChu = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                            GiaBangChu = reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
                            TrangThietBi = reader.IsDBNull(8) ? string.Empty : reader.GetString(8)
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
                var cmd = new MySqlCommand("SELECT MaPhong, MaNha, TenPhong, DienTich, GiaCoBan, TrangThai, GhiChu, GiaBangChu, TrangThietBi FROM Phong WHERE MaPhong=@MaPhong", conn);
                cmd.Parameters.AddWithValue("@MaPhong", maPhong);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        room = new RentedRoom
                        {
                            MaPhong = reader.GetInt32(0),
                            MaNha = reader.IsDBNull(1) ? 0 : reader.GetInt32(1),
                            TenPhong = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                            DienTich = reader.IsDBNull(3) ? 0 : reader.GetDecimal(3),
                            GiaCoBan = reader.IsDBNull(4) ? 0 : reader.GetDecimal(4),
                            TrangThai = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                            GhiChu = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                            GiaBangChu = reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
                            TrangThietBi = reader.IsDBNull(8) ? string.Empty : reader.GetString(8)
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
                var cmd = new MySqlCommand("INSERT INTO Phong(MaNha, TenPhong, DienTich, GiaCoBan, TrangThai, GhiChu, GiaBangChu, TrangThietBi) VALUES(@MaNha, @TenPhong, @DienTich, @GiaCoBan, @TrangThai, @GhiChu, @GiaBangChu, @TrangThietBi)", conn);
                cmd.Parameters.AddWithValue("@MaNha", room.MaNha);
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
