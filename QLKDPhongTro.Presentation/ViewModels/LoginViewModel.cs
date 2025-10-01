using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
using QLKDPhongTro.Presentation.Views.Windows;
using QLKDPhongTro.BusinessLayer.Controllers;
using QLKDPhongTro.DataLayer.Repositories;
using System.Threading.Tasks;

namespace QLKDPhongTro.Presentation.ViewModels
{
    public partial class LoginViewModel : ViewModelBase
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
            try
            {
                // Kiểm tra đang loading để tránh multiple clicks
                if (IsLoading)
                    return;

                IsLoading = true;

                if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu!",
                        "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // ✅ Gọi login có OTP
                var result = await _authController.LoginWithOtpAsync(Username, Password);

                if (result.IsSuccess)
                {
                    // ✅ Hiện thông báo OTP đã được gửi
                    MessageBox.Show(result.Message, "Thông báo",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    // ✅ Mở form OTP
                    var otpWindow = new OtpWindow
                    {
                        DataContext = new OtpViewModel(_authController)
                    };
                    otpWindow.Show();

                    // Đóng cửa sổ login
                    Application.Current.MainWindow?.Close();
                    Application.Current.MainWindow = otpWindow;
                }
                else
                {
                    MessageBox.Show(result.Message, "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Error);
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

        [RelayCommand]
        private void NavigateToRegister()
        {
            var registerWindow = new RegisterWindow();
            registerWindow.Show();

            // Đóng cửa sổ đăng nhập hiện tại
            Application.Current.MainWindow?.Close();
            Application.Current.MainWindow = registerWindow;
        }
    }
}
