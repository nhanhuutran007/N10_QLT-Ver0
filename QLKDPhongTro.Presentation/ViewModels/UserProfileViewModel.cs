using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QLKDPhongTro.BusinessLayer.Controllers;
using QLKDPhongTro.DataLayer.Models;
using QLKDPhongTro.DataLayer.Repositories;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace QLKDPhongTro.Presentation.ViewModels
{
    public partial class UserProfileViewModel : ObservableObject
    {
        private readonly IUserRepository _userRepository;
        private int _maUser;

        [ObservableProperty]
        private string _tenDangNhap = string.Empty;

        [ObservableProperty]
        private string _email = string.Empty;

        [ObservableProperty]
        private string _soDienThoai = string.Empty;

        [ObservableProperty]
        private string _hoTen = string.Empty;

        [ObservableProperty]
        private DateTime? _ngaySinh;

        private string _cccd = string.Empty;

        public string CCCD
        {
            get => _cccd;
            set => SetProperty(ref _cccd, value);
        }

        [ObservableProperty]
        private DateTime? _ngayCap;

        [ObservableProperty]
        private string _noiCap = string.Empty;

        [ObservableProperty]
        private string _diaChi = string.Empty;



        [ObservableProperty]
        private string _oldPassword = string.Empty;

        [ObservableProperty]
        private string _newPassword = string.Empty;

        [ObservableProperty]
        private string _confirmPassword = string.Empty;

        [ObservableProperty]
        private bool _isLoading = false;

        // Sự kiện để thông báo cho View rằng cần xóa các ô PasswordBox
        public event Action? PasswordFieldsCleared;

        public UserProfileViewModel()
        {
            _userRepository = new UserRepository();
            _ = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            try
            {
                if (Application.Current == null) return;

                await System.Threading.Tasks.Task.Delay(100);

                if (Application.Current == null) return;
                var windows = Application.Current.Windows;
                if (windows == null || windows.Count == 0) return;

                bool hasVisibleWindow = false;
                foreach (Window window in windows)
                {
                    if (window != null && window.IsLoaded && window.IsVisible)
                    {
                        hasVisibleWindow = true;
                        break;
                    }
                }

                if (!hasVisibleWindow) return;

                IsLoading = true;

                var currentUser = AuthController.CurrentUser;
                if (currentUser == null)
                {
                    return;
                }

                _maUser = currentUser.MaUser;

                // Load User data
                var userFromDb = await _userRepository.GetByMaUserAsync(_maUser);

                if (userFromDb != null)
                {
                    TenDangNhap = userFromDb.TenDangNhap ?? string.Empty;
                    Email = userFromDb.Email ?? string.Empty;
                    SoDienThoai = userFromDb.SoDienThoai ?? string.Empty;
                    HoTen = userFromDb.HoTen ?? string.Empty;
                    NgaySinh = userFromDb.NgaySinh;
                    CCCD = userFromDb.CCCD ?? string.Empty;
                    NgayCap = userFromDb.NgayCap;
                    NoiCap = userFromDb.NoiCap ?? string.Empty;
                    DiaChi = userFromDb.DiaChi ?? string.Empty;
                    // Removed Bank Info


                    // Sync AuthController
                    AuthController.CurrentUser.TenDangNhap = TenDangNhap;
                    AuthController.CurrentUser.Email = Email;
                    AuthController.CurrentUser.SoDienThoai = SoDienThoai;
                    AuthController.CurrentUser.HoTen = HoTen;
                    AuthController.CurrentUser.NgaySinh = NgaySinh;
                    AuthController.CurrentUser.CCCD = CCCD;
                    AuthController.CurrentUser.NgayCap = NgayCap;
                    AuthController.CurrentUser.NoiCap = NoiCap;
                    AuthController.CurrentUser.DiaChi = DiaChi;
                }
                else
                {
                    MessageBox.Show($"Không tìm thấy người dùng với ID {_maUser} trong cơ sở dữ liệu.", "Lỗi Dữ Liệu", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi khi tải dữ liệu người dùng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task UpdateProfile()
        {
            if (IsLoading) return;

            if (string.IsNullOrWhiteSpace(TenDangNhap))
            {
                MessageBox.Show("Vui lòng nhập tên đăng nhập!", "Thông tin bắt buộc", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(HoTen))
            {
                MessageBox.Show("Vui lòng nhập họ và tên!", "Thông tin bắt buộc", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                IsLoading = true;

                var updatePayload = new User
                {
                    MaUser = _maUser,
                    TenDangNhap = TenDangNhap,
                    Email = Email,
                    SoDienThoai = SoDienThoai,
                    HoTen = HoTen,
                    NgaySinh = NgaySinh,
                    CCCD = CCCD,
                    NgayCap = NgayCap,
                    NoiCap = NoiCap,
                    DiaChi = DiaChi
                    // TenTK, SoTK, LinkQr removed from update payload so they remain unchanged or handled by DB default
                };

                // 1. Update Profile (User table)
                var profileSuccess = await _userRepository.UpdateProfileForUserAsync(updatePayload);

                if (!profileSuccess)
                {
                    MessageBox.Show("Cập nhật hồ sơ thất bại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    IsLoading = false;
                    return;
                }

                // Sync AuthController
                if (AuthController.CurrentUser != null)
                {
                    AuthController.CurrentUser.TenDangNhap = TenDangNhap;
                    AuthController.CurrentUser.Email = Email;
                    AuthController.CurrentUser.SoDienThoai = SoDienThoai;
                    AuthController.CurrentUser.HoTen = HoTen;
                    AuthController.CurrentUser.NgaySinh = NgaySinh;
                    AuthController.CurrentUser.CCCD = CCCD;
                    AuthController.CurrentUser.NoiCap = NoiCap;
                    AuthController.CurrentUser.DiaChi = DiaChi;
                }

                // 2. Update Password if provided
                bool passwordUpdateAttempted = !string.IsNullOrWhiteSpace(OldPassword) || !string.IsNullOrWhiteSpace(NewPassword) || !string.IsNullOrWhiteSpace(ConfirmPassword);

                if (passwordUpdateAttempted)
                {
                    if (string.IsNullOrWhiteSpace(OldPassword) || string.IsNullOrWhiteSpace(NewPassword) || string.IsNullOrWhiteSpace(ConfirmPassword))
                    {
                        MessageBox.Show("Vui lòng nhập đầy đủ Mật khẩu cũ, Mật khẩu mới và Xác nhận mật khẩu!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                        IsLoading = false;
                        return;
                    }

                    if (NewPassword != ConfirmPassword)
                    {
                        MessageBox.Show("Mật khẩu xác nhận không khớp!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                        IsLoading = false;
                        return;
                    }

                    var passwordSuccess = await _userRepository.UpdatePasswordForUserAsync(_maUser, OldPassword, NewPassword);
                    if (!passwordSuccess)
                    {
                        MessageBox.Show("Mật khẩu cũ không đúng!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                        IsLoading = false;
                        return;
                    }

                    OldPassword = string.Empty;
                    NewPassword = string.Empty;
                    ConfirmPassword = string.Empty;
                    PasswordFieldsCleared?.Invoke();
                }

                MessageBox.Show("Cập nhật hồ sơ thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi cập nhật hồ sơ: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
