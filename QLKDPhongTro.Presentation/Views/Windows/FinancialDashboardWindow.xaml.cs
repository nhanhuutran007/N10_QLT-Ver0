using System.Windows;
using System.Windows.Input;
using QLKDPhongTro.Presentation.ViewModels;
using QLKDPhongTro.Presentation.Views.Components;

namespace QLKDPhongTro.Presentation.Views.Windows
{
    public partial class FinancialDashboardWindow : Window
    {
        private readonly FinancialDashboardViewModel _viewModel;

        public FinancialDashboardWindow()
        {
            InitializeComponent();

            // Khởi tạo ViewModel
            _viewModel = new FinancialDashboardViewModel();
            DataContext = _viewModel;

            // Đăng ký sự kiện
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // Load dữ liệu khi window được load
            _ = _viewModel.LoadDataAsync();
        }

        #region Window Control Events

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized
                ? WindowState.Normal
                : WindowState.Maximized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        #endregion

        #region Navigation Methods

        public void NavigateToContractManagement()
        {
            var contractWindow = new ContractManagementWindow();
            contractWindow.Show();
            this.Close();
        }

        public void NavigateToRoomManagement()
        {
            var roomWindow = new RoomWindow();
            roomWindow.Show();
            this.Close();
        }

        public void NavigateToTenantManagement()
        {
            var tenantWindow = new TenantManagementWindow();
            tenantWindow.Show();
            this.Close();
        }

        public void NavigateToOverview()
        {
            var overviewWindow = new OverviewWindow();
            overviewWindow.Show();
            this.Close();
        }

        public void ShowPaymentForm()
        {
            var paymentWindow = new PaymentFormWindow();
            paymentWindow.Owner = this;
            paymentWindow.Closed += (s, e) => _ = _viewModel.LoadDataAsync();
            paymentWindow.ShowDialog();
        }

        public void ShowExpenseForm()
        {
            var expenseWindow = new ExpenseFormWindow();
            expenseWindow.Owner = this;
            expenseWindow.Closed += (s, e) => _ = _viewModel.LoadDataAsync();
            expenseWindow.ShowDialog();
        }

        public void ShowSettings()
        {
            var settingsWindow = new SettingsWindow();
            settingsWindow.Owner = this;
            settingsWindow.ShowDialog();
        }

        public void ShowHelp()
        {
            MessageBox.Show(
                "Hướng dẫn sử dụng Quản lý Tài chính:\n\n" +
                "• Ghi nhận tiền thuê: Thêm khoản thu từ hợp đồng\n" +
                "• Ghi nhận chi phí: Thêm chi phí phát sinh\n" +
                "• Tạo công nợ tự động: Tạo công nợ cho tất cả hợp đồng\n" +
                "• Xuất báo cáo: Xuất báo cáo tài chính\n\n" +
                "Liên hệ hỗ trợ: support@qlyphongtro.com",
                "Hướng dẫn sử dụng",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        #endregion

        #region Event Handlers for Components

        // Xử lý sự kiện từ TopbarControl (nếu có)
        private void TopbarControl_MenuButtonClicked(object sender, RoutedEventArgs e)
        {
            // Hiển thị sidebar hoặc thực hiện hành động menu
            ToggleSidebar();
        }

        private void TopbarControl_SearchTextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            // Xử lý tìm kiếm
            if (sender is System.Windows.Controls.TextBox textBox)
            {
                _viewModel.SearchDebtText = textBox.Text;
            }
        }

        private void TopbarControl_SettingsButtonClicked(object sender, RoutedEventArgs e)
        {
            ShowSettings();
        }

        // Xử lý sự kiện từ SidebarControl (nếu có)
        private void SidebarControl_MenuItemClicked(object sender, RoutedEventArgs e)
        {
            // Xử lý chuyển đổi giữa các trang
            if (sender is SidebarControl sidebar)
            {
                HandleSidebarNavigation(sidebar.SelectedMenuItem);
            }
        }

        private void SidebarControl_LogoutClicked(object sender, RoutedEventArgs e)
        {
            PerformLogout();
        }

        #endregion

        #region Private Helper Methods

        private void ToggleSidebar()
        {
            // Logic ẩn/hiện sidebar
            // Cần implement dựa trên UI design của bạn
        }

        private void HandleSidebarNavigation(string menuItem)
        {
            switch (menuItem?.ToLower())
            {
                case "tổng quan":
                case "overview":
                    NavigateToOverview();
                    break;
                case "hợp đồng":
                case "contracts":
                    NavigateToContractManagement();
                    break;
                case "phòng":
                case "rooms":
                    NavigateToRoomManagement();
                    break;
                case "người thuê":
                case "tenants":
                    NavigateToTenantManagement();
                    break;
                case "tài chính":
                case "financial":
                    // Đã ở trang tài chính, không cần chuyển
                    break;
                case "cài đặt":
                case "settings":
                    ShowSettings();
                    break;
                case "trợ giúp":
                case "help":
                    ShowHelp();
                    break;
            }
        }

        private void PerformLogout()
        {
            var result = MessageBox.Show(
                "Bạn có chắc chắn muốn đăng xuất?",
                "Xác nhận đăng xuất",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // Đóng tất cả windows và mở login window
                var loginWindow = new LoginWindow();
                loginWindow.Show();

                // Đóng window hiện tại
                this.Close();
            }
        }

        // Xử lý keyboard shortcuts
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                switch (e.Key)
                {
                    case Key.N:
                        ShowPaymentForm();
                        e.Handled = true;
                        break;
                    case Key.E:
                        ShowExpenseForm();
                        e.Handled = true;
                        break;
                    case Key.R:
                        _ = _viewModel.AutoGenerateDebtsAsync();
                        e.Handled = true;
                        break;
                    case Key.P:
                        _ = _viewModel.ExportReportAsync();
                        e.Handled = true;
                        break;
                    case Key.F5:
                        _ = _viewModel.LoadDataAsync();
                        e.Handled = true;
                        break;
                }
            }
        }

        #endregion

        #region Window Drag Move

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        #endregion
    }
}