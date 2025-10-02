using QLKDPhongTro.Presentation.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace QLKDPhongTro.Presentation.Views.Windows
{
    public partial class RoomManagementWindow : Window
    {
        private readonly RentedRoomViewModel _viewModel;

        public RoomManagementWindow()
        {
            InitializeComponent();

            // Dùng cùng một instance ViewModel cho DataContext và _viewModel
            _viewModel = new RentedRoomViewModel();
            DataContext = _viewModel;

            // Load dữ liệu khi mở form
            Loaded += async (s, e) => await _viewModel.LoadRoomsAsync();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
            => WindowState = WindowState.Minimized;

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
            => WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;

        private void CloseButton_Click(object sender, RoutedEventArgs e)
            => Close();

        private void ViewRoom_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.SelectedRoom == null)
            {
                MessageBox.Show("Vui lòng chọn phòng để xem.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var viewRoomWindow = new ViewRoomWindow(_viewModel);
            viewRoomWindow.ShowDialog();
        }

        private void RoomCard_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is BusinessLayer.DTOs.RentedRoomDto room)
                _viewModel.SelectedRoom = room;
        }

        private void SidebarControl_MenuItemClicked(object sender, string menuItem)
        {
            // TODO: xử lý chuyển trang
        }

        private void SidebarControl_LogoutClicked(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
