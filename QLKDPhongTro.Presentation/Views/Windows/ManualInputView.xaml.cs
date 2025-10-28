using System.Windows;

namespace QLKDPhongTro.Presentation.Views.Windows
{
    public partial class ManualInputView : Window
    {
        public ManualInputView()
        {
            InitializeComponent();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}


