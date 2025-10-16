using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QLKDPhongTro.BusinessLayer.Controllers;
using QLKDPhongTro.BusinessLayer.DTOs;
using QLKDPhongTro.DataLayer.Repositories;
using QLKDPhongTro.Presentation.Utils;
using QLKDPhongTro.Presentation.Views.Windows;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Windows;

namespace QLKDPhongTro.Presentation.ViewModels
{
    public partial class ContractManagementViewModel : ViewModelBase
    {
        private readonly ContractController _conTractController;

        [ObservableProperty]
        private ObservableCollection<ContractDto> _contracts;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(EditContractCommand))]
        [NotifyCanExecuteChangedFor(nameof(DeleteContractCommand))]
        private ContractDto _selectedContract;

        public ContractManagementViewModel()
        {

            _conTractController = new ContractController(new ContractRepository());
            LoadContracts();
        }

        private void LoadContracts()
        {
            try
            {
                var contractList = _conTractController.GetAllHopDong();
                Contracts = new ObservableCollection<ContractDto>(contractList);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void AddContract()
        {
            try
            {
                var vm = new AddContractViewModel(_conTractController);
                var win = new AddContractWindow(vm)
                {
                    Owner = Application.Current?.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive)
                };

                var result = win.ShowDialog();
                if (result == true)
                {
                    LoadContracts();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở form tạo hợp đồng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private bool CanEditOrDelete() => SelectedContract != null;

        [RelayCommand(CanExecute = nameof(CanEditOrDelete))]
        private void EditContract()
        {
            MessageBox.Show($"Sửa hợp đồng cho: {SelectedContract.TenNguoiThue}");
        }

        [RelayCommand(CanExecute = nameof(CanEditOrDelete))]
        private void DeleteContract()
        {
            var result = MessageBox.Show($"Bạn có chắc chắn muốn xóa hợp đồng của '{SelectedContract.TenNguoiThue}'?", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _conTractController.DeleteHopDong(SelectedContract.MaHopDong);
                    MessageBox.Show("Xóa hợp đồng thành công!");
                    LoadContracts(); // Tải lại danh sách sau khi xóa
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xóa: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}