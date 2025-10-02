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
            if (MaximizeButton != null)
            {
                var path = MaximizeButton.Content as Path;
                if (path != null)
                {
                    if (isMaximized)
                    {
                        // Restore icon (two overlapping squares)
                        path.Data = Geometry.Parse("M4,4 L16,4 L16,16 L4,16 Z M6,6 L14,6 L14,14 L6,14 Z");
                    }
                    else
                    {
                        // Maximize icon (single square)
                        path.Data = Geometry.Parse("M6,6 L18,6 L18,18 L6,18 Z");
                    }
                }
            }
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
                if (LeftBorder != null)
                    LeftBorder.Visibility = Visibility.Collapsed;
            }
            else
            {
                if (LeftBorder != null)
                    LeftBorder.Visibility = Visibility.Visible;
            }
        }

        // Xử lý toggle password visibility
        private void TogglePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (PasswordBox == null) return;

                var parent = PasswordBox.Parent as Panel;
                if (parent == null) return;

                var index = parent.Children.IndexOf(PasswordBox);
                if (index == -1) return;

                // Kiểm tra xem hiện tại có phải PasswordBox hay TextBox
                var currentTextBox = parent.Children.OfType<TextBox>().FirstOrDefault(tb => tb.Name == "PasswordTextBox");

                if (currentTextBox == null)
                {
                    // Hiện tại là PasswordBox, chuyển sang TextBox
                    var textBox = new TextBox
                    {
                        Text = PasswordBox.Password,
                        Background = PasswordBox.Background,
                        BorderThickness = PasswordBox.BorderThickness,
                        Padding = PasswordBox.Padding,
                        FontSize = PasswordBox.FontSize,
                        Margin = PasswordBox.Margin,
                        Name = "PasswordTextBox",
                        VerticalContentAlignment = PasswordBox.VerticalContentAlignment
                    };

                    parent.Children.Remove(PasswordBox);
                    parent.Children.Insert(index, textBox);

                    // Update button icon
                    UpdateToggleButtonIcon(true);
                }
                else
                {
                    // Hiện tại là TextBox, chuyển về PasswordBox
                    PasswordBox.Password = currentTextBox.Text;
                    parent.Children.Remove(currentTextBox);
                    parent.Children.Insert(index, PasswordBox);

                    // Update button icon
                    UpdateToggleButtonIcon(false);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thay đổi hiển thị mật khẩu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateToggleButtonIcon(bool isPasswordVisible)
        {
            if (TogglePasswordButton != null)
            {
                var path = TogglePasswordButton.Content as Path;
                if (path != null)
                {
                    if (isPasswordVisible)
                    {
                        // Eye with slash icon
                        path.Data = Geometry.Parse("M12 4.5C7 4.5 2.73 7.61 1 12c1.73 4.39 6 7.5 11 7.5s9.27-3.11 11-7.5c-1.73-4.39-6-7.5-11-7.5zM12 17c-2.76 0-5-2.24-5-5s2.24-5 5-5 5 2.24 5 5-2.24 5-5 5zm0-8c-1.66 0-3 1.34-3 3s1.34 3 3 3 3-1.34 3-3-1.34-3-3-3z");
                    }
                    else
                    {
                        // Regular eye icon
                        path.Data = Geometry.Parse("M12 5C7 5 2.73 7.11 1 10.5C2.73 13.89 7 16 12 16C17 16 21.27 13.89 23 10.5C21.27 7.11 17 5 12 5ZM12 13.5C10.62 13.5 9.5 12.38 9.5 11C9.5 9.62 10.62 8.5 12 8.5C13.38 8.5 14.5 9.62 14.5 11C14.5 12.38 13.38 13.5 12 13.5ZM12 7C13.66 7 15 8.34 15 10C15 11.66 13.66 13 12 13C10.34 13 9 11.66 9 10C9 8.34 10.34 7 12 7Z");
                    }
                }
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
            if (PasswordBox != null && PasswordBox.Visibility == Visibility.Visible)
            {
                return PasswordBox.Password;
            }

            var passwordTextBox = this.FindName("PasswordTextBox") as TextBox;
            return passwordTextBox?.Text ?? string.Empty;
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
                registerWindow.Show();

                // Đóng cửa sổ login hiện tại
                Application.Current.MainWindow?.Close();
                Application.Current.MainWindow = registerWindow;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở màn hình đăng ký: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        // Xử lý quên mật khẩu
        private void ForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Tính năng quên mật khẩu sẽ được triển khai trong phiên bản tiếp theo!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
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
    }
}