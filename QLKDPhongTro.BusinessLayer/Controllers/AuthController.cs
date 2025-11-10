using QLKDPhongTro.BusinessLayer.DTOs;
using QLKDPhongTro.DataLayer.Models;
using QLKDPhongTro.DataLayer.Repositories;
using QLKDPhongTro.Presentation.Utils;
using System;
using System.Threading.Tasks;

namespace QLKDPhongTro.BusinessLayer.Controllers
{
    /// <summary>
    /// Controller xử lý logic nghiệp vụ cho Authentication
    /// </summary>
    public class AuthController
    {
        private readonly IUserRepository _userRepository;

        public AuthController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        /// <summary>
        /// Thông tin user hiện tại đã đăng nhập
        /// </summary>
        public static User? CurrentUser { get; set; }

        /// <summary>
        /// Xử lý đăng nhập thường
        /// </summary>
        public async Task<LoginResult> LoginAsync(string tenDangNhap, string matKhau)
        {
            try
            {
                if (string.IsNullOrEmpty(tenDangNhap) || string.IsNullOrEmpty(matKhau))
                {
                    return new LoginResult
                    {
                        IsSuccess = false,
                        Message = "Vui lòng nhập đầy đủ thông tin!"
                    };
                }

                var user = await _userRepository.LoginAsync(tenDangNhap, matKhau);

                if (user != null)
                {
                    // Set CurrentUser với đầy đủ thông tin từ database
                    CurrentUser = user;
                    
                    // Debug log để kiểm tra
                    System.Diagnostics.Debug.WriteLine($"=== Login Success ===");
                    System.Diagnostics.Debug.WriteLine($"CurrentUser set - MaAdmin: {user.MaAdmin}, TenDangNhap: '{user.TenDangNhap}', Email: '{user.Email}', SoDienThoai: '{user.SoDienThoai}'");
                    
                    return new LoginResult
                    {
                        IsSuccess = true,
                        Message = $"Chào mừng {user.TenDangNhap}!",
                        User = user
                    };
                }
                else
                {
                    return new LoginResult
                    {
                        IsSuccess = false,
                        Message = "Tên đăng nhập hoặc mật khẩu không đúng!"
                    };
                }
            }
            catch (Exception ex)
            {
                return new LoginResult
                {
                    IsSuccess = false,
                    Message = $"Lỗi đăng nhập: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Xử lý đăng ký
        /// </summary>
        public async Task<RegisterResult> RegisterAsync(string tenDangNhap, string email, string matKhau, string xacNhanMatKhau)
        {
            try
            {
                // Validate input
                var validationResult = ValidateRegistrationInput(tenDangNhap, email, matKhau, xacNhanMatKhau);
                if (!validationResult.IsValid)
                {
                    return new RegisterResult
                    {
                        IsSuccess = false,
                        Message = validationResult.Message
                    };
                }

                // Kiểm tra tên đăng nhập đã tồn tại
                if (await _userRepository.IsUsernameExistsAsync(tenDangNhap))
                {
                    return new RegisterResult
                    {
                        IsSuccess = false,
                        Message = "Tên đăng nhập đã tồn tại!"
                    };
                }

                // Kiểm tra email đã tồn tại
                if (await _userRepository.IsEmailExistsAsync(email))
                {
                    return new RegisterResult
                    {
                        IsSuccess = false,
                        Message = "Email đã tồn tại!"
                    };
                }

                // Tạo user mới
                var user = new User
                {
                    TenDangNhap = tenDangNhap,
                    Email = email,
                    MatKhau = matKhau // Sẽ được hash trong UserRepository
                };

                if (await _userRepository.RegisterAsync(user))
                {
                    return new RegisterResult
                    {
                        IsSuccess = true,
                        Message = "Đăng ký thành công!"
                    };
                }
                else
                {
                    return new RegisterResult
                    {
                        IsSuccess = false,
                        Message = "Đăng ký thất bại!"
                    };
                }
            }
            catch (Exception ex)
            {
                return new RegisterResult
                {
                    IsSuccess = false,
                    Message = $"Lỗi đăng ký: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Validate dữ liệu đăng ký
        /// </summary>
        private ValidationResult ValidateRegistrationInput(string tenDangNhap, string email, string matKhau, string xacNhanMatKhau)
        {
            if (string.IsNullOrEmpty(tenDangNhap))
                return new ValidationResult { IsValid = false, Message = "Vui lòng nhập tên đăng nhập!" };

            if (string.IsNullOrEmpty(email))
                return new ValidationResult { IsValid = false, Message = "Vui lòng nhập email!" };

            if (string.IsNullOrEmpty(matKhau))
                return new ValidationResult { IsValid = false, Message = "Vui lòng nhập mật khẩu!" };

            if (matKhau != xacNhanMatKhau)
                return new ValidationResult { IsValid = false, Message = "Mật khẩu xác nhận không khớp!" };

            if (!ValidatePassword(matKhau))
                return new ValidationResult { IsValid = false, Message = "Mật khẩu không đáp ứng yêu cầu!" };

            return new ValidationResult { IsValid = true };
        }

        /// <summary>
        /// Validate mật khẩu (>=6 ký tự, có số hoặc chữ hoa)
        /// </summary>
        private bool ValidatePassword(string password)
        {
            if (password.Length < 6) return false;

            bool hasUpper = false;
            bool hasDigit = false;

            foreach (char c in password)
            {
                if (char.IsUpper(c)) hasUpper = true;
                if (char.IsDigit(c)) hasDigit = true;
            }

            return hasUpper || hasDigit;
        }

        /// <summary>
        /// Đăng nhập có OTP
        /// </summary>
        public async Task<LoginResult> LoginWithOtpAsync(string tenDangNhap, string matKhau)
        {
            try
            {
                var user = await _userRepository.LoginAsync(tenDangNhap, matKhau);

                if (user != null)
                {
                    // Sinh OTP
                    var otp = OtpHelper.GenerateOtp();

                    // Gửi email
                    await EmailService.SendEmailAsync(
                        user.Email,
                        "Mã OTP đăng nhập",
                        $"Xin chào {user.TenDangNhap},\n\nMã OTP của bạn là: {otp}\nCó hiệu lực trong 5 phút."
                    );

                    CurrentUser = user;

                    return new LoginResult
                    {
                        IsSuccess = true,
                        Message = "Mật khẩu đúng. OTP đã được gửi tới email của bạn.",
                        User = user
                    };
                }
                else
                {
                    return new LoginResult
                    {
                        IsSuccess = false,
                        Message = "Tên đăng nhập hoặc mật khẩu không đúng!"
                    };
                }
            }
            catch (Exception ex)
            {
                return new LoginResult
                {
                    IsSuccess = false,
                    Message = $"Lỗi đăng nhập: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Xác thực OTP nhập từ form
        /// </summary>
        public bool VerifyOtp(string otp)
        {
            return OtpHelper.VerifyOtp(otp);
        }
    }
}
