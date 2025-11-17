using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QLKDPhongTro.DataLayer.Repositories;
using QLKDPhongTro.Presentation.Views.Windows;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace QLKDPhongTro.Presentation.ViewModels
{
    public partial class ForgotPasswordOtpViewModel : ObservableObject, IOtpEntryViewModel
    {
        private readonly UserRepository _userRepository;
        private readonly string _email;

        public ForgotPasswordOtpViewModel(string email)
        {
            _email = email;
            _userRepository = new UserRepository();
            VerifyOtpCommand = new AsyncRelayCommand(VerifyOtpAsync);
        }

        [ObservableProperty]
        private string _otpCode = string.Empty;

        [ObservableProperty]
        private bool _isVerifying = false;

        public ICommand VerifyOtpCommand { get; }

        private async Task VerifyOtpAsync()
        {
            if (string.IsNullOrWhiteSpace(OtpCode))
            {
                MessageBox.Show("Vui lòng nhập mã OTP!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                IsVerifying = true;

                bool isValid = await _userRepository.VerifyOtpAsync(OtpCode);
                if (isValid)
                {
                    MessageBox.Show("✅ Xác thực OTP thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Mở popup đặt lại mật khẩu
                    var resetWindow = new ResetPasswordWindow(_email)
                    {
                        WindowStartupLocation = WindowStartupLocation.CenterOwner,
                        Owner = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w is OtpLoginWindow)
                    };

                    resetWindow.ShowDialog();

                    // Đóng cửa sổ OTP sau khi xử lý xong
                    var otpWindow = Application.Current.Windows.OfType<OtpLoginWindow>().FirstOrDefault();
                    otpWindow?.Close();
                }
                else
                {
                    MessageBox.Show("❌ OTP không đúng hoặc đã hết hạn.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsVerifying = false;
            }
        }

        // IOtpEntryViewModel implementation
        string IOtpEntryViewModel.OtpCode
        {
            get => OtpCode;
            set => OtpCode = value;
        }
    }
}
