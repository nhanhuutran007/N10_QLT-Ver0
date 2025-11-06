using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using QLKDPhongTro.DataLayer.Models;
using MySql.Data.MySqlClient; // Đã chuyển sang MySQL

namespace QLKDPhongTro.DataLayer.Repositories
{
    public class HouseRepository : IHouseRepository
    {
        private readonly string connectionString;

        public HouseRepository()
        {
            // ===== CÁCH KẾT NỐI CŨ: SQL SERVER =====
            // connectionString = "Data Source=.;Initial Catalog=QLThueNhaV1;Integrated Security=True;TrustServerCertificate=True;Encrypt=False";
            
            // ===== CÁCH KẾT NỐI MỚI: MYSQL =====
            string server = "host80.vietnix.vn";
            string database = "githubio_QLT_Ver1";
            string username = "githubio_admin";
            string password = "nhanhuutran007";
            string port = "3306";
            
            connectionString = $"Server={server};Port={port};Database={database};Uid={username};Pwd={password};SslMode=Preferred;";
        }

        public async Task<List<House>> GetAllAsync()
        {
            var houses = new List<House>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var cmd = new MySqlCommand("SELECT MaNha, DiaChi, TongSoPhong, GhiChu FROM Nha", connection);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        houses.Add(new House
                        {
                            MaNha = reader.GetInt32(0),
                            DiaChi = reader.GetString(1),
                            TongSoPhong = reader.GetInt32(2),
                            GhiChu = reader.GetString(3)
                        });
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
                    if (await reader.ReadAsync())
                    {
                        house = new House
                        {
                            MaNha = reader.GetInt32(0),
                            DiaChi = reader.GetString(1),
                            TongSoPhong = reader.GetInt32(2),
                            GhiChu = reader.GetString(3)
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
