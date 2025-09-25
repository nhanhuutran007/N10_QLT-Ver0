using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;

namespace QLKDPhongTro.Presentation.ViewModels
{
    public partial class LoginViewModel : ViewModelBase
    {
        // Framework sẽ tự động tạo property "Username" từ field "_username"
        // và tự gọi OnPropertyChanged() khi giá trị thay đổi.
        [ObservableProperty]
        private string _username = string.Empty;

        [ObservableProperty]
        private string _password = string.Empty;

        // Framework sẽ tự động tạo một ICommand tên là "LoginCommand"
        // từ phương thức Login() này.
        [RelayCommand]
        private void Login()
        {
            // ===== LOGIC ĐĂNG NHẬP Ở ĐÂY =====
            // Trong tương lai, bạn sẽ gọi một service từ BusinessLayer
            // Ví dụ: var user = _authService.Login(Username, Password);

            // Kiểm tra thông tin đăng nhập
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Hiện tại, chúng ta chỉ hiển thị một thông báo để kiểm tra
            MessageBox.Show($"Tên đăng nhập: {Username}\nMật khẩu: {Password}\n\nĐang thực hiện đăng nhập...", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}