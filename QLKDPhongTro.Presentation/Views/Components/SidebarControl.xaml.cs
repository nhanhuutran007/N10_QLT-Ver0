using System;
using System.Windows;
using System.Windows.Controls;

namespace QLKDPhongTro.Presentation.Views.Components
{
    public partial class SidebarControl : UserControl
    {
        public event EventHandler<string> MenuItemClicked;
        public event EventHandler LogoutClicked;

        public SidebarControl()
        {
            InitializeComponent();
        }

        // Property để set tiêu đề trang
        public string PageTitle
        {
            get { return PageTitleTextBlock.Text; }
            set { PageTitleTextBlock.Text = value; }
        }

        // Property để set menu item được chọn
        public string SelectedMenuItem
        {
            get { return _selectedMenuItem; }
            set
            {
                _selectedMenuItem = value;
                UpdateMenuSelection();
            }
        }

        private string _selectedMenuItem = "";

        private void UpdateMenuSelection()
        {
            // TODO: Implement logic để highlight menu item được chọn
            // Có thể thêm logic để thay đổi background của các button
        }

        private void OverviewButton_Click(object sender, RoutedEventArgs e)
        {
            MenuItemClicked?.Invoke(this, "Overview");
        }

        private void RoomsButton_Click(object sender, RoutedEventArgs e)
        {
            MenuItemClicked?.Invoke(this, "Rooms");
        }

        private void TenantsButton_Click(object sender, RoutedEventArgs e)
        {
            MenuItemClicked?.Invoke(this, "Tenants");
        }

        private void InvoicesButton_Click(object sender, RoutedEventArgs e)
        {
            MenuItemClicked?.Invoke(this, "Invoices");
        }

        private void ContractsButton_Click(object sender, RoutedEventArgs e)
        {
            MenuItemClicked?.Invoke(this, "Contracts");
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            MenuItemClicked?.Invoke(this, "Settings");
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            LogoutClicked?.Invoke(this, EventArgs.Empty);
        }
    }
}
