using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using QLKDPhongTro.Presentation.ViewModels;

namespace QLKDPhongTro.Presentation.Views.Windows
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            this.DataContext = new LoginViewModel();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        // THÊM 3 PHƯƠNG THỨC NÀY VÀO
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        // Xử lý hiện/ẩn mật khẩu
        private void ShowPasswordCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            // Tìm PasswordBox hiện tại
            var passwordBox = this.FindName("PasswordBox") as PasswordBox;
            if (passwordBox != null)
            {
                // Tạo TextBox để hiển thị mật khẩu
                var textBox = new TextBox
                {
                    Text = passwordBox.Password,
                    Background = passwordBox.Background,
                    BorderBrush = passwordBox.BorderBrush,
                    BorderThickness = passwordBox.BorderThickness,
                    Padding = passwordBox.Padding,
                    FontSize = passwordBox.FontSize,
                    Margin = passwordBox.Margin
                };
                
                // Thay thế PasswordBox bằng TextBox
                var parent = passwordBox.Parent as Panel;
                if (parent != null)
                {
                    var index = parent.Children.IndexOf(passwordBox);
                    parent.Children.Remove(passwordBox);
                    parent.Children.Insert(index, textBox);
                    textBox.Name = "PasswordTextBox";
                    textBox.Tag = passwordBox.Tag;
                }
            }
        }

        private void ShowPasswordCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            // Tìm TextBox hiện tại
            var textBox = this.FindName("PasswordTextBox") as TextBox;
            if (textBox != null)
            {
                // Tạo lại PasswordBox
                var passwordBox = new PasswordBox
                {
                    Password = textBox.Text,
                    Background = textBox.Background,
                    BorderBrush = textBox.BorderBrush,
                    BorderThickness = textBox.BorderThickness,
                    Padding = textBox.Padding,
                    FontSize = textBox.FontSize,
                    Margin = textBox.Margin
                };
                
                // Thay thế TextBox bằng PasswordBox
                var parent = textBox.Parent as Panel;
                if (parent != null)
                {
                    var index = parent.Children.IndexOf(textBox);
                    parent.Children.Remove(textBox);
                    parent.Children.Insert(index, passwordBox);
                    passwordBox.Name = "PasswordBox";
                    passwordBox.Tag = textBox.Tag;
                }
            }
        }

        // Xử lý đăng nhập - đã được chuyển sang ViewModel

        // Xử lý quên mật khẩu
        private void ForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Tính năng quên mật khẩu sẽ được triển khai trong phiên bản tiếp theo!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Xử lý đăng ký - đã được chuyển sang ViewModel

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