using QLKDPhongTro.Presentation.ViewModels;
using System.Windows;

namespace QLKDPhongTro.Presentation.Views.Windows
{
    public partial class PaymentListView : Window
    {
        public PaymentListView()
        {
            InitializeComponent();

            // Sử dụng PaymentViewModel cho danh sách thanh toán
            var viewModel = new PaymentViewModel();
            this.DataContext = viewModel;
        }

        private void DataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }
    }
}