using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Linq;
using QLKDPhongTro.Presentation.ViewModels;

namespace QLKDPhongTro.Presentation.Views.Windows
{
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
            this.DataContext = new RegisterViewModel();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
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
            var confirmPasswordBox = this.FindName("ConfirmPasswordBox") as PasswordBox;
            
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
                textBox.TextChanged += PasswordTextBox_TextChanged;
                
                // Thay thế PasswordBox bằng TextBox
                var parent = passwordBox.Parent as Panel;
                if (parent != null)
                {
                    var index = parent.Children.IndexOf(passwordBox);
                    parent.Children.Remove(passwordBox);
                    parent.Children.Insert(index, textBox);
                    textBox.Name = "PasswordTextBox";
                }
            }

            if (confirmPasswordBox != null)
            {
                // Tạo TextBox để hiển thị mật khẩu xác nhận
                var confirmTextBox = new TextBox
                {
                    Text = confirmPasswordBox.Password,
                    Background = confirmPasswordBox.Background,
                    BorderBrush = confirmPasswordBox.BorderBrush,
                    BorderThickness = confirmPasswordBox.BorderThickness,
                    Padding = confirmPasswordBox.Padding,
                    FontSize = confirmPasswordBox.FontSize,
                    Margin = confirmPasswordBox.Margin
                };
                confirmTextBox.TextChanged += PasswordTextBox_TextChanged;
                
                // Thay thế ConfirmPasswordBox bằng TextBox
                var parent = confirmPasswordBox.Parent as Panel;
                if (parent != null)
                {
                    var index = parent.Children.IndexOf(confirmPasswordBox);
                    parent.Children.Remove(confirmPasswordBox);
                    parent.Children.Insert(index, confirmTextBox);
                    confirmTextBox.Name = "ConfirmPasswordTextBox";
                }
            }
        }

        private void ShowPasswordCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            // Tìm TextBox hiện tại
            var textBox = this.FindName("PasswordTextBox") as TextBox;
            var confirmTextBox = this.FindName("ConfirmPasswordTextBox") as TextBox;
            
            if (textBox != null)
            {
                // Tạo lại PasswordBox với mật khẩu từ TextBox
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
                passwordBox.PasswordChanged += PasswordBox_PasswordChanged;
                
                // Thay thế TextBox bằng PasswordBox
                var parent = textBox.Parent as Panel;
                if (parent != null)
                {
                    var index = parent.Children.IndexOf(textBox);
                    parent.Children.Remove(textBox);
                    parent.Children.Insert(index, passwordBox);
                    passwordBox.Name = "PasswordBox";
                }
            }

            if (confirmTextBox != null)
            {
                // Tạo lại ConfirmPasswordBox với mật khẩu từ TextBox
                var confirmPasswordBox = new PasswordBox
                {
                    Password = confirmTextBox.Text,
                    Background = confirmTextBox.Background,
                    BorderBrush = confirmTextBox.BorderBrush,
                    BorderThickness = confirmTextBox.BorderThickness,
                    Padding = confirmTextBox.Padding,
                    FontSize = confirmTextBox.FontSize,
                    Margin = confirmTextBox.Margin
                };
                confirmPasswordBox.PasswordChanged += ConfirmPasswordBox_PasswordChanged;
                
                // Thay thế TextBox bằng PasswordBox
                var parent = confirmTextBox.Parent as Panel;
                if (parent != null)
                {
                    var index = parent.Children.IndexOf(confirmTextBox);
                    parent.Children.Remove(confirmTextBox);
                    parent.Children.Insert(index, confirmPasswordBox);
                    confirmPasswordBox.Name = "ConfirmPasswordBox";
                }
            }
        }

        // Xử lý đăng ký
        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            // Lấy thông tin từ các trường
            var emailTextBox = this.FindName("EmailTextBox") as TextBox;
            var usernameTextBox = this.FindName("UsernameTextBox") as TextBox;
            
            string email = emailTextBox?.Text ?? "";
            string username = usernameTextBox?.Text ?? "";
            
            string password = "";
            string confirmPassword = "";
            
            // Lấy mật khẩu từ PasswordBox hoặc TextBox
            var passwordBox = this.FindName("PasswordBox") as PasswordBox;
            var passwordTextBox = this.FindName("PasswordTextBox") as TextBox;
            var confirmPasswordBox = this.FindName("ConfirmPasswordBox") as PasswordBox;
            var confirmPasswordTextBox = this.FindName("ConfirmPasswordTextBox") as TextBox;
            
            if (passwordBox != null)
                password = passwordBox.Password;
            else if (passwordTextBox != null)
                password = passwordTextBox.Text;

            if (confirmPasswordBox != null)
                confirmPassword = confirmPasswordBox.Password;
            else if (confirmPasswordTextBox != null)
                confirmPassword = confirmPasswordTextBox.Text;

            // Kiểm tra thông tin đăng ký
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(username) || 
                string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ tất cả thông tin!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Kiểm tra định dạng email
            if (!email.Contains("@") || !email.Contains("."))
            {
                MessageBox.Show("Vui lòng nhập địa chỉ email hợp lệ!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Kiểm tra mật khẩu khớp nhau
            if (password != confirmPassword)
            {
                MessageBox.Show("Mật khẩu và xác nhận mật khẩu không khớp!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Kiểm tra điều kiện mật khẩu
            if (!ValidatePassword(password))
            {
                MessageBox.Show("Mật khẩu không đáp ứng các điều kiện yêu cầu!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Hiển thị thông báo đăng ký
            MessageBox.Show($"Email: {email}\nTên đăng nhập: {username}\nMật khẩu: {password}\n\nĐang tạo tài khoản...", 
                           "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Xử lý thay đổi mật khẩu
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            if (passwordBox != null)
            {
                UpdatePasswordRequirements(passwordBox.Password);
            }
        }

        private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var confirmPasswordBox = sender as PasswordBox;
            if (confirmPasswordBox != null)
            {
                // Cũng cập nhật validation cho mật khẩu chính
                var passwordBox = this.FindName("PasswordBox") as PasswordBox;
                if (passwordBox != null)
                {
                    UpdatePasswordRequirements(passwordBox.Password);
                }
            }
        }

        private void PasswordTextBox_TextChanged(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null)
            {
                UpdatePasswordRequirements(textBox.Text);
            }
        }

        // Cập nhật hiển thị điều kiện mật khẩu
        private void UpdatePasswordRequirements(string password)
        {
            var lengthRequirement = this.FindName("PasswordLengthRequirement") as TextBlock;
            var uppercaseRequirement = this.FindName("PasswordUppercaseRequirement") as TextBlock;
            var digitRequirement = this.FindName("PasswordDigitRequirement") as TextBlock;

            if (lengthRequirement != null)
            {
                if (password.Length >= 6)
                {
                    lengthRequirement.Foreground = System.Windows.Media.Brushes.Green;
                }
                else
                {
                    lengthRequirement.Foreground = System.Windows.Media.Brushes.Red;
                }
            }

            if (uppercaseRequirement != null)
            {
                bool hasUppercase = password.Any(c => char.IsUpper(c));
                if (hasUppercase)
                {
                    uppercaseRequirement.Foreground = System.Windows.Media.Brushes.Green;
                }
                else
                {
                    uppercaseRequirement.Foreground = System.Windows.Media.Brushes.Red;
                }
            }

            if (digitRequirement != null)
            {
                bool hasDigit = password.Any(c => char.IsDigit(c));
                if (hasDigit)
                {
                    digitRequirement.Foreground = System.Windows.Media.Brushes.Green;
                }
                else
                {
                    digitRequirement.Foreground = System.Windows.Media.Brushes.Red;
                }
            }
        }

        // Kiểm tra điều kiện mật khẩu
        private bool ValidatePassword(string password)
        {
            // Kiểm tra độ dài tối thiểu 6 ký tự
            if (password.Length < 6)
                return false;

            // Kiểm tra có ít nhất 1 ký tự viết hoa
            bool hasUppercase = password.Any(c => char.IsUpper(c));
            
            // Kiểm tra có ít nhất 1 ký tự số
            bool hasDigit = password.Any(c => char.IsDigit(c));

            // Phải có ít nhất 1 trong 2 điều kiện: viết hoa HOẶC số
            return hasUppercase || hasDigit;
        }

        // Xử lý quay lại đăng nhập
        private void BackToLogin_Click(object sender, RoutedEventArgs e)
        {
            // Tạo và hiển thị cửa sổ đăng nhập
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            
            // Đóng cửa sổ đăng ký hiện tại
            this.Close();
        }
    }
}
