using QLKDPhongTro.Presentation.ViewModels;
using System.Windows;

namespace QLKDPhongTro.Presentation.Views.Windows
{
    public partial class ContractManagementWindow : Window
    {
        public ContractManagementWindow()
        {
            InitializeComponent(); // <-- đây là hàm auto-generated, không cần tự viết
            this.DataContext = new ContractManagementViewModel();
        }


    }
}
