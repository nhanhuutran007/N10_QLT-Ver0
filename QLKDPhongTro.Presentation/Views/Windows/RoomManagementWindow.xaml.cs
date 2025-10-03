using QLKDPhongTro.BusinessLayer.DTOs;
using QLKDPhongTro.Presentation.ViewModels;
using QLKDPhongTro.Presentation.Views.Windows;
using System;
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
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        // Handle sidebar button clicks
        private void OverviewButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = new DashWindow();
            mainWindow.Show();
            this.Close();
        }

        private void RoomsButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is RentedRoomViewModel viewModel)
            {
                viewModel.LoadRoomsCommand.Execute(null);
            }
        }

        private void TenantsButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Chuyển đến quản lý khách thuê", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void InvoicesButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Chuyển đến quản lý hóa đơn", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ContractsButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Chuyển đến quản lý hợp đồng", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Chuyển đến cài đặt", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Bạn có chắc muốn đăng xuất?", "Xác nhận đăng xuất",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                var loginWindow = Application.Current.Windows.OfType<LoginWindow>().FirstOrDefault();
                if (loginWindow != null)
                {
                    loginWindow.Show();
                }
                else
                {
                    Application.Current.Shutdown();
                }
                this.Close();
            }
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
    }
}