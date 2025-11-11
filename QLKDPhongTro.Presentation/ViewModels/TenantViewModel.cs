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

namespace QLKDPhongTro.Presentation.ViewModels
{
    public partial class TenantViewModel : ObservableObject
    {
        private readonly TenantController _tenantController;
        private readonly DispatcherTimer _statusTimer;
        private ObservableCollection<TenantDto> _allTenants = new();

        public TenantViewModel()
        {
            // !! Lưu ý: Đảm bảo bạn có file TenantRepository.cs
            // và TenantController.cs trong các thư mục tương ứng
            var repo = new TenantRepository();
            _tenantController = new TenantController(repo);

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

        public string[] GenderOptions { get; } = new[] { "Nam", "Nữ", "Khác" };

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
                        Tenants.Add(tenant);
                    }
                }
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
                NgayCap = DateTime.Today // Khởi tạo giá trị ngày cấp
            };

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

                // ✅ ĐÃ BỔ SUNG 2 DÒNG CÒN THIẾU
                NoiCap = tenant.NoiCap,
                NgayCap = tenant.NgayCap
            };
            // ⭐⭐⭐ KẾT THÚC SỬA LỖI ⭐⭐⭐

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
                Tenants.Clear();

                if (string.IsNullOrWhiteSpace(SearchText))
                {
                    foreach (var tenant in _allTenants)
                    {
                        Tenants.Add(tenant);
                    }
                }
                else
                {
                    var searchResults = await _tenantController.SearchTenantsByNameAsync(SearchText);
                    foreach (var tenant in searchResults)
                    {
                        Tenants.Add(tenant);
                    }
                }
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

            MessageBox.Show($"Chi tiết khách thuê: {SelectedTenant.HoTen}\nCCCD: {SelectedTenant.CCCD}\nSĐT: {SelectedTenant.SoDienThoai}",
                "Chi tiết khách thuê", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}