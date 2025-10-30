using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using QLKDPhongTro.Presentation.ViewModels;

namespace QLKDPhongTro.Presentation.Views.Windows
{
    public partial class DashWindow : Window
    {
        public DashWindow()
        {
            InitializeComponent();
            if (this.DataContext == null)
            {
                this.DataContext = new DashboardViewModel();
            }
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.MinHeight = 600;
            this.MinWidth = 800;
        }


        // Handler cho TopbarControl events - giờ đây TopbarControl tự xử lý logic chung
        private void TopbarControl_MenuButtonClicked(object sender, EventArgs e)
        {
            // Logic chung đã được chuyển vào TopbarControl.xaml.cs
            // Chỉ xử lý logic riêng của DashWindow nếu cần
        }

        private void TopbarControl_SearchTextChanged(object sender, string searchText)
        {
            // Logic search chung đã được chuyển vào TopbarControl.xaml.cs
            // Chỉ xử lý search logic riêng của DashWindow nếu cần
        }

        private void TopbarControl_SettingsButtonClicked(object sender, EventArgs e)
        {
            // Logic settings chung đã được chuyển vào TopbarControl.xaml.cs
            // Chỉ xử lý settings logic riêng của DashWindow nếu cần
        }

        // Handler cho SidebarControl events - giờ đây SidebarControl tự xử lý navigation
        private void SidebarControl_MenuItemClicked(object sender, string menuItem)
        {
            // Logic navigation đã được chuyển vào SidebarControl.xaml.cs
            // Chỉ xử lý logic riêng của DashWindow nếu cần
        }


    }
}
