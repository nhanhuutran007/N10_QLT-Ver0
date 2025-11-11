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
    public partial class ContractManagementViewModel : ViewModelBase
    {
        private readonly ContractController _contractController;
        private List<ContractDto> _allContracts = new List<ContractDto>();

        [ObservableProperty]
        private ObservableCollection<ContractDto> _contracts;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(EditContractCommand))]
        [NotifyCanExecuteChangedFor(nameof(DeleteContractCommand))]
        private ContractDto _selectedContract;

        // Phân trang
        private int _pageSize = 10;
        public int PageSize
        {
            get => _pageSize;
            set { _pageSize = value <= 0 ? 10 : value; OnPropertyChanged(nameof(PageSize)); ApplySortAndPage(); }
        }
        private int _pageIndex = 1; // 1-based
        public int PageIndex
        {
            get => _pageIndex;
            set { _pageIndex = value < 1 ? 1 : value; OnPropertyChanged(nameof(PageIndex)); ApplySortAndPage(); }
        }
        private int _totalPages = 1;
        public int TotalPages
        {
            get => _totalPages;
            set { _totalPages = value < 1 ? 1 : value; OnPropertyChanged(nameof(TotalPages)); }
        }

        // Sắp xếp
        private string _sortOrder = "newest"; // newest | oldest
        public string SortOrder
        {
            get => _sortOrder;
            set { _sortOrder = value; OnPropertyChanged(nameof(SortOrder)); ApplySortAndPage(); }
        }

        private string _paginationText = string.Empty;
        public string PaginationText
        {
            get => _paginationText;
            set { _paginationText = value; OnPropertyChanged(nameof(PaginationText)); }
        }

        // Commands phân trang
        public ICommand PrevPageCommand { get; }
        public ICommand NextPageCommand { get; }

        public ContractManagementViewModel()
        {
            _contractController = new ContractController(new ContractRepository());
            Contracts = new ObservableCollection<ContractDto>();

            // Initialize phân trang commands
            PrevPageCommand = new RelayCommand(
                () => { if (PageIndex > 1) PageIndex--; },
                () => PageIndex > 1);
            NextPageCommand = new RelayCommand(
                () => { if (PageIndex < TotalPages) PageIndex++; },
                () => PageIndex < TotalPages);

            _ = LoadContractsAsync();
        }

        // 🔹 Load toàn bộ danh sách hợp đồng
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

        // 🔹 Áp dụng sắp xếp và phân trang
        private void ApplySortAndPage()
        {
            if (Contracts == null) return;

            IEnumerable<ContractDto> query = _allContracts;

            // Sắp xếp theo NgayBatDau
            if (SortOrder == "newest")
                query = query.OrderByDescending(x => x.NgayBatDau);
            else
                query = query.OrderBy(x => x.NgayBatDau);

            // Tính tổng số trang
            var total = query.Count();
            TotalPages = (int)Math.Ceiling(total / (double)PageSize);
            if (TotalPages == 0) TotalPages = 1;
            if (PageIndex > TotalPages) PageIndex = TotalPages;

            // Lấy trang hiện tại
            var skip = (PageIndex - 1) * PageSize;
            var pageItems = query.Skip(skip).Take(PageSize).ToList();

            Contracts.Clear();
            foreach (var item in pageItems)
            {
                Contracts.Add(item);
            }

            UpdatePagination(total, skip, pageItems.Count);
            System.Windows.Input.CommandManager.InvalidateRequerySuggested();
        }

        // 🔹 Cập nhật text phân trang
        private void UpdatePagination(int total, int skip, int pageCount)
        {
            var from = total == 0 ? 0 : skip + 1;
            var to = skip + pageCount;
            PaginationText = $"Hiển thị {from} đến {to} trong {total}";
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
        private async Task DeleteContract()
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
                    // Nếu không có hợp đồng sắp hết hạn, vẫn reload toàn bộ
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

        // 🔹 Lệnh: Tải lại toàn bộ hợp đồng
        [RelayCommand]
        private async Task ReloadAllContractsAsync()
        {
            await LoadContractsAsync();
        }

        [RelayCommand]
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
