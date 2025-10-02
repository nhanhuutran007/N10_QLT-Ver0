using System.Windows;
using System.Windows.Input;
using QLKDPhongTro.Presentation.ViewModels;

namespace QLKDPhongTro.Presentation.Views.Windows
{
    public partial class RoomManagementWindow : Window
    {
        public RoomManagementWindow()
        {
            InitializeComponent();
            // Set DataContext here
            this.DataContext = new RentedRoomViewModel();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        // Handle individual sidebar button clicks
        private void OverviewButton_Click(object sender, RoutedEventArgs e)
        {
            // Handle overview navigation
        }

        private void RoomsButton_Click(object sender, RoutedEventArgs e)
        {
            // Already in rooms management, do nothing or refresh
        }

        private void TenantsButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to tenants management
        }

        private void InvoicesButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to invoices management
        }

        private void ContractsButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to contracts management
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to settings
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            // Handle logout
            var mainWindow = Application.Current.MainWindow;
            if (mainWindow != this)
            {
                mainWindow?.Close();
            }
            Application.Current.Shutdown();
        }

        private void RoomCard_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is BusinessLayer.DTOs.RentedRoomDto room)
            {
                if (DataContext is RentedRoomViewModel viewModel)
                {
                    viewModel.SelectedRoom = room;
                }
            }
        }
    }
}