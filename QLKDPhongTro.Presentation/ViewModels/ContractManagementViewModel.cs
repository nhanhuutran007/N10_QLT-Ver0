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
using System.Windows.Input;
using Microsoft.Win32;

namespace QLKDPhongTro.Presentation.ViewModels
{
    // Kế thừa ObservableObject để sử dụng SetProperty
    public partial class ContractManagementViewModel : ObservableObject
    {
        private readonly ContractController _contractController;

        // 1. KHAI BÁO TƯỜNG MINH PROPERTY "Contracts" (Sửa lỗi 'Contracts' does not exist)
        private ObservableCollection<ContractDto> _contracts;
        public ObservableCollection<ContractDto> Contracts
        {
            get => _contracts;
            set => SetProperty(ref _contracts, value);
        }

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(EditContractCommand))]
        [NotifyCanExecuteChangedFor(nameof(DeleteContractCommand))]

        private ContractDto _selectedContract;

        // Sắp xếp: newest | oldest (bind từ ComboBox SelectedValue Tag)
        [ObservableProperty]
        private string _sortOrder = "newest";

        // Commands
        public ICommand SendExpiryWarningEmailsCommand { get; }



        // Sử dụng ICommand thay vì RelayCommand cụ thể để tránh xung đột
        public ICommand AddContractCommand { get; }
        public ICommand EditContractCommand { get; }
        public ICommand DeleteContractCommand { get; }
        public ICommand LoadExpiringContractsCommand { get; }
        public ICommand ReloadAllContractsCommand { get; }
        public ICommand SendExpiryWarningEmailsCommand { get; }

        public ContractManagementViewModel()
        {
            _contractController = new ContractController(new ContractRepository());

            // Khởi tạo command
            SendExpiryWarningEmailsCommand = new Commands.RelayCommand(async () => await SendExpiryWarningEmailsAsync());

            AddContractCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(AddContract);

            EditContractCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(
                EditContract,
                () => SelectedContract != null); // Điều kiện check null

            DeleteContractCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(
                async () => await DeleteContract(),
                () => SelectedContract != null);

            LoadExpiringContractsCommand = new CommunityToolkit.Mvvm.Input.AsyncRelayCommand(LoadExpiringContractsAsync);
            ReloadAllContractsCommand = new CommunityToolkit.Mvvm.Input.AsyncRelayCommand(ReloadAllContractsAsync);
            SendExpiryWarningEmailsCommand = new CommunityToolkit.Mvvm.Input.AsyncRelayCommand(SendExpiryWarningEmailsAsync);

            _ = LoadContractsAsync();
        }

        // 🔹 Load toàn bộ danh sách hợp đồng
        private async System.Threading.Tasks.Task LoadContractsAsync()
        {
            try
            {
                var contractList = await _contractController.GetAllHopDongAsync();
                Contracts = new ObservableCollection<ContractDto>(ApplySorting(contractList));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Áp dụng sắp xếp theo SortOrder
        private IEnumerable<ContractDto> ApplySorting(IEnumerable<ContractDto> items)
        {
            if (items == null) return Enumerable.Empty<ContractDto>();
            return SortOrder == "oldest"
                ? items.OrderBy(x => x.NgayBatDau)
                : items.OrderByDescending(x => x.NgayBatDau);
        }

        partial void OnSortOrderChanged(string value)
        {
            if (Contracts == null) return;
            var sorted = ApplySorting(Contracts.ToList());
            Contracts = new ObservableCollection<ContractDto>(sorted);
        }

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
                    _ = LoadContractsAsync();
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
            if (SelectedContract == null) return;

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

        private async Task LoadExpiringContractsAsync()
        {
            try
            {
                int days = 30;
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
            // Đảm bảo method được gọi - hiển thị thông báo ngay
            MessageBox.Show("⏳ Bắt đầu gửi email cảnh báo...",
                            "Đang xử lý",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);

            try
            {
                int days = 30; // Hợp đồng sắp hết hạn trong 30 ngày

                // Gọi method async
                var result = await _contractController.SendExpiryWarningEmailsAsync(days);
                var (success, failed, errors) = result;

                // Xử lý kết quả
                if (success == 0 && failed == 0)
                {
                    if (errors != null && errors.Count > 0 && errors[0].Contains("Không có hợp đồng"))
                    {
                        MessageBox.Show("ℹ️ " + errors[0],
                                        "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("ℹ️ Không có hợp đồng nào sắp hết hạn trong 30 ngày tới.",
                                        "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    return;
                }

                string message = $"📧 Đã gửi email cảnh báo:\n\n✅ Thành công: {success} email\n❌ Thất bại: {failed} email";

                if (errors != null && errors.Count > 0)
                {
                    message += "\n\nChi tiết lỗi:\n" + string.Join("\n", errors.Take(5));
                    if (errors.Count > 5)
                    {
                        message += $"\n... và {errors.Count - 5} lỗi khác.";
                    }
                }

                MessageBox.Show(message,
                                success > 0 ? "Thành công" : "Có lỗi xảy ra",
                                MessageBoxButton.OK,
                                success > 0 ? MessageBoxImage.Information : MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                string errorMessage = $"❌ Lỗi khi gửi email cảnh báo:\n\n{ex.Message}";
                if (ex.InnerException != null)
                {
                    errorMessage += $"\n\nChi tiết: {ex.InnerException.Message}";
                }

                MessageBox.Show(errorMessage,
                                "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


    }
}