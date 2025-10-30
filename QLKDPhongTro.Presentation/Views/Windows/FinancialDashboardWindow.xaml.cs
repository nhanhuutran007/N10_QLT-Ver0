using QLKDPhongTro.BusinessLayer.DTOs;
using QLKDPhongTro.Presentation.ViewModels;
using QLKDPhongTro.Presentation.Views.Components;
using System;
using System.Windows;
using System.Windows.Input;

namespace QLKDPhongTro.Presentation.Views.Windows
{
    public partial class FinancialDashboardWindow : Window
    {
        private readonly FinancialDashboardViewModel _viewModel;

        public FinancialDashboardWindow()
        {
            InitializeComponent();

            _viewModel = new FinancialDashboardViewModel();
            DataContext = _viewModel;

            // Đăng ký sự kiện từ ViewModel
            _viewModel.ShowPaymentFormRequested += OnShowPaymentFormRequested;
            _viewModel.ShowExpenseFormRequested += OnShowExpenseFormRequested;
            _viewModel.ShowMessageRequested += OnShowMessageRequested;
            _viewModel.DataRefreshed += OnDataRefreshed;

            // Set window properties
            this.MinHeight = 600;
            this.MinWidth = 800;
            this.Height = 700;
            this.Width = 1200;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _ = _viewModel.LoadDataAsync();
        }

        #region ViewModel Event Handlers

        private void OnShowPaymentFormRequested(object sender, EventArgs e) => ShowPaymentForm();
        private void OnShowExpenseFormRequested(object sender, EventArgs e) => ShowExpenseForm();
        private void OnShowMessageRequested(object sender, string message)
            => MessageBox.Show(message, "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        private void OnDataRefreshed(object sender, EventArgs e) { /* Update UI if needed */ }

        #endregion

        #region Navigation Methods

        public void NavigateToContractManagement()
        {
            try
            {
                // var contractWindow = new ContractManagementWindow();
                // contractWindow.Show();
                // this.Close();
                MessageBox.Show("Chuyển đến Quản lý Hợp đồng", "Thông báo");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi chuyển đến Quản lý Hợp đồng: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void NavigateToRoomManagement()
        {
            try
            {
                // var roomWindow = new RoomWindow();
                // roomWindow.Show();
                // this.Close();
                MessageBox.Show("Chuyển đến Quản lý Phòng", "Thông báo");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi chuyển đến Quản lý Phòng: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void NavigateToTenantManagement()
        {
            try
            {
                // var tenantWindow = new TenantManagementWindow();
                // tenantWindow.Show();
                // this.Close();
                MessageBox.Show("Chuyển đến Quản lý Người thuê", "Thông báo");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi chuyển đến Quản lý Người thuê: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void NavigateToOverview()
        {
            try
            {
                // var overviewWindow = new DashWindow();
                // overviewWindow.Show();
                // this.Close();
                MessageBox.Show("Chuyển đến Tổng quan", "Thông báo");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi chuyển đến Tổng quan: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void ShowPaymentForm()
        {
            try
            {
                // var paymentWindow = new PaymentFormWindow();
                // paymentWindow.Owner = this;
                // paymentWindow.ShowDialog();
                MessageBox.Show("Mở form thanh toán", "Thông báo");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở form thanh toán: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void ShowExpenseForm()
        {
            try
            {
                // var expenseWindow = new ExpenseFormWindow();
                // expenseWindow.Owner = this;
                // expenseWindow.ShowDialog();
                MessageBox.Show("Mở form chi phí", "Thông báo");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở form chi phí: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region Sidebar Navigation

        private void SidebarControl_MenuItemClicked(object sender, string menuItem)
        {
            try
            {
                if (string.IsNullOrEmpty(menuItem)) return;

                switch (menuItem.ToLower())
                {
                    case "overview":
                        NavigateToOverview();
                        break;
                    case "rooms":
                        NavigateToRoomManagement();
                        break;
                    case "tenants":
                        NavigateToTenantManagement();
                        break;
                    case "contracts":
                        NavigateToContractManagement();
                        break;
                    case "financial":
                        // Đã ở trang tài chính, refresh data
                        _ = _viewModel.RefreshDataAsync();
                        break;
                    default:
                        MessageBox.Show($"Chức năng '{menuItem}' đang được phát triển", "Thông báo",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xử lý menu: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SidebarControl_LogoutClicked(object sender, System.EventArgs e)
        {
            try
            {
                var result = MessageBox.Show(
                    "Bạn có chắc chắn muốn đăng xuất?",
                    "Xác nhận đăng xuất",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi đăng xuất: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region Topbar Event Handlers

        private void TopbarControl_MenuButtonClicked(object sender, System.EventArgs e)
        {
            try
            {
                // Toggle sidebar visibility
                var sidebarColumn = this.FindName("SidebarControl") as System.Windows.Controls.Grid;
                if (sidebarColumn != null)
                {
                    sidebarColumn.Visibility = sidebarColumn.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xử lý nút menu: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TopbarControl_SearchTextChanged(object sender, string searchText)
        {
            try
            {
                if (_viewModel != null)
                {
                    _viewModel.SearchText = searchText;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tìm kiếm: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TopbarControl_SettingsButtonClicked(object sender, System.EventArgs e)
        {
            MessageBox.Show("Tính năng cài đặt đang được phát triển", "Thông báo",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        #endregion

        #region Keyboard Shortcuts

        protected override void OnKeyDown(KeyEventArgs e)
        {
            try
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
                            _ = _viewModel.RefreshDataAsync();
                            e.Handled = true;
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi phím tắt: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region Cleanup

        protected override void OnClosed(EventArgs e)
        {
            try
            {
                // Hủy đăng ký sự kiện từ ViewModel
                if (_viewModel != null)
                {
                    _viewModel.ShowPaymentFormRequested -= OnShowPaymentFormRequested;
                    _viewModel.ShowExpenseFormRequested -= OnShowExpenseFormRequested;
                    _viewModel.ShowMessageRequested -= OnShowMessageRequested;
                    _viewModel.DataRefreshed -= OnDataRefreshed;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Cleanup error: {ex.Message}");
            }
            finally
            {
                base.OnClosed(e);
            }
        }

        #endregion
    }
}