using QLKDPhongTro.BusinessLayer.DTOs;
using QLKDPhongTro.Presentation.ViewModels;
using QLKDPhongTro.Presentation.Views.Windows;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace QLKDPhongTro.Presentation.Views.Windows
{
    public partial class RoomManagementWindow : Window
    {
        private DateTime _lastClickTime;
        private object _lastClickedItem;

        public RoomManagementWindow()
        {
            InitializeComponent();
            this.DataContext = new RentedRoomViewModel();

            // Đảm bảo cửa sổ hiển thị ở giữa màn hình (giống Dashboard)
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            // Thiết lập kích thước tối thiểu (giống Dashboard)
            this.MinHeight = 600;
            this.MinWidth = 800;

            // Đảm bảo cửa sổ hiển thị ở giữa màn hình sau khi load (giống Dashboard)
            this.Loaded += (s, e) =>
            {
                var screenWidth = SystemParameters.PrimaryScreenWidth;
                var screenHeight = SystemParameters.PrimaryScreenHeight;
                this.Left = (screenWidth - this.Width) / 2;
                this.Top = (screenHeight - this.Height) / 2;
            };

            // Kết nối events với SidebarControl
            SidebarControl.MenuItemClicked += SidebarControl_MenuItemClicked;
            SidebarControl.LogoutClicked += SidebarControl_LogoutClicked;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        // Event handlers cho SidebarControl
        private void SidebarControl_MenuItemClicked(object sender, string menuItem)
        {
            switch (menuItem)
            {
                case "Overview":
                    // Tìm cửa sổ Dashboard hiện có hoặc tạo mới
                    var existingDashboard = Application.Current.Windows.OfType<DashWindow>().FirstOrDefault();
                    if (existingDashboard != null)
                    {
                        existingDashboard.Activate();
                        existingDashboard.WindowState = WindowState.Normal;
                    }
                    else
                    {
                        var mainWindow = new DashWindow();
                        mainWindow.Show();
                    }
                    this.Close();
                    break;
                case "Rooms":
                    if (DataContext is RentedRoomViewModel viewModel)
                    {
                        viewModel.LoadRoomsCommand.Execute(null);
                    }
                    break;
                case "Tenants":
                    MessageBox.Show("Chuyển đến quản lý khách thuê", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
                case "Invoices":
                    MessageBox.Show("Chuyển đến quản lý hóa đơn", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
                case "Contracts":
                    MessageBox.Show("Chuyển đến quản lý hợp đồng", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
                case "Settings":
                    MessageBox.Show("Chuyển đến cài đặt", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
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

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void RoomCard_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is RentedRoomDto room)
            {
                if (DataContext is RentedRoomViewModel viewModel)
                {
                    if (room == null)
                    {
                        MessageBox.Show("Không thể chọn phòng: Dữ liệu phòng không hợp lệ.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    viewModel.SelectedRoom = room;

                    // Handle double click to view room details
                    var currentTime = DateTime.Now;
                    if (_lastClickedItem == room && (currentTime - _lastClickTime).TotalMilliseconds < 500)
                    {
                        viewModel.ShowRoomDetailsWindow();
                        _lastClickedItem = null;
                        _lastClickTime = DateTime.MinValue;
                    }
                    else
                    {
                        _lastClickedItem = room;
                        _lastClickTime = currentTime;
                    }
                }
                else
                {
                    MessageBox.Show("Không thể truy cập ViewModel.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Không thể chọn phòng: Dữ liệu không hợp lệ.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SidebarControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}