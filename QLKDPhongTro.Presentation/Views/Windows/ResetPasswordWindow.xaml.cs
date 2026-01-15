using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls; // <--- Cần thêm dòng này để dùng Button, TextBox, PasswordBox
using System.Windows.Input;
using System.Windows.Media;    // <--- Cần thêm dòng này để dùng Brushes (Brushes.Green, Brushes.Red)
using QLKDPhongTro.Presentation.ViewModels;

namespace QLKDPhongTro.Presentation.Views.Windows
{
    public partial class ResetPasswordWindow : Window
    {
        private readonly ResetPasswordViewModel _viewModel;

        public ResetPasswordWindow(string email)
        {
            InitializeComponent();
            _viewModel = new ResetPasswordViewModel(email);
            DataContext = _viewModel;
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Bây giờ trình biên dịch đã hiểu Button, TextBox, PasswordBox là gì
            if (e.OriginalSource is Button || e.OriginalSource is TextBox || e.OriginalSource is PasswordBox)
                return;

            if (e.ButtonState == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (NewPasswordBox != null && ConfirmPasswordBox != null)
            {
                _viewModel.NewPassword = NewPasswordBox.Password;
                _viewModel.ConfirmPassword = ConfirmPasswordBox.Password;
                if (_viewModel.ResetCommand.CanExecute(null))
                {
                    _viewModel.ResetCommand.Execute(null);
                }
            }
        }

        private void NewPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
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
                if (NewPasswordBox != null)
                {
                    UpdatePasswordRequirements(NewPasswordBox.Password);
                }
            }
        }

        private void UpdatePasswordRequirements(string password)
        {
            // Lưu ý: Trong XAML bạn chưa đặt x:Name cho các TextBlock này trong code mẫu bạn gửi.
            // Hãy đảm bảo XAML có các TextBlock tên: PasswordLengthRequirement, PasswordUppercaseRequirement, PasswordDigitRequirement
            var lengthRequirement = this.FindName("PasswordLengthRequirement") as TextBlock;
            var uppercaseRequirement = this.FindName("PasswordUppercaseRequirement") as TextBlock;
            var digitRequirement = this.FindName("PasswordDigitRequirement") as TextBlock;

            if (lengthRequirement != null)
            {
                if (password.Length >= 6)
                    lengthRequirement.Foreground = Brushes.Green;
                else
                    lengthRequirement.Foreground = Brushes.Red;
            }

            if (uppercaseRequirement != null)
            {
                bool hasUppercase = password.Any(c => char.IsUpper(c));
                if (hasUppercase)
                    uppercaseRequirement.Foreground = Brushes.Green;
                else
                    uppercaseRequirement.Foreground = Brushes.Red;
            }

            if (digitRequirement != null)
            {
                bool hasDigit = password.Any(c => char.IsDigit(c));
                if (hasDigit)
                    digitRequirement.Foreground = Brushes.Green;
                else
                    digitRequirement.Foreground = Brushes.Red;
            }
        }

        private void BackToLogin_Click(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var loginWindow = new LoginWindow();
                loginWindow.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi chuyển về trang đăng nhập: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}