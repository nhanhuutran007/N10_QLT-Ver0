using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QLKDPhongTro.BusinessLayer.Controllers;
using QLKDPhongTro.BusinessLayer.DTOs;
using QLKDPhongTro.DataLayer.Repositories;
using QLKDPhongTro.Presentation.Views.Windows;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using CommunityToolkit.Mvvm.Input;


namespace QLKDPhongTro.Presentation.ViewModels
{
    public partial class ContractManagementViewModel : ViewModelBase
    {
        private readonly ContractController _contractController;

        [ObservableProperty]
        private ObservableCollection<ContractDto> _contracts;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(EditContractCommand))]
        [NotifyCanExecuteChangedFor(nameof(DeleteContractCommand))]
        private ContractDto _selectedContract;

        public ContractManagementViewModel()
        {
            _contractController = new ContractController(new ContractRepository());
            _ = LoadContractsAsync();
        }

        // 🔹 Load toàn bộ danh sách hợp đồng
        private async System.Threading.Tasks.Task LoadContractsAsync()
        {
            try
            {
                var contractList = await _contractController.GetAllHopDongAsync();
                Contracts = new ObservableCollection<ContractDto>(contractList);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // 🔹 Lệnh: Thêm hợp đồng mới
        [RelayCommand]
        private void AddContract()
        {
            try
            {
                var vm = new AddContractViewModel(_contractController);
                var win = new AddContractWindow(vm)
                {
                    Owner = Application.Current?.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive)
                };

                bool? result = win.ShowDialog();
                if (result == true)
                    _ = LoadContractsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi khi mở form tạo hợp đồng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanEditOrDelete() => SelectedContract != null;

        // 🔹 Lệnh: Sửa hợp đồng (mở lại AddContractWindow trong chế độ chỉnh sửa)
        [RelayCommand(CanExecute = nameof(CanEditOrDelete))]
        private void EditContract()
        {
            if (SelectedContract == null)
            {
                MessageBox.Show("⚠️ Vui lòng chọn hợp đồng để chỉnh sửa.");
                return;
            }

            try
            {
                var vm = new AddContractViewModel(_contractController, SelectedContract);
                var win = new AddContractWindow(vm)
                {
                    Owner = Application.Current?.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive)
                };

                bool? result = win.ShowDialog();
                if (result == true)
                    _ = LoadContractsAsync(); // reload list after editing
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi khi mở form sửa hợp đồng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // 🔹 Lệnh: Xóa hợp đồng
        [RelayCommand(CanExecute = nameof(CanEditOrDelete))]
        private void DeleteContract()
        {
            if (SelectedContract == null)
            {
                MessageBox.Show("⚠️ Vui lòng chọn hợp đồng để xóa.");
                return;
            }

            var confirm = MessageBox.Show(
                $"Bạn có chắc chắn muốn xóa hợp đồng của '{SelectedContract.TenNguoiThue}'?",
                "Xác nhận xóa",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (confirm == MessageBoxResult.Yes)
            {
                try
                {
                    _ = _contractController.DeleteHopDongAsync(SelectedContract.MaHopDong);
                    MessageBox.Show("✅ Hợp đồng đã được xóa thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    _ = LoadContractsAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"❌ Lỗi khi xóa: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        [RelayCommand]
        private async Task LoadExpiringContractsAsync()
        {
            try
            {
                int days = 30; // hoặc cho người dùng chọn
                var expiringContracts = await _contractController.GetExpiringContractsAsync(days);

                if (expiringContracts.Count == 0)
                {
                    MessageBox.Show($"Không có hợp đồng nào sắp hết hạn trong {days} ngày tới.",
                                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                Contracts = new ObservableCollection<ContractDto>(expiringContracts);

                MessageBox.Show($"Đã tải {expiringContracts.Count} hợp đồng sắp hết hạn trong {days} ngày tới.",
                                "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi khi tải hợp đồng sắp hết hạn: {ex.Message}",
                                "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task SendExpiryWarningEmailsAsync()
        {
            var result = await _contractController.SendExpiryWarningEmailsAsync(30);

            if (!result)
            {
                MessageBox.Show("✅ Không có hợp đồng nào sắp hết hạn trong 30 ngày tới.",
                                "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            MessageBox.Show("📧 Đã gửi email cảnh báo thành công!", "Thành công",
                            MessageBoxButton.OK, MessageBoxImage.Information);
        }




    }
}
