using System.Collections.Generic;
using System.Windows;

namespace QLKDPhongTro.Presentation.Utils
{
    public static class LogoutHelper
    {
        public static void PerformLogout()
        {
            try
            {
                // Lưu tham chiếu MainWindow hiện tại (nếu có)
                var oldMain = Application.Current.MainWindow;

                // Tạo LoginWindow mới nếu có
                var loginWindowType = typeof(Views.Windows.LoginWindow);
                Window? loginWindow = null;
                if (loginWindowType != null)
                {
                    loginWindow = System.Activator.CreateInstance(loginWindowType) as Window;
                }

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
                MessageBox.Show($"Đã xảy ra lỗi khi đăng xuất: {ex.Message}\\nỨng dụng sẽ đóng.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }
        }
    }
}
