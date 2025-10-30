using System.Windows;

namespace QLKDPhongTro.Presentation.Views.Windows
{
    public partial class FinancialWindow : Window
    {
        public FinancialWindow()
        {
            InitializeComponent();
        }

        private void ScanImageButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ScanImageView();
            dialog.Owner = this;
            dialog.ShowDialog();
        }

        private void ManualInputButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ManualInputView();
            dialog.Owner = this;
            dialog.ShowDialog();
        }
    }
}


