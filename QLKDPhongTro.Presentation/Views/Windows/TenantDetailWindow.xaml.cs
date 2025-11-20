using QLKDPhongTro.Presentation.ViewModels;
using System.Windows;

namespace QLKDPhongTro.Presentation.Views.Windows
{
    public partial class TenantDetailWindow : Window
    {
        private readonly TenantDetailViewModel _viewModel;
        private readonly int _tenantId;

        public TenantDetailWindow(int tenantId, TenantDetailViewModel viewModel)
        {
            InitializeComponent();
            _tenantId = tenantId;
            _viewModel = viewModel;
            DataContext = _viewModel;
            Loaded += TenantDetailWindow_Loaded;
        }

        private async void TenantDetailWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await _viewModel.LoadAsync(_tenantId);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

