using System.Windows;
using System.Windows.Controls;
using QLKDPhongTro.Presentation.ViewModels;
using QLKDPhongTro.DataLayer.Repositories;

namespace QLKDPhongTro.Presentation.Views.Windows
{
    public partial class OtpLoginWindow : Window
    {
        public OtpLoginWindow(string username, string email, string password)
        {
            InitializeComponent();

            // Khởi tạo UserRepository và ViewModel dùng lại logic OTP hiện có
            var userRepository = new UserRepository();
            DataContext = new OtpViewModel(userRepository, username, email, password);
        }

        // Constructor cho các luồng OTP khác (ví dụ: quên mật khẩu)
        public OtpLoginWindow(IOtpEntryViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void OtpBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (DataContext is not IOtpEntryViewModel vm)
                return;

            // Chỉ giữ 1 ký tự trong mỗi ô
            if (sender is TextBox textBox)
            {
                if (!string.IsNullOrEmpty(textBox.Text) && textBox.Text.Length > 1)
                {
                    textBox.Text = textBox.Text[^1].ToString();
                    textBox.CaretIndex = textBox.Text.Length;
                }

                // Tự động chuyển sang ô tiếp theo khi đã nhập 1 số
                if (textBox.Text.Length == 1)
                {
                    if (textBox == OtpBox1) OtpBox2.Focus();
                    else if (textBox == OtpBox2) OtpBox3.Focus();
                    else if (textBox == OtpBox3) OtpBox4.Focus();
                    else if (textBox == OtpBox4) OtpBox5.Focus();
                    else if (textBox == OtpBox5) OtpBox6.Focus();
                }
            }

            // Ghép 6 ô thành chuỗi OTP
            var code = (OtpBox1.Text + OtpBox2.Text + OtpBox3.Text +
                        OtpBox4.Text + OtpBox5.Text + OtpBox6.Text).Trim();

            vm.OtpCode = code;

            // Tự động submit khi đủ 6 ký tự
            if (code.Length == 6 && vm.VerifyOtpCommand != null && vm.VerifyOtpCommand.CanExecute(null))
            {
                vm.VerifyOtpCommand.Execute(null);
            }
        }

        private void BackToLogin_Click(object sender, RoutedEventArgs e)
        {
            // Mở lại màn hình đăng nhập
            var loginWindow = new LoginWindow
            {
                DataContext = new LoginViewModel(),
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };

            loginWindow.Show();
            Application.Current.MainWindow = loginWindow;
            Close();
        }
    }
}
