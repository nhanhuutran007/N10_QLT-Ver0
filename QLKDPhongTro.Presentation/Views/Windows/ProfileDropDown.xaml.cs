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
                
                // Mở lại cửa sổ đăng nhập
                var loginWindow = new LoginWindow();
                loginWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                loginWindow.Show();
                
                // Đóng cửa sổ hiện tại (nếu có)
                var currentWindow = Window.GetWindow(this);
                if (currentWindow != null)
                {
                    currentWindow.Close();
                }
            }
        }
    }
}
