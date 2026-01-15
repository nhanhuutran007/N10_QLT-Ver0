using QLKDPhongTro.Presentation.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace QLKDPhongTro.Presentation.Views.Windows
{
    public partial class EditPaymentDialog : Window
    {
        private readonly EditPaymentViewModel _viewModel;

        public EditPaymentDialog(int maThanhToan)
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi khởi tạo UI: {ex.Message}\n\nChi tiết: {ex.StackTrace}", 
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
                return;
            }

            try
            {
                _viewModel = new EditPaymentViewModel(maThanhToan);
                DataContext = _viewModel;

                _viewModel.CloseRequested += (s, result) =>
                {
                    DialogResult = result;
                    Close();
                };

                _viewModel.ShowMessageRequested += (s, msg) =>
                {
                    MessageBox.Show(msg, "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi khởi tạo ViewModel: {ex.Message}\n\nChi tiết: {ex.StackTrace}", 
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
                DragMove();
        }
    }
}

