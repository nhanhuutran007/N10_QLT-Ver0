using System.Windows;

namespace QLKDPhongTro.Presentation.Views.Windows
{
    public partial class InvoiceDetailView : Window
    {
        public InvoiceDetailView()
        {
            InitializeComponent();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
