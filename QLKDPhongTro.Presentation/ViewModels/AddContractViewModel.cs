using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QLKDPhongTro.BusinessLayer.Controllers;
using QLKDPhongTro.BusinessLayer.DTOs;
using QLKDPhongTro.BusinessLayer.Services;
using QLKDPhongTro.DataLayer.Repositories;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace QLKDPhongTro.Presentation.ViewModels
{
    public partial class AddContractViewModel : ObservableObject
    {
        private readonly ContractController _contractController;
        private readonly TenantRepository _tenantRepo;
        private readonly RentedRoomRepository _roomRepo;

        private readonly ContractDto _editingContract; // null = thêm mới, khác null = cập nhật

        public event EventHandler<bool> RequestClose;

        public bool IsEditMode => _editingContract != null;

        // ---- MODEL CHO COMBOBOX ----
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

        // ---- DANH SÁCH CHO COMBOBOX ----
        public ObservableCollection<NguoiThueTmp> NguoiThueList { get; } = new();
        public ObservableCollection<PhongTmp> PhongList { get; } = new();

        // ---- BINDING PROPERTIES ----
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

        [ObservableProperty]
        private string _formTitle = "Thêm hợp đồng mới";

        // ---- CONSTRUCTOR ----
        public AddContractViewModel(ContractController contractController, ContractDto editingContract = null)
        {
            _contractController = contractController;
            _tenantRepo = new TenantRepository();
            _roomRepo = new RentedRoomRepository();
            _editingContract = editingContract;

            _ = LoadLookupsAsync();
        }

        private async Task LoadLookupsAsync()
        {
            try
            {
                NguoiThueList.Clear();
                PhongList.Clear();

                var tenants = await _tenantRepo.GetAllAsync();
                var rooms = await _roomRepo.GetAllAsync();

                foreach (var t in tenants)
                    NguoiThueList.Add(new NguoiThueTmp { MaNguoiThue = t.MaKhachThue, HoTen = t.HoTen });

                foreach (var r in rooms)
                    PhongList.Add(new PhongTmp { MaPhong = r.MaPhong, TenPhong = r.TenPhong });

                // Nếu đang chỉnh sửa, gán lại dữ liệu
                if (IsEditMode)
                {
                    _formTitle = "Cập nhật hợp đồng";

                    SelectedNguoiThue = NguoiThueList.FirstOrDefault(x => x.MaNguoiThue == _editingContract.MaNguoiThue);
                    SelectedPhong = PhongList.FirstOrDefault(x => x.MaPhong == _editingContract.MaPhong);
                    NgayBatDau = _editingContract.NgayBatDau;
                    NgayKetThuc = _editingContract.NgayKetThuc;
                    TienCoc = _editingContract.TienCoc.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void Cancel() => RequestClose?.Invoke(this, false);

        [RelayCommand]
        private async Task SaveAsync()
        {
            if (SelectedNguoiThue == null)
            {
                MessageBox.Show("⚠️ Vui lòng chọn Người thuê.");
                return;
            }
            if (SelectedPhong == null)
            {
                MessageBox.Show("⚠️ Vui lòng chọn Phòng.");
                return;
            }
            if (!NgayBatDau.HasValue || !NgayKetThuc.HasValue)
            {
                MessageBox.Show("⚠️ Vui lòng chọn Ngày bắt đầu và Ngày kết thúc.");
                return;
            }
            if (NgayKetThuc <= NgayBatDau)
            {
                MessageBox.Show("⚠️ Ngày kết thúc phải lớn hơn Ngày bắt đầu.");
                return;
            }
            if (!decimal.TryParse(TienCoc, out decimal tienCocValue))
            {
                MessageBox.Show("⚠️ Tiền cọc không hợp lệ.");
                return;
            }

            try
            {
                if (!IsEditMode)
                {
                    // --- THÊM MỚI ---
                    string filePath = ContractTemplateService.CreateContractFile(
                        SelectedNguoiThue.HoTen,
                        SelectedPhong.TenPhong,
                        NgayBatDau.Value,
                        NgayKetThuc.Value,
                        tienCocValue);

                    var newContract = new ContractDto
                    {
                        MaNguoiThue = SelectedNguoiThue.MaNguoiThue,
                        MaPhong = SelectedPhong.MaPhong,
                        NgayBatDau = NgayBatDau.Value,
                        NgayKetThuc = NgayKetThuc.Value,
                        TienCoc = tienCocValue,
                        FileHopDong = filePath,
                        TrangThai = "Hiệu lực",

                    };

                    await _contractController.CreateHopDongAsync(newContract);
                    MessageBox.Show("✅ Hợp đồng đã được thêm thành công!");
                }
                else
                {
                    // --- CẬP NHẬT ---
                    _editingContract.MaNguoiThue = SelectedNguoiThue.MaNguoiThue;
                    _editingContract.MaPhong = SelectedPhong.MaPhong;
                    _editingContract.NgayBatDau = NgayBatDau.Value;
                    _editingContract.NgayKetThuc = NgayKetThuc.Value;
                    _editingContract.TienCoc = tienCocValue;

                    await _contractController.UpdateHopDongAsync(_editingContract);
                    MessageBox.Show("✅ Hợp đồng đã được cập nhật thành công!");
                }

                RequestClose?.Invoke(this, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi khi lưu hợp đồng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
