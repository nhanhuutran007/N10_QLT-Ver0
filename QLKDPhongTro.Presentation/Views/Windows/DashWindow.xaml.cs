using System.Windows;
using System.Windows.Input;
using QLKDPhongTro.Presentation.ViewModels;

namespace QLKDPhongTro.Presentation.Views.Windows
{
    public partial class DashWindow : Window
    {
        public DashWindow()
        {
            InitializeComponent();
            DataContext = new DashboardViewModel();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
    }
}