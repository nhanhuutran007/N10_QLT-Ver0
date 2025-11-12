using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QLKDPhongTro.BusinessLayer.Controllers;
using QLKDPhongTro.BusinessLayer.DTOs;
using QLKDPhongTro.DataLayer.Repositories;
using QLKDPhongTro.Presentation.Commands;
using QLKDPhongTro.Presentation.Views.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.Diagnostics; // Added for logging

namespace QLKDPhongTro.Presentation.ViewModels
{
    public partial class RentedRoomViewModel : ObservableObject
    {
        private readonly RentedRoomController _rentedRoomController;
        private readonly DispatcherTimer _statusTimer;
        private List<RentedRoomDto> _allRooms = new();

        public RentedRoomViewModel()
        {
            var repo = new RentedRoomRepository();
            _rentedRoomController = new RentedRoomController(repo);

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

            // Initialize pagination commands
            PrevPageCommand = new RelayCommand(
                () => { if (PageIndex > 1) PageIndex--; },
                () => PageIndex > 1);
            NextPageCommand = new RelayCommand(
                () => { if (PageIndex < TotalPages) PageIndex++; },
                () => PageIndex < TotalPages);

            // Initialize row action commands
            EditRoomRowCommand = new RelayCommand<RentedRoomDto>(EditRoomFromRow);
            DeleteRoomRowCommand = new RelayCommand<RentedRoomDto>(async room => await DeleteRoomFromRow(room));
            ViewRoomRowCommand = new RelayCommand<RentedRoomDto>(ViewRoomFromRow);

            LoadRoomsCommand.Execute(null);
        }

        [ObservableProperty] private ObservableCollection<RentedRoomDto> _rooms = new();
        [ObservableProperty] private RentedRoomDto? _selectedRoom;
        [ObservableProperty] private bool _isLoading;
        [ObservableProperty] private string _statusMessage = string.Empty;
        [ObservableProperty] private RentedRoomDto _newRoom = new();

        // Phân trang
        private int _pageSize = 5;
        public int PageSize
        {
            get => _pageSize;
            set { _pageSize = value <= 0 ? 5 : value; OnPropertyChanged(nameof(PageSize)); ApplySortAndPage(); }
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

        // Tìm kiếm
        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set { _searchText = value ?? string.Empty; OnPropertyChanged(nameof(SearchText)); PageIndex = 1; ApplySortAndPage(); }
        }

        private string _paginationText = string.Empty;
        public string PaginationText
        {
            get => _paginationText;
            set { _paginationText = value; OnPropertyChanged(nameof(PaginationText)); }
        }

        // Lệnh phân trang
        public ICommand PrevPageCommand { get; }
        public ICommand NextPageCommand { get; }

        // Commands cho DataGrid row actions
        public ICommand EditRoomRowCommand { get; }
        public ICommand DeleteRoomRowCommand { get; }
        public ICommand ViewRoomRowCommand { get; }

        [ObservableProperty] private string _title = "Thêm phòng mới";
        [ObservableProperty] private string _buttonContent = "Thêm phòng";
        [ObservableProperty] private IAsyncRelayCommand _saveCommand = null!;

