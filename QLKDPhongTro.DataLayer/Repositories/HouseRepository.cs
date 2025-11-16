using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using QLKDPhongTro.DataLayer.Models;
using MySql.Data.MySqlClient; // Đã chuyển sang MySQL

namespace QLKDPhongTro.DataLayer.Repositories
{
    public class HouseRepository : IHouseRepository
    {
        // Sử dụng ConnectDB chung để quản lý connection string
        private string connectionString => ConnectDB.GetConnectionString();

        public async Task<List<House>> GetAllAsync()
        {
            var houses = new List<House>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var cmd = new MySqlCommand("SELECT MaNha, DiaChi, TongSoPhong, GhiChu FROM Nha", connection);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    var maNhaOrdinal = reader.GetOrdinal("MaNha");
                    var diaChiOrdinal = reader.GetOrdinal("DiaChi");
                    var tongSoPhongOrdinal = reader.GetOrdinal("TongSoPhong");
                    var ghiChuOrdinal = reader.GetOrdinal("GhiChu");

                    while (await reader.ReadAsync())
                    {
                        var house = new House
                        {
                            MaNha = reader.IsDBNull(maNhaOrdinal) ? 0 : reader.GetInt32(maNhaOrdinal),
                            DiaChi = reader.IsDBNull(diaChiOrdinal) ? string.Empty : reader.GetString(diaChiOrdinal),
                            TongSoPhong = reader.IsDBNull(tongSoPhongOrdinal) ? 0 : reader.GetInt32(tongSoPhongOrdinal),
                            GhiChu = reader.IsDBNull(ghiChuOrdinal) ? string.Empty : reader.GetString(ghiChuOrdinal)
                        };

                        houses.Add(house);
                    }
                }
            }
            return houses;
        }

        public async Task<House?> GetByIdAsync(int maNha)
        {
            House? house = null;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var cmd = new MySqlCommand("SELECT MaNha, DiaChi, TongSoPhong, GhiChu FROM Nha WHERE MaNha=@MaNha", connection);
                cmd.Parameters.AddWithValue("@MaNha", maNha);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    var maNhaOrdinal = reader.GetOrdinal("MaNha");
                    var diaChiOrdinal = reader.GetOrdinal("DiaChi");
                    var tongSoPhongOrdinal = reader.GetOrdinal("TongSoPhong");
                    var ghiChuOrdinal = reader.GetOrdinal("GhiChu");

                    if (await reader.ReadAsync())
                    {
                        house = new House
                        {
                            MaNha = reader.IsDBNull(maNhaOrdinal) ? 0 : reader.GetInt32(maNhaOrdinal),
                            DiaChi = reader.IsDBNull(diaChiOrdinal) ? string.Empty : reader.GetString(diaChiOrdinal),
                            TongSoPhong = reader.IsDBNull(tongSoPhongOrdinal) ? 0 : reader.GetInt32(tongSoPhongOrdinal),
                            GhiChu = reader.IsDBNull(ghiChuOrdinal) ? string.Empty : reader.GetString(ghiChuOrdinal)
                        };
                    }
                }
            }
            return house;
        }

        public async Task<bool> CreateAsync(House house)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var cmd = new MySqlCommand("INSERT INTO Nha(DiaChi, TongSoPhong, GhiChu) VALUES(@DiaChi, @TongSoPhong, @GhiChu)", connection);
                cmd.Parameters.AddWithValue("@DiaChi", house.DiaChi);
                cmd.Parameters.AddWithValue("@TongSoPhong", house.TongSoPhong);
                cmd.Parameters.AddWithValue("@GhiChu", house.GhiChu);
                return await cmd.ExecuteNonQueryAsync() > 0;
            }
        }

        public async Task<bool> UpdateAsync(House house)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var cmd = new MySqlCommand("UPDATE Nha SET DiaChi=@DiaChi, TongSoPhong=@TongSoPhong, GhiChu=@GhiChu WHERE MaNha=@MaNha", connection);
                cmd.Parameters.AddWithValue("@DiaChi", house.DiaChi);
                cmd.Parameters.AddWithValue("@TongSoPhong", house.TongSoPhong);
                cmd.Parameters.AddWithValue("@GhiChu", house.GhiChu);
                cmd.Parameters.AddWithValue("@MaNha", house.MaNha);
                return await cmd.ExecuteNonQueryAsync() > 0;
            }
        }

        public async Task<bool> DeleteAsync(int maNha)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var cmd = new MySqlCommand("DELETE FROM Nha WHERE MaNha=@MaNha", connection);
                cmd.Parameters.AddWithValue("@MaNha", maNha);
                return await cmd.ExecuteNonQueryAsync() > 0;
            }
        }
    }
}
