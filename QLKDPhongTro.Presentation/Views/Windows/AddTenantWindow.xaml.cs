using QLKDPhongTro.Presentation.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace QLKDPhongTro.Presentation.Views.Windows
{
    public partial class AddTenantWindow : Window
    {
        public AddTenantWindow(TenantViewModel viewModel)
        {
            InitializeComponent();

            // Gán DataContext là ViewModel được truyền vào từ màn hình chính
            this.DataContext = viewModel;
        }

        // Xử lý sự kiện nút Đóng
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Cho phép kéo di chuyển cửa sổ (vì WindowStyle=None)
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            // Chỉ cho phép kéo khi click vào vùng trống, tránh conflict với textbox
            this.DragMove();
        }
    }
}