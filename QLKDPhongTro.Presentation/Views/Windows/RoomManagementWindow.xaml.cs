using System.Windows;
using System.Windows.Input;
using QLKDPhongTro.Presentation.ViewModels;
using QLKDPhongTro.Presentation.Views.Windows;
using System;

namespace QLKDPhongTro.Presentation.Views.Windows
{
    public partial class RoomManagementWindow : Window
    {
        private DateTime _lastClickTime;
        private object _lastClickedItem;

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

        private void RoomCard_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                var currentTime = DateTime.Now;
                var currentItem = (sender as FrameworkElement)?.DataContext;

                // Kiểm tra double click (trong vòng 300ms và cùng một item)
                if (currentItem != null &&
                    currentItem == _lastClickedItem &&
                    (currentTime - _lastClickTime).TotalMilliseconds < 300)
                {
                    // Double click detected
                    if (currentItem is BusinessLayer.DTOs.RentedRoomDto room)
                    {
                        if (DataContext is RentedRoomViewModel viewModel)
                        {
                            viewModel.SelectedRoom = room;
                            viewModel.ShowRoomDetailsWindow();
                        }
                    }

                    // Reset sau khi xử lý double click
                    _lastClickedItem = null;
                    _lastClickTime = DateTime.MinValue;
                }
                else
                {
                    // Single click - chỉ cập nhật thời gian và item
                    _lastClickedItem = currentItem;
                    _lastClickTime = currentTime;
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }
}