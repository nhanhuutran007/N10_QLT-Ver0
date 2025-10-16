// Presentation/Views/Windows/AddContractWindow.xaml.cs
using System.Windows;
using QLKDPhongTro.Presentation.ViewModels;

namespace QLKDPhongTro.Presentation.Views.Windows
{
    public partial class AddContractWindow : Window
    {
        public AddContractWindow(AddContractViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;

            vm.RequestClose += (s, e) =>
            {
                this.DialogResult = e; // e = true khi lưu; false khi hủy
                this.Close();
            };
        }
    }
}
