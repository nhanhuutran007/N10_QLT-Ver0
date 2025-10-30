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
            if (Topbar != null)
            {
                Topbar.SearchTextChanged += Topbar_SearchTextChanged;
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
    }
}
