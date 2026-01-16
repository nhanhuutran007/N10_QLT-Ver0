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
                // Sử dụng NavigationHelper để chuyển về LoginWindow
                // Lấy cửa sổ hiện tại đang active
                var currentWindow = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive) 
                                    ?? Application.Current.MainWindow;

                NavigationHelper.NavigateTo<Views.Windows.LoginWindow>(currentWindow);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi khi đăng xuất: {ex.Message}\\nỨng dụng sẽ đóng.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }
        }
    }
}
