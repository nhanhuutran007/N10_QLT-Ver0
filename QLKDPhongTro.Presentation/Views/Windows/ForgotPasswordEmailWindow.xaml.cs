using System.Windows;
using System.Windows.Controls; // <--- Cần thêm dòng này để sử dụng Button và TextBox
using System.Windows.Input;
using QLKDPhongTro.Presentation.ViewModels;

namespace QLKDPhongTro.Presentation.Views.Windows
{
    public partial class ForgotPasswordEmailWindow : Window
    {
        public ForgotPasswordEmailWindow()
        {
            InitializeComponent();
            DataContext = new ForgotPasswordEmailViewModel();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Trình biên dịch giờ đã hiểu Button và TextBox là gì nhờ namespace System.Windows.Controls
            if (e.OriginalSource is Button || e.OriginalSource is TextBox)
                return;

            if (e.ButtonState == MouseButtonState.Pressed)
            {
                DragMove();
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
            catch (System.Exception ex)
            {
                MessageBox.Show($"Lỗi khi chuyển về trang đăng nhập: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}