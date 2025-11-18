using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QLKDPhongTro.BusinessLayer.Controllers;
using QLKDPhongTro.DataLayer.Repositories;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace QLKDPhongTro.Presentation.ViewModels
{
    public partial class ResetPasswordViewModel : ObservableObject
    {
        private readonly AuthController _authController;
        private readonly string _email;

        public ResetPasswordViewModel(string email)
        {
            _email = email;
            var userRepository = new UserRepository();
            _authController = new AuthController(userRepository);
        }

        [ObservableProperty]
        private string _newPassword = string.Empty;

        [ObservableProperty]
        private string _confirmPassword = string.Empty;

        [ObservableProperty]
        private bool _isSaving = false;

        [RelayCommand]
        private async Task ResetAsync()
        {
            if (string.IsNullOrWhiteSpace(NewPassword) || string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ mật khẩu mới!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (NewPassword != ConfirmPassword)
            {
                MessageBox.Show("Mật khẩu xác nhận không khớp!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                if (IsSaving)
                    return;

                IsSaving = true;

                var result = await _authController.ResetPasswordAsync(_email, NewPassword);
                if (result.IsSuccess)
                {
                    MessageBox.Show(result.Message, "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    CloseCurrentWindow();
                }
                else
                {
                    MessageBox.Show(result.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi đặt lại mật khẩu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsSaving = false;
            }
        }

        private void CloseCurrentWindow()
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.DataContext == this)
                {
                    window.Close();
                    break;
                }
            }
        }
    }
}
