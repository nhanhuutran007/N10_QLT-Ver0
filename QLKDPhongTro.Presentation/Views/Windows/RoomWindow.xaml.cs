using QLKDPhongTro.Presentation.ViewModels;
using QLKDPhongTro.Presentation.Utils;
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

            this.DataContext = new RentedRoomViewModel();

            // Thêm event handler để kiểm tra và đóng ứng dụng khi đóng cửa sổ
            this.Closed += RoomWindow_Closed;
        }

        // Hàm thêm lại để fix lỗi
        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }

        private void RoomWindow_Closed(object? sender, EventArgs e)
        {
            // Kiểm tra và đóng ứng dụng nếu không còn cửa sổ nào mở
            WindowHelper.CheckAndShutdownIfNoWindows(this);
        }
    }
}