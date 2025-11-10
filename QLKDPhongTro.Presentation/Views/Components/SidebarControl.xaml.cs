using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using QLKDPhongTro.Presentation.Views.Windows;

namespace QLKDPhongTro.Presentation.Views.Components
{
    public partial class SidebarControl : UserControl
    {
        public event EventHandler<string> MenuItemClicked;
        public event EventHandler LogoutClicked;

        private bool _isInitializing = true;

        public SidebarControl()
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

        private void UpdateMenuSelection()
        {
            var currentWindow = Window.GetWindow(this);
            if (currentWindow == null) return;

            // Reset
            OverviewButton.IsChecked = false;
            AssetsButton.IsChecked = false;
            TenantsButton.IsChecked = false;
            ContractsButton.IsChecked = false;
            FinancialButton.IsChecked = false;
            PaymentButton.IsChecked = false;
            MaintenanceButton.IsChecked = false;
            ReportsButton.IsChecked = false;
            SecurityButton.IsChecked = false;

            // Match theo kiểu cửa sổ
            switch (currentWindow)
            {
                case DashWindow:
                    OverviewButton.IsChecked = true; break;
                case RoomWindow:
                    AssetsButton.IsChecked = true; break;
                case TenantManagementWindow:
                    TenantsButton.IsChecked = true; break;
                case ContractManagementWindow:
                    ContractsButton.IsChecked = true; break;
                case FinancialDashboardWindow:
                    FinancialButton.IsChecked = true; break;
                case PaymentListView:
                    PaymentButton.IsChecked = true; break;
                case MaintenanceListView:
                    MaintenanceButton.IsChecked = true; break;
                case ReportWindow:
                    ReportsButton.IsChecked = true; break;
                case UserSecurityWindow:
                    SecurityButton.IsChecked = true; break;
                default:
                    OverviewButton.IsChecked = false; break;
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

            var newWindow = new T();
            newWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            newWindow.Show();
            Application.Current.MainWindow = newWindow;
            currentWindow.Close();

            // Cập nhật selection theo window mới
            UpdateMenuSelection();
        }

        // Handlers với logic navigation tập trung - sử dụng event system để tránh đệ quy
        private void Overview_Checked(object sender, RoutedEventArgs e)
        {
            // Không thực hiện navigation nếu đang trong quá trình khởi tạo
            if (_isInitializing)
                return;

            NavigateToWindow<DashWindow>();
        }

        private void Assets_Checked(object sender, RoutedEventArgs e)
        {
            // Không thực hiện navigation nếu đang trong quá trình khởi tạo
            if (_isInitializing)
                return;

            NavigateToWindow<RoomWindow>();
        }

        private void Tenants_Checked(object sender, RoutedEventArgs e)
        {
            // Không thực hiện navigation nếu đang trong quá trình khởi tạo
            if (_isInitializing)
                return;

            NavigateToWindow<TenantManagementWindow>();
        }

        private void Contracts_Checked(object sender, RoutedEventArgs e)
        {
            // Không thực hiện navigation nếu đang trong quá trình khởi tạo
            if (_isInitializing)
                return;

            NavigateToWindow<ContractManagementWindow>();
        }
        private void Financial_Checked(object sender, RoutedEventArgs e)
        {
            if (_isInitializing)
                return;
            NavigateToWindow<FinancialDashboardWindow>();
        }

        private void Payment_Checked(object sender, RoutedEventArgs e)
        {
            if (_isInitializing)
                return;
            NavigateToWindow<PaymentListView>();
        }

        private void Maintenance_Checked(object sender, RoutedEventArgs e)
        {
            if (_isInitializing)
                return;
            NavigateToWindow<MaintenanceListView>();
        }

        private void Reports_Checked(object sender, RoutedEventArgs e)
        {
            if (_isInitializing)
                return;
            NavigateToWindow<ReportWindow>();
        }

        private void Security_Checked(object sender, RoutedEventArgs e)
        {
            if (_isInitializing)
                return;
            NavigateToWindow<UserSecurityWindow>();
        }
    }
}
