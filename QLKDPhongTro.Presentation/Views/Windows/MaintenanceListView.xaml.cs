using QLKDPhongTro.Presentation.ViewModels;
using System.Windows;

namespace QLKDPhongTro.Presentation.Views.Windows
{
    public partial class MaintenanceListView : Window
    {
        public MaintenanceListView()
        {
            InitializeComponent();
            DataContext = new MaintenanceListViewModel();
            if (TopbarControl != null)
            {
                TopbarControl.SearchTextChanged += Topbar_SearchTextChanged;
            }
        }

        private void Topbar_SearchTextChanged(object? sender, string e)
        {
            if (DataContext is MaintenanceListViewModel vm)
            {
                // Reset về trang 1 khi tìm kiếm
                vm.PageIndex = 1;
                vm.SearchText = e ?? string.Empty;
            }
        }

        private void AddMaintenance_Click(object sender, RoutedEventArgs e)
        {
            var popup = new QrPopupWindow
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            popup.ShowDialog();
        }
    }
}
