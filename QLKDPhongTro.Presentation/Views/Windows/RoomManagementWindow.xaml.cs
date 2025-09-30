using System;
using System.Windows;
using System.Windows.Input;

namespace QLKDPhongTro.Presentation.Views.Windows
{
    /// <summary>
    /// Interaction logic for RoomManagementWindow.xaml
    /// </summary>
    public partial class RoomManagementWindow : Window
    {
        public RoomManagementWindow()
        {
            InitializeComponent();
            
            // Đảm bảo cửa sổ hiển thị ở giữa màn hình
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            
            // Thiết lập kích thước tối thiểu
            this.MinHeight = 700;
            this.MinWidth = 1200;
            
            // Đảm bảo cửa sổ hiển thị ở giữa màn hình sau khi load
            this.Loaded += (s, e) => {
                var screenWidth = SystemParameters.PrimaryScreenWidth;
                var screenHeight = SystemParameters.PrimaryScreenHeight;
                this.Left = (screenWidth - this.Width) / 2;
                this.Top = (screenHeight - this.Height) / 2;
            };
        }

        #region Window Control Events

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        #endregion

        #region Navigation Events

        private void OverviewButton_Click(object sender, RoutedEventArgs e)
        {
            // Chuyển về Dashboard
            var dashboardWindow = new DashWindow();
            
            // Đảm bảo cửa sổ mới hiển thị ở giữa màn hình
            dashboardWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            
            // Tính toán vị trí chính xác để đảm bảo ở giữa màn hình
            var screenWidth = SystemParameters.PrimaryScreenWidth;
            var screenHeight = SystemParameters.PrimaryScreenHeight;
            var windowWidth = dashboardWindow.Width;
            var windowHeight = dashboardWindow.Height;
            
            dashboardWindow.Left = (screenWidth - windowWidth) / 2;
            dashboardWindow.Top = (screenHeight - windowHeight) / 2;
            
            dashboardWindow.Show();
            this.Close();
        }

        private void RoomsButton_Click(object sender, RoutedEventArgs e)
        {
            // Đã ở trang quản lý phòng
            // Không cần làm gì
        }

        private void TenantsButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Chuyển đến trang quản lý khách thuê
            MessageBox.Show("Chức năng quản lý khách thuê sẽ được phát triển sau", "Thông báo", 
                          MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BillsButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Chuyển đến trang quản lý hóa đơn
            MessageBox.Show("Chức năng quản lý hóa đơn sẽ được phát triển sau", "Thông báo", 
                          MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ContractsButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Chuyển đến trang quản lý hợp đồng
            MessageBox.Show("Chức năng quản lý hợp đồng sẽ được phát triển sau", "Thông báo", 
                          MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Chuyển đến trang cài đặt
            MessageBox.Show("Chức năng cài đặt sẽ được phát triển sau", "Thông báo", 
                          MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Bạn có chắc chắn muốn đăng xuất?", "Xác nhận đăng xuất", 
                                       MessageBoxButton.YesNo, MessageBoxImage.Question);
            
            if (result == MessageBoxResult.Yes)
            {
                // TODO: Xử lý đăng xuất
                var loginWindow = new LoginWindow();
                loginWindow.Show();
                this.Close();
            }
        }

        #endregion

        #region Room Management Events

        private void AddRoom_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Mở form thêm phòng mới
            MessageBox.Show("Chức năng thêm phòng sẽ được phát triển sau", "Thông báo", 
                          MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void EditRoom_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Mở form sửa phòng
            MessageBox.Show("Chức năng sửa phòng sẽ được phát triển sau", "Thông báo", 
                          MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ViewRoom_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Mở form xem chi tiết phòng
            MessageBox.Show("Chức năng xem chi tiết phòng sẽ được phát triển sau", "Thông báo", 
                          MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void DeleteRoom_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Xác nhận và xóa phòng
            var result = MessageBox.Show("Bạn có chắc chắn muốn xóa phòng đã chọn?", "Xác nhận xóa", 
                                       MessageBoxButton.YesNo, MessageBoxImage.Warning);
            
            if (result == MessageBoxResult.Yes)
            {
                MessageBox.Show("Chức năng xóa phòng sẽ được phát triển sau", "Thông báo", 
                              MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        #endregion
    }
}
