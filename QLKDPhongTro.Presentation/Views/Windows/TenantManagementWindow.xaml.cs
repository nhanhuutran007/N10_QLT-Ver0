using QLKDPhongTro.BusinessLayer.DTOs;
using QLKDPhongTro.Presentation.ViewModels;
using QLKDPhongTro.Presentation.Views.Windows;
using QLKDPhongTro.Presentation.Utils;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace QLKDPhongTro.Presentation.Views.Windows
{
    public partial class TenantManagementWindow : Window
    {
        private DateTime _lastClickTime;
        private object _lastClickedItem;

        public TenantManagementWindow()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.MinHeight = 600;
            this.MinWidth = 800;
            this.Height = 700;
            this.Width = 1200;

            // Khởi tạo DataContext với ViewModel
            this.DataContext = new TenantViewModel();

            // Load dữ liệu từ database
            LoadTenantData();

            // Thêm event handler cho việc thay đổi kích thước cửa sổ
            this.SizeChanged += TenantManagementWindow_SizeChanged;

            // Thêm event handler để kiểm tra và đóng ứng dụng khi đóng cửa sổ
            this.Closed += TenantManagementWindow_Closed;
        }

        // Load dữ liệu khách thuê
        private void LoadTenantData()
        {
            if (DataContext is TenantViewModel viewModel)
            {
                // Load tenants data từ database
                viewModel.LoadTenantsCommand?.Execute(null);
            }
        }

        // Handler cho TopbarControl events
        private void TopbarControl_MenuButtonClicked(object sender, EventArgs e)
        {
            // Logic chung đã được chuyển vào TopbarControl.xaml.cs
            // Chỉ xử lý logic riêng của TenantManagementWindow nếu cần
        }

        private void TopbarControl_SearchTextChanged(object sender, string searchText)
        {
            // Logic search chung đã được chuyển vào TopbarControl.xaml.cs
            // Chỉ xử lý search logic riêng của TenantManagementWindow nếu cần
            if (DataContext is TenantViewModel viewModel)
            {
                // TODO: Implement search logic
                System.Diagnostics.Debug.WriteLine($"Searching for: {searchText}");
            }
        }

        private void TopbarControl_SettingsButtonClicked(object sender, EventArgs e)
        {
            // Logic settings chung đã được chuyển vào TopbarControl.xaml.cs
            // Chỉ xử lý settings logic riêng của TenantManagementWindow nếu cần
        }

        // Handler cho SidebarControl events
        private void SidebarControl_MenuItemClicked(object sender, string menuItem)
        {
            // Logic navigation đã được chuyển vào SidebarControl.xaml.cs
            // Chỉ xử lý logic riêng của TenantManagementWindow nếu cần
        }

        private void TenantCard_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is TenantDto tenant)
            {
                if (DataContext is TenantViewModel viewModel)
                {
                    if (tenant == null)
                    {
                        MessageBox.Show("Không thể chọn khách thuê: Dữ liệu khách thuê không hợp lệ.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    viewModel.SelectedTenant = tenant;

                    // Handle double click to view tenant details
                    var currentTime = DateTime.Now;
                    if (_lastClickedItem == tenant && (currentTime - _lastClickTime).TotalMilliseconds < 500)
                    {
                        viewModel.ShowTenantDetailsWindow();
                        _lastClickedItem = null;
                        _lastClickTime = DateTime.MinValue;
                    }
                    else
                    {
                        _lastClickedItem = tenant;
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
                MessageBox.Show("Không thể chọn khách thuê: Dữ liệu không hợp lệ.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SidebarControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        // Event handler cho SizeChanged
        private void TenantManagementWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Cập nhật layout khi kích thước cửa sổ thay đổi
            UpdateTenantCardsLayout();
        }

        // Cập nhật layout của tenant cards
        private void UpdateTenantCardsLayout()
        {
            // Tìm ItemsControl trong XAML
            var tenantsItemsControl = FindName("TenantsItemsControl") as ItemsControl;
            if (tenantsItemsControl?.ItemsPanel != null)
            {
                // Sử dụng WrapPanel để tự động wrap các card
                var itemsPanelTemplate = new ItemsPanelTemplate();
                var wrapPanelFactory = new FrameworkElementFactory(typeof(WrapPanel));
                wrapPanelFactory.SetValue(WrapPanel.OrientationProperty, Orientation.Horizontal);
                wrapPanelFactory.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
                wrapPanelFactory.SetValue(MarginProperty, new Thickness(0));

                itemsPanelTemplate.VisualTree = wrapPanelFactory;
                tenantsItemsControl.ItemsPanel = itemsPanelTemplate;
            }
        }

        // Event handler cho Button click (thêm khách thuê)
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is TenantViewModel viewModel)
            {
                // Trigger add tenant command
                viewModel.ShowAddTenantPanelCommand?.Execute(null);
            }
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void TenantManagementWindow_Closed(object? sender, EventArgs e)
        {
            // Kiểm tra và đóng ứng dụng nếu không còn cửa sổ nào mở
            WindowHelper.CheckAndShutdownIfNoWindows(this);
        }

        private void TenantSearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
