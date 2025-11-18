using System.Windows;
using System.Windows.Input;
using QLKDPhongTro.Presentation.ViewModels;

namespace QLKDPhongTro.Presentation.Views.Windows
{
    public partial class AddRoomWindow : Window
    {
        // Constructor mặc định (để tránh lỗi XAML designer)
        public AddRoomWindow()
        {
            InitializeComponent();
        }

        // Constructor nhận ViewModel để binding dữ liệu
        public AddRoomWindow(RentedRoomViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }

        // Logic kéo thả cửa sổ (vì WindowStyle="None")
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                try
                {
                    this.DragMove();
                }
                catch { /* Bỏ qua lỗi nếu chuột nhả ra quá nhanh */ }
            }
        }

        // Logic nút đóng
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}