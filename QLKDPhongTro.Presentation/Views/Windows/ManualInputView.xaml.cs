using System.Windows;
using QLKDPhongTro.Presentation.ViewModels;

namespace QLKDPhongTro.Presentation.Views.Windows
{
    public partial class ManualInputView : Window
    {
        private readonly ManualInputViewModel _viewModel;

        public ManualInputView()
        {
            InitializeComponent();
            _viewModel = new ManualInputViewModel();
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

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}


