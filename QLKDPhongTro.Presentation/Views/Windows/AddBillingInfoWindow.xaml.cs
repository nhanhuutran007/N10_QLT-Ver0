using QLKDPhongTro.Presentation.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace QLKDPhongTro.Presentation.Views.Windows
{
    public partial class AddBillingInfoWindow : Window
    {
        public AddBillingInfoWindow()
        {
            InitializeComponent();

            var viewModel = new PaymentFormViewModel();

            // Lắng nghe sự kiện lưu thành công từ ViewModel
            viewModel.PaymentSaved += (s, e) =>
            {
                this.DialogResult = true; // Trả về true cho cửa sổ cha (List View)
                this.Close();
            };

            this.DataContext = viewModel;
        }

        // Xử lý kéo cửa sổ (vì WindowStyle="None")
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
                this.DragMove();
        }
    }
}