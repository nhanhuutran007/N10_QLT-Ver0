using QLKDPhongTro.BusinessLayer.Controllers;
using QLKDPhongTro.DataLayer.Models;
using QLKDPhongTro.DataLayer.Repositories;
using QLKDPhongTro.Presentation.Commands;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace QLKDPhongTro.Presentation.ViewModels
{
    public class MaintenanceIncidentViewModel : MaintenanceIncident, INotifyPropertyChanged
    {
        private string _tenPhong = string.Empty;
        public string TenPhong
        {
            get => _tenPhong;
            set { _tenPhong = value; OnPropertyChanged(); }
        }

        private bool _isEditing;
        public bool IsEditing
        {
            get => _isEditing;
            set { _isEditing = value; OnPropertyChanged(); }
        }
        // Backup fields for cancel (chỉ backup các trường có thể chỉnh sửa)
        private string _oldTrangThai = string.Empty;
        private decimal _oldChiPhi;
        private DateTime _oldNgayBaoCao;
        private DateTime? _oldNgayCoTheSua;
        public void BeginEdit()
        {
            // Backup các trường có thể chỉnh sửa: NgayBaoCao, NgayCoTheSua, ChiPhi và TrangThai
            _oldNgayBaoCao = NgayBaoCao;
            _oldNgayCoTheSua = NgayCoTheSua;
            _oldTrangThai = TrangThai;
            _oldChiPhi = ChiPhi;
            IsEditing = true;
        }
        public void CancelEdit()
        {
            // Restore các trường có thể chỉnh sửa: NgayBaoCao, NgayCoTheSua, ChiPhi và TrangThai
            NgayBaoCao = _oldNgayBaoCao;
            NgayCoTheSua = _oldNgayCoTheSua;
            TrangThai = _oldTrangThai;
            ChiPhi = _oldChiPhi;
            IsEditing = false;
            OnPropertyChanged(null);
        }
        public void EndEdit()
        {
            IsEditing = false;
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? prop = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }

    public class MaintenanceListViewModel : INotifyPropertyChanged
    {
        private readonly MaintenanceController _controller;
        private MaintenanceIncidentViewModel? _selectedIncident;

        // Danh sách đầy đủ và danh sách trang hiện tại
        private List<MaintenanceIncidentViewModel> _allMaintenances = new();
        public ObservableCollection<MaintenanceIncidentViewModel> Maintenances { get; set; } = new();

        // Phân trang
        private int _pageSize = 5;
        public int PageSize
        {
            get => _pageSize;
            set { _pageSize = value <= 0 ? 5 : value; OnPropertyChanged(); ApplySortAndPage(); }
        }
        private int _pageIndex = 1; // 1-based
        public int PageIndex
        {
            get => _pageIndex;
            set { _pageIndex = value < 1 ? 1 : value; OnPropertyChanged(); ApplySortAndPage(); }
        }
        private int _totalPages = 1;
        public int TotalPages
        {
            get => _totalPages;
            set { _totalPages = value < 1 ? 1 : value; OnPropertyChanged(); }
        }

        // Sắp xếp
        private string _sortOrder = "newest"; // newest | oldest
        public string SortOrder
        {
            get => _sortOrder;
            set { _sortOrder = value; OnPropertyChanged(); ApplySortAndPage(); }
        }

        // Tìm kiếm
        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set { _searchText = value ?? string.Empty; OnPropertyChanged(); PageIndex = 1; ApplySortAndPage(); }
        }

        private string _paginationText = string.Empty;
        public string PaginationText
        {
            get => _paginationText;
            set { _paginationText = value; OnPropertyChanged(); }
        }

        public MaintenanceIncidentViewModel? SelectedIncident
        {
            get => _selectedIncident;
            set { _selectedIncident = value; OnPropertyChanged(); }
        }

        public ICommand EditCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand PrevPageCommand { get; }
        public ICommand NextPageCommand { get; }

        public MaintenanceListViewModel()
        {
            var maintenanceRepo = new MaintenanceRepository();
            var roomRepo = new RentedRoomRepository();
            var tenantRepo = new TenantRepository();
            _controller = new MaintenanceController(maintenanceRepo, null, roomRepo, tenantRepo);
            EditCommand = new RelayCommand<MaintenanceIncidentViewModel>(EditRow);
            SaveCommand = new RelayCommand<MaintenanceIncidentViewModel>(async item => await SaveRow(item));
            CancelCommand = new RelayCommand<MaintenanceIncidentViewModel>(CancelRow);
            DeleteCommand = new RelayCommand<MaintenanceIncidentViewModel>(async item => await DeleteRow(item));
            PrevPageCommand = new RelayCommand(() => { if (PageIndex > 1) PageIndex--; }, () => PageIndex > 1);
            NextPageCommand = new RelayCommand(() => { if (PageIndex < TotalPages) PageIndex++; }, () => PageIndex < TotalPages);
            _ = LoadDataAsync();
        }

        public async Task LoadDataAsync()
        {
            // 1) Luôn hiển thị dữ liệu cục bộ trước để UI phản hồi nhanh
            await RefreshFromDbAsync();

            // 2) Chạy đồng bộ Google Sheets ở nền với timeout + retry (không chặn UI)
            _ = Task.Run(async () =>
            {
                const int maxRetries = 2;
                var delayBetweenRetriesMs = 1500;
                var syncTimeout = TimeSpan.FromSeconds(30);

                for (int attempt = 0; attempt <= maxRetries; attempt++)
                {
                    try
                    {
                        // Áp timeout cho tác vụ đồng bộ ngay cả khi controller không hỗ trợ token
                        var syncTask = _controller.SyncFromGoogleSheetsAsync();
                        var completed = await Task.WhenAny(syncTask, Task.Delay(syncTimeout)) == syncTask;
                        if (!completed)
                            throw new TimeoutException("Đồng bộ Google Sheets quá thời gian chờ");

                        var addedCount = await syncTask; // đã xong, lấy kết quả thực
                        if (addedCount > 0)
                        {
                            System.Diagnostics.Debug.WriteLine($"Đã thêm {addedCount} bảo trì mới từ Google Sheets");
                            // Tải lại dữ liệu và cập nhật UI
                            await RefreshFromDbAsync();
                        }
                        break; // thành công hoặc không có dữ liệu mới -> thoát retry
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Lỗi đồng bộ Google Sheets (lần {attempt + 1}): {ex.Message}");
                        if (attempt == maxRetries) break; // dừng sau lần cuối
                        await Task.Delay(delayBetweenRetriesMs);
                    }
                }
            });
        }

        private async Task RefreshFromDbAsync()
        {
            var data = await _controller.GetAllForCurrentUserAsync();
            var roomRepo = new RentedRoomRepository();
            
            _allMaintenances = new List<MaintenanceIncidentViewModel>();
            foreach (var i in data)
            {
                // Lấy tên phòng từ database
                string tenPhong = $"Phòng {i.MaPhong}"; // Mặc định
                try
                {
                    var room = await roomRepo.GetByIdAsync(i.MaPhong);
                    if (room != null && !string.IsNullOrEmpty(room.TenPhong))
                    {
                        tenPhong = room.TenPhong;
                    }
                }
                catch
                {
                    // Nếu không lấy được, dùng mặc định
                }

                _allMaintenances.Add(new MaintenanceIncidentViewModel
                {
                    MaSuCo = i.MaSuCo,
                    MaPhong = i.MaPhong,
                    TenPhong = tenPhong,
                    MoTaSuCo = i.MoTaSuCo,
                    NgayBaoCao = i.NgayBaoCao,
                    NgayCoTheSua = i.NgayCoTheSua,
                    TrangThai = i.TrangThai,
                    ChiPhi = i.ChiPhi
                });
            }

            // Reset trang và áp lại phân trang/sắp xếp trên thread UI
            PageIndex = 1;
            ApplySortAndPage();
        }

        private void ApplySortAndPage()
        {
            // Lọc theo tìm kiếm
            IEnumerable<MaintenanceIncidentViewModel> query = _allMaintenances;
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var keyword = SearchText.Trim().ToLowerInvariant();
                query = query.Where(x =>
                    (!string.IsNullOrEmpty(x.MoTaSuCo) && x.MoTaSuCo.ToLowerInvariant().Contains(keyword))
                    || x.MaPhong.ToString().Contains(keyword)
                    || x.MaSuCo.ToString().Contains(keyword)
                );
            }

            // Sắp xếp
            if (SortOrder == "newest")
                query = query.OrderByDescending(x => x.NgayBaoCao);
            else
                query = query.OrderBy(x => x.NgayBaoCao);

            // Tính tổng trang
            var total = query.Count();
            TotalPages = (int)Math.Ceiling(total / (double)PageSize);
            if (TotalPages == 0) TotalPages = 1;
            if (PageIndex > TotalPages) PageIndex = TotalPages;

            // Lấy trang hiện tại
            var skip = (PageIndex - 1) * PageSize;
            var pageItems = query.Skip(skip).Take(PageSize).ToList();

            Maintenances.Clear();
            foreach (var item in pageItems) Maintenances.Add(item);

            UpdatePagination(total, skip, pageItems.Count);
            System.Windows.Input.CommandManager.InvalidateRequerySuggested();
        }

        private void UpdatePagination(int total, int skip, int pageCount)
        {
            var from = total == 0 ? 0 : skip + 1;
            var to = skip + pageCount;
            PaginationText = $"Hiển thị {from} đến {to} trong {total}";
        }

        private void EditRow(MaintenanceIncidentViewModel? item)
        {
            if (item == null) return;
            item.BeginEdit();
        }
        private void CancelRow(MaintenanceIncidentViewModel? item)
        {
            if (item == null) return;
            item.CancelEdit();
        }
        private async Task SaveRow(MaintenanceIncidentViewModel? item)
        {
            if (item == null) return;
            await _controller.UpdateAsync(item);
            item.EndEdit();
            await LoadDataAsync();
            System.Windows.MessageBox.Show("Lưu thành công!", "Thành công", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }
        private async Task DeleteRow(MaintenanceIncidentViewModel? item)
        {
            if (item == null) return;
            var result = System.Windows.MessageBox.Show($"Bạn có chắc muốn xóa sự cố {item.MaSuCo}?", "Xác nhận xóa", System.Windows.MessageBoxButton.YesNo);
            if (result != System.Windows.MessageBoxResult.Yes) return;
            await _controller.DeleteAsync(item.MaSuCo);
            _allMaintenances.Remove(item);
            ApplySortAndPage();
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? prop = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
