using QLKDPhongTro.DataLayer.Models;
using QLKDPhongTro.DataLayer.Utils;
using QLKDPhongTro.Presentation.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
// using System.Data.SqlClient; // Để sử dụng SQL Server (đã chuyển sang MySQL)
using MySql.Data.MySqlClient; // Sử dụng MySQL

namespace QLKDPhongTro.DataLayer.Repositories 
{
    /// <summary>
    /// Repository xử lý dữ liệu User
    /// </summary>
    public class UserRepository : IUserRepository
    {
        // Sử dụng ConnectDB chung để quản lý connection string
        private string connectionString => ConnectDB.GetConnectionString();

        /// <summary>
        /// Lấy tất cả users
        /// </summary>
        public async Task<List<User>> GetAllAsync()
        {
            var users = new List<User>();
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    var query = "SELECT MaAdmin, TenDangNhap, Email, SoDienThoai FROM Admin";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                users.Add(new User
                                {
                                    MaAdmin = Convert.ToInt32(reader["MaAdmin"]),
                                    TenDangNhap = reader["TenDangNhap"].ToString() ?? string.Empty,
                                    Email = reader["Email"].ToString() ?? string.Empty,
                                    SoDienThoai = reader["SoDienThoai"].ToString() ?? string.Empty
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

        /// <summary>
        /// Lấy user theo ID
        /// </summary>
        public async Task<User?> GetByIdAsync(string id)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    var query = "SELECT MaAdmin, TenDangNhap, Email, SoDienThoai FROM Admin WHERE MaAdmin = @MaAdmin";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@MaAdmin", Convert.ToInt32(id));
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return new User
                                {
                                    MaAdmin = Convert.ToInt32(reader["MaAdmin"]),
                                    TenDangNhap = reader["TenDangNhap"].ToString() ?? string.Empty,
                                    Email = reader["Email"].ToString() ?? string.Empty,
                                    SoDienThoai = reader["SoDienThoai"].ToString() ?? string.Empty
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

        /// <summary>
        /// Lấy user theo username
        /// </summary>
        public async Task<User?> GetByUsernameAsync(string username)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    var query = "SELECT MaAdmin, TenDangNhap, Email, SoDienThoai FROM Admin WHERE TenDangNhap = @TenDangNhap";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@TenDangNhap", username);
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return new User
                                {
                                    MaAdmin = Convert.ToInt32(reader["MaAdmin"]),
                                    TenDangNhap = reader["TenDangNhap"].ToString() ?? string.Empty,
                                    Email = reader["Email"].ToString() ?? string.Empty,
                                    SoDienThoai = reader["SoDienThoai"].ToString() ?? string.Empty
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting user by username: {ex.Message}");
            }
            return null;
        }

        /// <summary>
        /// Tạo user mới
        /// </summary>
        public async Task<bool> CreateAsync(User user)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    var query = @"INSERT INTO Admin (TenDangNhap, MatKhau, Email, SoDienThoai) 
                                  VALUES (@TenDangNhap, @MatKhau, @Email, @SoDienThoai)";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@TenDangNhap", user.TenDangNhap);
                        command.Parameters.AddWithValue("@MatKhau", user.MatKhau);
                        command.Parameters.AddWithValue("@Email", user.Email);
                        command.Parameters.AddWithValue("@SoDienThoai", user.SoDienThoai);
                        
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

        /// <summary>
        /// Cập nhật user
        /// </summary>
        public async Task<bool> UpdateAsync(User user)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    var query = @"UPDATE Admin 
                                  SET TenDangNhap = @TenDangNhap, 
                                      MatKhau = @MatKhau, 
                                      Email = @Email,
                                      SoDienThoai = @SoDienThoai
                                  WHERE MaAdmin = @MaAdmin";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@MaAdmin", user.MaAdmin);
                        command.Parameters.AddWithValue("@TenDangNhap", user.TenDangNhap);
                        command.Parameters.AddWithValue("@MatKhau", user.MatKhau);
                        command.Parameters.AddWithValue("@Email", user.Email);
                        command.Parameters.AddWithValue("@SoDienThoai", user.SoDienThoai);
                        
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

        /// <summary>
        /// Xóa user
        /// </summary>
        public async Task<bool> DeleteAsync(string id)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    var query = "DELETE FROM Admin WHERE MaAdmin = @MaAdmin";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@MaAdmin", Convert.ToInt32(id));
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
        /// Kiểm tra tên đăng nhập đã tồn tại chưa
        /// </summary>
        public async Task<bool> IsUsernameExistsAsync(string username)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    var query = "SELECT COUNT(*) FROM Admin WHERE TenDangNhap = @TenDangNhap";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@TenDangNhap", username);
                        var result = await command.ExecuteScalarAsync();
                        var count = result != null ? Convert.ToInt32(result) : 0;
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
                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    var query = "SELECT COUNT(*) FROM Admin WHERE Email = @Email";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Email", email);
                        var result = await command.ExecuteScalarAsync();
                        var count = result != null ? Convert.ToInt32(result) : 0;
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
        /// Đăng nhập user
        /// </summary>
        public async Task<User?> LoginAsync(string username, string password)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    var query = "SELECT MaAdmin, TenDangNhap, MatKhau, Email, SoDienThoai FROM Admin WHERE TenDangNhap = @TenDangNhap";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@TenDangNhap", username);
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                var storedPassword = reader["MatKhau"].ToString();
                                var maAdmin = Convert.ToInt32(reader["MaAdmin"]);
                                var tenDangNhap = reader["TenDangNhap"].ToString() ?? string.Empty;
                                
                                Console.WriteLine($"Debug Login - Username: '{username}', Password: '{password}'");
                                Console.WriteLine($"Debug Login - Stored Password: '{storedPassword}'");
                                Console.WriteLine($"Debug Login - MaAdmin: '{maAdmin}'");
                                Console.WriteLine($"Debug Login - Password length: {password?.Length}, Stored length: {storedPassword?.Length}");
                                Console.WriteLine($"Debug Login - Password equals: {storedPassword == password}");
                                
                                // Kiểm tra mật khẩu plain text trước (cho tài khoản cũ)
                                if (storedPassword?.Trim() == password?.Trim())
                                {
                                    Console.WriteLine("Debug Login - Plain text password match!");
                                    return new User
                                    {
                                        MaAdmin = maAdmin,
                                        TenDangNhap = tenDangNhap,
                                        Email = reader["Email"].ToString() ?? string.Empty,
                                        SoDienThoai = reader["SoDienThoai"].ToString() ?? string.Empty
                                    };
                                }
                                
                                // Kiểm tra mật khẩu đã hash (cho tài khoản mới)
                                if (!string.IsNullOrEmpty(storedPassword) && PasswordHelper.VerifyPassword(password, storedPassword))
                                {
                                    Console.WriteLine("Debug Login - Hashed password match!");
                                    return new User
                                    {
                                        MaAdmin = maAdmin,
                                        TenDangNhap = tenDangNhap,
                                        Email = reader["Email"].ToString() ?? string.Empty,
                                        SoDienThoai = reader["SoDienThoai"].ToString() ?? string.Empty
                                    };
                                }
                                
                                Console.WriteLine("Debug Login - No password match found!");
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
        //public async Task<bool> RegisterAsync(User user)
        //{
        //    try
        //    {
        //        // Kiểm tra tài khoản đã tồn tại chưa
        //        if (await IsUsernameExistsAsync(user.TenDangNhap) || await IsEmailExistsAsync(user.Email))
        //        {
        //            return false;
        //        }

        //        // Mã hóa mật khẩu
        //        user.MatKhau = PasswordHelper.HashPassword(user.MatKhau);

        //        // Lưu vào database
        //        return await CreateAsync(user);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error registering user: {ex.Message}");
        //        return false;
        //    }
        //}
        /// <summary>
        /// Gửi OTP đăng ký tới email người dùng
        /// </summary>
        public async Task<bool> SendOtpToEmailAsync(string email)
        {
            try
            {
                var otp = OtpHelper.GenerateOtp();
                await EmailService.SendEmailAsync(email, "Mã OTP đăng ký", $"Mã OTP của bạn là: {otp}");
                return true;
            }
            catch
            {
                return false;
            }
        }


        // Phiên bản async của VerifyOtp
        public Task<bool> VerifyOtpAsync(string otp)
        {
            bool result = OtpHelper.VerifyOtp(otp);
            return Task.FromResult(result);
        }

        // Đăng ký với OTP
        public async Task<bool> RegisterWithOtpAsync(User user, string otp)
        {
            if (!await VerifyOtpAsync(otp))
            {
                Console.WriteLine("OTP không hợp lệ hoặc đã hết hạn!");
                return false;
            }

            return await RegisterAsync(user);
        }

        /// <summary>
        /// Phương thức RegisterAsync hiện tại vẫn được giữ nguyên
        /// </summary>
        public async Task<bool> RegisterAsync(User user)
        {
            try
            {
                if (await IsUsernameExistsAsync(user.TenDangNhap) || await IsEmailExistsAsync(user.Email))
                {
                    return false;
                }

                user.MatKhau = PasswordHelper.HashPassword(user.MatKhau);
                return await CreateAsync(user);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error registering user: {ex.Message}");
                return false;
            }
        }
    }
}

