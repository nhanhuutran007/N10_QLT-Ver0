using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using QLKDPhongTro.Presentation.Views.Windows;
using QLKDPhongTro.BusinessLayer.Controllers;
using QLKDPhongTro.DataLayer.Repositories;

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

                // // ✅ Gọi login có OTP (TẠM TẮT)
                // var result = await _authController.LoginWithOtpAsync(Username, Password);

                // ✅ Đăng nhập THÔNG THƯỜNG: kiểm tra đúng user/password là vào Dashboard
                var result = await _authController.LoginAsync(Username, Password);

                if (result.IsSuccess)
                {
                    // // ✅ Thông báo OTP đã gửi (TẠM TẮT)
                    // MessageBox.Show(result.Message, "Thông báo",
                    //     MessageBoxButton.OK, MessageBoxImage.Information);

                    // Bỏ thông báo, chuyển thẳng vào Dashboard
                    // Mở Dashboard
                    var dashboardWindow = new DashWindow
                    {
                        DataContext = new DashboardViewModel()
                    };

                    // Đóng cửa sổ đăng nhập hiện tại trước
                    var loginWindow = Application.Current.Windows.OfType<LoginWindow>().FirstOrDefault();
                    loginWindow?.Close();

                    // Đặt Dashboard làm MainWindow và hiển thị
                    Application.Current.MainWindow = dashboardWindow;
                    dashboardWindow.Show();
                }
                else
                {
                    MessageBox.Show(result.Message, "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Error);

                    // Reset button state khi đăng nhập thất bại
                    ResetLoginButtonState();
                }
                //var result = await _authController.LoginAsync(Username, Password);
                //if (result.IsSuccess)
                //{
                //    MessageBox.Show("Đăng nhập thành công!", "Thông báo",
                //        MessageBoxButton.OK, MessageBoxImage.Information);
                //    // Mở Dashboard
                //    var dashboardWindow = new DashWindow
                //    {
                //        DataContext = new DashboardViewModel()
                //    };
                //    dashboardWindow.Show();
                //    Application.Current.MainWindow?.Close();
                //    Application.Current.MainWindow = dashboardWindow;
                //}
                //else
                //{
                //    MessageBox.Show(result.Message, "Lỗi",
                //        MessageBoxButton.OK, MessageBoxImage.Error);
                //    // Reset button state khi đăng nhập thất bại
                //    ResetLoginButtonState();
                //}
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
