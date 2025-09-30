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

        #region SidebarControl Events

        private void SidebarControl_MenuItemClicked(object sender, string menuItem)
        {
            switch (menuItem)
            {
                case "Overview":
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
                    break;
                case "Rooms":
                    // Đã ở trang quản lý phòng
                    // Không cần làm gì
                    break;
                case "Tenants":
                    // TODO: Chuyển đến trang quản lý khách thuê
                    MessageBox.Show("Chức năng quản lý khách thuê sẽ được phát triển sau", "Thông báo", 
                                  MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
                case "Invoices":
                    // TODO: Chuyển đến trang quản lý hóa đơn
                    MessageBox.Show("Chức năng quản lý hóa đơn sẽ được phát triển sau", "Thông báo", 
                                  MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
                case "Contracts":
                    // TODO: Chuyển đến trang quản lý hợp đồng
                    MessageBox.Show("Chức năng quản lý hợp đồng sẽ được phát triển sau", "Thông báo", 
                                  MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
                case "Settings":
                    // TODO: Chuyển đến trang cài đặt
                    MessageBox.Show("Chức năng cài đặt sẽ được phát triển sau", "Thông báo", 
                                  MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
            }
        }

        private void SidebarControl_LogoutClicked(object sender, EventArgs e)
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
            try
            {
                // Tạo hiệu ứng mờ cho cửa sổ chính
                this.Opacity = 0.9;
                this.Effect = new System.Windows.Media.Effects.BlurEffect { Radius = 2 };
                
                // Tạo và hiển thị popup thêm phòng
                var addRoomWindow = new AddRoomWindow();
                addRoomWindow.Owner = this; // Đặt cửa sổ hiện tại là owner
                
                // Hiển thị popup và chờ kết quả
                bool? result = addRoomWindow.ShowDialog();
                
                // Khôi phục trạng thái bình thường của cửa sổ chính
                this.Opacity = 1.0;
                this.Effect = null;
                
                // Nếu người dùng thêm phòng thành công, có thể refresh danh sách
                if (result == true)
                {
                    // TODO: Refresh danh sách phòng
                    MessageBox.Show("Danh sách phòng đã được cập nhật!", "Thông báo", 
                                  MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                // Đảm bảo khôi phục trạng thái bình thường ngay cả khi có lỗi
                this.Opacity = 1.0;
                this.Effect = null;
                
                MessageBox.Show($"Lỗi khi mở form thêm phòng: {ex.Message}", "Lỗi", 
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditRoom_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Tạo hiệu ứng mờ cho cửa sổ chính
                this.Opacity = 0.9;
                this.Effect = new System.Windows.Media.Effects.BlurEffect { Radius = 2 };
                
                // Tạo và hiển thị popup chỉnh sửa phòng
                var editRoomWindow = new EditRoomWindow();
                editRoomWindow.Owner = this; // Đặt cửa sổ hiện tại là owner
                
                // TODO: Lấy thông tin phòng được chọn (có thể từ selected room hoặc parameter)
                // Ví dụ: editRoomWindow.SetRoomData("1", "Phòng 1", "7", "2,500,000", "Đặt trước", "Mô tả phòng...");
                editRoomWindow.SetRoomData("1", "Phòng 1", "7", "2,500,000", "Đặt trước", "Phòng đẹp, thoáng mát, gần trung tâm thành phố");
                
                // Hiển thị popup và chờ kết quả
                bool? result = editRoomWindow.ShowDialog();
                
                // Khôi phục trạng thái bình thường của cửa sổ chính
                this.Opacity = 1.0;
                this.Effect = null;
                
                // Nếu người dùng cập nhật phòng thành công, có thể refresh danh sách
                if (result == true)
                {
                    // TODO: Refresh danh sách phòng
                    MessageBox.Show("Danh sách phòng đã được cập nhật!", "Thông báo", 
                                  MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                // Đảm bảo khôi phục trạng thái bình thường ngay cả khi có lỗi
                this.Opacity = 1.0;
                this.Effect = null;
                
                MessageBox.Show($"Lỗi khi mở form chỉnh sửa phòng: {ex.Message}", "Lỗi", 
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ViewRoom_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Tạo hiệu ứng mờ cho cửa sổ chính
                this.Opacity = 0.9;
                this.Effect = new System.Windows.Media.Effects.BlurEffect { Radius = 2 };
                
                // Tạo và hiển thị popup xem chi tiết phòng
                var viewRoomWindow = new ViewRoomWindow();
                viewRoomWindow.Owner = this; // Đặt cửa sổ hiện tại là owner
                
                // TODO: Lấy thông tin phòng được chọn (có thể từ selected room hoặc parameter)
                // Ví dụ: viewRoomWindow.SetRoomData("1", "Phòng 1", "7", "2,500,000", "Đặt trước", "Mô tả phòng...", "25m²", "Tầng 2", "Đông Nam", "2 người", "5.0");
                viewRoomWindow.SetRoomData("1", "Phòng 1", "7", "2,500,000", "Đặt trước", 
                                         "Phòng đẹp, thoáng mát, gần trung tâm thành phố", 
                                         "25m²", "Tầng 2", "Đông Nam", "2 người", "5.0");
                
                // Hiển thị popup và chờ kết quả
                bool? result = viewRoomWindow.ShowDialog();
                
                // Khôi phục trạng thái bình thường của cửa sổ chính
                this.Opacity = 1.0;
                this.Effect = null;
            }
            catch (Exception ex)
            {
                // Đảm bảo khôi phục trạng thái bình thường ngay cả khi có lỗi
                this.Opacity = 1.0;
                this.Effect = null;
                
                MessageBox.Show($"Lỗi khi mở form xem chi tiết phòng: {ex.Message}", "Lỗi", 
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
