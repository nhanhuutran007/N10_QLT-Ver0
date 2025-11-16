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
                                var emailOrdinal = reader.GetOrdinal("Email");
                                var soDienThoaiOrdinal = reader.GetOrdinal("SoDienThoai");
                                users.Add(new User
                                {
                                    MaAdmin = Convert.ToInt32(reader["MaAdmin"]),
                                    TenDangNhap = reader.IsDBNull(reader.GetOrdinal("TenDangNhap")) ? string.Empty : reader["TenDangNhap"].ToString() ?? string.Empty,
                                    Email = reader.IsDBNull(emailOrdinal) ? string.Empty : reader.GetString(emailOrdinal),
                                    SoDienThoai = reader.IsDBNull(soDienThoaiOrdinal) ? string.Empty : reader.GetString(soDienThoaiOrdinal)
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
                                var emailOrdinal = reader.GetOrdinal("Email");
                                var soDienThoaiOrdinal = reader.GetOrdinal("SoDienThoai");
                                return new User
                                {
                                    MaAdmin = Convert.ToInt32(reader["MaAdmin"]),
                                    TenDangNhap = reader.IsDBNull(reader.GetOrdinal("TenDangNhap")) ? string.Empty : reader.GetString(reader.GetOrdinal("TenDangNhap")),
                                    Email = reader.IsDBNull(emailOrdinal) ? string.Empty : reader.GetString(emailOrdinal),
                                    SoDienThoai = reader.IsDBNull(soDienThoaiOrdinal) ? string.Empty : reader.GetString(soDienThoaiOrdinal)
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
        /// Lấy user theo MaAdmin (int)
        /// </summary>
        public async Task<User?> GetByMaAdminAsync(int maAdmin)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    var query = "SELECT MaAdmin, TenDangNhap, Email, SoDienThoai FROM Admin WHERE MaAdmin = @MaAdmin";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@MaAdmin", maAdmin);
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                var emailOrdinal = reader.GetOrdinal("Email");
                                var soDienThoaiOrdinal = reader.GetOrdinal("SoDienThoai");
                                return new User
                                {
                                    MaAdmin = Convert.ToInt32(reader["MaAdmin"]),
                                    TenDangNhap = reader.IsDBNull(reader.GetOrdinal("TenDangNhap")) ? string.Empty : reader.GetString(reader.GetOrdinal("TenDangNhap")),
                                    Email = reader.IsDBNull(emailOrdinal) ? string.Empty : reader.GetString(emailOrdinal),
                                    SoDienThoai = reader.IsDBNull(soDienThoaiOrdinal) ? string.Empty : reader.GetString(soDienThoaiOrdinal)
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting user by MaAdmin: {ex.Message}");
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
                                var emailOrdinal = reader.GetOrdinal("Email");
                                var soDienThoaiOrdinal = reader.GetOrdinal("SoDienThoai");
                                return new User
                                {
                                    MaAdmin = Convert.ToInt32(reader["MaAdmin"]),
                                    TenDangNhap = reader.IsDBNull(reader.GetOrdinal("TenDangNhap")) ? string.Empty : reader.GetString(reader.GetOrdinal("TenDangNhap")),
                                    Email = reader.IsDBNull(emailOrdinal) ? string.Empty : reader.GetString(emailOrdinal),
                                    SoDienThoai = reader.IsDBNull(soDienThoaiOrdinal) ? string.Empty : reader.GetString(soDienThoaiOrdinal)
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
                    var query = @"INSERT INTO Admin (TenDangNhap, MatKhau, Email, SoDienThoai, MaNha) 
                                  VALUES (@TenDangNhap, @MatKhau, @Email, @SoDienThoai, @MaNha)";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@TenDangNhap", user.TenDangNhap);
                        command.Parameters.AddWithValue("@MatKhau", user.MatKhau);
                        command.Parameters.AddWithValue("@Email", user.Email);
                        command.Parameters.AddWithValue("@SoDienThoai", user.SoDienThoai);
                        command.Parameters.AddWithValue("@MaNha", user.MaNha);
                        
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
        /// Cập nhật thông tin profile (không bao gồm mật khẩu)
        /// </summary>
        public async Task<bool> UpdateProfileAsync(int maAdmin, string tenDangNhap, string email, string soDienThoai)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    var query = @"UPDATE Admin 
                                  SET TenDangNhap = @TenDangNhap, 
                                      Email = @Email,
                                      SoDienThoai = @SoDienThoai
                                  WHERE MaAdmin = @MaAdmin";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@MaAdmin", maAdmin);
                        command.Parameters.AddWithValue("@TenDangNhap", tenDangNhap);
                        command.Parameters.AddWithValue("@Email", email ?? string.Empty);
                        command.Parameters.AddWithValue("@SoDienThoai", soDienThoai ?? string.Empty);
                        
                        var result = await command.ExecuteNonQueryAsync();
                        return result > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating profile: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Cập nhật mật khẩu
        /// </summary>
        public async Task<bool> UpdatePasswordAsync(int maAdmin, string oldPassword, string newPassword)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    
                    // Kiểm tra mật khẩu cũ
                    var checkQuery = "SELECT MatKhau FROM Admin WHERE MaAdmin = @MaAdmin";
                    string? storedPassword = null;
                    using (var checkCommand = new MySqlCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@MaAdmin", maAdmin);
                        var result = await checkCommand.ExecuteScalarAsync();
                        storedPassword = result?.ToString();
                    }

                    if (string.IsNullOrEmpty(storedPassword))
                    {
                        return false;
                    }

                    // Kiểm tra mật khẩu cũ (plain text hoặc hashed)
                    bool passwordMatch = false;
                    if (storedPassword.Trim() == oldPassword.Trim())
                    {
                        passwordMatch = true;
                    }
                    else if (PasswordHelper.VerifyPassword(oldPassword, storedPassword))
                    {
                        passwordMatch = true;
                    }

                    if (!passwordMatch)
                    {
                        return false;
                    }

                    // Cập nhật mật khẩu mới (hash)
                    var updateQuery = @"UPDATE Admin 
                                       SET MatKhau = @MatKhau
                                       WHERE MaAdmin = @MaAdmin";
                    using (var updateCommand = new MySqlCommand(updateQuery, connection))
                    {
                        var hashedPassword = PasswordHelper.HashPassword(newPassword);
                        updateCommand.Parameters.AddWithValue("@MaAdmin", maAdmin);
                        updateCommand.Parameters.AddWithValue("@MatKhau", hashedPassword);
                        
                        var result = await updateCommand.ExecuteNonQueryAsync();
                        return result > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating password: {ex.Message}");
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
        /// Kiểm tra MaNha có tồn tại trong bảng Nha không
        /// </summary>
        public async Task<bool> IsMaNhaExistsAsync(int maNha)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    var query = "SELECT COUNT(*) FROM Nha WHERE MaNha = @MaNha";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@MaNha", maNha);
                        var result = await command.ExecuteScalarAsync();
                        var count = result != null ? Convert.ToInt32(result) : 0;
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking MaNha exists: {ex.Message}");
                return false;
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
                    var query = "SELECT MaAdmin, TenDangNhap, MatKhau, Email, SoDienThoai, MaNha FROM Admin WHERE TenDangNhap = @TenDangNhap";
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
                                
                                var emailOrdinal = reader.GetOrdinal("Email");
                                var soDienThoaiOrdinal = reader.GetOrdinal("SoDienThoai");
                                var maNhaOrdinal = reader.GetOrdinal("MaNha");
                                
                                // Kiểm tra mật khẩu plain text trước (cho tài khoản cũ)
                                if (storedPassword?.Trim() == password?.Trim())
                                {
                                    Console.WriteLine("Debug Login - Plain text password match!");
                                    return new User
                                    {
                                        MaAdmin = maAdmin,
                                        TenDangNhap = tenDangNhap,
                                        Email = reader.IsDBNull(emailOrdinal) ? string.Empty : reader.GetString(emailOrdinal),
                                        SoDienThoai = reader.IsDBNull(soDienThoaiOrdinal) ? string.Empty : reader.GetString(soDienThoaiOrdinal),
                                        MaNha = reader.IsDBNull(maNhaOrdinal) ? 0 : reader.GetInt32(maNhaOrdinal)
                                    };
                                }
                                
                                // Kiểm tra mật khẩu đã hash (cho tài khoản mới)
                                if (!string.IsNullOrEmpty(storedPassword) && !string.IsNullOrEmpty(password) && PasswordHelper.VerifyPassword(password, storedPassword))
                                {
                                    Console.WriteLine("Debug Login - Hashed password match!");
                                    return new User
                                    {
                                        MaAdmin = maAdmin,
                                        TenDangNhap = tenDangNhap,
                                        Email = reader.IsDBNull(emailOrdinal) ? string.Empty : reader.GetString(emailOrdinal),
                                        SoDienThoai = reader.IsDBNull(soDienThoaiOrdinal) ? string.Empty : reader.GetString(soDienThoaiOrdinal),
                                        MaNha = reader.IsDBNull(maNhaOrdinal) ? 0 : reader.GetInt32(maNhaOrdinal)
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

                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using (var transaction = await connection.BeginTransactionAsync())
                    {
                        try
                        {
                            // Tạo Nha nếu chưa tồn tại với MaNha được nhập
                            var insertHouseQuery = @"INSERT INTO Nha (MaNha, DiaChi, TongSoPhong, GhiChu)
                                                     VALUES (@MaNha, 'Chưa cập nhật', 1, NULL)";
                            using (var houseCmd = new MySqlCommand(insertHouseQuery, connection, (MySqlTransaction)transaction))
                            {
                                houseCmd.Parameters.AddWithValue("@MaNha", user.MaNha);
                                try
                                {
                                    await houseCmd.ExecuteNonQueryAsync();
                                }
                                catch (MySqlException ex) when (ex.Number == 1062)
                                {
                                    // MaNha đã tồn tại -> bỏ qua
                                }
                            }

                            // Tạo Admin gắn với MaNha
                            var insertAdminQuery = @"INSERT INTO Admin (TenDangNhap, MatKhau, Email, SoDienThoai, MaNha)
                                                    VALUES (@TenDangNhap, @MatKhau, @Email, @SoDienThoai, @MaNha)";
                            using (var adminCmd = new MySqlCommand(insertAdminQuery, connection, (MySqlTransaction)transaction))
                            {
                                adminCmd.Parameters.AddWithValue("@TenDangNhap", user.TenDangNhap);
                                adminCmd.Parameters.AddWithValue("@MatKhau", user.MatKhau);
                                adminCmd.Parameters.AddWithValue("@Email", user.Email ?? string.Empty);
                                adminCmd.Parameters.AddWithValue("@SoDienThoai", user.SoDienThoai ?? string.Empty);
                                adminCmd.Parameters.AddWithValue("@MaNha", user.MaNha);

                                var rows = await adminCmd.ExecuteNonQueryAsync();
                                if (rows <= 0)
                                {
                                    await transaction.RollbackAsync();
                                    return false;
                                }
                            }

                            await transaction.CommitAsync();
                            return true;
                        }
                        catch
                        {
                            await transaction.RollbackAsync();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error registering user: {ex.Message}");
                return false;
            }
        }
    }
}

