using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using QLKDPhongTro.DataLayer.Models;

namespace QLKDPhongTro.DataLayer.Repositories
{
    public class HouseRepository : IHouseRepository
    {
        private readonly string connectionString;

        public HouseRepository()
        {
            connectionString = "Data Source=.;Initial Catalog=QLKDPhongTro;Integrated Security=True;TrustServerCertificate=True;Encrypt=False";
        }

        public async Task<List<House>> GetAllAsync()
        {
            var houses = new List<House>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand("SELECT MaNha, DiaChi, TongSoPhong, GhiChu FROM Houses", connection);
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
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand("SELECT MaNha, DiaChi, TongSoPhong, GhiChu FROM Houses WHERE MaNha=@MaNha", connection);
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
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand("INSERT INTO Houses(DiaChi, TongSoPhong, GhiChu) VALUES(@DiaChi, @TongSoPhong, @GhiChu)", connection);
                cmd.Parameters.AddWithValue("@DiaChi", house.DiaChi);
                cmd.Parameters.AddWithValue("@TongSoPhong", house.TongSoPhong);
                cmd.Parameters.AddWithValue("@GhiChu", house.GhiChu);
                return await cmd.ExecuteNonQueryAsync() > 0;
            }
        }

        public async Task<bool> UpdateAsync(House house)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand("UPDATE Houses SET DiaChi=@DiaChi, TongSoPhong=@TongSoPhong, GhiChu=@GhiChu WHERE MaNha=@MaNha", connection);
                cmd.Parameters.AddWithValue("@DiaChi", house.DiaChi);
                cmd.Parameters.AddWithValue("@TongSoPhong", house.TongSoPhong);
                cmd.Parameters.AddWithValue("@GhiChu", house.GhiChu);
                cmd.Parameters.AddWithValue("@MaNha", house.MaNha);
                return await cmd.ExecuteNonQueryAsync() > 0;
            }
        }

        public async Task<bool> DeleteAsync(int maNha)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand("DELETE FROM Houses WHERE MaNha=@MaNha", connection);
                cmd.Parameters.AddWithValue("@MaNha", maNha);
                return await cmd.ExecuteNonQueryAsync() > 0;
            }
        }
    }
}
