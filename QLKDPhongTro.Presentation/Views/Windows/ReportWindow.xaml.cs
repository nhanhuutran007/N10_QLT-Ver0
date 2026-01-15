using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using QLKDPhongTro.Presentation.ViewModels;
using QLKDPhongTro.Presentation.Utils;
using System.Threading.Tasks;

namespace QLKDPhongTro.Presentation.Views.Windows
{
    public partial class ReportWindow : Window
    {
        private FinancialDashboardViewModel? _viewModel;
        public ReportWindow()
        {
            InitializeComponent();
            // Sử dụng ViewModel tài chính có kết nối Controller/Repository
            _viewModel = new FinancialDashboardViewModel();
            this.DataContext = _viewModel;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.MinHeight = 600;
            this.MinWidth = 800;

            // Sự kiện vòng đời
            this.Loaded += ReportWindow_Loaded;
            this.Closed += ReportWindow_Closed;

            // Đăng ký các sự kiện từ ViewModel để hiển thị thông báo
            if (_viewModel != null)
            {
                _viewModel.ShowMessageRequested += ViewModel_ShowMessageRequested;
            }
        }

        private async void ReportWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (_viewModel != null)
            {
                // Tải dữ liệu ban đầu
                await _viewModel.InitializeAsync();
            }
        }

        private void ReportWindow_Closed(object? sender, EventArgs e)
        {
            if (_viewModel != null)
            {
                _viewModel.ShowMessageRequested -= ViewModel_ShowMessageRequested;
            }

            // Kiểm tra và đóng ứng dụng nếu không còn cửa sổ nào mở
            WindowHelper.CheckAndShutdownIfNoWindows(this);
        }

        private void ViewModel_ShowMessageRequested(object? sender, string e)
        {
            if (!string.IsNullOrWhiteSpace(e))
            {
                MessageBox.Show(e, "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }


        // Handler cho TopbarControl events - giờ đây TopbarControl tự xử lý logic chung
        private void TopbarControl_MenuButtonClicked(object sender, EventArgs e)
        {
            // Logic chung đã được chuyển vào TopbarControl.xaml.cs
            // Chỉ xử lý logic riêng của ReportWindow nếu cần
        }

        private void TopbarControl_SearchTextChanged(object sender, string searchText)
        {
            // Logic search chung đã được chuyển vào TopbarControl.xaml.cs
            // Chỉ xử lý search logic riêng của ReportWindow nếu cần
            if (_viewModel != null)
            {
                _viewModel.SearchText = searchText ?? string.Empty;
                _ = _viewModel.SearchDebtsAsync();
            }
        }

        private void TopbarControl_SettingsButtonClicked(object sender, EventArgs e)
        {
            // Logic settings chung đã được chuyển vào TopbarControl.xaml.cs
            // Chỉ xử lý settings logic riêng của ReportWindow nếu cần
        }

        // Handler cho SidebarControl events - giờ đây SidebarControl tự xử lý navigation
        private void SidebarControl_MenuItemClicked(object sender, string menuItem)
        {
            // Logic navigation đã được chuyển vào SidebarControl.xaml.cs
            // Chỉ xử lý logic riêng của ReportWindow nếu cần
        }

        // Handler cho nút Xuất - mở popup
        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            if (ExportPopup != null)
            {
                ExportPopup.IsOpen = true;
            }
        }

        // Handler cho nút Xuất báo cáo trong popup - đóng popup sau khi xuất
        private void ExportConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (ExportPopup != null)
            {
                ExportPopup.IsOpen = false;
            }
        }

    }
}

