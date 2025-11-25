using System;
using System.Windows;
using QLKDPhongTro.Presentation.ViewModels;
using QLKDPhongTro.Presentation.Utils;

namespace QLKDPhongTro.Presentation.Views.Windows
{
    public partial class UserSecurityWindow : Window
    {
        private bool _isClosing = false;

        public UserSecurityWindow()
        {
            InitializeComponent();

            // ViewModel sẽ được tạo tự động bởi XAML
            // Đăng ký sự kiện Loaded để có thể truy cập ViewModel sau khi nó được tạo
            Loaded += UserSecurityWindow_Loaded;
            
            // Đăng ký sự kiện Closing để đánh dấu cửa sổ đang đóng
            Closing += UserSecurityWindow_Closing;
            // Đăng ký sự kiện Closed để kiểm tra và đóng ứng dụng
            Closed += UserSecurityWindow_Closed;
        }

        private void UserSecurityWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _isClosing = true;
        }

        private void UserSecurityWindow_Closed(object? sender, EventArgs e)
        {
            // Kiểm tra và đóng ứng dụng nếu không còn cửa sổ nào mở
            WindowHelper.CheckAndShutdownIfNoWindows(this);
        }

        private void UserSecurityWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Đăng ký sự kiện từ ViewModel để có thể xóa các ô PasswordBox
            if (DataContext is UserSecurityViewModel viewModel)
            {
                viewModel.PasswordFieldsCleared += OnPasswordFieldsCleared;
            }

            // Gỡ bỏ đăng ký sự kiện khi cửa sổ đóng lại để tránh rò rỉ bộ nhớ
            Unloaded += (s, args) =>
            {
                if (DataContext is UserSecurityViewModel vm)
                {
                    vm.PasswordFieldsCleared -= OnPasswordFieldsCleared;
                }
            };
        }

        // Handler cho TopbarControl events
        private void TopbarControl_MenuButtonClicked(object sender, EventArgs e)
        {
            // Logic chung đã được chuyển vào TopbarControl.xaml.cs
            // Chỉ xử lý logic riêng của UserSecurityWindow nếu cần
        }

        private void TopbarControl_SearchTextChanged(object sender, string searchText)
        {
            // Logic search chung đã được chuyển vào TopbarControl.xaml.cs
            // Chỉ xử lý search logic riêng của UserSecurityWindow nếu cần
        }

        private void TopbarControl_SettingsButtonClicked(object sender, EventArgs e)
        {
            // Logic settings chung đã được chuyển vào TopbarControl.xaml.cs
            // Chỉ xử lý settings logic riêng của UserSecurityWindow nếu cần
        }

        // Handler cho SidebarControl events
        private void SidebarControl_MenuItemClicked(object sender, string menuItem)
        {
            // Logic navigation đã được chuyển vào SidebarControl.xaml.cs
            // Chỉ xử lý logic riêng của UserSecurityWindow nếu cần
        }

        /// <summary>
        /// Xóa nội dung của các ô PasswordBox khi ViewModel yêu cầu.
        /// </summary>
        private void OnPasswordFieldsCleared()
        {
            OldPasswordBox.Clear();
            NewPasswordBox.Clear();
            ConfirmPasswordBox.Clear();
        }

        // --- CÁC PHƯƠNG THỨC NÀY LÀ CẦN THIẾT CHO PASSWORD BOX ---
        // Chúng cập nhật các thuộc tính trong ViewModel mỗi khi người dùng nhập mật khẩu.

        private void OldPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is UserSecurityViewModel viewModel && sender is System.Windows.Controls.PasswordBox passwordBox)
            {
                viewModel.OldPassword = passwordBox.Password;
            }
        }

        private void NewPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is UserSecurityViewModel viewModel && sender is System.Windows.Controls.PasswordBox passwordBox)
            {
                viewModel.NewPassword = passwordBox.Password;
            }
        }

        private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is UserSecurityViewModel viewModel && sender is System.Windows.Controls.PasswordBox passwordBox)
            {
                viewModel.ConfirmPassword = passwordBox.Password;
            }
        }

        // --- KHÔNG CẦN CÁC PHƯƠNG THỨC TextChanged NỮA ---
        // Binding `Mode=TwoWay` trong XAML đã tự động xử lý việc này.
    }
}