using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using QLKDPhongTro.BusinessLayer.Controllers;
using QLKDPhongTro.BusinessLayer.DTOs;

namespace QLKDPhongTro.Presentation.Views.Windows
{
    public partial class AddBillingInfoWindow : Window
    {
        private readonly FinancialController _financialController = FinancialController.CreateDefault();

        private int? _selectedCustomerId;

        public AddBillingInfoWindow()
        {
            InitializeComponent();
            Loaded += AddBillingInfoWindow_Loaded;
            // Đặt ngày mặc định là ngày hiện tại
            CreatedDatePicker.SelectedDate = DateTime.Now;
        }

        private async void AddBillingInfoWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var tenants = await _financialController.GetAllTenantsAsync();
                CustomerComboBox.ItemsSource = tenants.Select(t => new { Id = t.MaKhachThue, Name = t.HoTen }).ToList();
                CustomerComboBox.DisplayMemberPath = "Name";
                CustomerComboBox.SelectedValuePath = "Id";

                // Không load hợp đồng ban đầu, chỉ load khi chọn khách hàng
                ContractIdComboBox.ItemsSource = null;
                ContractIdComboBox.IsEnabled = false;
                RoomNameTextBox.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void CustomerComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                if (CustomerComboBox.SelectedValue == null)
                {
                    ContractIdComboBox.ItemsSource = null;
                    ContractIdComboBox.IsEnabled = false;
                    ContractIdComboBox.SelectedValue = null;
                    RoomNameTextBox.Text = "";
                    _selectedCustomerId = null;
                    return;
                }

                var maKhachThue = (int)CustomerComboBox.SelectedValue;
                _selectedCustomerId = maKhachThue;

                // Lấy hợp đồng của khách hàng được chọn
                var contracts = await _financialController.GetActiveContractsByTenantAsync(maKhachThue);

                if (contracts == null || contracts.Count == 0)
                {
                    MessageBox.Show("Người thuê chưa có hợp đồng, cần tạo hợp đồng trước rồi quay lại sau!!", 
                        "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    ContractIdComboBox.ItemsSource = null;
                    ContractIdComboBox.IsEnabled = false;
                    ContractIdComboBox.SelectedValue = null;
                    RoomNameTextBox.Text = "";
                    return;
                }

                // Lấy thông tin phòng cho từng hợp đồng để hiển thị
                var contractList = new List<dynamic>();
                foreach (var contract in contracts)
                {
                    var room = await _financialController.GetRoomByIdAsync(contract.MaPhong);
                    var roomName = room?.TenPhong ?? $"Phòng {contract.MaPhong}";
                    var displayName = contracts.Count > 1 
                        ? $"HD{contract.MaHopDong} - {roomName} ({contract.NgayBatDau:dd/MM/yyyy} - {contract.NgayKetThuc:dd/MM/yyyy})"
                        : $"HD{contract.MaHopDong} - {roomName}";
                    
                    contractList.Add(new { Id = contract.MaHopDong, Name = displayName });
                }

                // Cập nhật danh sách hợp đồng
                ContractIdComboBox.ItemsSource = contractList;
                ContractIdComboBox.DisplayMemberPath = "Name";
                ContractIdComboBox.SelectedValuePath = "Id";
                ContractIdComboBox.IsEnabled = true;

                // Tự động chọn hợp đồng đầu tiên nếu chỉ có 1 hợp đồng
                if (contracts.Count == 1)
                {
                    ContractIdComboBox.SelectedIndex = 0;
                    // Tự động điền tên phòng
                    var firstContract = contracts[0];
                    var room = await _financialController.GetRoomByIdAsync(firstContract.MaPhong);
                    RoomNameTextBox.Text = room?.TenPhong ?? $"Phòng {firstContract.MaPhong}";
                }
                else
                {
                    // Nếu có nhiều hợp đồng, xóa tên phòng cho đến khi chọn hợp đồng
                    RoomNameTextBox.Text = "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải hợp đồng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void ContractIdComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                if (ContractIdComboBox.SelectedValue == null)
                {
                    RoomNameTextBox.Text = "";
                    return;
                }

                var maHopDong = (int)ContractIdComboBox.SelectedValue;
                var contract = await _financialController.GetContractByIdAsync(maHopDong);
                
                if (contract != null)
                {
                    var room = await _financialController.GetRoomByIdAsync(contract.MaPhong);
                    RoomNameTextBox.Text = room?.TenPhong ?? $"Phòng {contract.MaPhong}";
                }
                else
                {
                    RoomNameTextBox.Text = "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải thông tin phòng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Kiểm tra khách hàng đã được chọn
                if (CustomerComboBox.SelectedValue == null)
                {
                    MessageBox.Show("Vui lòng chọn khách hàng", "Thiếu dữ liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (ContractIdComboBox.SelectedValue == null)
                {
                    MessageBox.Show("Vui lòng chọn mã hợp đồng", "Thiếu dữ liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Xác thực: hợp đồng phải thuộc về khách hàng đã chọn
                var selectedCustomerId = (int)CustomerComboBox.SelectedValue;
                var selectedContractId = (int)ContractIdComboBox.SelectedValue;

                var contract = await _financialController.GetContractByIdAsync(selectedContractId);
                if (contract == null || contract.MaNguoiThue != selectedCustomerId)
                {
                    MessageBox.Show("Hợp đồng không thuộc về khách hàng đã chọn. Vui lòng chọn lại!", 
                        "Lỗi xác thực", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Tháng/năm: lấy theo ngày chọn
                var date = CreatedDatePicker.SelectedDate ?? DateTime.Now;
                var thangNam = $"{date:MM/yyyy}";

                // Lấy chỉ số và đơn giá - mặc định là 0 nếu không nhập
                var soDien = ParseDecimalNullable(SoDienInput.Text) ?? 0;
                var donGiaDien = ParseDecimalNullable(DonGiaDienInput.Text) ?? 0;
                var soNuoc = ParseDecimalNullable(SoNuocInput.Text) ?? 0;
                var donGiaNuoc = ParseDecimalNullable(DonGiaNuocInput.Text) ?? 0;

                // Tính tiền từ chỉ số * đơn giá
                decimal tienDien = soDien * donGiaDien;
                decimal tienNuoc = soNuoc * donGiaNuoc;

                var dto = new CreatePaymentDto
                {
                    MaHopDong = (int)ContractIdComboBox.SelectedValue,
                    ThangNam = thangNam,
                    TienThue = ParseDecimal(TienThueInput.Text),
                    TienDien = tienDien,
                    TienNuoc = tienNuoc,
                    SoDien = soDien,
                    SoNuoc = soNuoc,
                    DonGiaDien = donGiaDien,
                    DonGiaNuoc = donGiaNuoc,
                    TienInternet = ParseDecimal(TienInternetInput.Text),
                    TienVeSinh = ParseDecimal(TienVeSinhInput.Text),
                    TienGiuXe = ParseDecimal(TienGiuXeInput.Text),
                    ChiPhiKhac = ParseDecimal(ChiPhiKhacInput.Text)
                };

                var result = await _financialController.CreatePaymentAsync(dto);
                if (!result.IsValid)
                {
                    MessageBox.Show(result.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                MessageBox.Show("Đã thêm thanh toán thành công", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi thêm thanh toán: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static decimal ParseDecimal(string? text)
        {
            if (decimal.TryParse(text, out var v)) return v;
            return 0m;
        }

        private static decimal? ParseDecimalNullable(string? text)
        {
            if (string.IsNullOrWhiteSpace(text)) return null;
            if (decimal.TryParse(text, out var v)) return v;
            return null;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}

