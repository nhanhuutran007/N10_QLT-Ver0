using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QLKDPhongTro.BusinessLayer.Controllers;
using QLKDPhongTro.BusinessLayer.DTOs;
using QLKDPhongTro.DataLayer.Repositories;
using QLKDPhongTro.DataLayer.Models;
using QLKDPhongTro.Presentation.Views.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.Diagnostics;

namespace QLKDPhongTro.Presentation.ViewModels
{
    public partial class RentedRoomViewModel : ObservableObject
    {
        private readonly RentedRoomController _rentedRoomController;
        private readonly MaintenanceController _maintenanceController;
        private readonly ContractController _contractController; // Controller mới để lấy dữ liệu tài chính
        private readonly TenantController _tenantController;

        private readonly DispatcherTimer _statusTimer;
        private List<RentedRoomDto> _allRooms = new();

        public RentedRoomViewModel()
        {
            // Khởi tạo các Repository & Controller
            var roomRepo = new RentedRoomRepository();
            _rentedRoomController = new RentedRoomController(roomRepo);

            // Maintenance Controller
            _maintenanceController = new MaintenanceController(new MaintenanceRepository(), null, roomRepo);

            // Contract Controller (Quan trọng cho phần Tài chính)
            _contractController = new ContractController(new ContractRepository());
            _tenantController = new TenantController(new TenantRepository(), roomRepo);

            _statusTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(5)
            };
            _statusTimer.Tick += (s, e) =>
            {
                StatusMessage = string.Empty;
                _statusTimer.Stop();
            };

            _title = "Thêm phòng mới";
            _buttonContent = "Thêm phòng";
            _saveCommand = SaveRoomCommand;

            // Commands
            PrevPageCommand = new RelayCommand(() => { if (PageIndex > 1) PageIndex--; }, () => PageIndex > 1);
            NextPageCommand = new RelayCommand(() => { if (PageIndex < TotalPages) PageIndex++; }, () => PageIndex < TotalPages);

            EditRoomRowCommand = new RelayCommand<RentedRoomDto>(EditRoomFromRow);
            DeleteRoomRowCommand = new RelayCommand<RentedRoomDto>(async room => await DeleteRoomFromRow(room));
            ViewRoomRowCommand = new RelayCommand<RentedRoomDto>(ViewRoomFromRow); // Command xem chi tiết

            LoadRoomsCommand.Execute(null);
        }

        // === Properties ===
        [ObservableProperty] private ObservableCollection<RentedRoomDto> _rooms = new();
        [ObservableProperty] private RentedRoomDto? _selectedRoom;

        // Properties chi tiết cho ViewRoomWindow
        [ObservableProperty] private decimal _tamTinh;
        [ObservableProperty] private decimal _tienCocHienCo;
        [ObservableProperty] private decimal _tienCocConDu;
        [ObservableProperty] private decimal _tongTienTinhToan;
        [ObservableProperty] private string _tongTienHienThi = string.Empty;
        [ObservableProperty] private ObservableCollection<string> _selectedRoomAmenities = new();
        [ObservableProperty] private ObservableCollection<MaintenanceIncident> _selectedRoomMaintenance = new();
        [ObservableProperty] private string _currentTenantName = "Chưa có người thuê";
        [ObservableProperty] private string _currentTenantPhone = string.Empty;
        [ObservableProperty] private string _currentTenantStatus = "Trống";
        [ObservableProperty] private string _currentContractRange = string.Empty;
        [ObservableProperty] private string _tenantStatusNote = string.Empty;
        [ObservableProperty] private string _roomStatusDetail = string.Empty;
        [ObservableProperty] private bool _hasActiveTenant;
        [ObservableProperty] private ObservableCollection<RoomTenantDto> _currentTenants = new();
        [ObservableProperty] private int _openMaintenanceCount;
        [ObservableProperty] private bool _hasActiveContract = false;
        [ObservableProperty] private bool _needsContractWarning = false;

        public bool HasMaintenanceIssues => SelectedRoomMaintenance?.Count > 0;
        public bool HasCurrentTenants => CurrentTenants?.Count > 0;

        partial void OnCurrentTenantsChanged(ObservableCollection<RoomTenantDto> value)
        {
            OnPropertyChanged(nameof(HasCurrentTenants));
            UpdateRoomStatusDetail();
        }

