using QLKDPhongTro.BusinessLayer.DTOs;
using QLKDPhongTro.DataLayer.Models;
using QLKDPhongTro.DataLayer.Repositories;
using QLKDPhongTro.DataLayer.Utils;
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
                    //// Sinh OTP
                    var otp = OtpHelper.GenerateOtp();

                    // Gửi email với layout HTML đẹp (tương tự mail nhắc thanh toán)
                    var subject = "Mã OTP đăng nhập hệ thống quản lý thuê nhà";
                    var body = $@"<!DOCTYPE html>
<html lang='vi'>
<head>
    <meta charset='UTF-8' />
    <meta name='viewport' content='width=device-width, initial-scale=1.0' />
    <title>Mã OTP đăng nhập</title>
    <style>
        body {{
            margin: 0;
            padding: 0;
            background-color: #f3f4f6;
            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
            color: #111827;
        }}
        .wrapper {{
            width: 100%;
            background-color: #f3f4f6;
            padding: 24px 0;
        }}
        .container {{
            max-width: 640px;
            margin: 0 auto;
            background-color: #ffffff;
            border-radius: 8px;
            overflow: hidden;
            box-shadow: 0 10px 25px rgba(15, 23, 42, 0.08);
        }}
        .header {{
            background: linear-gradient(135deg, #0ea5e9, #6366f1);
            padding: 20px 28px;
            color: #ffffff;
        }}
        .brand-name {{
            font-size: 20px;
            font-weight: 700;
            letter-spacing: 0.03em;
        }}
        .subtitle {{
            font-size: 13px;
            opacity: 0.9;
            margin-top: 4px;
        }}
        .content {{
            padding: 24px 28px 28px 28px;
        }}
        .greeting {{
            font-size: 15px;
            margin-bottom: 12px;
        }}
        .lead {{
            font-size: 14px;
            color: #374151;
            margin-bottom: 18px;
        }}
        .otp-card {{
            border-radius: 10px;
            border: 1px solid #e5e7eb;
            padding: 18px 20px;
            background: linear-gradient(135deg, #eff6ff 0%, #f9fafb 60%, #fefce8 100%);
            margin-bottom: 18px;
            text-align: center;
        }}
        .otp-label {{
            font-size: 13px;
            text-transform: uppercase;
            letter-spacing: 0.12em;
            color: #6b7280;
            margin-bottom: 8px;
        }}
        .otp-value {{
            font-size: 28px;
            font-weight: 700;
            letter-spacing: 0.35em;
            color: #111827;
        }}
        .note {{
            font-size: 12px;
            color: #6b7280;
            line-height: 1.6;
            margin-top: 10px;
        }}
        .footer {{
            padding: 16px 28px 22px 28px;
            font-size: 11px;
            color: #9ca3af;
            text-align: center;
        }}
        .divider {{
            height: 1px;
            background: linear-gradient(to right, transparent, #e5e7eb, transparent);
            margin: 18px 0 14px 0;
        }}
        @media (max-width: 640px) {{
            .container {{
                border-radius: 0;
            }}
            .content {{
                padding: 20px 18px 22px 18px;
            }}
            .header {{
                padding: 18px 18px;
            }}
        }}
    </style>
</head>
<body>
    <div class='wrapper'>
        <div class='container'>
            <div class='header'>
                <div class='brand-name'>HỆ THỐNG QUẢN LÝ THUÊ NHÀ</div>
                <div class='subtitle'>Xác thực đăng nhập bằng mã OTP</div>
            </div>
            <div class='content'>
                <div class='greeting'>Xin chào {user.TenDangNhap},</div>
                <div class='lead'>
                    Bạn vừa yêu cầu đăng nhập vào hệ thống. Vui lòng sử dụng mã OTP bên dưới để hoàn tất bước xác thực.
                </div>
                <div class='otp-card'>
                    <div class='otp-label'>MÃ OTP ĐĂNG NHẬP</div>
                    <div class='otp-value'>{otp}</div>
                    <div class='note' style='margin-top:12px;'>
                        Mã OTP này có hiệu lực trong <strong>5 phút</strong>. 
                        Vui lòng không chia sẻ mã này cho bất kỳ ai.
                    </div>
                </div>
                <div class='note'>
                    Nếu bạn không thực hiện yêu cầu đăng nhập này, có thể tài khoản của bạn đang bị truy cập trái phép. 
                    Hãy đổi mật khẩu ngay sau khi đăng nhập thành công hoặc liên hệ quản trị viên để được hỗ trợ.
                </div>
                <div class='divider'></div>
                <div class='note'>
                    Trân trọng,<br/>
                    Đội ngũ vận hành hệ thống Quản lý Thuê Nhà
                </div>
            </div>
            <div class='footer'>
                Email được gửi tự động, vui lòng không trả lời trực tiếp.
            </div>
        </div>
    </div>
</body>
</html>";

                    await EmailService.SendEmailAsync(
                        user.Email,
                        subject,
                        body
                    );

                    //CurrentUser = user;

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

        /// <summary>
        /// Gửi OTP đặt lại mật khẩu tới email
        /// </summary>
        public async Task<LoginResult> SendResetPasswordOtpAsync(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    return new LoginResult
                    {
                        IsSuccess = false,
                        Message = "Vui lòng nhập email!"
                    };
                }

                var user = await _userRepository.GetByEmailAsync(email);
                if (user == null)
                {
                    return new LoginResult
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy tài khoản với email này!"
                    };
                }

                var otp = OtpHelper.GenerateOtp();

                var subject = "Mã OTP đặt lại mật khẩu hệ thống quản lý thuê nhà";
                var body = $@"<!DOCTYPE html>
<html lang='vi'>
<head>
    <meta charset='UTF-8' />
    <meta name='viewport' content='width=device-width, initial-scale=1.0' />
    <title>Mã OTP đặt lại mật khẩu</title>
    <style>
        body {{
            margin: 0;
            padding: 0;
            background-color: #f3f4f6;
            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
            color: #111827;
        }}
        .wrapper {{
            width: 100%;
            background-color: #f3f4f6;
            padding: 24px 0;
        }}
        .container {{
            max-width: 640px;
            margin: 0 auto;
            background-color: #ffffff;
            border-radius: 8px;
            overflow: hidden;
            box-shadow: 0 10px 25px rgba(15, 23, 42, 0.08);
        }}
        .header {{
            background: linear-gradient(135deg, #0ea5e9, #6366f1);
            padding: 20px 28px;
            color: #ffffff;
        }}
        .brand-name {{
            font-size: 20px;
            font-weight: 700;
            letter-spacing: 0.03em;
        }}
        .subtitle {{
            font-size: 13px;
            opacity: 0.9;
            margin-top: 4px;
        }}
        .content {{
            padding: 24px 28px 28px 28px;
        }}
        .greeting {{
            font-size: 15px;
            margin-bottom: 12px;
        }}
        .lead {{
            font-size: 14px;
            color: #374151;
            margin-bottom: 18px;
        }}
        .otp-card {{
            border-radius: 10px;
            border: 1px solid #e5e7eb;
            padding: 18px 20px;
            background: linear-gradient(135deg, #eff6ff 0%, #f9fafb 60%, #fefce8 100%);
            margin-bottom: 18px;
            text-align: center;
        }}
        .otp-label {{
            font-size: 13px;
            text-transform: uppercase;
            letter-spacing: 0.12em;
            color: #6b7280;
            margin-bottom: 8px;
        }}
        .otp-value {{
            font-size: 28px;
            font-weight: 700;
            letter-spacing: 0.35em;
            color: #111827;
        }}
        .note {{
            font-size: 12px;
            color: #6b7280;
            line-height: 1.6;
            margin-top: 10px;
        }}
        .footer {{
            padding: 16px 28px 22px 28px;
            font-size: 11px;
            color: #9ca3af;
            text-align: center;
        }}
        .divider {{
            height: 1px;
            background: linear-gradient(to right, transparent, #e5e7eb, transparent);
            margin: 18px 0 14px 0;
        }}
    </style>
</head>
<body>
    <div class='wrapper'>
        <div class='container'>
            <div class='header'>
                <div class='brand-name'>HỆ THỐNG QUẢN LÝ THUÊ NHÀ</div>
                <div class='subtitle'>Đặt lại mật khẩu tài khoản</div>
            </div>
            <div class='content'>
                <div class='greeting'>Xin chào {user.TenDangNhap},</div>
                <div class='lead'>
                    Bạn vừa yêu cầu đặt lại mật khẩu. Vui lòng sử dụng mã OTP bên dưới để xác nhận yêu cầu.
                </div>
                <div class='otp-card'>
                    <div class='otp-label'>MÃ OTP ĐẶT LẠI MẬT KHẨU</div>
                    <div class='otp-value'>{otp}</div>
                    <div class='note'>
                        Mã OTP này có hiệu lực trong <strong>5 phút</strong>. Vui lòng không chia sẻ mã này cho bất kỳ ai.
                    </div>
                </div>
                <div class='divider'></div>
                <div class='note'>
                    Nếu bạn không yêu cầu đặt lại mật khẩu, hãy bỏ qua email này.
                </div>
            </div>
            <div class='footer'>
                Email được gửi tự động, vui lòng không trả lời trực tiếp.
            </div>
        </div>
    </div>
</body>
</html>";

                await EmailService.SendEmailAsync(user.Email, subject, body);

                CurrentUser = user;

                return new LoginResult
                {
                    IsSuccess = true,
                    Message = "OTP đặt lại mật khẩu đã được gửi tới email của bạn.",
                    User = user
                };
            }
            catch (Exception ex)
            {
                return new LoginResult
                {
                    IsSuccess = false,
                    Message = $"Lỗi khi gửi OTP đặt lại mật khẩu: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Đặt lại mật khẩu sau khi đã xác thực OTP, cho phép cập nhật cả tên đăng nhập
        /// </summary>
        public async Task<LoginResult> ResetPasswordAsync(string email, string newPassword, string? newUsername = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(newPassword))
                {
                    return new LoginResult
                    {
                        IsSuccess = false,
                        Message = "Email và mật khẩu mới không được để trống!"
                    };
                }

                if (!ValidatePassword(newPassword))
                {
                    return new LoginResult
                    {
                        IsSuccess = false,
                        Message = "Mật khẩu mới không đáp ứng yêu cầu (tối thiểu 6 ký tự và có số hoặc chữ hoa)!"
                    };
                }

                var user = await _userRepository.GetByEmailAsync(email);
                if (user == null)
                {
                    return new LoginResult
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy tài khoản với email này!"
                    };
                }

                // Nếu có truyền tên đăng nhập mới và khác tên hiện tại thì cập nhật
                if (!string.IsNullOrWhiteSpace(newUsername) &&
                    !string.Equals(newUsername, user.TenDangNhap, StringComparison.OrdinalIgnoreCase))
                {
                    if (await _userRepository.IsUsernameExistsAsync(newUsername))
                    {
                        return new LoginResult
                        {
                            IsSuccess = false,
                            Message = "Tên đăng nhập mới đã tồn tại. Vui lòng chọn tên khác!"
                        };
                    }

                    user.TenDangNhap = newUsername;
                }

                var hashedPassword = PasswordHelper.HashPassword(newPassword);
                user.MatKhau = hashedPassword;

                var success = await _userRepository.UpdateAsync(user);

                if (!success)
                {
                    return new LoginResult
                    {
                        IsSuccess = false,
                        Message = "Đặt lại mật khẩu thất bại. Vui lòng thử lại sau!"
                    };
                }

                return new LoginResult
                {
                    IsSuccess = true,
                    Message = "Đặt lại mật khẩu thành công! Bạn có thể đăng nhập với mật khẩu mới."
                };
            }
            catch (Exception ex)
            {
                return new LoginResult
                {
                    IsSuccess = false,
                    Message = $"Lỗi đặt lại mật khẩu: {ex.Message}"
                };
            }
        }
    }
}
