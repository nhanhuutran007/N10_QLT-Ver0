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
    public partial class RentedRoomViewModel : ObservableObject
    {
        private readonly RentedRoomController _rentedRoomController;
        private readonly DispatcherTimer _statusTimer;

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

            LoadRoomsCommand.Execute(null);
        }

        [ObservableProperty] private ObservableCollection<RentedRoomDto> _rooms = new();
        [ObservableProperty] private RentedRoomDto? _selectedRoom;
        [ObservableProperty] private bool _isLoading;
        [ObservableProperty] private string _statusMessage = string.Empty;
        [ObservableProperty] private RentedRoomDto _newRoom = new();
        [ObservableProperty] private string _searchText = string.Empty;

        // Add status options for combobox
        public string[] StatusOptions { get; } = new[] { "Trống", "Đã thuê", "Bảo trì" };

        public async Task LoadRoomsAsync()
        {
            Rooms.Clear();
            var rooms = await _rentedRoomController.GetAllRoomsAsync();
            if (rooms != null)
            {
                foreach (var room in rooms)
                    Rooms.Add(room);
            }
        }

        [RelayCommand]
        private async Task LoadRooms()
        {
            try
            {
                IsLoading = true;
                var list = await _rentedRoomController.GetAllRoomsAsync();
                Rooms = list != null ? new ObservableCollection<RentedRoomDto>(list) : new ObservableCollection<RentedRoomDto>();
                StatusMessage = "Tải danh sách phòng trọ thành công.";
                _statusTimer.Start();
            }
            catch (Exception ex)
            {
                StatusMessage = $"Lỗi khi tải phòng: {ex.Message}";
                MessageBox.Show(StatusMessage, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private void ShowAddRoomPanel()
        {
            NewRoom = new RentedRoomDto
            {
                TrangThai = "Trống" // Set default status
            };
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
                MessageBox.Show("Vui lòng chọn phòng cần sửa.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            NewRoom = new RentedRoomDto
            {
                MaPhong = SelectedRoom.MaPhong,
                TenPhong = SelectedRoom.TenPhong,
                DienTich = SelectedRoom.DienTich,
                GiaCoBan = SelectedRoom.GiaCoBan,
                TrangThai = SelectedRoom.TrangThai,
                GhiChu = SelectedRoom.GhiChu
            };

            var window = new AddRoomWindow(this)
            {
                Owner = Application.Current.Windows.OfType<Window>().FirstOrDefault(x => x.IsActive) ?? Application.Current.MainWindow
            };
            window.ShowDialog();
        }

        [RelayCommand]
        private async Task SaveRoom()
        {
            // Validate data before saving
            if (!ValidateRoomData())
                return;

            try
            {
                IsLoading = true;

                // Ensure status is a valid string, not a control
                if (string.IsNullOrEmpty(NewRoom.TrangThai) || NewRoom.TrangThai.Length > 50)
                {
                    NewRoom.TrangThai = "Trống"; // Default value
                }

                var msg = await _rentedRoomController.CreateRoomAsync(NewRoom);
                if (msg.Contains("thành công", StringComparison.OrdinalIgnoreCase))
                {
                    await LoadRoomsAsync();
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
                MessageBox.Show("Vui lòng chọn phòng cần sửa.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!ValidateRoomData())
                return;

            try
            {
                IsLoading = true;

                // Ensure status is a valid string
                if (string.IsNullOrEmpty(NewRoom.TrangThai) || NewRoom.TrangThai.Length > 50)
                {
                    NewRoom.TrangThai = "Trống";
                }

                var ok = await _rentedRoomController.UpdateRoomAsync(NewRoom);
                if (ok)
                {
                    // Update the existing room in the collection
                    var room = Rooms.FirstOrDefault(r => r.MaPhong == NewRoom.MaPhong);
                    if (room != null)
                    {
                        room.TenPhong = NewRoom.TenPhong;
                        room.DienTich = NewRoom.DienTich;
                        room.GiaCoBan = NewRoom.GiaCoBan;
                        room.TrangThai = NewRoom.TrangThai;
                        room.GhiChu = NewRoom.GhiChu;
                    }

                    StatusMessage = "Cập nhật phòng thành công.";
                    MessageBox.Show(StatusMessage, "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    CloseAddRoomWindows();
                }
                else
                {
                    MessageBox.Show("Cập nhật phòng thất bại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                _statusTimer.Start();
            }
            catch (Exception ex)
            {
                StatusMessage = $"Lỗi khi cập nhật phòng: {ex.Message}";
                MessageBox.Show(StatusMessage, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private bool ValidateRoomData()
        {
            if (string.IsNullOrEmpty(NewRoom.TenPhong))
            {
                MessageBox.Show("Vui lòng điền tên phòng.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (NewRoom.DienTich < 5)
            {
                MessageBox.Show("Diện tích phải lớn hơn hoặc bằng 5 m².", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (NewRoom.GiaCoBan < 500000)
            {
                MessageBox.Show("Giá cơ bản phải lớn hơn hoặc bằng 500,000.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
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

        // ... rest of your methods (DeleteRoom, SearchRooms, etc.) remain the same
        [RelayCommand]
        private void ViewRoom()
        {
            if (SelectedRoom == null)
            {
                MessageBox.Show("Vui lòng chọn phòng để xem.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            ShowRoomDetailsWindow();
        }

        public void ShowRoomDetailsWindow()
        {
            if (SelectedRoom == null) return;

            var viewRoomWindow = new ViewRoomWindow(this)
            {
                Owner = Application.Current.Windows.OfType<Window>().FirstOrDefault(x => x.IsActive) ?? Application.Current.MainWindow
            };
            viewRoomWindow.ShowDialog();
        }

        [RelayCommand]
        private async Task DeleteRoom()
        {
            if (SelectedRoom == null)
            {
                MessageBox.Show("Vui lòng chọn phòng để xóa.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var confirm = MessageBox.Show($"Bạn có chắc muốn xóa phòng {SelectedRoom.TenPhong} (Mã: {SelectedRoom.MaPhong})?",
                "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (confirm != MessageBoxResult.Yes) return;

            try
            {
                IsLoading = true;
                var ok = await _rentedRoomController.DeleteRoomAsync(SelectedRoom.MaPhong);
                if (ok)
                {
                    Rooms.Remove(SelectedRoom);
                    SelectedRoom = null;
                    StatusMessage = "Xóa phòng thành công.";
                    MessageBox.Show(StatusMessage, "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    _statusTimer.Start();
                }
                else
                {
                    MessageBox.Show("Xóa phòng thất bại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
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
        private async Task SearchRooms()
        {
            try
            {
                IsLoading = true;
                var list = await _rentedRoomController.GetAllRoomsAsync();
                if (list != null)
                {
                    var filteredList = string.IsNullOrEmpty(SearchText)
                        ? list
                        : list.Where(r => r.TenPhong.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                                          r.MaPhong.ToString().Contains(SearchText, StringComparison.OrdinalIgnoreCase));
                    Rooms = new ObservableCollection<RentedRoomDto>(filteredList);
                }
                else
                {
                    Rooms = new ObservableCollection<RentedRoomDto>();
                }
                StatusMessage = "Tìm kiếm phòng thành công.";
                _statusTimer.Start();
            }
            catch (Exception ex)
            {
                StatusMessage = $"Lỗi khi tìm kiếm phòng: {ex.Message}";
                MessageBox.Show(StatusMessage, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task UpdateStatus(string newStatus)
        {
            if (SelectedRoom == null)
            {
                MessageBox.Show("Vui lòng chọn phòng để cập nhật trạng thái.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                IsLoading = true;
                var ok = await _rentedRoomController.UpdateRoomStatusAsync(SelectedRoom.MaPhong, newStatus);
                if (ok)
                {
                    SelectedRoom.TrangThai = newStatus;
                    StatusMessage = "Cập nhật trạng thái thành công.";
                    MessageBox.Show(StatusMessage, "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    _statusTimer.Start();
                }
                else
                {
                    MessageBox.Show("Cập nhật trạng thái thất bại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Lỗi khi cập nhật trạng thái: {ex.Message}";
                MessageBox.Show(StatusMessage, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private void MinimizeWindow()
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        [RelayCommand]
        private void MaximizeWindow()
        {
            Application.Current.MainWindow.WindowState =
                Application.Current.MainWindow.WindowState == WindowState.Maximized
                ? WindowState.Normal
                : WindowState.Maximized;
        }

        [RelayCommand]
        private void CloseWindow()
        {
            Application.Current.MainWindow.Close();
        }
    }
}