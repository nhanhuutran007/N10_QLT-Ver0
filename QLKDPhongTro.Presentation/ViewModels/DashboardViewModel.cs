using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QLKDPhongTro.Presentation.Views.Windows;
using System.Collections.ObjectModel;
using System.Windows;

namespace QLKDPhongTro.Presentation.ViewModels
{
    public partial class DashboardViewModel : ViewModelBase
    {
        // Các thuộc tính thống kê
        [ObservableProperty]
        private int _totalRooms = 24;

        [ObservableProperty]
        private int _occupiedRooms = 18;

        [ObservableProperty]
        private int _emptyRooms = 6;

        [ObservableProperty]
        private int _totalTenants = 32;

        [ObservableProperty]
        private decimal _monthlyRevenue = 45000000;

        [ObservableProperty]
        private double _growthRate = 12.5;

        [ObservableProperty]
        private string _currentUser = "Admin User";

        [ObservableProperty]
        private string _userEmail = "admin@example.com";

        // Danh sách thông báo
        [ObservableProperty]
        private ObservableCollection<NotificationItem> _notifications = new();

        public DashboardViewModel()
        {
            InitializeNotifications();
        }

        private void InitializeNotifications()
        {
            Notifications = new ObservableCollection<NotificationItem>
            {
                new NotificationItem
                {
                    Title = "Hợp đồng sắp hết hạn",
                    Message = "3 hợp đồng sẽ hết hạn trong 7 ngày tới",
                    Type = NotificationType.Warning,
                    Icon = "⚠️"
                },
                new NotificationItem
                {
                    Title = "Phòng trống",
                    Message = "Phòng 201, 305 cần dọn dẹp",
                    Type = NotificationType.Info,
                    Icon = "🆓"
                },
                new NotificationItem
                {
                    Title = "Thanh toán mới",
                    Message = "Phòng 102 đã thanh toán tiền thuê",
                    Type = NotificationType.Success,
                    Icon = "✅"
                }
            };
        }

        // Các lệnh điều hướng
        [RelayCommand]
        private void NavigateToOverview()
        {
            MessageBox.Show("Đã chuyển đến trang Tổng quan", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        [RelayCommand]
        private void NavigateToRooms()
        {
            var rentedRoomWindow = new RoomWindow();
            rentedRoomWindow.Show();
            // Close the current dashboard window
            Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.DataContext == this)?.Close();
        }

        [RelayCommand]
        private void NavigateToTenants()
        {
            var tenantWindow = new TenantManagementWindow();
            tenantWindow.Show();
            // Close the current dashboard window
            Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.DataContext == this)?.Close();
        }

        [RelayCommand]
        private void NavigateToBills()
        {
            MessageBox.Show("Đã chuyển đến trang Hóa đơn", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        [RelayCommand]
        private void NavigateToContracts()
        {
            // Lấy cửa sổ Dashboard hiện tại
            var dashboard = Application.Current.Windows
                .OfType<Window>()
                .FirstOrDefault(w => w.DataContext is DashboardViewModel);

            if (dashboard == null)
            {
                MessageBox.Show("Không tìm thấy cửa sổ Dashboard!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Ẩn Dashboard
            dashboard.Hide();

            // Mở cửa sổ Hợp đồng
            var contractWindow = new ContractManagementWindow
            {
                Owner = dashboard, // 👈 Gắn Owner là Dashboard
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ShowInTaskbar = true
            };

            // Khi cửa sổ Hợp đồng đóng → hiện lại Dashboard
            contractWindow.Closed += (s, e) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    dashboard.Show();
                    dashboard.Activate();
                });
            };

            // Mở theo kiểu không chặn UI
            contractWindow.Show();
        }

        [RelayCommand]
        private void NavigateToSettings()
        {
            MessageBox.Show("Đã chuyển đến trang Cài đặt", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        [RelayCommand]
        private void Logout()
        {
            var result = MessageBox.Show("Bạn có chắc chắn muốn đăng xuất?", "Xác nhận đăng xuất", 
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            
            if (result == MessageBoxResult.Yes)
            {
                // Logic đăng xuất sẽ được triển khai ở đây
                Application.Current.Shutdown();
            }
        }

        // Các lệnh hành động nhanh
        [RelayCommand]
        private void AddRoom()
        {
            MessageBox.Show("Tính năng thêm phòng sẽ được triển khai", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        [RelayCommand]
        private void AddTenant()
        {
            MessageBox.Show("Tính năng thêm khách thuê sẽ được triển khai", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        [RelayCommand]
        private void CreateBill()
        {
            MessageBox.Show("Tính năng tạo hóa đơn sẽ được triển khai", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        [RelayCommand]
        private void NewContract()
        {
            MessageBox.Show("Tính năng tạo hợp đồng mới sẽ được triển khai", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        [RelayCommand]
        private void ViewAllNotifications()
        {
            MessageBox.Show("Tính năng xem tất cả thông báo sẽ được triển khai", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    // Lớp hỗ trợ cho thông báo
    public class NotificationItem
    {
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public NotificationType Type { get; set; }
        public string Icon { get; set; } = string.Empty;
    }

    public enum NotificationType
    {
        Info,
        Warning,
        Success,
        Error
    }
}
