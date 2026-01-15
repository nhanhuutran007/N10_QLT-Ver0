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
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;

namespace QLKDPhongTro.Presentation.ViewModels
{
    // Kế thừa ObservableObject để sử dụng SetProperty
    public partial class ContractManagementViewModel : ObservableObject
    {
        private readonly ContractController _contractController;
        private List<ContractDto> _allContracts = new();
        private List<ContractDto> _filteredContracts = new();

        // 1. KHAI BÁO TƯỜNG MINH PROPERTY "Contracts"
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

        partial void OnSelectedContractChanged(ContractDto? value)
        {
            // Đảm bảo các command được cập nhật khi SelectedContract thay đổi
            EditContractCommand.NotifyCanExecuteChanged();
            DeleteContractCommand.NotifyCanExecuteChanged();
        }

        // Sắp xếp: newest | oldest (bind từ ComboBox SelectedValue Tag)
        [ObservableProperty]
        private string _sortOrder = "newest";

        // Search
        [ObservableProperty]
        private string _searchText = string.Empty;

        partial void OnSearchTextChanged(string value)
        {
            SearchContractsCommand.Execute(null);
        }

        // Pagination
        [ObservableProperty]
        private string _pageSize = "5"; // bind từ ComboBox Tag (string)

        [ObservableProperty]
        private int _pageIndex = 1;

        [ObservableProperty]
        private int _totalPages = 1;

        [ObservableProperty]
        private string _paginationText = string.Empty;

        partial void OnSortOrderChanged(string value)
        {
            ApplySort();
            UpdatePagination(resetPageIndex: true);
        }

        partial void OnPageSizeChanged(string value)
        {
            UpdatePagination(resetPageIndex: true);
        }

        public ContractManagementViewModel()
        {
            _contractController = new ContractController(new ContractRepository());
            Contracts = new ObservableCollection<ContractDto>();
            _ = LoadContractsAsync();
        }

        // 🔹 Load toàn bộ danh sách hợp đồng
        private async System.Threading.Tasks.Task LoadContractsAsync()
        {
            try
            {
                var contractList = await _contractController.GetAllHopDongAsync();
                _allContracts = contractList.ToList();
                
                // Đồng bộ filtered + áp dụng sắp xếp & phân trang
                _filteredContracts = _allContracts.ToList();
                ApplySort();
                PageIndex = 1;
                UpdatePagination(resetPageIndex: false);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Áp dụng sắp xếp theo SortOrder
        private void ApplySort()
        {
            IEnumerable<ContractDto> src = _filteredContracts;
            if (string.Equals(SortOrder, "oldest", StringComparison.OrdinalIgnoreCase))
            {
                src = src.OrderBy(x => x.NgayBatDau);
            }
            else
            {
                src = src.OrderByDescending(x => x.NgayBatDau);
            }
            _filteredContracts = src.ToList();
        }

        // Tìm kiếm hợp đồng
        [RelayCommand]
        private void SearchContracts()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                _filteredContracts = _allContracts.ToList();
            }
            else
            {
                var searchLower = SearchText.ToLowerInvariant();
                _filteredContracts = _allContracts.Where(c =>
                    (c.MaHopDong.ToString().Contains(searchLower)) ||
                    (c.TenNguoiThue != null && c.TenNguoiThue.ToLowerInvariant().Contains(searchLower)) ||
                    (c.TenPhong != null && c.TenPhong.ToLowerInvariant().Contains(searchLower)) ||
                    (c.TrangThai != null && c.TrangThai.ToLowerInvariant().Contains(searchLower))
                ).ToList();
            }

            ApplySort();
            PageIndex = 1;
            UpdatePagination(resetPageIndex: false);
        }

        // Phân trang
        private void UpdatePagination(bool resetPageIndex)
        {
            int size = 5;
            if (!int.TryParse(PageSize, out size) || size <= 0) size = 5;

            TotalPages = Math.Max(1, (int)Math.Ceiling((_filteredContracts.Count) / (double)size));
            if (resetPageIndex) PageIndex = 1;
            if (PageIndex > TotalPages) PageIndex = TotalPages;
            if (PageIndex < 1) PageIndex = 1;

            Contracts?.Clear();
            if (Contracts == null)
            {
                Contracts = new ObservableCollection<ContractDto>();
            }

            if (_filteredContracts.Count > 0)
            {
                int start = (PageIndex - 1) * size;
                var pageItems = _filteredContracts.Skip(start).Take(size);
                foreach (var item in pageItems)
                {
                    Contracts.Add(item);
                }
                int end = Math.Min(start + size, _filteredContracts.Count);
                PaginationText = $"Hiển thị {start + 1}-{end} của {_filteredContracts.Count}";
            }
            else
            {
                PaginationText = "Không có dữ liệu";
            }
        }

        // Điều hướng trang
        [RelayCommand]
        private void NextPage()
        {
            if (PageIndex < TotalPages)
            {
                PageIndex++;
                UpdatePagination(resetPageIndex: false);
            }
        }

        [RelayCommand]
        private void PrevPage()
        {
            if (PageIndex > 1)
            {
                PageIndex--;
                UpdatePagination(resetPageIndex: false);
            }
        }

        [RelayCommand]
        private async Task AddContract()
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
                    await LoadContractsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi khi mở form tạo hợp đồng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand(CanExecute = nameof(CanEditOrDelete))]
        private async Task EditContract()
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
                    await LoadContractsAsync();
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

        private bool CanEditOrDelete() => SelectedContract != null;

        [RelayCommand]
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

                // Cập nhật _allContracts và _filteredContracts
                _allContracts = expiringContracts.ToList();
                _filteredContracts = _allContracts.ToList();
                
                // Áp dụng sắp xếp & phân trang
                ApplySort();
                PageIndex = 1;
                UpdatePagination(resetPageIndex: false);

                MessageBox.Show($"Đã tải {expiringContracts.Count} hợp đồng sắp hết hạn trong {days} ngày tới.",
                                "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi khi tải hợp đồng sắp hết hạn: {ex.Message}",
                                "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
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