using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QLKDPhongTro.BusinessLayer.Controllers;
using QLKDPhongTro.Presentation.Views.Windows;
using System.Windows;
using System.Windows.Input;

namespace QLKDPhongTro.Presentation.ViewModels
{
    public partial class OtpViewModel : ViewModelBase
    {
        private readonly AuthController _authController;

        public OtpViewModel(AuthController authController)
        {
            _authController = authController;
        }

        [ObservableProperty]
        private string _otpCode = string.Empty;

        [ObservableProperty]
        private bool _isLoading = false;

        [RelayCommand]
        private void VerifyOtp()
        {
            try
            {
                IsLoading = true;

                if (string.IsNullOrEmpty(OtpCode))
                {
                    MessageBox.Show("Vui lòng nhập mã OTP!", "Thông báo",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (_authController.VerifyOtp(OtpCode))
                {
                    MessageBox.Show("✅ Xác thực OTP thành công!", "Thông báo",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    // Mở Dashboard
                    var dashboardWindow = new DashWindow();
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
                IsLoading = false;
            }
        }
    }
}

