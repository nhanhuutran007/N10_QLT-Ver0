using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
using QLKDPhongTro.Presentation.Services;
using QLKDPhongTro.Presentation.Views.Windows;

namespace QLKDPhongTro.Presentation.ViewModels
{
    public partial class LoginViewModel : ViewModelBase
    {
        private readonly AuthService _authService;

        public LoginViewModel()
        {
            _authService = new AuthService();
        }

        // Framework sẽ tự động tạo property "Username" từ field "_username"
        // và tự gọi OnPropertyChanged() khi giá trị thay đổi.
        [ObservableProperty]
        private string _username = string.Empty;

        [ObservableProperty]
        private string _password = string.Empty;

        [ObservableProperty]
        private bool _isLoading = false;

        // Framework sẽ tự động tạo một ICommand tên là "LoginCommand"
        // từ phương thức Login() này.
        [RelayCommand]
        private async Task Login()
        {
            try
            {
                IsLoading = true;

                // Kiểm tra thông tin đăng nhập
                if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }


                // Thực hiện đăng nhập
                var result = await _authService.LoginAsync(Username, Password);

                if (result.IsSuccess)
                {
                    MessageBox.Show(result.Message, "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    
                    // Chuyển đến Dashboard
                    var dashboardWindow = new DashWindow();
                    dashboardWindow.Show();
                    
                    // Đóng cửa sổ đăng nhập
                    Application.Current.MainWindow?.Close();
                    Application.Current.MainWindow = dashboardWindow;
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