using QLKDPhongTro.Presentation.ViewModels;
using QLKDPhongTro.Presentation.Utils;
using System.Windows;

namespace QLKDPhongTro.Presentation.Views.Windows
{
    public partial class ContractManagementWindow : Window
    {
        public ContractManagementWindow()
        {
            InitializeComponent(); // <-- đây là hàm auto-generated, không cần tự viết
            this.DataContext = new ContractManagementViewModel();

            // Thêm event handler để kiểm tra và đóng ứng dụng khi đóng cửa sổ
            this.Closed += ContractManagementWindow_Closed;
        }

        private void ContractManagementWindow_Closed(object? sender, EventArgs e)
        {
            // Kiểm tra và đóng ứng dụng nếu không còn cửa sổ nào mở
            WindowHelper.CheckAndShutdownIfNoWindows(this);
        }

    }
}
