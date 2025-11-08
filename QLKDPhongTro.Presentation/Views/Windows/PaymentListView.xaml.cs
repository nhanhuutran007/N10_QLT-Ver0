using System.Windows;
using QLKDPhongTro.Presentation.ViewModels;
using QLKDPhongTro.Presentation.Views.Windows;

namespace QLKDPhongTro.Presentation.Views.Windows
{
    public partial class PaymentListView : Window
    {
        public PaymentListView()
        {
            InitializeComponent();
            DataContext = new PaymentViewModel();
        }

        private async void AddPaymentButton_Click(object sender, RoutedEventArgs e)
        {
            var win = new AddBillingInfoWindow
            {
                Owner = this
            };
            var result = win.ShowDialog();
            if (result == true && DataContext is PaymentViewModel vm)
            {
                await vm.LoadDataAsync();
            }
        }
    }
}
