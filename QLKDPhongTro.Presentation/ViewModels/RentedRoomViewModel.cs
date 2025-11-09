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
using System.Diagnostics; // Added for logging

namespace QLKDPhongTro.Presentation.ViewModels
{
    public partial class RentedRoomViewModel : ObservableObject
    {
        private readonly RentedRoomController _rentedRoomController;
        private readonly DispatcherTimer _statusTimer;
        private ObservableCollection<RentedRoomDto> _allRooms = new();

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

            LoadRoomsCommand.Execute(null);
        }

        [ObservableProperty] private ObservableCollection<RentedRoomDto> _rooms = new();
        [ObservableProperty] private RentedRoomDto? _selectedRoom;
        [ObservableProperty] private bool _isLoading;
        [ObservableProperty] private string _statusMessage = string.Empty;
        [ObservableProperty] private RentedRoomDto _newRoom = new();
        [ObservableProperty] private string _searchText = string.Empty;

        partial void OnSearchTextChanged(string value)
        {
            SearchRoomsCommand.Execute(null);
        }

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
                _allRooms.Clear();
                Rooms.Clear();
                if (rooms != null)
                {
                    foreach (var room in rooms)
                    {
                        _allRooms.Add(room);
                        Rooms.Add(room);
                    }
                }
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
                var ok = await _rentedRoomController.UpdateRoomAsync(NewRoom);
                if (ok)
                {
                    // Update the existing room in the Rooms collection
                    var room = Rooms.FirstOrDefault(r => r.MaPhong == NewRoom.MaPhong);
                    if (room != null)
                    {
                        int index = Rooms.IndexOf(room);
                        Rooms[index] = new RentedRoomDto
                        {
                            MaPhong = NewRoom.MaPhong,
                            TenPhong = NewRoom.TenPhong,
                            DienTich = NewRoom.DienTich,
                            GiaCoBan = NewRoom.GiaCoBan,
                            TrangThai = NewRoom.TrangThai,
                            GhiChu = NewRoom.GhiChu,
                            GiaBangChu = NewRoom.GiaBangChu,
                            TrangThietBi = NewRoom.TrangThietBi
                        };
                        Debug.WriteLine($"Room updated in collection: MaPhong={NewRoom.MaPhong}, TrangThai={NewRoom.TrangThai}");
                    }
                    else
                    {
                        // If the room is not found, reload the entire list
                        Debug.WriteLine("Room not found in collection, reloading rooms.");
                        await LoadRooms();
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
                    Rooms.Remove(SelectedRoom);
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

        [RelayCommand]
        private void SearchRooms()
        {
            try
            {
                if (string.IsNullOrEmpty(SearchText))
                {
                    Rooms = new ObservableCollection<RentedRoomDto>(_allRooms);
                }
                else
                {
                    var filteredList = _allRooms.Where(r =>
                        r.TenPhong.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                        r.MaPhong.ToString().Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                        r.TrangThai.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                        (r.GhiChu != null && r.GhiChu.Contains(SearchText, StringComparison.OrdinalIgnoreCase)));

                    Rooms = new ObservableCollection<RentedRoomDto>(filteredList);
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Lỗi khi tìm kiếm phòng: {ex.Message}";
                MessageBox.Show(StatusMessage, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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