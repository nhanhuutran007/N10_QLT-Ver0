using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using QLKDPhongTro.DataLayer.Models;
using QLKDPhongTro.DataLayer.Repositories;
using QLKDPhongTro.Presentation.Views.Windows;
using QLKDPhongTro.Presentation.Utils;

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
        private string _maNha = string.Empty;

        [ObservableProperty]
        private string _password = string.Empty;

        [ObservableProperty]
        private string _confirmPassword = string.Empty;

        [ObservableProperty]
        private bool _isLoading = false;

        [RelayCommand]
        public async Task RegisterAsync()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Email) ||
                string.IsNullOrWhiteSpace(MaNha) || string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Thông báo", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(MaNha, out var maNhaParsed) || maNhaParsed <= 0)
            {
                MessageBox.Show("Mã nhà phải là số nguyên dương!", "Thông báo", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (Password != ConfirmPassword)
            {
                MessageBox.Show("Mật khẩu xác nhận không khớp!", "Thông báo", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Kiểm tra định dạng email
            if (!Email.Contains("@") || !Email.Contains("."))
            {
                MessageBox.Show("Vui lòng nhập địa chỉ email hợp lệ!", "Thông báo", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Kiểm tra điều kiện mật khẩu
            if (Password.Length < 6)
            {
                MessageBox.Show("Mật khẩu phải có ít nhất 6 ký tự!", "Thông báo", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Kiểm tra có ít nhất 1 ký tự viết hoa HOẶC 1 ký tự số
            bool hasUppercase = Password.Any(c => char.IsUpper(c));
            bool hasDigit = Password.Any(c => char.IsDigit(c));
            if (!hasUppercase && !hasDigit)
            {
                MessageBox.Show("Mật khẩu phải chứa ít nhất 1 ký tự viết hoa HOẶC 1 ký tự số!", "Thông báo", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            IsLoading = true;
            try
            {
                // Tạo tài khoản mới trực tiếp
                var newUser = new User
                {
                    TenDangNhap = Username,
                    Email = Email,
                    MatKhau = Password,
                    SoDienThoai = "", // Có thể thêm trường này nếu cần
                    MaNha = maNhaParsed
                };

                bool accountCreated = await _userRepository.RegisterAsync(newUser);
                
                if (accountCreated)
                {
                    // Hiển thị thông báo thành công
                    MessageBox.Show("🎉 Tài khoản đã được tạo thành công!\n\n" +
                                  $"Tên đăng nhập: {Username}\n" +
                                  $"Email: {Email}\n\n" +
                                  "Bạn có thể đăng nhập ngay bây giờ.", 
                                  "Đăng ký thành công", 
                                  MessageBoxButton.OK, 
                                  MessageBoxImage.Information);

                    // Chuyển về màn hình đăng nhập
                    // Chuyển về màn hình đăng nhập
                    var currentRegisterWindow = Application.Current.Windows.OfType<RegisterWindow>().FirstOrDefault();
                    NavigationHelper.NavigateTo<LoginWindow>(currentRegisterWindow);
                }
                else
                {
                    MessageBox.Show("❌ Không thể tạo tài khoản.\n\n" +
                                  "Có thể tên đăng nhập hoặc email đã tồn tại.\n" +
                                  "Vui lòng thử lại với thông tin khác.", 
                                  "Lỗi tạo tài khoản", 
                                  MessageBoxButton.OK, 
                                  MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tạo tài khoản: {ex.Message}", "Lỗi", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }


        [RelayCommand]
        private void NavigateToLogin()
        {
            var currentWindow = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
            NavigationHelper.NavigateTo<LoginWindow>(currentWindow);

            // Application.Current.MainWindow = loginWindow; // Handled by NavigationHelper or implicitly
        }
    }
}
