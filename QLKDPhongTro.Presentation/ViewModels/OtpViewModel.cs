using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QLKDPhongTro.DataLayer.Models;
using QLKDPhongTro.DataLayer.Repositories;
using QLKDPhongTro.Presentation.Views.Windows;
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
                return;

            bool isValid = await _userRepository.VerifyOtpAsync(OtpCode);
            if (isValid)
            {
                var user = new User { TenDangNhap = _username, Email = _email, MatKhau = _password };
                bool created = await _userRepository.RegisterAsync(user);
                if (created)
                    MessageBox.Show("Đăng ký thành công!");
                else
                    MessageBox.Show("Tạo tài khoản thất bại!");

                Application.Current.Windows.OfType<OtpWindow>().FirstOrDefault()?.Close();
            }
            else
            {
                MessageBox.Show("OTP không hợp lệ!");
            }
        }

    }
}
