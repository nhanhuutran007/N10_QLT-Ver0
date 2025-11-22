using System.Linq;
using System.Windows;
using QLKDPhongTro.BusinessLayer.DTOs;

namespace QLKDPhongTro.Presentation.Views.Windows
{
    public partial class SelectNewContractHolderWindow : Window
    {
        public RoomTenantDto? SelectedTenant { get; private set; }

        public SelectNewContractHolderWindow(System.Collections.Generic.List<RoomTenantDto> tenants)
        {
            InitializeComponent();
            
            TenantsListBox.ItemsSource = tenants;
            TenantsListBox.SelectionChanged += TenantsListBox_SelectionChanged;
        }

        private void TenantsListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ConfirmButton.IsEnabled = TenantsListBox.SelectedItem != null;
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (TenantsListBox.SelectedItem is RoomTenantDto selected)
            {
                SelectedTenant = selected;
                DialogResult = true;
                Close();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedTenant = null;
            DialogResult = false;
            Close();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedTenant = null;
            DialogResult = false;
            Close();
        }
    }
}

