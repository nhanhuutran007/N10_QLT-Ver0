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
                // Đóng tất cả các cửa sổ hiện tại
                var windowsToClose = new List<Window>();
                foreach (Window window in Application.Current.Windows)
                {
                    if (window != Application.Current.MainWindow)
                    {
                        windowsToClose.Add(window);
                    }
                }

                // Đóng các cửa sổ
                foreach (var window in windowsToClose)
                {
                    window.Close();
                }

                // Thử mở cửa sổ đăng nhập nếu tồn tại
                TryOpenLoginWindow();

                // Đóng cửa sổ hiện tại (nếu có)
                var currentWindow = Window.GetWindow(this);
                if (currentWindow != null && currentWindow != Application.Current.MainWindow)
                {
                    currentWindow.Close();
                }

                // Đóng main window nếu cần
                if (Application.Current.MainWindow != null)
                {
                    Application.Current.MainWindow.Close();
                }
            }
            catch (System.Exception ex)
            {
                // Fallback: đóng ứng dụng nếu có lỗi
                MessageBox.Show($"Đã xảy ra lỗi khi đăng xuất: {ex.Message}\nỨng dụng sẽ đóng.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }
        }

        private void TryOpenLoginWindow()
        {
            try
            {
                // Sử dụng reflection để kiểm tra và tạo LoginWindow nếu tồn tại
                var loginWindowType = System.Type.GetType("QLKDPhongTro.Presentation.Views.Windows.LoginWindow");
                if (loginWindowType != null)
                {
                    var loginWindow = System.Activator.CreateInstance(loginWindowType) as Window;
                    if (loginWindow != null)
                    {
                        loginWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                        loginWindow.Show();
                    }
                }
                else
                {
                    // Nếu không tìm thấy LoginWindow, thông báo và đóng ứng dụng
                    MessageBox.Show("Đăng xuất thành công. Ứng dụng sẽ đóng.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    Application.Current.Shutdown();
                }
            }
            catch
            {
                // Fallback: thông báo và đóng ứng dụng
                MessageBox.Show("Đăng xuất thành công. Ứng dụng sẽ đóng.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                Application.Current.Shutdown();
            }
        }
    }
}