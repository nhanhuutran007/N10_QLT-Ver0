using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QLKDPhongTro.BusinessLayer.Controllers;
using QLKDPhongTro.BusinessLayer.DTOs;
using QLKDPhongTro.BusinessLayer.Services;
using QLKDPhongTro.DataLayer.Repositories;
using QLKDPhongTro.Presentation.Utils;
using QLKDPhongTro.Presentation.Views.Windows;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;

namespace QLKDPhongTro.Presentation.ViewModels
{
    public partial class AddContractViewModel : ObservableObject
    {
        private readonly ContractController _contractController;
        private readonly TenantRepository _tenantRepo;
        private readonly RentedRoomRepository _roomRepo;

        public event EventHandler<bool> RequestClose; // true = saved, false = canceled

        // Model hiển thị tạm trong ComboBox
        public class NguoiThueTmp
        {
            public int MaNguoiThue { get; set; }
            public string HoTen { get; set; }
        }

        public class PhongTmp
        {
            public int MaPhong { get; set; }
            public string TenPhong { get; set; }
        }

        public ObservableCollection<NguoiThueTmp> NguoiThueList { get; } = new();
        public ObservableCollection<PhongTmp> PhongList { get; } = new();

        [ObservableProperty]
        private NguoiThueTmp _selectedNguoiThue;

        [ObservableProperty]
        private PhongTmp _selectedPhong;

        [ObservableProperty]
        private DateTime? _ngayBatDau = DateTime.Today;

        [ObservableProperty]
        private DateTime? _ngayKetThuc = DateTime.Today.AddMonths(12);

        [ObservableProperty]
        private string _tienCoc;

        [ObservableProperty]
        private string _ghiChu;

        public AddContractViewModel(ContractController contractController)
        {
            _contractController = contractController;
            _tenantRepo = new TenantRepository();
            _roomRepo = new RentedRoomRepository();

            // gọi hàm load dữ liệu async
            _ = LoadLookupsAsync();
        }

        /// <summary>
        /// Load danh sách người thuê và phòng từ database
        /// </summary>
        private async Task LoadLookupsAsync()
        {
            try
            {
                NguoiThueList.Clear();
                PhongList.Clear();

                // lấy danh sách người thuê và phòng từ repo
                var tenants = await _tenantRepo.GetAllAsync();
                var rooms = await _roomRepo.GetAllAsync();

                foreach (var t in tenants)
                {
                    NguoiThueList.Add(new NguoiThueTmp
                    {
                        MaNguoiThue = t.MaKhachThue,
                        HoTen = t.HoTen
                    });
                }

                foreach (var r in rooms)
                {
                    PhongList.Add(new PhongTmp
                    {
                        MaPhong = r.MaPhong,
                        TenPhong = r.TenPhong
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void Cancel()
        {
            RequestClose?.Invoke(this, false);
        }

        [RelayCommand]
        private async Task SaveAsync()
        {
            if (SelectedNguoiThue == null)
            {
                MessageBox.Show("Vui lòng chọn Người thuê.");
                return;
            }
            if (SelectedPhong == null)
            {
                MessageBox.Show("Vui lòng chọn Phòng.");
                return;
            }
            if (!NgayBatDau.HasValue || !NgayKetThuc.HasValue)
            {
                MessageBox.Show("Vui lòng chọn ngày bắt đầu và kết thúc.");
                return;
            }
            if (NgayKetThuc <= NgayBatDau)
            {
                MessageBox.Show("Ngày kết thúc phải lớn hơn ngày bắt đầu.");
                return;
            }
            if (!decimal.TryParse(TienCoc, out decimal tienCocValue))
            {
                MessageBox.Show("Tiền cọc không hợp lệ.");
                return;
            }

            try
            {
                // 1️⃣ Tạo file hợp đồng
                string filePath = ContractTemplateService.CreateContractFile(
                    SelectedNguoiThue.HoTen,
                    SelectedPhong.TenPhong,
                    NgayBatDau.Value,
                    NgayKetThuc.Value,
                    tienCocValue);

                // 2️⃣ Lưu hợp đồng vào DB
                _contractController.CreateHopDongAsync(new QLKDPhongTro.BusinessLayer.DTOs.ContractDto
                {
                    MaNguoiThue = SelectedNguoiThue.MaNguoiThue,
                    MaPhong = SelectedPhong.MaPhong,
                    NgayBatDau = NgayBatDau.Value,
                    NgayKetThuc = NgayKetThuc.Value,
                    TienCoc = tienCocValue,
                    FileHopDong = filePath,
                    TrangThai = "Hiệu lực"
                });

                // 3️⃣ Thông báo và đóng form
                MessageBox.Show($"✅ Hợp đồng đã được tạo và lưu thành công!\nFile: {filePath}",
                    "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                RequestClose?.Invoke(this, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tạo hợp đồng:\n{ex.Message}",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
