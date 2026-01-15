using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using QLKDPhongTro.BusinessLayer.Controllers;
using QLKDPhongTro.BusinessLayer.DTOs;
using QLKDPhongTro.DataLayer.Repositories;
using QLKDPhongTro.Presentation.Utils;

namespace QLKDPhongTro.Presentation.Views.Windows
{
    public partial class HouseInfoWindow : Window
    {
        private readonly HouseController _houseController;
        private int _maNha;

        public HouseInfoWindow()
        {
            InitializeComponent();
            _houseController = new HouseController(new HouseRepository());
            Loaded += HouseInfoWindow_Loaded;
            Closed += HouseInfoWindow_Closed;
        }

        private async void HouseInfoWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (AuthController.CurrentUser == null)
                {
                    MessageBox.Show("Vui lòng đăng nhập lại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    Close();
                    return;
                }

                _maNha = AuthController.CurrentUser.MaNha;
                await LoadHouseInfoAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải thông tin nhà: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task LoadHouseInfoAsync()
        {
            var houses = await _houseController.GetAllHousesAsync();
            var house = houses.Find(h => h.MaNha == _maNha);
            if (house == null)
            {
                MessageBox.Show("Không tìm thấy thông tin nhà cho tài khoản hiện tại.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Chỉ hiển thị mã nhà, không kèm địa chỉ
            MaNhaTextBox.Text = house.MaNha.ToString();
            DiaChiTextBox.Text = house.DiaChi;
            TinhThanhTextBox.Text = house.TinhThanh;
            TongSoPhongTextBox.Text = house.TongSoPhong.ToString();
            GhiChuTextBox.Text = house.GhiChu;
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Xóa trạng thái lỗi cũ
                TongSoPhongTextBox.ClearValue(System.Windows.Controls.Control.BorderBrushProperty);
                TongSoPhongTextBox.ClearValue(System.Windows.Controls.Control.BorderThicknessProperty);
                TongSoPhongTextBox.ToolTip = null;
                TinhThanhTextBox.ClearValue(System.Windows.Controls.Control.BorderBrushProperty);
                TinhThanhTextBox.ClearValue(System.Windows.Controls.Control.BorderThicknessProperty);

                if (!int.TryParse(TongSoPhongTextBox.Text, out var tongSoPhong))
                {
                    TongSoPhongTextBox.BorderBrush = System.Windows.Media.Brushes.Red;
                    TongSoPhongTextBox.BorderThickness = new Thickness(1);
                    TongSoPhongTextBox.ToolTip = "Tổng số phòng phải là số nguyên từ 1 đến 10.";
                    MessageBox.Show("Tổng số phòng phải là số nguyên.", "Lỗi dữ liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var tinhThanh = TinhThanhTextBox.Text?.Trim() ?? string.Empty;
                if (string.IsNullOrWhiteSpace(tinhThanh))
                {
                    MessageBox.Show("Vui lòng nhập Tỉnh/Thành phố.", "Lỗi dữ liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
                    TinhThanhTextBox.BorderBrush = System.Windows.Media.Brushes.Red;
                    TinhThanhTextBox.BorderThickness = new Thickness(1);
                    return;
                }

                // Ràng buộc: 1 <= TongSoPhong <= 10
                if (tongSoPhong < 1 || tongSoPhong > 10)
                {
                    TongSoPhongTextBox.BorderBrush = System.Windows.Media.Brushes.Red;
                    TongSoPhongTextBox.BorderThickness = new Thickness(1);
                    TongSoPhongTextBox.ToolTip = "Tổng số phòng phải từ 1 đến 10.";
                    MessageBox.Show("Tổng số phòng phải từ 1 đến 10.", "Lỗi dữ liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Không cho giảm tổng số phòng xuống nhỏ hơn số phòng hiện có trong bảng Phong
                var roomRepo = new RentedRoomRepository();
                var roomsOfHouse = await roomRepo.GetAllByMaNhaAsync(_maNha);
                var currentRoomCount = roomsOfHouse?.Count ?? 0;

                if (tongSoPhong < currentRoomCount)
                {
                    TongSoPhongTextBox.BorderBrush = System.Windows.Media.Brushes.Red;
                    TongSoPhongTextBox.BorderThickness = new Thickness(1);
                    TongSoPhongTextBox.ToolTip = $"Hiện nhà đang có {currentRoomCount} phòng, tổng số phòng không được nhỏ hơn {currentRoomCount}.";
                    MessageBox.Show($"Hiện nhà đang có {currentRoomCount} phòng. Tổng số phòng không được nhỏ hơn số phòng hiện có.",
                        "Lỗi dữ liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var dto = new HouseDto
                {
                    MaNha = _maNha,
                    DiaChi = DiaChiTextBox.Text?.Trim() ?? string.Empty,
                    TinhThanh = tinhThanh,
                    TongSoPhong = tongSoPhong,
                    GhiChu = GhiChuTextBox.Text?.Trim() ?? string.Empty
                };

                var success = await _houseController.UpdateHouseAsync(dto);
                if (success)
                {
                    MessageBox.Show("Cập nhật thông tin nhà thành công.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    DialogResult = true;
                    Close();
                }
                else
                {
                    MessageBox.Show("Cập nhật thông tin nhà thất bại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu thông tin nhà: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void HouseInfoWindow_Closed(object? sender, EventArgs e)
        {
            // Kiểm tra và đóng ứng dụng nếu không còn cửa sổ nào mở
            WindowHelper.CheckAndShutdownIfNoWindows(this);
        }
    }
}