        [ObservableProperty] private bool _isLoading;
        [ObservableProperty] private string _statusMessage = string.Empty;
        [ObservableProperty] private RentedRoomDto _newRoom = new();

        // Pagination & Filter Properties
        private int _pageSize = 5;
        public int PageSize { get => _pageSize; set { _pageSize = value <= 0 ? 5 : value; OnPropertyChanged(nameof(PageSize)); ApplySortAndPage(); } }
        private int _pageIndex = 1;
        public int PageIndex { get => _pageIndex; set { _pageIndex = value < 1 ? 1 : value; OnPropertyChanged(nameof(PageIndex)); ApplySortAndPage(); } }
        private int _totalPages = 1;
        public int TotalPages { get => _totalPages; set { _totalPages = value < 1 ? 1 : value; OnPropertyChanged(nameof(TotalPages)); } }
        private string _sortOrder = "newest";
        public string SortOrder { get => _sortOrder; set { _sortOrder = value; OnPropertyChanged(nameof(SortOrder)); ApplySortAndPage(); } }
        private string _searchText = string.Empty;
        public string SearchText { get => _searchText; set { _searchText = value ?? string.Empty; OnPropertyChanged(nameof(SearchText)); PageIndex = 1; ApplySortAndPage(); } }
        private string _paginationText = string.Empty;
        public string PaginationText { get => _paginationText; set { _paginationText = value; OnPropertyChanged(nameof(PaginationText)); } }

        public ICommand PrevPageCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand EditRoomRowCommand { get; }
        public ICommand DeleteRoomRowCommand { get; }
        public ICommand ViewRoomRowCommand { get; }

        [ObservableProperty] private string _title = "Thêm phòng mới";
        [ObservableProperty] private string _buttonContent = "Thêm phòng";
        [ObservableProperty] private IAsyncRelayCommand _saveCommand = null!;
        public string[] StatusOptions { get; } = new[] { "Trống", "Đang thuê", "Dự kiến", "Đang bảo trì" };


        // === LOGIC CHÍNH: TẢI DỮ LIỆU CHI TIẾT ===

        // Hàm được gọi tự động khi SelectedRoom thay đổi
        partial void OnSelectedRoomChanged(RentedRoomDto? value)
        {
            if (value == null)
            {
                ResetDetailProperties();
                return;
            }

            // 1. Parse tiện ích từ chuỗi (Dữ liệu tĩnh)
            SelectedRoomAmenities = new ObservableCollection<string>(ParseAmenities(value.TrangThietBi));

            // 2. Gọi hàm async để tải dữ liệu động từ DB (Bảo trì & Tài chính)
            // Sử dụng fire-and-forget pattern nhưng an toàn vì cập nhật trên UI Thread
            _ = LoadRoomRealDataAsync(value.MaPhong, value.GiaCoBan);
            UpdateRoomStatusDetail();
        }

        private void ResetDetailProperties()
        {
            TamTinh = 0;
            TienCocHienCo = 0;
            TienCocConDu = 0;
            TongTienTinhToan = 0;
            TongTienHienThi = "0 VNĐ";
            SelectedRoomAmenities = new ObservableCollection<string>();
            SelectedRoomMaintenance = new ObservableCollection<MaintenanceIncident>();
            OpenMaintenanceCount = 0;
            CurrentTenantName = "Chưa có người thuê";
            CurrentTenantPhone = string.Empty;
            CurrentTenantStatus = "Trống";
            CurrentContractRange = string.Empty;
            TenantStatusNote = string.Empty;
            RoomStatusDetail = string.Empty;
            HasActiveTenant = false;
            CurrentTenants = new ObservableCollection<RoomTenantDto>();
        }

        // Hàm tải dữ liệu thật từ Database
        private async Task LoadRoomRealDataAsync(int maPhong, decimal giaCoBan)
        {
            try
            {
                // A. Tải dữ liệu Bảo trì (Giữ nguyên)
                var incidents = await _maintenanceController.GetByRoomAsync(maPhong) ?? new List<MaintenanceIncident>();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    SelectedRoomMaintenance = new ObservableCollection<MaintenanceIncident>(incidents);
                    OpenMaintenanceCount = incidents.Count(i => !string.Equals(i.TrangThai, "Hoàn tất", StringComparison.OrdinalIgnoreCase));
                    OnPropertyChanged(nameof(HasMaintenanceIssues));
                });

