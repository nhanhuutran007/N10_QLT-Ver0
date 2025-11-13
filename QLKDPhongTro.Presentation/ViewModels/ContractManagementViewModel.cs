using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QLKDPhongTro.BusinessLayer.Controllers;
using QLKDPhongTro.BusinessLayer.DTOs;
using QLKDPhongTro.DataLayer.Repositories;
using QLKDPhongTro.Presentation.Views.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace QLKDPhongTro.Presentation.ViewModels
{
    // Kế thừa ObservableObject để sử dụng SetProperty
    public partial class ContractManagementViewModel : ObservableObject
    {
        private readonly ContractController _contractController;
        private List<ContractDto> _allContracts = new List<ContractDto>();

        // 1. KHAI BÁO TƯỜNG MINH PROPERTY "Contracts" (Sửa lỗi 'Contracts' does not exist)
        private ObservableCollection<ContractDto> _contracts;
        public ObservableCollection<ContractDto> Contracts
        {
            get => _contracts;
            set => SetProperty(ref _contracts, value);
        }

        // 2. KHAI BÁO TƯỜNG MINH PROPERTY "SelectedContract" (Sửa lỗi 'SelectedContract' does not exist)
        private ContractDto? _selectedContract;
        public ContractDto? SelectedContract
        {
            get => _selectedContract;
            set
            {
                if (SetProperty(ref _selectedContract, value))
                {
                    // Khi chọn dòng khác, cập nhật trạng thái nút Sửa/Xóa
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        // Phân trang
        private int _pageSize = 10;
        public int PageSize
        {
            get => _pageSize;
            set
            {
                int newValue = value <= 0 ? 10 : value;
                if (SetProperty(ref _pageSize, newValue))
                {
                    ApplySortAndPage();
                }
            }
        }

        private int _pageIndex = 1;
        public int PageIndex
        {
            get => _pageIndex;
            set
            {
                int newValue = value < 1 ? 1 : value;
                if (SetProperty(ref _pageIndex, newValue))
                {
                    ApplySortAndPage();
                }
            }
        }

        private int _totalPages = 1;
        public int TotalPages
        {
            get => _totalPages;
            set => SetProperty(ref _totalPages, value < 1 ? 1 : value);
        }

        // Sắp xếp
        private string _sortOrder = "newest";
        public string SortOrder
        {
            get => _sortOrder;
            set
            {
                if (SetProperty(ref _sortOrder, value))
                {
                    ApplySortAndPage();
                }
            }
        }

        private string _paginationText = string.Empty;
        public string PaginationText
        {
            get => _paginationText;
            set => SetProperty(ref _paginationText, value);
        }

        // Commands
        public ICommand PrevPageCommand { get; }
        public ICommand NextPageCommand { get; }

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
            Contracts = new ObservableCollection<ContractDto>();

            // Khởi tạo Commands thủ công để đảm bảo tương thích
            // Lưu ý: Dùng CommunityToolkit.Mvvm.Input.RelayCommand rõ ràng nếu cần, 
            // nhưng ở đây ta dùng constructor chuẩn.

            PrevPageCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(
                () => { if (PageIndex > 1) PageIndex--; },
                () => PageIndex > 1);

            NextPageCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(
                () => { if (PageIndex < TotalPages) PageIndex++; },
                () => PageIndex < TotalPages);

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

        private async Task LoadContractsAsync()
        {
            try
            {
                var contractList = await _contractController.GetAllHopDongAsync();
                _allContracts = contractList;
                PageIndex = 1;
                ApplySortAndPage();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplySortAndPage()
        {
            if (Contracts == null) return;

            IEnumerable<ContractDto> query = _allContracts;

            if (SortOrder == "newest")
                query = query.OrderByDescending(x => x.NgayBatDau);
            else
                query = query.OrderBy(x => x.NgayBatDau);

            var total = query.Count();
            TotalPages = (int)Math.Ceiling(total / (double)PageSize);
            if (TotalPages == 0) TotalPages = 1;
            if (PageIndex > TotalPages) PageIndex = TotalPages;

            var skip = (PageIndex - 1) * PageSize;
            var pageItems = query.Skip(skip).Take(PageSize).ToList();

            Contracts.Clear();
            foreach (var item in pageItems)
            {
                Contracts.Add(item);
            }

            UpdatePagination(total, skip, pageItems.Count);

            // 3. SỬA LỖI NOTIFY COMMAND: Dùng CommandManager để refresh tất cả nút bấm UI
            CommandManager.InvalidateRequerySuggested();
        }

        private void UpdatePagination(int total, int skip, int pageCount)
        {
            var from = total == 0 ? 0 : skip + 1;
            var to = skip + pageCount;
            PaginationText = $"Hiển thị {from} đến {to} trong {total}";
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

        private async Task DeleteContract()
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
                    await _contractController.DeleteHopDongAsync(SelectedContract.MaHopDong);
                    MessageBox.Show("✅ Hợp đồng đã được xóa thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    await LoadContractsAsync();
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
                    await LoadContractsAsync();
                    return;
                }

                _allContracts = expiringContracts;
                PageIndex = 1;
                ApplySortAndPage();

                MessageBox.Show($"Đã tải {expiringContracts.Count} hợp đồng sắp hết hạn trong {days} ngày tới.",
                                "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi khi tải hợp đồng sắp hết hạn: {ex.Message}",
                                "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task ReloadAllContractsAsync()
        {
            await LoadContractsAsync();
        }

        private async Task SendExpiryWarningEmailsAsync()
        {
            try
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
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi khi gửi email cảnh báo: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}