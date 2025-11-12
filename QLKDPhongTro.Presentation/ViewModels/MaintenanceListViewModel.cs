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

namespace QLKDPhongTro.Presentation.ViewModels
{
    public class MaintenanceIncidentViewModel : MaintenanceIncident, INotifyPropertyChanged
    {
        private bool _isEditing;
        public bool IsEditing
        {
            get => _isEditing;
            set { _isEditing = value; OnPropertyChanged(); }
        }
        // Backup fields for cancel
        private int _oldMaPhong;
        private string _oldMoTaSuCo = string.Empty;
        private DateTime _oldNgayBaoCao;
        private string _oldTrangThai = string.Empty;
        private decimal _oldChiPhi;
        public void BeginEdit()
        {
            _oldMaPhong = MaPhong;
            _oldMoTaSuCo = MoTaSuCo;
            _oldNgayBaoCao = NgayBaoCao;
            _oldTrangThai = TrangThai;
            _oldChiPhi = ChiPhi;
            IsEditing = true;
        }
        public void CancelEdit()
        {
            MaPhong = _oldMaPhong;
            MoTaSuCo = _oldMoTaSuCo;
            NgayBaoCao = _oldNgayBaoCao;
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
            _controller = new MaintenanceController(maintenanceRepo, null, roomRepo);
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
            try
            {
                // Đồng bộ dữ liệu từ Google Sheets trước khi load
                var addedCount = await _controller.SyncFromGoogleSheetsAsync();
                if (addedCount > 0)
                {
                    // Có thể hiển thị thông báo nếu muốn
                    System.Diagnostics.Debug.WriteLine($"Đã thêm {addedCount} bảo trì mới từ Google Sheets");
                }
            }
            catch (Exception ex)
            {
                // Log lỗi nhưng không chặn việc load dữ liệu từ database
                System.Diagnostics.Debug.WriteLine($"Lỗi đồng bộ Google Sheets: {ex.Message}");
                // Có thể hiển thị thông báo cho người dùng nếu cần
                System.Windows.MessageBox.Show(
                    $"Không thể đồng bộ từ Google Sheets: {ex.Message}\nVui lòng kiểm tra kết nối internet và quyền truy cập sheet.",
                    "Cảnh báo",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Warning);
            }

            // Load dữ liệu từ database
            var data = await _controller.GetAllAsync();
            _allMaintenances = new List<MaintenanceIncidentViewModel>();
            foreach (var i in data)
                _allMaintenances.Add(new MaintenanceIncidentViewModel
                {
                    MaSuCo = i.MaSuCo,
                    MaPhong = i.MaPhong,
                    MoTaSuCo = i.MoTaSuCo,
                    NgayBaoCao = i.NgayBaoCao,
                    TrangThai = i.TrangThai,
                    ChiPhi = i.ChiPhi
                });
            // Reset trang nếu ngoài phạm vi
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
