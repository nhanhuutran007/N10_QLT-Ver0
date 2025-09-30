using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
using QLKDPhongTro.Presentation.Views.Windows;
using QLKDPhongTro.BusinessLayer.Controllers;
using QLKDPhongTro.BusinessLayer.DTOs;
using QLKDPhongTro.DataLayer.Models;
using QLKDPhongTro.DataLayer.Repositories;

namespace QLKDPhongTro.Presentation.ViewModels
{
    public partial class RegisterViewModel : ViewModelBase
    {
        private readonly AuthController _authController;

        public RegisterViewModel()
        {
            var userRepository = new UserRepository();
            _authController = new AuthController(userRepository);
        }

        [ObservableProperty]
        private string _username = string.Empty;

        [ObservableProperty]
        private string _email = string.Empty;

        [ObservableProperty]
        private string _password = string.Empty;

        [ObservableProperty]
        private string _confirmPassword = string.Empty;

        [ObservableProperty]
        private bool _isLoading = false;

        [RelayCommand]
        private async Task Register()
        {
            try
            {
                IsLoading = true;

                // Kiểm tra thông tin đăng ký
                if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Email) || 
                    string.IsNullOrEmpty(Password) || string.IsNullOrEmpty(ConfirmPassword))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Thực hiện đăng ký
                var result = await _authController.RegisterAsync(Username, Email, Password, ConfirmPassword);

                if (result.IsSuccess)
                {
                    MessageBox.Show(result.Message, "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    
                    // Chuyển về màn hình đăng nhập
                    var loginWindow = new LoginWindow();
                    loginWindow.Show();
                    
                    // Đóng cửa sổ đăng ký
                    Application.Current.MainWindow?.Close();
                    Application.Current.MainWindow = loginWindow;
                }
                else
                {
                    MessageBox.Show(result.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private void NavigateToLogin()
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            
            // Đóng cửa sổ đăng ký hiện tại
            Application.Current.MainWindow?.Close();
            Application.Current.MainWindow = loginWindow;
        }
    }
}
