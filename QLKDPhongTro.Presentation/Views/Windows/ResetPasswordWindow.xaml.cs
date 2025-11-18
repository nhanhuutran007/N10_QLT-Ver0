using System.Windows;
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

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.NewPassword = NewPasswordBox.Password;
            _viewModel.ConfirmPassword = ConfirmPasswordBox.Password;
            _viewModel.ResetCommand.Execute(null);
        }
    }
}
