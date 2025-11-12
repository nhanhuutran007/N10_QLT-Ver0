using QLKDPhongTro.Presentation.ViewModels;
using System;
using System.Windows;

namespace QLKDPhongTro.Presentation.Views.Windows
{
    public partial class RoomWindow : Window
    {
        public RoomWindow()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.MinHeight = 600;
            this.MinWidth = 800;
            
            // Khởi tạo DataContext với ViewModel
            this.DataContext = new RentedRoomViewModel();
            
            // Load dữ liệu từ database
            LoadRoomData();
        }

        // Handler cho TopbarControl events
        private void TopbarControl_MenuButtonClicked(object sender, EventArgs e)
        {
            // Logic chung đã được chuyển vào TopbarControl.xaml.cs
        }

        private void TopbarControl_SearchTextChanged(object sender, string searchText)
        {
            // Logic search chung đã được chuyển vào TopbarControl.xaml.cs
        }

        private void TopbarControl_SettingsButtonClicked(object sender, EventArgs e)
        {
            // Logic settings chung đã được chuyển vào TopbarControl.xaml.cs
        }

        // Handler cho SidebarControl events
        private void SidebarControl_MenuItemClicked(object sender, string menuItem)
        {
            // Logic navigation đã được chuyển vào SidebarControl.xaml.cs
        }

        // Load dữ liệu từ database
        private void LoadRoomData()
        {
            if (DataContext is RentedRoomViewModel viewModel)
            {
                // Load rooms data từ database
                viewModel.LoadRoomsCommand?.Execute(null);
            }
        }
    }
}

