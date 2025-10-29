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
                    Dispatcher.Invoke(() => _isInitializing = false);
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
            // TODO: Implement logic để highlight menu item được chọn
            // Có thể thêm logic để thay đổi background của các button
        }


        // Centralized navigation helper method
        private void NavigateToWindow<T>() where T : Window, new()
        {
            if (_isInitializing)
                return;

            var currentWindow = Window.GetWindow(this);
            if (currentWindow == null)
                return;

            // Nếu là cùng loại window thì bỏ qua
            if (currentWindow is T)
                return;

            if (typeof(T) == typeof(ContractManagementWindow))
            {
                // Ẩn dashboard
                currentWindow.Hide();

                var contractWindow = new ContractManagementWindow();
                contractWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;

                // Khi tắt hợp đồng → show lại dashboard
                contractWindow.Closed += (s, e) =>
                {
                    currentWindow.Show();
                    currentWindow.Activate();
                };

                contractWindow.Show();
            }
            else if (typeof(T) == typeof(PaymentListView))
            {
                // Ẩn dashboard
                currentWindow.Hide();

                var paymentWindow = new PaymentListView();
                paymentWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;

                // Khi tắt payment list → show lại dashboard
                paymentWindow.Closed += (s, e) =>
                {
                    currentWindow.Show();
                    currentWindow.Activate();
                };

                paymentWindow.Show();
            }
            else
            {
                // Các cửa sổ khác vẫn đóng dashboard cũ
                var newWindow = new T();
                newWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                newWindow.Show();
                currentWindow.Close();
            }
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
    }
}
