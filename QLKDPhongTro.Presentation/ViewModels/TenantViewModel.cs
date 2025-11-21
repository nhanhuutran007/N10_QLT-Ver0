using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QLKDPhongTro.BusinessLayer.Controllers;
using QLKDPhongTro.BusinessLayer.DTOs;
using QLKDPhongTro.DataLayer.Repositories;
using QLKDPhongTro.Presentation.Views.Windows;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.Collections.Generic;

namespace QLKDPhongTro.Presentation.ViewModels
{
    public partial class TenantViewModel : ObservableObject
    {
        private readonly TenantController _tenantController;
        private readonly RentedRoomController _roomController;
        private readonly DispatcherTimer _statusTimer;
        private ObservableCollection<TenantDto> _allTenants = new();
        private List<TenantDto> _filteredTenants = new();

        public TenantViewModel()
        {
            // !! Lưu ý: Đảm bảo bạn có file TenantRepository.cs
            // và TenantController.cs trong các thư mục tương ứng
            var repo = new TenantRepository();
            _tenantController = new TenantController(repo);
            _roomController = new RentedRoomController(new RentedRoomRepository());

            _statusTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(5)
            };
            _statusTimer.Tick += (s, e) =>
            {
                StatusMessage = string.Empty;
                _statusTimer.Stop();
            };

            _title = "Thêm khách thuê mới";
            _buttonContent = "Thêm khách thuê";
            _saveCommand = SaveTenantCommand;

            LoadTenantsCommand.Execute(null);
            LoadRoomsCommand.Execute(null);
        }

        [ObservableProperty] private ObservableCollection<TenantDto> _tenants = new();
        [ObservableProperty] private TenantDto? _selectedTenant;
        [ObservableProperty] private bool _isLoading;
        [ObservableProperty] private string _statusMessage = string.Empty;
        [ObservableProperty] private TenantDto _newTenant = new();
        [ObservableProperty] private string _searchText = string.Empty;

        partial void OnSearchTextChanged(string value)
        {
            SearchTenantsCommand.Execute(null);
        }

        [ObservableProperty] private string _title = "Thêm khách thuê mới";
        [ObservableProperty] private string _buttonContent = "Thêm khách thuê";
        [ObservableProperty] private IAsyncRelayCommand _saveCommand = null!;

        // Danh sách phòng để chọn
        [ObservableProperty] private ObservableCollection<RentedRoomDto> _rooms = new();
        [ObservableProperty] private RentedRoomDto? _selectedRoom;

        public string[] GenderOptions { get; } = new[] { "Nam", "Nữ", "Khác" };
        public string[] TenantStatuses { get; } = new[] { "Đang ở", "Đặt cọc", "Sắp trả phòng", "Đã trả phòng" };

        // === Pagination & Sorting ===
        [ObservableProperty] private string _sortOrder = "newest"; // newest | oldest
        [ObservableProperty] private string _pageSize = "5"; // bind từ ComboBox Tag (string)
        [ObservableProperty] private int _pageIndex = 1;
        [ObservableProperty] private int _totalPages = 1;
        [ObservableProperty] private string _paginationText = string.Empty;

        partial void OnSortOrderChanged(string value)
        {
            ApplySort();
            UpdatePagination(resetPageIndex: true);
        }

