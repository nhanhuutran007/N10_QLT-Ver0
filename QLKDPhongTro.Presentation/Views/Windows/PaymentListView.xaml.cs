using System.Windows;
using QLKDPhongTro.Presentation.ViewModels;

namespace QLKDPhongTro.Presentation.Views.Windows
{
    public partial class PaymentListView : Window
    {
        public PaymentListView()
        {
            InitializeComponent();
            DataContext = new PaymentViewModel();
        }
    }
}
