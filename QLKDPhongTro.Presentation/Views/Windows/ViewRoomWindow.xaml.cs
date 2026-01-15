using System.Windows;
using System.Windows.Input;
using QLKDPhongTro.Presentation.ViewModels;
using QLKDPhongTro.Presentation.Utils;

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
            this.Closed += ViewRoomWindow_Closed;
        }

        private void ViewRoomWindow_Closed(object? sender, EventArgs e)
        {
            // Kiểm tra và đóng ứng dụng nếu không còn cửa sổ nào mở
            WindowHelper.CheckAndShutdownIfNoWindows(this);
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