                // B. Tải dữ liệu Tài chính (Hợp đồng)
                var activeContract = await _contractController.GetActiveContractByRoomIdAsync(maPhong);
                TenantDetailDto? tenantDetail = null;
                if (activeContract != null)
                {
                    tenantDetail = await _tenantController.GetTenantDetailAsync(activeContract.MaNguoiThue);
                }
                var roomTenants = await _tenantController.GetTenantsByRoomIdAsync(maPhong);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    CurrentTenants = new ObservableCollection<RoomTenantDto>(roomTenants);
                    var displayTenant = CurrentTenants.FirstOrDefault(t => t.IsContractHolder)
                        ?? CurrentTenants.FirstOrDefault();

                    // Kiểm tra xem phòng có người thuê nhưng chưa có hợp đồng
                    HasActiveContract = activeContract != null;
                    NeedsContractWarning = roomTenants != null && roomTenants.Any() && activeContract == null;

                    if (activeContract != null)
                    {
                        // === SỬA LỖI Ở ĐÂY ===

                        // 1. Sửa lỗi 'decimal' and 'decimal': 
                        // Kiểm tra nếu Giá thuê trong hợp đồng > 0 thì lấy, nếu bằng 0 (hoặc null trong DB convert về 0) thì lấy giá gốc
                        TamTinh = activeContract.GiaThue > 0 ? activeContract.GiaThue : giaCoBan;

                        // 2. Sửa lỗi 'decimal' and 'int':
                        // Vì TienCoc là decimal không null, gán trực tiếp, không dùng ??
                        TienCocHienCo = activeContract.TienCoc;
                    }
                    else
                    {
                        // Phòng trống: Lấy giá niêm yết, cọc = 0
                        TamTinh = giaCoBan;
                        TienCocHienCo = 0;
                    }

                    // Tính toán logic hiển thị (tiền cọc không tự trừ)
                    TongTienTinhToan = TamTinh;

                    // Format hiển thị
                    if (activeContract != null)
                    {
                        // Nếu đang thuê: hiển thị tổng số tiền phải thu theo hóa đơn
                        TongTienHienThi = $"{TamTinh:N0} VNĐ";
                    }
                    else
                    {
                        // Nếu trống: Chỉ hiển thị giá phòng
                        TongTienHienThi = $"{TamTinh:N0} VNĐ (Giá niêm yết)";
                    }

                    if (displayTenant != null)
                    {
                        HasActiveTenant = displayTenant.IsContractHolder;
                        CurrentTenantName = displayTenant.HoTen;
                        CurrentTenantPhone = displayTenant.SoDienThoai ?? string.Empty;
                        CurrentTenantStatus = displayTenant.TrangThaiNguoiThue;
                        CurrentContractRange = displayTenant.ContractRangeDisplay;
                    }
                    else
                    {
                        HasActiveTenant = false;
                        CurrentTenantName = "Chưa có người thuê";
                        CurrentTenantPhone = string.Empty;
                        CurrentTenantStatus = "Trống";
                        CurrentContractRange = string.Empty;
                    }

