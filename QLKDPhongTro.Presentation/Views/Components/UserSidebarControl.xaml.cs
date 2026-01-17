using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using QLKDPhongTro.Presentation.Views.Windows;
using QLKDPhongTro.Presentation.Views.Windows_User; // Namespace cho các window của User (sẽ tạo sau)
using QLKDPhongTro.Presentation.Utils;

namespace QLKDPhongTro.Presentation.Views.Components
{
    public partial class UserSidebarControl : UserControl
    {
        public event EventHandler<string> MenuItemClicked;
        public event EventHandler LogoutClicked;

        private bool _isInitializing = true;

        public UserSidebarControl()
        {
            InitializeComponent();
            // Đánh dấu đã khởi tạo xong sau một khoảng thời gian ngắn
            this.Loaded += (s, e) =>
            {
                System.Threading.Tasks.Task.Delay(100).ContinueWith(_ =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        _isInitializing = false;
                        UpdateMenuSelection();
                    });
                });
            };
        }

        // Property để set tiêu đề trang
        public string PageTitle
        {
            get { return _pageTitle; }
            set { _pageTitle = value; }
        }

        private string _pageTitle = "";

        // Property để set menu item được chọn
        public string SelectedMenuItem
        {
            get { return _selectedMenuItem; }
            set
            {
                _selectedMenuItem = value;
                UpdateMenuSelection();
            }
        }

        private string _selectedMenuItem = "";

        public void UpdateMenuSelection()
        {
            var currentWindow = Window.GetWindow(this);
            if (currentWindow == null) return;

            // Reset
            OverviewButton.IsChecked = false;
            HousingInfoButton.IsChecked = false;
            ContractsButton.IsChecked = false;
            MaintenanceButton.IsChecked = false;
            PaymentButton.IsChecked = false;
            SecurityButton.IsChecked = false;

            // Match theo kiểu cửa sổ
            // NOTE: Sẽ cần thêm các Window tương ứng cho User nếu chúng khác với Admin
            // Hiện tại dùng các Window admin cho những tính năng chung hoặc placeholder
            switch (currentWindow)
            {
                case UserDashboardWindow: // Sẽ tạo
                    OverviewButton.IsChecked = true; break;
                // case ViewRoomWindow: // Ví dụ cho Thông tin nhà
                //     HousingInfoButton.IsChecked = true; break;
                case UserHousingWindow:
                    HousingInfoButton.IsChecked = true; break;
                case UserContractWindow:
                    ContractsButton.IsChecked = true; break;
                case UserProfileWindow:
                    SecurityButton.IsChecked = true; break;
                // Các case khác sẽ thêm khi có window cụ thể
                default:
                    if (currentWindow is UserDashboardWindow) 
                        OverviewButton.IsChecked = true; 
                    break;
            }
        }

        // Centralized navigation helper method
        private void NavigateToWindow<T>() where T : Window, new()
        {
            if (_isInitializing)
                return;

            var currentWindow = Window.GetWindow(this);
            if (currentWindow == null)
                return;

            if (currentWindow is T)
                return;

            NavigationHelper.NavigateTo<T>(currentWindow);
            Application.Current.MainWindow = Application.Current.Windows.OfType<T>().FirstOrDefault();

            // Cập nhật selection theo window mới
            UpdateMenuSelection();
        }

        private void Overview_Checked(object sender, RoutedEventArgs e)
        {
            if (_isInitializing) return;
            NavigateToWindow<UserDashboardWindow>();
        }

        private void HousingInfo_Checked(object sender, RoutedEventArgs e)
        {
            if (_isInitializing) return;
            NavigateToWindow<UserHousingWindow>();
        }

        private void Contracts_Checked(object sender, RoutedEventArgs e)
        {
            if (_isInitializing) return;
            NavigateToWindow<UserContractWindow>();
        }

        private void Maintenance_Checked(object sender, RoutedEventArgs e)
        {
            if (_isInitializing) return;
            // TODO: Navigate to User Maintenance Window
             MessageBox.Show("Tính năng Bảo trì đang phát triển");
        }

        private void Payment_Checked(object sender, RoutedEventArgs e)
        {
            if (_isInitializing) return;
            // TODO: Navigate to User Payment Window
             MessageBox.Show("Tính năng Thanh toán đang phát triển");
        }

        private void Security_Checked(object sender, RoutedEventArgs e)
        {
            if (_isInitializing) return;
            NavigateToWindow<UserProfileWindow>(); // Sử dụng UserProfileWindow vừa tạo
        }
    }
}
