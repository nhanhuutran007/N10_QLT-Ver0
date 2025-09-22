using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using QLKDPhongTro.Models;
using QLKDPhongTro.Utils;

namespace QLKDPhongTro.Services
{
    /// <summary>
    /// Service xử lý logic nghiệp vụ cho User
    /// </summary>
    public class UserService : BaseService<User>
    {
        public override async Task<List<User>> GetAllAsync()
        {
            var users = new List<User>();
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    var query = "SELECT Acc, TenDangNhap, Email FROM tblDangNhap";
                    using (var command = new SqlCommand(query, connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                users.Add(new User
                                {
                                    Acc = reader["Acc"].ToString(),
                                    TenDangNhap = reader["TenDangNhap"].ToString(),
                                    Email = reader["Email"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting users: {ex.Message}");
            }
            return users;
        }

        public override async Task<User> GetByIdAsync(string id)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    var query = "SELECT Acc, TenDangNhap, Email FROM tblDangNhap WHERE Acc = @Acc";
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Acc", id);
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return new User
                                {
                                    Acc = reader["Acc"].ToString(),
                                    TenDangNhap = reader["TenDangNhap"].ToString(),
                                    Email = reader["Email"].ToString()
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting user by id: {ex.Message}");
            }
            return null;
        }

        public override async Task<bool> CreateAsync(User entity)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    var query = @"INSERT INTO tblDangNhap (Acc, TenDangNhap, MatKhau, Email) 
                                  VALUES (@Acc, @TenDangNhap, @MatKhau, @Email)";
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Acc", entity.Acc);
                        command.Parameters.AddWithValue("@TenDangNhap", entity.TenDangNhap);
                        command.Parameters.AddWithValue("@MatKhau", entity.MatKhau);
                        command.Parameters.AddWithValue("@Email", entity.Email);
                        
                        var result = await command.ExecuteNonQueryAsync();
                        return result > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating user: {ex.Message}");
                return false;
            }
        }

        public override async Task<bool> UpdateAsync(User entity)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    var query = @"UPDATE tblDangNhap 
                                  SET TenDangNhap = @TenDangNhap, 
                                      MatKhau = @MatKhau, 
                                      Email = @Email 
                                  WHERE Acc = @Acc";
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Acc", entity.Acc);
                        command.Parameters.AddWithValue("@TenDangNhap", entity.TenDangNhap);
                        command.Parameters.AddWithValue("@MatKhau", entity.MatKhau);
                        command.Parameters.AddWithValue("@Email", entity.Email);
                        
                        var result = await command.ExecuteNonQueryAsync();
                        return result > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating user: {ex.Message}");
                return false;
            }
        }

        public override async Task<bool> DeleteAsync(string id)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    var query = "DELETE FROM tblDangNhap WHERE Acc = @Acc";
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Acc", id);
                        var result = await command.ExecuteNonQueryAsync();
                        return result > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting user: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Đăng nhập user
        /// </summary>
        public async Task<User> LoginAsync(string tenDangNhap, string matKhau)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    var query = "SELECT Acc, TenDangNhap, MatKhau, Email FROM tblDangNhap WHERE TenDangNhap = @TenDangNhap";
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@TenDangNhap", tenDangNhap);
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                var storedPassword = reader["MatKhau"].ToString();
                                
                                // Kiểm tra mật khẩu đã hash (ưu tiên)
                                if (PasswordHelper.VerifyPassword(matKhau, storedPassword))
                                {
                                    return new User
                                    {
                                        Acc = reader["Acc"].ToString(),
                                        TenDangNhap = reader["TenDangNhap"].ToString(),
                                        Email = reader["Email"].ToString()
                                    };
                                }
                                
                                // Fallback: Kiểm tra với mật khẩu plain text (cho tài khoản cũ)
                                if (storedPassword == matKhau)
                                {
                                    return new User
                                    {
                                        Acc = reader["Acc"].ToString(),
                                        TenDangNhap = reader["TenDangNhap"].ToString(),
                                        Email = reader["Email"].ToString()
                                    };
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error logging in: {ex.Message}");
            }
            return null;
        }

        /// <summary>
        /// Đăng ký user mới
        /// </summary>
        public async Task<bool> RegisterAsync(User user)
        {
            try
            {
                // Kiểm tra tài khoản đã tồn tại chưa
                if (await IsUsernameExistsAsync(user.TenDangNhap) || await IsEmailExistsAsync(user.Email))
                {
                    return false;
                }

                // Tạo mã tài khoản mới
                user.Acc = "USER" + DateTime.Now.ToString("yyyyMMddHHmmss");
                
                // Mã hóa mật khẩu
                user.MatKhau = PasswordHelper.HashPassword(user.MatKhau);

                // Lưu vào database
                return await CreateAsync(user);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error registering user: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Kiểm tra tên đăng nhập đã tồn tại chưa
        /// </summary>
        public async Task<bool> IsUsernameExistsAsync(string tenDangNhap)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    var query = "SELECT COUNT(*) FROM tblDangNhap WHERE TenDangNhap = @TenDangNhap";
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@TenDangNhap", tenDangNhap);
                        var count = (int)await command.ExecuteScalarAsync();
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking username exists: {ex.Message}");
                return true; // Trả về true để tránh tạo trùng lặp khi có lỗi
            }
        }

        /// <summary>
        /// Kiểm tra email đã tồn tại chưa
        /// </summary>
        public async Task<bool> IsEmailExistsAsync(string email)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    var query = "SELECT COUNT(*) FROM tblDangNhap WHERE Email = @Email";
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Email", email);
                        var count = (int)await command.ExecuteScalarAsync();
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking email exists: {ex.Message}");
                return true; // Trả về true để tránh tạo trùng lặp khi có lỗi
            }
        }

        /// <summary>
        /// Cập nhật mật khẩu plain text thành hash
        /// </summary>
        public async Task<bool> UpdatePasswordToHashAsync(string tenDangNhap, string plainPassword)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    var query = "UPDATE tblDangNhap SET MatKhau = @MatKhau WHERE TenDangNhap = @TenDangNhap";
                    using (var command = new SqlCommand(query, connection))
                    {
                        var hashedPassword = PasswordHelper.HashPassword(plainPassword);
                        command.Parameters.AddWithValue("@MatKhau", hashedPassword);
                        command.Parameters.AddWithValue("@TenDangNhap", tenDangNhap);
                        
                        var result = await command.ExecuteNonQueryAsync();
                        return result > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating password to hash: {ex.Message}");
                return false;
            }
        }
    }
}
