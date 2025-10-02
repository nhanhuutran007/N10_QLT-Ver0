using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QLKDPhongTro.DataLayer.Models;
using QLKDPhongTro.DataLayer.Repositories;
using QLKDPhongTro.Presentation.Views.Windows;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace QLKDPhongTro.Presentation.ViewModels
{
    public partial class OtpViewModel : ObservableObject
    {
        private readonly UserRepository _userRepository;
        private readonly string _username;
        private readonly string _email;
        private readonly string _password;

        public OtpViewModel(UserRepository userRepository, string username, string email, string password)
        {
            _userRepository = userRepository;
            _username = username;
            _email = email;
            _password = password;
        }

        [ObservableProperty]
        private string _otpCode = string.Empty;

        [ObservableProperty]
        private bool _isVerifying = false;

        [RelayCommand]
        private async Task VerifyOtpAsync()
        {
            if (string.IsNullOrWhiteSpace(OtpCode))
            {
                MessageBox.Show("Vui lòng nhập mã OTP!", "Thông báo", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                IsVerifying = true;

                bool isValid = await _userRepository.VerifyOtpAsync(OtpCode);
                if (isValid)
                {
                    MessageBox.Show("✅ Xác thực OTP thành công!", "Thông báo",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    // Mở Dashboard
                    var dashboardWindow = new DashWindow
                    {
                        DataContext = new DashboardViewModel()
                    };
                    dashboardWindow.Show();

                    // Đóng OTP window
                    Application.Current.MainWindow?.Close();
                    Application.Current.MainWindow = dashboardWindow;
                }
                else
                {
                    MessageBox.Show("❌ OTP không đúng hoặc đã hết hạn.", "Lỗi",
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
                IsVerifying = false;
            }
        }

    }
}
