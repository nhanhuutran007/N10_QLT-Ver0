using System;
using System.Linq;
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
            // Không thực hiện navigation nếu đang trong quá trình khởi tạo
            if (_isInitializing)
                return;
                
            var currentWindow = Window.GetWindow(this);
            
            // Kiểm tra currentWindow có null không
            if (currentWindow == null)
            {
                // Nếu không tìm thấy window hiện tại, chỉ tạo window mới
                var newWindow = new T();
                newWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                newWindow.Show();
                return;
            }
            
            // Nếu đã ở window cùng loại thì không cần chuyển
            if (currentWindow is T)
                return;
            
            // Kiểm tra xem window đã tồn tại chưa
            var existingWindow = Application.Current.Windows.OfType<T>().FirstOrDefault();
            
            if (existingWindow != null)
            {
                existingWindow.Activate();
                existingWindow.WindowState = WindowState.Normal;
            }
            else
            {
                var newWindow = new T();
                newWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                newWindow.Show();
            }
            
            // Đóng cửa sổ hiện tại (chỉ khi currentWindow không null)
            currentWindow.Close();
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

        private void Finance_Checked(object sender, RoutedEventArgs e)
        {
            // Không thực hiện navigation nếu đang trong quá trình khởi tạo
            if (_isInitializing)
                return;

            NavigateToWindow<FinancialWindow>();
        }
    }
}
