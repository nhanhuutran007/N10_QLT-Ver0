using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Linq;
using QLKDPhongTro.Presentation.ViewModels;

namespace QLKDPhongTro.Presentation.Views.Windows
{
    public partial class LoginWindow : Window
    {
        private bool _isPasswordVisible = false; // Theo dõi trạng thái hiển thị mật khẩu
        private TextBox? _currentPasswordTextBox = null; // Lưu trữ TextBox hiện tại
        
        public LoginWindow()
        {
            InitializeComponent();
            this.DataContext = new LoginViewModel();

            // Đặt cửa sổ ở giữa màn hình
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            // Thêm responsive behavior
            this.SizeChanged += LoginWindow_SizeChanged;

            // Focus vào email field khi load và reset button state
            this.Loaded += (s, e) => 
            {
                EmailTextBox?.Focus();
                ResetLoginButton();
            };
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Không drag khi click vào các nút window hoặc các control khác
            if (e.OriginalSource is Button || e.OriginalSource is TextBox || e.OriginalSource is PasswordBox)
                return;
                
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Không drag khi click vào các nút window hoặc các control khác
            if (e.OriginalSource is Button || e.OriginalSource is TextBox || e.OriginalSource is PasswordBox)
                return;
                
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        // Custom Window Button Event Handlers
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MaximizeRestore_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
                UpdateMaximizeButtonIcon(false);
            }
            else
            {
                this.WindowState = WindowState.Maximized;
                UpdateMaximizeButtonIcon(true);
            }
        }

        private void UpdateMaximizeButtonIcon(bool isMaximized)
        {
            // Không cần cập nhật icon vì đã dùng nút hệ thống
            // Method này giữ lại để tương thích nhưng không làm gì
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        // Xử lý responsive design
        private void LoginWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Ẩn phần bên trái khi cửa sổ quá nhỏ
            if (e.NewSize.Width < 800)
            {
                // Tìm Grid cột trái thông qua tên cột
                var leftColumn = this.FindName("LeftBorder") as FrameworkElement;
                if (leftColumn != null)
                    leftColumn.Visibility = Visibility.Collapsed;
            }
            else
            {
                var leftColumn = this.FindName("LeftBorder") as FrameworkElement;
                if (leftColumn != null)
                    leftColumn.Visibility = Visibility.Visible;
            }
        }


