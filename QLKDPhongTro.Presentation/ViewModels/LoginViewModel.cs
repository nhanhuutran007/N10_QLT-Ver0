using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
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
                IsLoading = true;

                var result = await _authController.LoginAsync(Username, Password);

                if (result.IsSuccess)
                {
                    MessageBox.Show(result.Message, "Thông báo",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    var dashboardWindow = new DashWindow()
                    {
                        DataContext = new DashboardViewModel()
                    };

                    dashboardWindow.Show();

                    // Đóng cửa sổ login cũ và gán MainWindow mới
                    Application.Current.MainWindow?.Close();
                    Application.Current.MainWindow = dashboardWindow;
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

            Application.Current.MainWindow?.Close();
            Application.Current.MainWindow = registerWindow;
        }
    }
}