        partial void OnPageSizeChanged(string value)
        {
            UpdatePagination(resetPageIndex: true);
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
        private async Task LoadTenants()
        {
            try
            {
                IsLoading = true;
                var tenants = await _tenantController.GetAllTenantsAsync();
                _allTenants.Clear();
                Tenants.Clear();
                if (tenants != null)
                {
                    foreach (var tenant in tenants)
                    {
                        _allTenants.Add(tenant);
                    }
                }

                // Đồng bộ filtered + áp dụng sắp xếp & phân trang
                _filteredTenants = _allTenants.ToList();
                ApplySort();
                PageIndex = 1;
                UpdatePagination(resetPageIndex: false);
            }
            catch (Exception ex)
            {
                StatusMessage = $"Lỗi khi tải danh sách khách thuê: {ex.Message}";
                MessageBox.Show(StatusMessage, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private void ShowAddTenantPanel()
        {
            NewTenant = new TenantDto
            {
                GioiTinh = "Nam",
                NgaySinh = DateTime.Today,
                NgayCap = DateTime.Today,
                TrangThai = TenantStatuses.First()
            };
            SelectedRoom = null; // Reset phòng được chọn

            Title = "Thêm khách thuê mới";
            ButtonContent = "Thêm khách thuê";
            SaveCommand = SaveTenantCommand;

            var window = new AddTenantWindow(this)
            {
                Owner = Application.Current.Windows.OfType<Window>().FirstOrDefault(x => x.IsActive) ?? Application.Current.MainWindow
            };
            window.ShowDialog();
        }

        [RelayCommand]
        private async Task LoadRooms()
        {
            try
            {
                var rooms = await _roomController.GetAllRoomsAsync();
                Rooms.Clear();
                foreach (var room in rooms)
                {
                    Rooms.Add(room);
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Lỗi khi tải danh sách phòng: {ex.Message}";
            }
        }

        [RelayCommand]
        private void ShowEditTenantPanel(TenantDto tenant)
        {
            if (tenant == null)
            {
                MessageBox.Show("Không tìm thấy thông tin khách thuê để chỉnh sửa.",
                                "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // ⭐⭐⭐ BẮT ĐẦU SỬA LỖI ⭐⭐⭐
            NewTenant = new TenantDto
            {
                MaKhachThue = tenant.MaKhachThue,
                HoTen = tenant.HoTen,
                CCCD = tenant.CCCD,
                SoDienThoai = tenant.SoDienThoai,
                Email = tenant.Email,
                DiaChi = tenant.DiaChi,
                NgaySinh = tenant.NgaySinh,
                GioiTinh = tenant.GioiTinh,
                NgheNghiep = tenant.NgheNghiep,
                GhiChu = tenant.GhiChu,
                MaPhong = tenant.MaPhong,
                TrangThai = tenant.TrangThai,

                // ✅ ĐÃ BỔ SUNG 2 DÒNG CÒN THIẾU
                NoiCap = tenant.NoiCap,
                NgayCap = tenant.NgayCap
            };
            // ⭐⭐⭐ KẾT THÚC SỬA LỖI ⭐⭐⭐

            // Chọn phòng tương ứng với MaPhong của tenant
            if (tenant.MaPhong.HasValue)
            {
                SelectedRoom = Rooms.FirstOrDefault(r => r.MaPhong == tenant.MaPhong.Value);
            }
            else
            {
                SelectedRoom = null;
            }

            Title = "Chỉnh sửa khách thuê";
            ButtonContent = "Cập nhật";
            SaveCommand = UpdateTenantCommand;

            var window = new AddTenantWindow(this)
            {
                Owner = Application.Current.Windows.OfType<Window>().FirstOrDefault(x => x.IsActive)
                         ?? Application.Current.MainWindow
            };
            window.ShowDialog();
        }


        [RelayCommand]
        private async Task SaveTenant()
        {
            try
            {
                IsLoading = true;
                
                // Gán MaPhong từ phòng được chọn
                if (SelectedRoom != null)
                {
                    NewTenant.MaPhong = SelectedRoom.MaPhong;
                }
                if (string.IsNullOrWhiteSpace(NewTenant.TrangThai))
                {
                    NewTenant.TrangThai = TenantStatuses.First();
                }

                var result = await _tenantController.CreateTenantAsync(NewTenant);

                if (result.IsValid)
                {
                    StatusMessage = result.Message;
                    await LoadTenants();

                    var addWindow = Application.Current.Windows.OfType<AddTenantWindow>().FirstOrDefault();
                    addWindow?.Close();
                }
                else
                {
                    StatusMessage = result.Message;
                    MessageBox.Show(result.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Lỗi khi thêm khách thuê: {ex.Message}";
                MessageBox.Show(StatusMessage, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task UpdateTenant()
        {
            try
            {
                IsLoading = true;
                
                // Gán MaPhong từ phòng được chọn
                if (SelectedRoom != null)
                {
                    NewTenant.MaPhong = SelectedRoom.MaPhong;
                }
                if (string.IsNullOrWhiteSpace(NewTenant.TrangThai))
                {
                    NewTenant.TrangThai = TenantStatuses.First();
                }

                var result = await _tenantController.UpdateTenantAsync(NewTenant);

                if (result.IsValid)
                {
                    StatusMessage = result.Message;
                    await LoadTenants();

                    var addWindow = Application.Current.Windows.OfType<AddTenantWindow>().FirstOrDefault();
                    addWindow?.Close();
                }
                else
                {
                    StatusMessage = result.Message;
                    MessageBox.Show(result.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Lỗi khi cập nhật khách thuê: {ex.Message}";
                MessageBox.Show(StatusMessage, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task DeleteTenant(TenantDto tenant)
        {
            if (tenant == null)
            {
                MessageBox.Show("Không tìm thấy khách thuê để xóa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var confirm = MessageBox.Show($"Bạn có chắc muốn xóa khách thuê '{tenant.HoTen}' không?",
                                          "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (confirm != MessageBoxResult.Yes) return;

            try
            {
                IsLoading = true;
                var result = await _tenantController.DeleteTenantAsync(tenant.MaKhachThue);

                if (result.IsValid)
                {
                    StatusMessage = result.Message;
                    await LoadTenants();
                }
                else
                {
                    MessageBox.Show(result.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xóa khách thuê: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }


        [RelayCommand]
        private async Task SearchTenants()
        {
            try
            {
                IsLoading = true;
                if (string.IsNullOrWhiteSpace(SearchText))
                {
                    _filteredTenants = _allTenants.ToList();
                }
                else
                {
                    var searchResults = await _tenantController.SearchTenantsByNameAsync(SearchText);
                    _filteredTenants = searchResults.ToList();
                }

                ApplySort();
                PageIndex = 1;
                UpdatePagination(resetPageIndex: false);
            }
            catch (Exception ex)
            {
                StatusMessage = $"Lỗi khi tìm kiếm: {ex.Message}";
                MessageBox.Show(StatusMessage, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        public void ShowTenantDetailsWindow()
        {
            if (SelectedTenant == null)
            {
                MessageBox.Show("Vui lòng chọn khách thuê để xem chi tiết", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var detailVm = new TenantDetailViewModel(_tenantController);
            var detailWindow = new TenantDetailWindow(SelectedTenant.MaKhachThue, detailVm)
            {
                Owner = Application.Current.MainWindow
            };
            detailWindow.ShowDialog();
        }

        // === Row-level Commands khớp với XAML ===
        [RelayCommand]
        private void ViewTenantRow(TenantDto tenant)
        {
            if (tenant == null) return;
            SelectedTenant = tenant;
            ShowTenantDetailsWindow();
        }

        [RelayCommand]
        private void EditTenantRow(TenantDto tenant)
        {
            if (tenant == null) return;
            ShowEditTenantPanel(tenant);
        }

        [RelayCommand]
        private async Task DeleteTenantRow(TenantDto tenant)
        {
            if (tenant == null) return;
            await DeleteTenant(tenant);
        }

        // === Helpers: sort + paginate ===
        private void ApplySort()
        {
            IEnumerable<TenantDto> src = _filteredTenants;
            if (string.Equals(SortOrder, "oldest", StringComparison.OrdinalIgnoreCase))
            {
                src = src.OrderBy(t => t.NgayTao);
            }
            else
            {
                src = src.OrderByDescending(t => t.NgayTao);
            }
            _filteredTenants = src.ToList();
        }

        private void UpdatePagination(bool resetPageIndex)
        {
            int size = 5;
            if (!int.TryParse(PageSize, out size) || size <= 0) size = 5;

            TotalPages = Math.Max(1, (int)Math.Ceiling((_filteredTenants.Count) / (double)size));
            if (resetPageIndex) PageIndex = 1;
            if (PageIndex > TotalPages) PageIndex = TotalPages;
            if (PageIndex < 1) PageIndex = 1;

            Tenants.Clear();
            if (_filteredTenants.Count > 0)
            {
                int start = (PageIndex - 1) * size;
                var pageItems = _filteredTenants.Skip(start).Take(size);
                foreach (var item in pageItems)
                {
                    Tenants.Add(item);
                }
                int end = Math.Min(start + size, _filteredTenants.Count);
                PaginationText = $"Hiển thị {start + 1}-{end} của {_filteredTenants.Count}";
            }
            else
            {
                PaginationText = "Không có dữ liệu";
            }
        }
    }
}