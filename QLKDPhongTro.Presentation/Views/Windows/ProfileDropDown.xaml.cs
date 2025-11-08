using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace QLKDPhongTro.Presentation.Views.Windows
{
    public partial class ProfileDropDown : UserControl
    {
        public ProfileDropDown()
        {
            InitializeComponent();
        }

        private void Support_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Chức năng hỗ trợ sẽ được phát triển trong tương lai.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
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