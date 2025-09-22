using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using QLKDPhongTro.Models;
using QLKDPhongTro.Services;
using QLKDPhongTro.Utils;
using QLKDPhongTro.Views;

namespace QLKDPhongTro.Controllers
{
    /// <summary>
    /// Controller xử lý logic đăng nhập, đăng ký
    /// </summary>
    public class AuthController : BaseController
    {
        private UserService userService;
        
        /// <summary>
        /// Thông tin user hiện tại đã đăng nhập
        /// </summary>
        public static User CurrentUser { get; set; }

        public AuthController(Form form) : base(form)
        {
            userService = new UserService();
        }

        /// <summary>
        /// Xử lý đăng nhập
        /// </summary>
        public async Task<bool> LoginAsync(string tenDangNhap, string matKhau)
        {
            try
            {
                if (string.IsNullOrEmpty(tenDangNhap) || string.IsNullOrEmpty(matKhau))
                {
                    ShowWarning("Vui lòng nhập đầy đủ thông tin!");
                    return false;
                }

                var user = await userService.LoginAsync(tenDangNhap, matKhau);
                if (user != null)
                {
                    ShowSuccess($"Chào mừng {user.TenDangNhap}!");
                    
                    // Lưu thông tin user vào session (có thể mở rộng sau)
                    CurrentUser = user;
                    
                    // Chuyển đến form chính
                    NavigateToMainForm();
                    return true;
                }
                else
                {
                    ShowError("Tên đăng nhập hoặc mật khẩu không đúng!");
                    return false;
                }
            }
            catch (Exception ex)
            {
                ShowError($"Lỗi đăng nhập: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Xử lý đăng ký
        /// </summary>
        public async Task<bool> RegisterAsync(string tenDangNhap, string email, string matKhau, string xacNhanMatKhau)
        {
            try
            {
                // Validate input
                if (!ValidateRegistrationInput(tenDangNhap, email, matKhau, xacNhanMatKhau))
                    return false;

                // Kiểm tra tên đăng nhập đã tồn tại
                if (await userService.IsUsernameExistsAsync(tenDangNhap))
                {
                    ShowError("Tên đăng nhập đã tồn tại!");
                    return false;
                }

                // Kiểm tra email đã tồn tại
                if (await userService.IsEmailExistsAsync(email))
                {
                    ShowError("Email đã tồn tại!");
                    return false;
                }

                // Tạo user mới
                var user = new User
                {
                    TenDangNhap = tenDangNhap,
                    Email = email,
                    MatKhau = matKhau // Sẽ được hash trong UserService
                };

                if (await userService.RegisterAsync(user))
                {
                    ShowSuccess("Đăng ký thành công!");
                    return true;
                }
                else
                {
                    ShowError("Đăng ký thất bại!");
                    return false;
                }
            }
            catch (Exception ex)
            {
                ShowError($"Lỗi đăng ký: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Validate dữ liệu đăng ký
        /// </summary>
        private bool ValidateRegistrationInput(string tenDangNhap, string email, string matKhau, string xacNhanMatKhau)
        {
            if (string.IsNullOrEmpty(tenDangNhap))
            {
                ShowWarning("Vui lòng nhập tên đăng nhập!");
                return false;
            }

            if (string.IsNullOrEmpty(email))
            {
                ShowWarning("Vui lòng nhập email!");
                return false;
            }

            if (string.IsNullOrEmpty(matKhau))
            {
                ShowWarning("Vui lòng nhập mật khẩu!");
                return false;
            }

            if (matKhau != xacNhanMatKhau)
            {
                ShowWarning("Mật khẩu xác nhận không khớp!");
                return false;
            }

            if (!ValidatePassword(matKhau))
            {
                ShowWarning("Mật khẩu không đáp ứng yêu cầu!");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validate mật khẩu
        /// </summary>
        private bool ValidatePassword(string password)
        {
            if (password.Length < 6)
                return false;

            bool hasUpper = false;
            bool hasDigit = false;

            foreach (char c in password)
            {
                if (char.IsUpper(c))
                    hasUpper = true;
                if (char.IsDigit(c))
                    hasDigit = true;
            }

            return hasUpper || hasDigit;
        }

        /// <summary>
        /// Chuyển đến form đăng nhập
        /// </summary>
        public void NavigateToLogin()
        {
            var loginForm = new frmDangNhap();
            loginForm.Show();
            currentForm.Hide();
        }

        /// <summary>
        /// Chuyển đến form đăng ký
        /// </summary>
        public void NavigateToRegister()
        {
            var registerForm = new frmDangKy();
            registerForm.Show();
            currentForm.Hide();
        }

        /// <summary>
        /// Chuyển đến form chính sau khi đăng nhập thành công
        /// </summary>
        public void NavigateToMainForm()
        {
            var mainForm = new frmFormApp();
            mainForm.Show();
            currentForm.Hide();
        }
    }
}
