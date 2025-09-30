using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using QLKDPhongTro.Presentation.ViewModels;

namespace QLKDPhongTro.Presentation.Views.Windows
{
    public partial class DashWindow : Window
    {
        public DashWindow()
        {
            InitializeComponent();
            this.DataContext = new DashboardViewModel();
            
            // Đảm bảo cửa sổ hiển thị ở giữa màn hình
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            
            // Thiết lập kích thước tối thiểu
            this.MinHeight = 600;
            this.MinWidth = 800;
            
            // Đảm bảo cửa sổ hiển thị ở giữa màn hình sau khi load
            this.Loaded += (s, e) => {
                var screenWidth = SystemParameters.PrimaryScreenWidth;
                var screenHeight = SystemParameters.PrimaryScreenHeight;
                this.Left = (screenWidth - this.Width) / 2;
                this.Top = (screenHeight - this.Height) / 2;
            };
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        // Các phương thức điều khiển cửa sổ
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        // Event handlers cho SidebarControl
        private void SidebarControl_MenuItemClicked(object sender, string menuItem)
        {
            switch (menuItem)
            {
                case "Overview":
                    MessageBox.Show("Đã chuyển đến trang Tổng quan", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
                case "Rooms":
                    // Chuyển đến trang quản lý phòng
                    var roomManagementWindow = new RoomManagementWindow();
                    
                    // Đảm bảo cửa sổ mới hiển thị ở giữa màn hình
                    roomManagementWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    
                    // Tính toán vị trí chính xác để đảm bảo ở giữa màn hình
                    var screenWidth = SystemParameters.PrimaryScreenWidth;
                    var screenHeight = SystemParameters.PrimaryScreenHeight;
                    var windowWidth = roomManagementWindow.Width;
                    var windowHeight = roomManagementWindow.Height;
                    
                    roomManagementWindow.Left = (screenWidth - windowWidth) / 2;
                    roomManagementWindow.Top = (screenHeight - windowHeight) / 2;
                    
                    roomManagementWindow.Show();
                    this.Close();
                    break;
                case "Tenants":
                    MessageBox.Show("Đã chuyển đến trang Khách thuê", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
                case "Invoices":
                    MessageBox.Show("Đã chuyển đến trang Hóa đơn", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
                case "Contracts":
                    MessageBox.Show("Đã chuyển đến trang Hợp đồng", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
                case "Settings":
                    MessageBox.Show("Đã chuyển đến trang Cài đặt", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
            }
        }

        private void SidebarControl_LogoutClicked(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Bạn có chắc chắn muốn đăng xuất?", "Xác nhận đăng xuất", 
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            
            if (result == MessageBoxResult.Yes)
            {
                // Quay lại cửa sổ đăng nhập
                var loginWindow = new LoginWindow();
                loginWindow.Show();
                this.Close();
            }
        }

        // Các phương thức hành động nhanh
        private void AddRoom_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Tính năng thêm phòng sẽ được triển khai", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void AddTenant_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Tính năng thêm khách thuê sẽ được triển khai", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void CreateBill_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Tính năng tạo hóa đơn sẽ được triển khai", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void NewContract_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Tính năng tạo hợp đồng mới sẽ được triển khai", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ViewAllNotifications_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Tính năng xem tất cả thông báo sẽ được triển khai", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}