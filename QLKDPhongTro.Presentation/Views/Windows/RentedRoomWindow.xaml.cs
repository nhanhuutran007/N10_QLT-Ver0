using System.Windows;
using QLKDPhongTro.Presentation.ViewModels;

namespace QLKDPhongTro.Presentation.Views.Windows
{
    public partial class RentedRoomWindow : Window
    {
        public RentedRoomWindow()
        {
            InitializeComponent();
            DataContext = new RentedRoomViewModel();
        }

        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BackToDashboard_Click(object sender, RoutedEventArgs e)
        {
            var dashboard = new DashWindow(); // Giả sử bạn có DashboardWindow
            dashboard.Show();
            Close();
        }
    }
}