                    TenantStatusNote = tenantDetail?.StayInfo?.ConsistencyMessage
                        ?? (displayTenant != null
                            ? (displayTenant.IsContractHolder
                                ? "Người đứng tên đang cư trú."
                                : "Khách thuê lịch sử gần nhất.")
                            : "Phòng sẵn sàng cho khách mới.");
                    UpdateRoomStatusDetail();
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading room details: {ex.Message}");
            }
        }

        // === Các hàm CRUD & Helper ===

        [RelayCommand]
        private async Task LoadRooms()
        {
            try
            {
                IsLoading = true;
                var rooms = await _rentedRoomController.GetAllRoomsAsync();
                _allRooms = rooms?.ToList() ?? new List<RentedRoomDto>();
                PageIndex = 1;
                ApplySortAndPage();
            }
            catch (Exception ex)
            {
                StatusMessage = $"Lỗi: {ex.Message}";
                MessageBox.Show(StatusMessage);
            }
            finally { IsLoading = false; }
        }

        private async Task RefreshRoomsData()
        {
            try
            {
                IsLoading = true;
                var rooms = await _rentedRoomController.GetAllRoomsAsync();
                _allRooms = rooms?.ToList() ?? new List<RentedRoomDto>();
            }
            finally { IsLoading = false; }
        }

        private void ApplySortAndPage()
        {
            IEnumerable<RentedRoomDto> query = _allRooms;

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var keyword = SearchText.Trim().ToLowerInvariant();
                query = query.Where(r =>
                    r.TenPhong.ToLowerInvariant().Contains(keyword) ||
                    r.MaPhong.ToString().Contains(keyword) ||
                    r.TrangThai.ToLowerInvariant().Contains(keyword)
                );
            }

            if (SortOrder == "newest") query = query.OrderByDescending(r => r.MaPhong);
            else query = query.OrderBy(r => r.MaPhong);

            var total = query.Count();
            TotalPages = (int)Math.Ceiling(total / (double)PageSize);
            if (TotalPages == 0) TotalPages = 1;
            if (PageIndex > TotalPages) PageIndex = TotalPages;

            var pageItems = query.Skip((PageIndex - 1) * PageSize).Take(PageSize).ToList();

            Rooms.Clear();
            foreach (var item in pageItems) Rooms.Add(item);

            var from = total == 0 ? 0 : ((PageIndex - 1) * PageSize) + 1;
            var to = Math.Min(PageIndex * PageSize, total);
            PaginationText = $"Hiển thị {from} đến {to} trong {total}";
        }

        [RelayCommand]
        private async Task ShowAddRoomPanel()
        {
            var validationMessage = await _rentedRoomController.CheckCanCreateRoomAsync();
            if (!string.IsNullOrEmpty(validationMessage)) { MessageBox.Show(validationMessage); return; }

            NewRoom = new RentedRoomDto { TrangThai = "Trống" };
            Title = "Thêm phòng mới"; ButtonContent = "Thêm phòng"; SaveCommand = SaveRoomCommand;

            var window = new AddRoomWindow(this) { Owner = Application.Current.MainWindow };
            window.ShowDialog();
        }

        [RelayCommand]
        private void ShowEditRoomPanel()
        {
            if (SelectedRoom == null) return;

            // Clone object để sửa
            NewRoom = new RentedRoomDto
            {
                MaPhong = SelectedRoom.MaPhong,
                TenPhong = SelectedRoom.TenPhong,
                DienTich = SelectedRoom.DienTich,
                GiaCoBan = SelectedRoom.GiaCoBan,
                TrangThai = SelectedRoom.TrangThai,
                GhiChu = SelectedRoom.GhiChu,
                GiaBangChu = SelectedRoom.GiaBangChu,
                TrangThietBi = SelectedRoom.TrangThietBi
            };

            Title = "Sửa thông tin phòng"; ButtonContent = "Cập nhật"; SaveCommand = UpdateRoomCommand;
            var window = new AddRoomWindow(this) { Owner = Application.Current.MainWindow };
            window.ShowDialog();
        }

        [RelayCommand]
        private async Task SaveRoom()
        {
            if (!ValidateRoomData()) return;
            try
            {
                IsLoading = true;
                var msg = await _rentedRoomController.CreateRoomAsync(NewRoom);
                if (msg.Contains("thành công"))
                {
                    await LoadRooms();
                    MessageBox.Show(msg, "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    CloseAddRoomWindows();
                }
                else MessageBox.Show(msg);
            }
            finally { IsLoading = false; }
        }

        [RelayCommand]
        private async Task UpdateRoom()
        {
            if (!ValidateRoomData()) return;
            try
            {
                IsLoading = true;

                // Kiểm tra nếu trạng thái thay đổi, validate trước
                if (SelectedRoom != null && SelectedRoom.TrangThai != NewRoom.TrangThai)
                {
                    // FIX: Đổi tên biến thành statusSuccess và statusMessage để tránh trùng
                    var (statusSuccess, statusMessage) = await _rentedRoomController.UpdateRoomStatusAsync(NewRoom.MaPhong, NewRoom.TrangThai);
                    if (!statusSuccess)
                    {
                        MessageBox.Show(statusMessage ?? "Không thể cập nhật trạng thái phòng", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }

                // FIX: Đổi tên biến thành updateSuccess và updateMessage (hoặc giữ nguyên nếu bên trên đã đổi)
                var (updateSuccess, updateMessage) = await _rentedRoomController.UpdateRoomAsync(NewRoom);

                if (updateSuccess)
                {
                    await RefreshRoomsData();
                    ApplySortAndPage();
                    MessageBox.Show("Cập nhật thành công", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    CloseAddRoomWindows();
                }
                else
                {
                    MessageBox.Show(updateMessage ?? "Cập nhật thất bại", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            finally { IsLoading = false; }
        }

        // Hàm mở cửa sổ chi tiết (Được gọi từ nút 👁️ trên DataGrid)
        private void ViewRoomFromRow(RentedRoomDto? room)
        {
            if (room == null) return;
            SelectedRoom = room; // Sẽ kích hoạt OnSelectedRoomChanged tải dữ liệu

            var viewRoomWindow = new ViewRoomWindow(this)
            {
                Owner = Application.Current.MainWindow
            };
            viewRoomWindow.ShowDialog();
        }

        // Helper methods
        private void EditRoomFromRow(RentedRoomDto? room) { if (room != null) { SelectedRoom = room; ShowEditRoomPanel(); } }
        private async Task DeleteRoomFromRow(RentedRoomDto? room)
        {
            if (room != null)
            {
                SelectedRoom = room;
                var confirm = MessageBox.Show($"Xóa phòng {room.TenPhong}?", "Xác nhận", MessageBoxButton.YesNo);
                if (confirm == MessageBoxResult.Yes)
                {
                    var (success, errorMessage) = await _rentedRoomController.DeleteRoomAsync(room.MaPhong);
                    if (success)
                    {
                        await LoadRooms();
                        MessageBox.Show("Đã xóa phòng thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show(errorMessage ?? "Không thể xóa phòng", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
        }

        private void CloseAddRoomWindows() => Application.Current.Windows.OfType<AddRoomWindow>().ToList().ForEach(w => w.Close());
        [RelayCommand] private void CancelAddEdit() => CloseAddRoomWindows();

        private bool ValidateRoomData()
        {
            if (string.IsNullOrEmpty(NewRoom.TenPhong)) return false;
            if (NewRoom.DienTich <= 0 || NewRoom.GiaCoBan < 0) return false;
            return true;
        }

        private static IEnumerable<string> ParseAmenities(string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return Array.Empty<string>();
            return raw.Split(new[] { ',', ';', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                      .Select(s => s.Trim())
                      .Where(s => !string.IsNullOrEmpty(s));
        }

        private void UpdateRoomStatusDetail()
        {
            if (SelectedRoom == null ||
                !string.Equals(SelectedRoom.TrangThai, "Dự kiến", StringComparison.OrdinalIgnoreCase))
            {
                RoomStatusDetail = string.Empty;
                return;
            }

            var tenantStatuses = CurrentTenants?
                .Select(t => t.TrangThaiNguoiThue)
                .Where(s => !string.IsNullOrWhiteSpace(s)) ?? Enumerable.Empty<string>();

            var hasDeposit = tenantStatuses.Any(status =>
                string.Equals(status, "Đặt cọc", StringComparison.OrdinalIgnoreCase));
            var hasLeavingSoon = tenantStatuses.Any(status =>
                string.Equals(status, "Sắp trả phòng", StringComparison.OrdinalIgnoreCase));

            if (hasDeposit)
            {
                RoomStatusDetail = "Dự kiến người thuê chuẩn bị vào ở";
            }
            else if (hasLeavingSoon)
            {
                RoomStatusDetail = "Dự kiến người thuê rời đi";
            }
            else
            {
                RoomStatusDetail = "Phòng đang trong trạng thái dự kiến.";
            }
        }

        // Window control commands
        [RelayCommand] private void CloseWindow() => Application.Current.Windows.OfType<RoomWindow>().FirstOrDefault()?.Close();
        [RelayCommand] private void MinimizeWindow() { var w = Application.Current.Windows.OfType<RoomWindow>().FirstOrDefault(); if (w != null) w.WindowState = WindowState.Minimized; }
        [RelayCommand] private void MaximizeWindow() { var w = Application.Current.Windows.OfType<RoomWindow>().FirstOrDefault(); if (w != null) w.WindowState = w.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized; }
    }
}