using System.Windows;
using System.Windows.Input;
using QLKDPhongTro.Presentation.ViewModels;

namespace QLKDPhongTro.Presentation.Views.Windows
{
    public partial class ViewRoomWindow : Window
    {
        public ViewRoomWindow()
        {
            InitializeComponent();
        }

        public ViewRoomWindow(RentedRoomViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                try
                {
                    this.DragMove();
                }
                catch { }
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}