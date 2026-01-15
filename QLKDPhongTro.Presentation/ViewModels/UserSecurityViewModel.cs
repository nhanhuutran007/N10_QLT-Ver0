using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QLKDPhongTro.BusinessLayer.Controllers;
using QLKDPhongTro.DataLayer.Models;
using QLKDPhongTro.DataLayer.Repositories; // Đảm bảo bạn có using này
using System;
using System.Threading.Tasks;
using System.Windows;

namespace QLKDPhongTro.Presentation.ViewModels
{
    public partial class UserSecurityViewModel : ObservableObject
    {
        private readonly IUserRepository _userRepository;
        private int _maAdmin;

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
        private string _tenTK = string.Empty;

        [ObservableProperty]
        private string _soTK = string.Empty;

        [ObservableProperty]
        private string _linkQr = string.Empty;

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

        public UserSecurityViewModel()
        {
            _userRepository = new UserRepository();

            // Gọi phương thức async từ constructor một cách an toàn để tải dữ liệu.
            // Dấu gạch dưới `_ =` để cho biết chúng ta không cần đợi tác vụ này hoàn thành.
            _ = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            try
            {
                // Kiểm tra xem ứng dụng có đang trong quá trình shutdown không
                if (Application.Current == null)
                {
                    return;
                }

                // Đợi một chút để đảm bảo window đã được load hoàn toàn
                await System.Threading.Tasks.Task.Delay(100);

                // Kiểm tra lại sau khi delay - nếu Application đã bị shutdown, return ngay
                try
                {
                    if (Application.Current == null)
                    {
                        return;
                    }

                    // Kiểm tra xem có cửa sổ nào còn tồn tại và đang visible không
                    // Nếu không có, có thể đang trong quá trình shutdown
                    var windows = Application.Current.Windows;
                    if (windows == null || windows.Count == 0)
                    {
                        return;
                    }

                    // Kiểm tra xem có cửa sổ nào đang visible không
                    bool hasVisibleWindow = false;
                    foreach (Window window in windows)
                    {
                        try
                        {
                            if (window != null && window.IsLoaded && window.IsVisible)
                            {
                                hasVisibleWindow = true;
                                break;
                            }
                        }
                        catch
                        {
                            // Nếu có lỗi khi kiểm tra window, bỏ qua
                            continue;
                        }
                    }

                    // Nếu không có cửa sổ nào visible, có thể đang shutdown - không làm gì cả
                    if (!hasVisibleWindow)
                    {
                        return;
                    }
                }
                catch
                {
                    // Nếu có lỗi khi kiểm tra, giả sử đang shutdown và return
                    return;
                }

                IsLoading = true;

                var currentUser = AuthController.CurrentUser;
                // **Điểm kiểm tra quan trọng nhất**
                if (currentUser == null)
                {
                    // Nếu đến đây, có nghĩa là có cửa sổ đang visible
                    // Nhưng để an toàn, chỉ hiển thị message nếu Application vẫn còn tồn tại
                    // và không phải đang trong quá trình shutdown
                    try
                    {
                        // Kiểm tra lại một lần nữa trước khi hiển thị MessageBox
                        if (Application.Current != null && Application.Current.MainWindow != null)
                        {
                            // Kiểm tra xem có cửa sổ nào vẫn còn visible không
                            bool stillHasVisibleWindow = false;
                            foreach (Window window in Application.Current.Windows)
                            {
                                try
                                {
                                    if (window != null && window.IsLoaded && window.IsVisible)
                                    {
                                        stillHasVisibleWindow = true;
                                        break;
                                    }
                                }
                                catch
                                {
                                    // Bỏ qua nếu có lỗi
                                    continue;
                                }
                            }

                            // Chỉ hiển thị message nếu vẫn còn cửa sổ visible
                            if (stillHasVisibleWindow)
                            {
                                MessageBox.Show("Không tìm thấy thông tin người dùng. Vui lòng đăng nhập lại.", "Lỗi Xác Thực", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                    catch
                    {
                        // Nếu có lỗi, không hiển thị message (có thể đang shutdown)
                    }
                    // Dừng thực thi nếu không có người dùng nào được đăng nhập.
                    return;
                }

                _maAdmin = currentUser.MaAdmin;

                // Luôn tải dữ liệu mới nhất từ database khi màn hình được mở
                var userFromDb = await _userRepository.GetByMaAdminAsync(_maAdmin);

                if (userFromDb != null)
                {
                    // Chỉ cần gán giá trị. [ObservableProperty] sẽ tự động thông báo cho UI.
                    TenDangNhap = userFromDb.TenDangNhap ?? string.Empty;
                    Email = userFromDb.Email ?? string.Empty;
                    SoDienThoai = userFromDb.SoDienThoai ?? string.Empty;
                    HoTen = userFromDb.HoTen ?? string.Empty;
                    NgaySinh = userFromDb.NgaySinh;
                    CCCD = userFromDb.CCCD ?? string.Empty;
                    NgayCap = userFromDb.NgayCap;
                    NoiCap = userFromDb.NoiCap ?? string.Empty;
                    DiaChi = userFromDb.DiaChi ?? string.Empty;
                    TenTK = userFromDb.TenTK ?? string.Empty;
                    SoTK = userFromDb.SoTK ?? string.Empty;
                    LinkQr = userFromDb.LinkQr ?? string.Empty;

                    // Đồng bộ lại AuthController.CurrentUser phòng trường hợp dữ liệu đã thay đổi
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
                    MessageBox.Show($"Không tìm thấy người dùng với ID {_maAdmin} trong cơ sở dữ liệu.", "Lỗi Dữ Liệu", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi khi tải dữ liệu người dùng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                // Luôn đảm bảo IsLoading được set về false, ngay cả khi có lỗi
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
                    MaAdmin = _maAdmin,
                    TenDangNhap = TenDangNhap,
                    Email = Email,
                    SoDienThoai = SoDienThoai,
                    HoTen = HoTen,
                    NgaySinh = NgaySinh,
                    CCCD = CCCD,
                    NgayCap = NgayCap,
                    NoiCap = NoiCap,
                    DiaChi = DiaChi,
                    TenTK = TenTK,
                    SoTK = SoTK,
                    LinkQr = LinkQr
                };

                // 1. Cập nhật thông tin profile
                var profileSuccess = await _userRepository.UpdateProfileAsync(updatePayload);

                if (!profileSuccess)
                {
                    MessageBox.Show("Cập nhật hồ sơ thất bại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    IsLoading = false; // Dừng lại nếu lỗi
                    return;
                }

                // Cập nhật lại thông tin trong AuthController để đồng bộ
                if (AuthController.CurrentUser != null)
                {
                    AuthController.CurrentUser.TenDangNhap = TenDangNhap;
                    AuthController.CurrentUser.Email = Email;
                    AuthController.CurrentUser.SoDienThoai = SoDienThoai;
                    AuthController.CurrentUser.HoTen = HoTen;
                    AuthController.CurrentUser.NgaySinh = NgaySinh;
                    AuthController.CurrentUser.CCCD = CCCD;
                    AuthController.CurrentUser.NgayCap = NgayCap;
                    AuthController.CurrentUser.NoiCap = NoiCap;
                    AuthController.CurrentUser.DiaChi = DiaChi;
                    AuthController.CurrentUser.TenTK = TenTK;
                    AuthController.CurrentUser.SoTK = SoTK;
                    AuthController.CurrentUser.LinkQr = LinkQr;
                }

                // 2. Cập nhật mật khẩu nếu người dùng có nhập
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

                    var passwordSuccess = await _userRepository.UpdatePasswordAsync(_maAdmin, OldPassword, NewPassword);
                    if (!passwordSuccess)
                    {
                        MessageBox.Show("Mật khẩu cũ không đúng!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                        IsLoading = false;
                        return;
                    }

                    // Xóa các trường mật khẩu trong ViewModel
                    OldPassword = string.Empty;
                    NewPassword = string.Empty;
                    ConfirmPassword = string.Empty;
                    // Kích hoạt sự kiện để View xóa các ô PasswordBox
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