        // Xử lý đăng nhập
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!(DataContext is LoginViewModel viewModel))
                {
                    MessageBox.Show("Lỗi khởi tạo ViewModel!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Kiểm tra đang loading để tránh multiple clicks
                if (viewModel.IsLoading)
                    return;

                // Disable button ngay lập tức
                LoginButton.IsEnabled = false;

                // Cập nhật ViewModel với dữ liệu từ UI
                viewModel.Username = EmailTextBox?.Text?.Trim() ?? string.Empty;
                viewModel.Password = GetCurrentPassword();

                // Gọi login command từ ViewModel (ViewModel sẽ tự validate)
                viewModel.LoginCommand?.Execute(null);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi đăng nhập: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                // Re-enable button nếu có lỗi
                LoginButton.IsEnabled = true;
            }
        }

        // Reset login button state
        public void ResetLoginButton()
        {
            try
            {
                if (LoginButton != null)
                {
                    LoginButton.IsEnabled = true;
                }

                if (DataContext is LoginViewModel viewModel)
                {
                    viewModel.IsLoading = false;
                }
            }
            catch (Exception ex)
            {
                // Log error nếu cần
                System.Diagnostics.Debug.WriteLine($"Error resetting login button: {ex.Message}");
            }
        }

        // Lấy password hiện tại từ PasswordBox hoặc TextBox
        private string GetCurrentPassword()
        {
            if (_isPasswordVisible && _currentPasswordTextBox != null)
            {
                return _currentPasswordTextBox.Text;
            }
            
            // Tìm PasswordBox hiện tại trong UI
            var passwordBox = this.FindName("PasswordBox") as PasswordBox;
            return passwordBox?.Password ?? string.Empty;
        }

        // Xử lý tạo tài khoản mới
        private void CreateAccount_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var registerWindow = new RegisterWindow
                {
                    DataContext = new RegisterViewModel()
                };
                // Đóng cửa sổ login hiện tại
                Application.Current.MainWindow?.Close();
                Application.Current.MainWindow = registerWindow;
                registerWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở màn hình đăng ký: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        // Xử lý quên mật khẩu
        private void ForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var popup = new ForgotPasswordEmailWindow
                {
                    Owner = this,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };

                popup.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở chức năng quên mật khẩu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        // Xử lý thay đổi password
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                // Cập nhật ViewModel với password mới
                if (DataContext is LoginViewModel viewModel)
                {
                    viewModel.Password = passwordBox.Password;
                }
            }
        }

        // Xử lý hiện/ẩn mật khẩu - Logic đơn giản và hiệu quả
        private void ShowPasswordCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            ShowPassword();
        }

        private void ShowPasswordCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            HidePassword();
        }

        private void ShowPassword()
        {
            try
            {
                if (_isPasswordVisible) return;

                // Tìm PasswordBox hiện tại trong UI (có thể đã được tạo lại)
                var currentPasswordBox = FindPasswordBox();
                if (currentPasswordBox == null) return;

                // Tìm StackPanel chứa PasswordBox
                var stackPanel = FindParent<StackPanel>(currentPasswordBox);
                if (stackPanel == null) return;

                var index = stackPanel.Children.IndexOf(currentPasswordBox);
                if (index == -1) return;

                // Tạo TextBox để hiển thị mật khẩu
                var textBox = new TextBox
                {
                    Text = currentPasswordBox.Password,
                    Style = (Style)this.FindResource("InputTextBoxStyle"),
                    Margin = currentPasswordBox.Margin,
                    Name = "PasswordTextBox",
                    VerticalContentAlignment = currentPasswordBox.VerticalContentAlignment
                };
                
                // Thêm event handler cho KeyDown
                textBox.KeyDown += PasswordTextBox_KeyDown;

                // Thay thế PasswordBox bằng TextBox
                stackPanel.Children.Remove(currentPasswordBox);
                stackPanel.Children.Insert(index, textBox);
                
                _currentPasswordTextBox = textBox;
                _isPasswordVisible = true;
                
                // Focus vào TextBox mới
                textBox.Focus();
                textBox.CaretIndex = textBox.Text.Length;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi hiển thị mật khẩu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void HidePassword()
        {
            try
            {
                if (!_isPasswordVisible) return;

                // Tìm TextBox hiện tại trong UI
                var currentTextBox = FindPasswordTextBox();
                if (currentTextBox == null) return;

                // Tìm StackPanel chứa TextBox
                var stackPanel = FindParent<StackPanel>(currentTextBox);
                if (stackPanel == null) return;

                var index = stackPanel.Children.IndexOf(currentTextBox);
                if (index == -1) return;

                // Tạo lại PasswordBox với mật khẩu từ TextBox
                var passwordBox = new PasswordBox
                {
                    Password = currentTextBox.Text,
                    Style = (Style)this.FindResource("InputPasswordBoxStyle"),
                    Margin = currentTextBox.Margin,
                    Name = "PasswordBox",
                    VerticalContentAlignment = currentTextBox.VerticalContentAlignment
                };
                
                // Thêm event handler cho KeyDown
                passwordBox.KeyDown += PasswordBox_KeyDown;

                // Thay thế TextBox bằng PasswordBox
                stackPanel.Children.Remove(currentTextBox);
                stackPanel.Children.Insert(index, passwordBox);
                
                _currentPasswordTextBox = null;
                _isPasswordVisible = false;
                
                // Focus vào PasswordBox mới
                passwordBox.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi ẩn mật khẩu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Helper methods để tìm control một cách đáng tin cậy
        private PasswordBox? FindPasswordBox()
        {
            // Tìm PasswordBox trong toàn bộ visual tree
            return FindVisualChild<PasswordBox>(this);
        }

        private TextBox? FindPasswordTextBox()
        {
            // Tìm TextBox có tên "PasswordTextBox" trong toàn bộ visual tree
            return FindVisualChild<TextBox>(this, tb => tb.Name == "PasswordTextBox");
        }

        // Generic method để tìm visual child
        private T? FindVisualChild<T>(DependencyObject parent, Func<T, bool>? predicate = null) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T result)
                {
                    if (predicate == null || predicate(result))
                        return result;
                }
                
                var childOfChild = FindVisualChild<T>(child, predicate);
                if (childOfChild != null)
                    return childOfChild;
            }
            return null;
        }

        // Helper method để tìm parent control
        private T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);
            
            if (parentObject == null) return null;
            
            if (parentObject is T parent)
                return parent;
            else
                return FindParent<T>(parentObject);
        }

        // Xử lý Enter trong TextBox mật khẩu
        private void PasswordTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                LoginButton_Click(sender, e);
            }
        }

        // Xử lý nhấn Enter trong EmailTextBox
        private void EmailTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // Chuyển focus sang PasswordBox
                PasswordBox.Focus();
            }
        }

        // Xử lý nhấn Enter trong PasswordBox
        private void PasswordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // Thực hiện đăng nhập
                LoginButton_Click(sender, e);
            }
        }
    }
}