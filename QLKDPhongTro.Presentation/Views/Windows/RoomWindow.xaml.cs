using QLKDPhongTro.Presentation.ViewModels;
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
        }

        // Hàm thêm lại để fix lỗi
        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }
    }
}