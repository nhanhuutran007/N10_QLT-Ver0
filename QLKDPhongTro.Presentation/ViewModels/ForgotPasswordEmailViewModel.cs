using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QLKDPhongTro.BusinessLayer.Controllers;
using QLKDPhongTro.DataLayer.Repositories;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace QLKDPhongTro.Presentation.ViewModels
{
    public partial class ForgotPasswordEmailViewModel : ObservableObject
    {
        private readonly AuthController _authController;

        public ForgotPasswordEmailViewModel()
        {
            var userRepository = new UserRepository();
            _authController = new AuthController(userRepository);
        }

        [ObservableProperty]
        private string _email = string.Empty;

        [ObservableProperty]
        private bool _isSending = false;

        [RelayCommand]
        private async Task SendOtpAsync()
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                MessageBox.Show("Vui lòng nhập email!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                if (IsSending)
                    return;

                IsSending = true;

                var result = await _authController.SendResetPasswordOtpAsync(Email.Trim());

                if (result.IsSuccess)
                {
                    MessageBox.Show(result.Message, "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Mở màn hình nhập OTP bằng OtpLoginWindow (tái sử dụng layout)
                    var otpWindow = new Views.Windows.OtpLoginWindow(new ForgotPasswordOtpViewModel(Email.Trim()))
                    {
                        WindowStartupLocation = WindowStartupLocation.CenterScreen
                    };

                    otpWindow.ShowDialog();

                    // Đóng popup hiện tại sau khi OTP flow kết thúc
                    CloseCurrentWindow();
                }
                else
                {
                    MessageBox.Show(result.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi gửi OTP: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsSending = false;
            }
        }

        private void CloseCurrentWindow()
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.DataContext == this)
                {
                    window.Close();
                    break;
                }
            }
        }
    }
}