        public string[] StatusOptions { get; } = new[] { "Trống", "Đang thuê", "Đang bảo trì" };

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
                StatusMessage = $"Lỗi khi tải danh sách phòng: {ex.Message}";
                MessageBox.Show(StatusMessage, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        // Method để refresh dữ liệu mà không reset PageIndex
        private async Task RefreshRoomsData()
        {
            try
            {
                IsLoading = true;
                var rooms = await _rentedRoomController.GetAllRoomsAsync();
                _allRooms = rooms?.ToList() ?? new List<RentedRoomDto>();
                // Không reset PageIndex ở đây, để giữ nguyên trang hiện tại
            }
            catch (Exception ex)
            {
                StatusMessage = $"Lỗi khi tải lại danh sách phòng: {ex.Message}";
                MessageBox.Show(StatusMessage, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void ApplySortAndPage()
        {
            IEnumerable<RentedRoomDto> query = _allRooms;
            
            // Tìm kiếm
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var keyword = SearchText.Trim().ToLowerInvariant();
                query = query.Where(r =>
                    r.TenPhong.ToLowerInvariant().Contains(keyword) ||
                    r.MaPhong.ToString().ToLowerInvariant().Contains(keyword) ||
                    r.TrangThai.ToLowerInvariant().Contains(keyword) ||
                    (!string.IsNullOrEmpty(r.GhiChu) && r.GhiChu.ToLowerInvariant().Contains(keyword)) ||
                    r.DienTich.ToString().ToLowerInvariant().Contains(keyword) ||
                    r.GiaCoBan.ToString().ToLowerInvariant().Contains(keyword) ||
                    r.GiaCoBan.ToString("N0").ToLowerInvariant().Contains(keyword)
                );
            }

            // Sắp xếp theo MaPhong
            if (SortOrder == "newest")
                query = query.OrderByDescending(r => r.MaPhong);
            else
                query = query.OrderBy(r => r.MaPhong);

            var total = query.Count();
            TotalPages = (int)Math.Ceiling(total / (double)PageSize);
            if (TotalPages == 0) TotalPages = 1;
            if (PageIndex > TotalPages) PageIndex = TotalPages;

            var skip = (PageIndex - 1) * PageSize;
            var pageItems = query.Skip(skip).Take(PageSize).ToList();

            Rooms.Clear();
            foreach (var item in pageItems)
            {
                Rooms.Add(item);
            }

            var from = total == 0 ? 0 : skip + 1;
            var to = skip + pageItems.Count;
            PaginationText = $"Hiển thị {from} đến {to} trong {total}";
            System.Windows.Input.CommandManager.InvalidateRequerySuggested();
        }

        [RelayCommand]
        private void ShowAddRoomPanel()
        {
            NewRoom = new RentedRoomDto
            {
                TrangThai = "Trống"
            };

            Title = "Thêm phòng mới";
            ButtonContent = "Thêm phòng";
            SaveCommand = SaveRoomCommand;

            var window = new AddRoomWindow(this)
            {
                Owner = Application.Current.Windows.OfType<Window>().FirstOrDefault(x => x.IsActive) ?? Application.Current.MainWindow
            };
            window.ShowDialog();
        }

        [RelayCommand]
        private void ShowEditRoomPanel()
        {
            if (SelectedRoom == null)
            {
                MessageBox.Show("Vui lòng chọn một phòng để sửa.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

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

            Title = "Sửa thông tin phòng";
            ButtonContent = "Cập nhật";
            SaveCommand = UpdateRoomCommand;

            var window = new AddRoomWindow(this)
            {
                Owner = Application.Current.Windows.OfType<Window>().FirstOrDefault(x => x.IsActive) ?? Application.Current.MainWindow
            };
            window.ShowDialog();
        }

        [RelayCommand]
        private async Task SaveRoom()
        {
            if (!ValidateRoomData())
                return;

            if (NewRoom.TrangThai == "Đang bảo trì")
            {
                MessageBox.Show("Không thể thêm phòng mới với trạng thái 'Đang bảo trì'. Vui lòng chọn 'Trống' hoặc 'Đang thuê'.",
                    "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                IsLoading = true;
                var msg = await _rentedRoomController.CreateRoomAsync(NewRoom);
                if (msg != null && msg.Contains("thành công", StringComparison.OrdinalIgnoreCase))
                {
                    await LoadRooms();
                    StatusMessage = msg;
                    MessageBox.Show(msg, "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    CloseAddRoomWindows();
                }
                else
                {
                    MessageBox.Show(msg ?? "Thêm phòng thất bại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                _statusTimer.Start();
            }
            catch (Exception ex)
            {
                StatusMessage = $"Lỗi khi lưu phòng: {ex.Message}";
                MessageBox.Show(StatusMessage, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task UpdateRoom()
        {
            if (SelectedRoom == null)
            {
                MessageBox.Show("Vui lòng chọn một phòng để sửa.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!ValidateRoomData())
                return;

            try
            {
                IsLoading = true;
                Debug.WriteLine($"Updating room: MaPhong={NewRoom.MaPhong}, TrangThai={NewRoom.TrangThai}");
                
                // Lưu lại PageIndex hiện tại để giữ nguyên trang sau khi refresh
                var currentPageIndex = PageIndex;
                var currentMaPhong = NewRoom.MaPhong;
                
                var ok = await _rentedRoomController.UpdateRoomAsync(NewRoom);
                if (ok)
                {
                    // Reload data to ensure consistency with pagination
                    await RefreshRoomsData();
                    
                    // Khôi phục lại PageIndex và tìm lại phòng đã cập nhật
                    PageIndex = currentPageIndex;
                    ApplySortAndPage();
                    
                    // Tìm và chọn lại phòng đã cập nhật
                    var updatedRoom = Rooms.FirstOrDefault(r => r.MaPhong == currentMaPhong);
                    if (updatedRoom == null)
                    {
                        // Nếu không tìm thấy trong trang hiện tại, tìm trong toàn bộ danh sách và chuyển đến trang đó
                        var allRoomsQuery = _allRooms.AsEnumerable();
                        
                        // Áp dụng filter tương tự như trong ApplySortAndPage
                        if (!string.IsNullOrWhiteSpace(SearchText))
                        {
                            var keyword = SearchText.Trim().ToLowerInvariant();
                            allRoomsQuery = allRoomsQuery.Where(r =>
                                r.TenPhong.ToLowerInvariant().Contains(keyword) ||
                                r.MaPhong.ToString().ToLowerInvariant().Contains(keyword) ||
                                r.TrangThai.ToLowerInvariant().Contains(keyword) ||
                                (!string.IsNullOrEmpty(r.GhiChu) && r.GhiChu.ToLowerInvariant().Contains(keyword)) ||
                                r.DienTich.ToString().ToLowerInvariant().Contains(keyword) ||
                                r.GiaCoBan.ToString().ToLowerInvariant().Contains(keyword) ||
                                r.GiaCoBan.ToString("N0").ToLowerInvariant().Contains(keyword)
                            );
                        }
                        
                        // Áp dụng sort tương tự
                        if (SortOrder == "newest")
                            allRoomsQuery = allRoomsQuery.OrderByDescending(r => r.MaPhong);
                        else
                            allRoomsQuery = allRoomsQuery.OrderBy(r => r.MaPhong);
                        
                        var allRoomsList = allRoomsQuery.ToList();
                        var roomIndex = allRoomsList.FindIndex(r => r.MaPhong == currentMaPhong);
                        if (roomIndex >= 0)
                        {
                            // Tính toán trang chứa phòng này
                            var targetPage = (roomIndex / PageSize) + 1;
                            PageIndex = targetPage;
                            ApplySortAndPage();
                            updatedRoom = Rooms.FirstOrDefault(r => r.MaPhong == currentMaPhong);
                        }
                    }
                    
                    if (updatedRoom != null)
                    {
                        SelectedRoom = updatedRoom;
                    }
                    
                    StatusMessage = "Cập nhật phòng thành công.";
                    MessageBox.Show(StatusMessage, "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    CloseAddRoomWindows();
                }
                else
                {
                    Debug.WriteLine("UpdateRoomAsync returned false.");
                    MessageBox.Show("Cập nhật phòng thất bại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                _statusTimer.Start();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"UpdateRoom exception: {ex.Message}");
                StatusMessage = $"Lỗi khi cập nhật phòng: {ex.Message}";
                MessageBox.Show(StatusMessage, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task DeleteRoom()
        {
            if (SelectedRoom == null)
            {
                MessageBox.Show("Vui lòng chọn một phòng để xóa.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (SelectedRoom.TrangThai == "Đang thuê")
            {
                MessageBox.Show($"Không thể xóa phòng {SelectedRoom.TenPhong} (Mã: {SelectedRoom.MaPhong}) vì đang có người thuê.\nVui lòng kết thúc hợp đồng trước.",
                    "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var confirm = MessageBox.Show(
                $"Bạn có chắc muốn xóa phòng {SelectedRoom.TenPhong} (Mã: {SelectedRoom.MaPhong})?",
                "Xác nhận xóa",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (confirm != MessageBoxResult.Yes)
                return;

            try
            {
                IsLoading = true;
                var ok = await _rentedRoomController.DeleteRoomAsync(SelectedRoom.MaPhong);
                if (ok)
                {
                    // Reload data to ensure consistency with pagination
                    await LoadRooms();
                    SelectedRoom = null; // Clear selection after deletion
                    StatusMessage = "Đã xóa phòng thành công.";
                    MessageBox.Show(StatusMessage, "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Không thể xóa phòng.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                _statusTimer.Start();
            }
            catch (Exception ex)
            {
                StatusMessage = $"Lỗi khi xóa phòng: {ex.Message}";
                MessageBox.Show(StatusMessage, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private void ViewRoom()
        {
            if (SelectedRoom == null)
            {
                MessageBox.Show("Vui lòng chọn một phòng để xem.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            ShowRoomDetailsWindow();
        }

        public void ShowRoomDetailsWindow()
        {
            if (SelectedRoom == null)
            {
                MessageBox.Show("Vui lòng chọn một phòng để xem.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var viewRoomWindow = new ViewRoomWindow(this)
            {
                Owner = Application.Current.Windows.OfType<Window>().FirstOrDefault(x => x.IsActive) ?? Application.Current.MainWindow
            };
            viewRoomWindow.ShowDialog();
        }

        // Commands for DataGrid row actions
        private void EditRoomFromRow(RentedRoomDto? room)
        {
            if (room == null) return;
            SelectedRoom = room;
            ShowEditRoomPanel();
        }

        private async Task DeleteRoomFromRow(RentedRoomDto? room)
        {
            if (room == null) return;
            SelectedRoom = room;
            await DeleteRoom();
        }

        private void ViewRoomFromRow(RentedRoomDto? room)
        {
            if (room == null) return;
            SelectedRoom = room;
            ShowRoomDetailsWindow();
        }


        [RelayCommand]
        private void MinimizeWindow()
        {
            var window = Application.Current.Windows.OfType<RoomWindow>().FirstOrDefault();
            if (window != null)
            {
                window.WindowState = WindowState.Minimized;
            }
        }

        [RelayCommand]
        private void MaximizeWindow()
        {
            var window = Application.Current.Windows.OfType<RoomWindow>().FirstOrDefault();
            if (window != null)
            {
                window.WindowState = window.WindowState == WindowState.Maximized
                    ? WindowState.Normal
                    : WindowState.Maximized;
            }
        }

        [RelayCommand]
        private void CloseWindow()
        {
            var window = Application.Current.Windows.OfType<RoomWindow>().FirstOrDefault();
            window?.Close();
        }

        private bool ValidateRoomData()
        {
            if (string.IsNullOrEmpty(NewRoom.TenPhong))
            {
                MessageBox.Show("Vui lòng điền tên phòng.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (NewRoom.DienTich <= 0)
            {
                MessageBox.Show("Diện tích phải lớn hơn 0 m².", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (NewRoom.GiaCoBan < 0)
            {
                MessageBox.Show("Giá cơ bản không được âm.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (string.IsNullOrEmpty(NewRoom.TrangThai))
            {
                MessageBox.Show("Vui lòng chọn trạng thái phòng.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void CloseAddRoomWindows()
        {
            var addRoomWindows = Application.Current.Windows.OfType<AddRoomWindow>();
            foreach (var window in addRoomWindows)
            {
                window.Close();
            }
        }

        [RelayCommand]
        private void CancelAddEdit()
        {
            NewRoom = new RentedRoomDto();
            CloseAddRoomWindows();
        }
    }
}