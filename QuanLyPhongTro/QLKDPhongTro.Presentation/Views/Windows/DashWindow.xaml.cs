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

        // Các phương thức điều hướng menu
        private void OverviewButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Đã chuyển đến trang Tổng quan", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void RoomsButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Đã chuyển đến trang Quản lý phòng", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void TenantsButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Đã chuyển đến trang Khách thuê", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BillsButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Đã chuyển đến trang Hóa đơn", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ContractsButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Đã chuyển đến trang Hợp đồng", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Đã chuyển đến trang Cài đặt", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
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