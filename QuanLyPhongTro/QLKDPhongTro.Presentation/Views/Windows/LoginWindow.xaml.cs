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

        // Xử lý đăng nhập
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            // Lấy tên đăng nhập
            var usernameTextBox = this.FindName("UsernameTextBox") as TextBox;
            string username = usernameTextBox?.Text ?? "";
            
            string password = "";
            
            // Lấy mật khẩu từ PasswordBox hoặc TextBox
            var passwordBox = this.FindName("PasswordBox") as PasswordBox;
            var passwordTextBox = this.FindName("PasswordTextBox") as TextBox;
            
            if (passwordBox != null)
                password = passwordBox.Password;
            else if (passwordTextBox != null)
                password = passwordTextBox.Text;

            // Kiểm tra thông tin đăng nhập
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Hiển thị thông báo đăng nhập
            MessageBox.Show($"Tên đăng nhập: {username}\nMật khẩu: {password}\n\nĐang thực hiện đăng nhập...", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Xử lý quên mật khẩu
        private void ForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Tính năng quên mật khẩu sẽ được triển khai trong phiên bản tiếp theo!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Xử lý đăng ký
        private void Register_Click(object sender, RoutedEventArgs e)
        {
            // Tạo và hiển thị cửa sổ đăng ký
            var registerWindow = new RegisterWindow();
            registerWindow.Show();
            
            // Đóng cửa sổ đăng nhập hiện tại
            this.Close();
        }
    }
}