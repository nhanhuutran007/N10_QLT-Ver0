using System.Windows;
using QLKDPhongTro.BusinessLayer.DTOs;

namespace QLKDPhongTro.Presentation.Views.Windows
{
    public partial class InvoiceDetailView : Window
    {
        public InvoiceDetailDto? InvoiceData
        {
            get => (InvoiceDetailDto?)DataContext;
            set => DataContext = value;
        }

        public InvoiceDetailView()
        {
            InitializeComponent();
        }

        public InvoiceDetailView(InvoiceDetailDto invoiceData) : this()
        {
            InvoiceData = invoiceData;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
