using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QLKDPhongTro.BusinessLayer.Controllers;
using QLKDPhongTro.BusinessLayer.DTOs;
using QLKDPhongTro.DataLayer.Repositories;
using QLKDPhongTro.Presentation.Views.Windows;

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
        [ObservableProperty] private bool _isAddEditPanelVisible;
        [ObservableProperty] private RentedRoomDto _newRoom = new();

        [RelayCommand]
        private async Task LoadRooms()
        {
            try
            {
                IsLoading = true;
                var list = await _rentedRoomController.GetAllRoomsAsync();
                Rooms = new ObservableCollection<RentedRoomDto>(list ?? new());
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
            NewRoom = new RentedRoomDto();
            IsAddEditPanelVisible = true;
            var window = new AddRoomWindow(this);
            window.Owner = Application.Current.MainWindow; // thêm dòng này cho chắc
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
                GhiChu = SelectedRoom.GhiChu,
                SoGiuong = SelectedRoom.SoGiuong
            };
            IsAddEditPanelVisible = true;
            var window = new EditRoomWindow(this);
            if (window.ShowDialog() == true)
            {
                IsAddEditPanelVisible = false;
            }
        }

        [RelayCommand]
        private async Task SaveRoom()
        {
            // Validation
            if (NewRoom.MaPhong <= 0)
            {
                MessageBox.Show("Mã phòng phải là số nguyên dương.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrEmpty(NewRoom.TenPhong))
            {
                MessageBox.Show("Vui lòng điền tên phòng.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (NewRoom.DienTich < 5)
            {
                MessageBox.Show("Diện tích phải lớn hơn hoặc bằng 5 m².", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (NewRoom.GiaCoBan < 500000)
            {
                MessageBox.Show("Giá cơ bản phải lớn hơn hoặc bằng 500,000.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (NewRoom.SoGiuong <= 0)
            {
                MessageBox.Show("Số giường phải lớn hơn 0.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrEmpty(NewRoom.TrangThai))
            {
                MessageBox.Show("Vui lòng chọn trạng thái phòng.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                IsLoading = true;
                bool isAdding = !Rooms.Any(r => r.MaPhong == NewRoom.MaPhong);

                if (isAdding)
                {
                    var msg = await _rentedRoomController.CreateRoomAsync(NewRoom);
                    if (msg.Contains("thành công", StringComparison.OrdinalIgnoreCase))
                    {
                        Rooms.Add(NewRoom);
                        StatusMessage = msg;
                        MessageBox.Show(msg, "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                        IsAddEditPanelVisible = false;
                        Application.Current.Windows.OfType<AddRoomWindow>().FirstOrDefault()?.Close();
                    }
                    else
                    {
                        MessageBox.Show(msg ?? "Thêm phòng thất bại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    var ok = await _rentedRoomController.UpdateRoomAsync(NewRoom);
                    if (ok)
                    {
                        var room = Rooms.FirstOrDefault(r => r.MaPhong == NewRoom.MaPhong);
                        if (room != null)
                        {
                            room.TenPhong = NewRoom.TenPhong;
                            room.DienTich = NewRoom.DienTich;
                            room.GiaCoBan = NewRoom.GiaCoBan;
                            room.TrangThai = NewRoom.TrangThai;
                            room.GhiChu = NewRoom.GhiChu;
                            room.SoGiuong = NewRoom.SoGiuong;
                        }
                        StatusMessage = "Cập nhật phòng thành công.";
                        MessageBox.Show(StatusMessage, "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                        IsAddEditPanelVisible = false;
                        Application.Current.Windows.OfType<EditRoomWindow>().FirstOrDefault()?.Close();
                    }
                    else
                    {
                        MessageBox.Show("Cập nhật phòng thất bại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
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
        private void CancelAddEdit()
        {
            IsAddEditPanelVisible = false;
            NewRoom = new RentedRoomDto();
            Application.Current.Windows.OfType<AddRoomWindow>().FirstOrDefault()?.Close();
            Application.Current.Windows.OfType<EditRoomWindow>().FirstOrDefault()?.Close();
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
    }
}