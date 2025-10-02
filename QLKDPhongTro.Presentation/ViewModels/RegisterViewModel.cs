using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using System.Windows;
using QLKDPhongTro.DataLayer.Models;
using QLKDPhongTro.DataLayer.Repositories;
using QLKDPhongTro.Presentation.Views.Windows;

namespace QLKDPhongTro.Presentation.ViewModels
{
    public partial class RegisterViewModel : ObservableObject
    {
        private readonly UserRepository _userRepository;

        public RegisterViewModel()
        {
            _userRepository = new UserRepository();
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
        private async Task RegisterAsync()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Email) ||
                string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!");
                return;
            }

            if (Password != ConfirmPassword)
            {
                MessageBox.Show("Mật khẩu xác nhận không khớp!");
                return;
            }

            IsLoading = true;
            try
            {
                bool otpSent = await _userRepository.SendOtpToEmailAsync(Email);
                if (!otpSent)
                {
                    MessageBox.Show("Gửi OTP thất bại. Vui lòng thử lại!");
                    return;
                }

                // Mở cửa sổ OTP
                var otpWindow = new OtpWindow(Username, Email, Password);
                otpWindow.Owner = Application.Current.MainWindow;
                otpWindow.ShowDialog();
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

            Application.Current.MainWindow?.Close();
            Application.Current.MainWindow = loginWindow;
        }
    }
}
