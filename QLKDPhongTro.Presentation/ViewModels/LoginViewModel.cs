using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using QLKDPhongTro.Presentation.Views.Windows;
using QLKDPhongTro.BusinessLayer.Controllers;
using QLKDPhongTro.DataLayer.Repositories;
using QLKDPhongTro.Presentation.ViewModels;

namespace QLKDPhongTro.Presentation.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly AuthController _authController;

        public LoginViewModel()
        {
            var userRepository = new UserRepository();
            _authController = new AuthController(userRepository);
        }

        [ObservableProperty]
        private string _username = string.Empty;

        [ObservableProperty]
        private string _password = string.Empty;

        [ObservableProperty]
        private bool _isLoading = false;

        [RelayCommand]
        private async Task Login()
        {
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu!",
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Kiểm tra đang loading để tránh multiple clicks
                if (IsLoading)
                    return;

                IsLoading = true;

                // Đăng nhập trực tiếp (OTP đang tắt tạm thời)
          
                var otpResult = await _authController.LoginWithOtpAsync(Username, Password);
                var loginResult = await _authController.LoginAsync(Username, Password);
                if (loginResult.IsSuccess && loginResult.User != null)
                {
                    // Mở dashboard sau khi đăng nhập thành công
                    // FIX: Truyền đủ 3 tham số (Username, Email, Password) khớp với OtpLoginWindow
                    var otpWindow = new OtpLoginWindow(Username, loginResult.User.Email, Password);

                    //otpWindow.Show();
                    var dashWindow = new DashWindow();
                    dashWindow.Show();

                    // Đóng LoginWindow
                    var loginWindow = Application.Current.Windows.OfType<LoginWindow>().FirstOrDefault();
                    loginWindow?.Close();

                    Application.Current.MainWindow = loginWindow;
                }
                else
                {
                    MessageBox.Show(loginResult.Message, "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    // Reset button state khi đăng nhập thất bại
                    ResetLoginButtonState();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        // Reset button state khi đăng nhập thất bại
        private void ResetLoginButtonState()
        {
            try
            {
                // Tìm LoginWindow trong Application.Current.Windows
                var loginWindow = Application.Current.Windows.OfType<LoginWindow>().FirstOrDefault();
                if (loginWindow != null)
                {
                    // Gọi method reset button từ LoginWindow
                    loginWindow.ResetLoginButton();
                }
            }
            catch (Exception ex)
            {
                // Log error nếu cần
                System.Diagnostics.Debug.WriteLine($"Error resetting login button: {ex.Message}");
            }
        }

        [RelayCommand]
        private void NavigateToRegister()
        {
            var registerWindow = new RegisterWindow();
            registerWindow.Show();

            Application.Current.MainWindow?.Close();
            Application.Current.MainWindow = registerWindow;
        }
    }
}
