using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using QLKDPhongTro.BusinessLayer.Controllers;

namespace QLKDPhongTro.Presentation.Views.Windows
{
    public partial class ProfileDropDown : UserControl, INotifyPropertyChanged
    {
        private string _userName = string.Empty;
        private string _userEmail = string.Empty;

        public string UserName
        {
            get => _userName;
            set
            {
                if (_userName != value)
                {
                    _userName = value;
                    OnPropertyChanged();
                }
            }
        }

        public string UserEmail
        {
            get => _userEmail;
            set
            {
                if (_userEmail != value)
                {
                    _userEmail = value;
                    OnPropertyChanged();
                }
            }
        }

        public ProfileDropDown()
        {
            InitializeComponent();
            DataContext = this;
            Loaded += ProfileDropDown_Loaded;
        }

        private void ProfileDropDown_Loaded(object sender, RoutedEventArgs e)
        {
            LoadUserInfo();
        }

        /// <summary>
        /// Load thông tin người dùng từ AuthController.CurrentUser
        /// </summary>
        public void LoadUserInfo()
        {
            try
            {
                var currentUser = AuthController.CurrentUser;
                if (currentUser != null)
                {
                    UserName = currentUser.TenDangNhap ?? string.Empty;
                    UserEmail = currentUser.Email ?? string.Empty;
                }
                else
                {
                    UserName = "Người dùng";
                    UserEmail = "N/A";
                }
            }
            catch
            {
                UserName = "Người dùng";
                UserEmail = "N/A";
            }
        }

        /// <summary>
        /// Refresh thông tin người dùng (public method để có thể gọi từ bên ngoài)
        /// </summary>
        public void RefreshUserInfo()
        {
            LoadUserInfo();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Support_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var window = new HouseInfoWindow
                {
                    Owner = Application.Current.MainWindow
                };
                window.ShowDialog();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Không thể mở màn hình thông tin nhà: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LogOut_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Bạn có chắc chắn muốn đăng xuất?", "Xác nhận đăng xuất", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                PerformLogout();
            }
        }

        private void PerformLogout()
        {
            try
            {
                // Lưu tham chiếu MainWindow hiện tại (nếu có)
                var oldMain = Application.Current.MainWindow;

                // Mở LoginWindow và set làm MainWindow mới
                var loginWindow = CreateLoginWindow();
                if (loginWindow != null)
                {
                    loginWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    loginWindow.Show();
                    Application.Current.MainWindow = loginWindow;
                }

                // Đóng tất cả các cửa sổ khác (trừ loginWindow mới)
                var windowsToClose = new List<Window>();
                foreach (Window window in Application.Current.Windows)
                {
                    if (loginWindow != null && window == loginWindow) continue;
                    windowsToClose.Add(window);
                }
                foreach (var window in windowsToClose)
                {
                    window.Close();
                }

                // Nếu không tạo được LoginWindow, fallback đóng ứng dụng
                if (loginWindow == null)
                {
                    MessageBox.Show("Đăng xuất thành công. Ứng dụng sẽ đóng.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    Application.Current.Shutdown();
                }
            }
            catch (System.Exception ex)
            {
                // Fallback: đóng ứng dụng nếu có lỗi
                MessageBox.Show($"Đã xảy ra lỗi khi đăng xuất: {ex.Message}\nỨng dụng sẽ đóng.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }
        }

        private Window CreateLoginWindow()
        {
            // Tạo LoginWindow bằng kiểu rõ ràng nếu sẵn có
            var loginWindowType = typeof(LoginWindow);
            if (loginWindowType != null)
            {
                return System.Activator.CreateInstance(loginWindowType) as Window;
            }
            return null;
        }

        private void TryOpenLoginWindow()
        {
            // Không còn dùng: logic mở login di chuyển sang PerformLogout để đảm bảo thứ tự set MainWindow
        }
    }
}