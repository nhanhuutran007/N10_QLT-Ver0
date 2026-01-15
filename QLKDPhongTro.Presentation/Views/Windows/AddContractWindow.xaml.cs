// Presentation/Views/Windows/AddContractWindow.xaml.cs
using System.Windows;
using System.Windows.Controls;
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

        private void CopyPathMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is AddContractViewModel vm && !string.IsNullOrEmpty(vm.ContractSavePath))
            {
                try
                {
                    System.Windows.Clipboard.SetText(vm.ContractSavePath);
                    MessageBox.Show("Đã sao chép đường dẫn vào clipboard!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch
                {
                    MessageBox.Show("Không thể sao chép đường dẫn!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
