using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using QLKDPhongTro.BusinessLayer.Controllers;
using QLKDPhongTro.BusinessLayer.DTOs;
using QLKDPhongTro.BusinessLayer.Services;
using QLKDPhongTro.DataLayer.Models;
using QLKDPhongTro.DataLayer.Repositories;
using System;
using System.Collections.ObjectModel;
using System.IO;
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
        private readonly UserRepository _userRepo;
        private readonly HouseRepository _houseRepo;

        private readonly ContractDto _editingContract; // null = thêm mới, khác null = cập nhật
        private readonly string _defaultContractFolder;
        private bool _hasCustomSavePath;

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
        private bool _hasSelectedRoom = false;

        [ObservableProperty]
        private DateTime? _ngayBatDau = DateTime.Today;

        [ObservableProperty]
        private DateTime? _ngayKetThuc = DateTime.Today.AddMonths(12);

        [ObservableProperty]
        private string _tienCoc = "0";

        [ObservableProperty]
        private string _dieuKhoanRieng;

        [ObservableProperty]
        private string _formTitle = "Thêm hợp đồng mới";

        [ObservableProperty]
        private string _contractSavePath;

        // ---- CONSTRUCTOR ----
        public AddContractViewModel(ContractController contractController, ContractDto editingContract = null)
        {
            _contractController = contractController;
            _tenantRepo = new TenantRepository();
            _roomRepo = new RentedRoomRepository();
            _userRepo = new UserRepository();
            _houseRepo = new HouseRepository();
            _editingContract = editingContract;
            _defaultContractFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "HopDongPhongTro");

            Directory.CreateDirectory(_defaultContractFolder);
            _hasCustomSavePath = false;
            ContractSavePath = GenerateDefaultSavePath();

            _ = LoadLookupsAsync();
        }

        private async Task LoadLookupsAsync()
        {
            try
            {
                NguoiThueList.Clear();
                PhongList.Clear();

                var rooms = await _roomRepo.GetAllAsync();
                var contractRepository = new ContractRepository();

                // Chỉ hiển thị phòng chưa có hợp đồng hiệu lực
                foreach (var r in rooms)
                {
                    // Kiểm tra xem phòng đã có hợp đồng hiệu lực chưa
                    var activeContract = await contractRepository.GetActiveByRoomIdAsync(r.MaPhong);
                    
                    // Nếu đang chỉnh sửa và phòng này là phòng đang chỉnh sửa, vẫn hiển thị
                    bool isEditingThisRoom = IsEditMode && _editingContract != null && _editingContract.MaPhong == r.MaPhong;
                    
                    // Chỉ thêm phòng nếu chưa có hợp đồng hoặc đang chỉnh sửa phòng đó
                    if (activeContract == null || isEditingThisRoom)
                    {
                        if (!PhongList.Any(p => p.MaPhong == r.MaPhong))
                        {
                            PhongList.Add(new PhongTmp { MaPhong = r.MaPhong, TenPhong = r.TenPhong });
                        }
                    }
                }

                // Nếu đang chỉnh sửa, gán lại dữ liệu
                if (IsEditMode)
                {
                    FormTitle = "Cập nhật hợp đồng";

                    // Chọn phòng trước, sau đó OnSelectedPhongChanged sẽ tự động load danh sách người thuê
                    SelectedPhong = PhongList.FirstOrDefault(x => x.MaPhong == _editingContract.MaPhong);
                    NgayBatDau = _editingContract.NgayBatDau;
                    NgayKetThuc = _editingContract.NgayKetThuc;

                    // Khi chỉnh sửa, giữ nguyên tiền cọc cũ
                    if (_editingContract.TienCoc > 0)
                    {
                        TienCoc = _editingContract.TienCoc.ToString("N0", System.Globalization.CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        _ = UpdateTienCocFromRoomAsync();
                    }

                    DieuKhoanRieng = _editingContract.GhiChu;
                    if (!string.IsNullOrWhiteSpace(_editingContract.FileHopDong))
                    {
                        ContractSavePath = _editingContract.FileHopDong;
                        _hasCustomSavePath = true;
                    }
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
        private void SelectSavePath()
        {
            try
            {
                var dialog = new SaveFileDialog
                {
                    Title = "Chọn nơi lưu hợp đồng",
                    Filter = "Word Document (*.docx)|*.docx",
                    FileName = Path.GetFileName(ContractSavePath) ?? "HopDong.docx",
                    InitialDirectory = Path.GetDirectoryName(ContractSavePath) ?? _defaultContractFolder,
                    AddExtension = true,
                    DefaultExt = ".docx"
                };

                if (dialog.ShowDialog() == true)
                {
                    var selectedPath = dialog.FileName;
                    ContractSavePath = selectedPath;
                    _hasCustomSavePath = true;
                    OnPropertyChanged(nameof(ContractSavePath));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Không thể chọn nơi lưu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

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
            // Parse tiền cọc (loại bỏ dấu phẩy nếu có)
            string cleanTienCoc = TienCoc?.Replace(",", "").Trim() ?? "0";
            if (!decimal.TryParse(cleanTienCoc, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out decimal tienCocValue) || tienCocValue <= 0)
            {
                MessageBox.Show("⚠️ Tiền cọc không hợp lệ. Vui lòng nhập số tiền hợp lệ.");
                return;
            }
            if (string.IsNullOrWhiteSpace(ContractSavePath))
            {
                MessageBox.Show("⚠️ Vui lòng chọn nơi lưu hợp đồng.");
                return;
            }

            var normalizedSavePath = Path.ChangeExtension(ContractSavePath, ".docx");
            var saveDirectory = Path.GetDirectoryName(normalizedSavePath);
            if (string.IsNullOrWhiteSpace(saveDirectory))
            {
                MessageBox.Show("⚠️ Đường dẫn lưu hợp đồng không hợp lệ.");
                return;
            }

            try
            {
                // === Lấy thông tin người thuê & phòng ===
                var tenant = await _tenantRepo.GetByIdAsync(SelectedNguoiThue.MaNguoiThue);
                var room = await _roomRepo.GetByIdAsync(SelectedPhong.MaPhong);

                if (tenant == null || room == null)
                {
                    MessageBox.Show("❌ Không tìm thấy thông tin người thuê hoặc phòng.");
                    return;
                }

                // === Lấy thông tin bên A (chủ nhà) từ admin hiện tại ===
                var currentUser = AuthController.CurrentUser;
                if (currentUser == null)
                {
                    MessageBox.Show("❌ Không xác định được tài khoản quản trị. Vui lòng đăng nhập lại.");
                    return;
                }

                var admin = await _userRepo.GetByMaAdminAsync(currentUser.MaAdmin) ?? currentUser;
                var house = room.MaNha != 0 ? await _houseRepo.GetByIdAsync(room.MaNha) : null;

                string tenA = string.IsNullOrWhiteSpace(admin.HoTen) ? admin.TenDangNhap : admin.HoTen;
                DateTime ngaySinhA = admin.NgaySinh ?? new DateTime(1980, 1, 1);
                string cccdA = string.IsNullOrWhiteSpace(admin.CCCD) ? "Đang cập nhật" : admin.CCCD;
                DateTime ngayCapA = admin.NgayCap ?? DateTime.Today;
                string noiCapA = string.IsNullOrWhiteSpace(admin.NoiCap) ? "Đang cập nhật" : admin.NoiCap;
                string diaChiA = !string.IsNullOrWhiteSpace(admin.DiaChi)
                    ? admin.DiaChi
                    : (house?.DiaChi ?? "Đang cập nhật");
                string dienThoaiA = string.IsNullOrWhiteSpace(admin.SoDienThoai) ? "Đang cập nhật" : admin.SoDienThoai;
                string noiTaoHopDong = !string.IsNullOrWhiteSpace(house?.TinhThanh)
                    ? house.TinhThanh
                    : (!string.IsNullOrWhiteSpace(house?.DiaChi)
                        ? house.DiaChi
                        : (admin.DiaChi ?? "TP.HCM"));

                // === Bên B (lấy từ người thuê) ===
                string tenB = tenant.HoTen;
                DateTime ngaySinhB = tenant.NgaySinh ?? DateTime.Now;
                string cccdB = tenant.CCCD;
                DateTime ngayCapB = tenant.NgayCap ?? DateTime.Now;
                string noiCapB = tenant.NoiCap ?? "";
                string diaChiB = tenant.DiaChi ?? "";
                string dienThoaiB = tenant.SoDienThoai ?? "";

                // === Thông tin phòng ===
                string tenPhong = room.TenPhong;
                string diaChiPhong = house?.DiaChi ?? diaChiA;
                decimal dienTich = room.DienTich;
                string trangThietBi = room.TrangThietBi ?? "";

                // === Giá thuê & thời hạn ===
                decimal giaThue = room.GiaCoBan;
                string giaBangChu = NumberToVietnameseText((long)giaThue);
                string ngayTraTien = "Ngày 05 hàng tháng";
                int thoiHanNam = Math.Max(1, NgayKetThuc.Value.Year - NgayBatDau.Value.Year);
                DateTime ngayGiaoNha = NgayBatDau.Value;
                string dieuKhoanRieng = string.IsNullOrWhiteSpace(DieuKhoanRieng) ? null : DieuKhoanRieng.Trim();

                // === Tạo file hợp đồng DOCX + PDF ===
                var contractFiles = ContractTemplateService.CreateContractFile(
                    noiTaoHopDong, DateTime.Now,       // Nơi tạo + Ngày tạo
                    tenA, ngaySinhA, cccdA, ngayCapA, noiCapA, diaChiA, dienThoaiA,
                    tenB, ngaySinhB, cccdB, ngayCapB, noiCapB, diaChiB, dienThoaiB,
                    tenPhong, diaChiPhong, dienTich, trangThietBi,
                    giaThue, giaBangChu, ngayTraTien, thoiHanNam, ngayGiaoNha, dieuKhoanRieng,
                    normalizedSavePath
                );
                string filePath = contractFiles.PdfPath ?? contractFiles.DocxPath;

                if (!IsEditMode)
                {
                    // --- Thêm mới ---
                    var newContract = new ContractDto
                    {
                        MaNguoiThue = tenant.MaKhachThue,
                        MaPhong = room.MaPhong,
                        NgayBatDau = NgayBatDau.Value,
                        NgayKetThuc = NgayKetThuc.Value,
                        TienCoc = tienCocValue,
                        FileHopDong = filePath,
                        TrangThai = "Hiệu lực",
                        GhiChu = dieuKhoanRieng
                    };

                    int maHopDong = await _contractController.CreateHopDongAsync(newContract);
                    MessageBox.Show("✅ Hợp đồng đã được thêm và tạo file thành công!");
                }
                else
                {
                    // --- Cập nhật ---
                    _editingContract.MaNguoiThue = tenant.MaKhachThue;
                    _editingContract.MaPhong = room.MaPhong;
                    _editingContract.NgayBatDau = NgayBatDau.Value;
                    _editingContract.NgayKetThuc = NgayKetThuc.Value;
                    _editingContract.TienCoc = tienCocValue;
                    _editingContract.FileHopDong = filePath;
                    _editingContract.GhiChu = dieuKhoanRieng;

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

        // ======= HÀM PHỤ: Chuyển số thành chữ tiếng Việt =======
        private static string NumberToVietnameseText(long number)
        {
            if (number == 0) return "Không đồng";

            string[] dv = { "", "nghìn", "triệu", "tỷ" };
            string[] cs = { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };

            string result = "";
            int i = 0;

            while (number > 0)
            {
                int threeDigits = (int)(number % 1000);
                number /= 1000;

                string block = DocBaChuSo(threeDigits, cs);
                if (block != "")
                {
                    result = $"{block} {dv[i]} {result}".Trim();
                }
                i++;
            }

            result = char.ToUpper(result[0]) + result.Substring(1) + " đồng";
            return result;
        }

        private static string DocBaChuSo(int number, string[] cs)
        {
            int tram = number / 100;
            int chuc = (number % 100) / 10;
            int donvi = number % 10;
            string result = "";

            if (tram > 0)
            {
                result += $"{cs[tram]} trăm ";
                if (chuc == 0 && donvi > 0)
                    result += "lẻ ";
            }

            if (chuc > 1)
            {
                result += $"{cs[chuc]} mươi ";
                if (donvi == 1) result += "mốt ";
                else if (donvi == 5) result += "lăm ";
                else if (donvi > 0) result += $"{cs[donvi]} ";
            }
            else if (chuc == 1)
            {
                result += "mười ";
                if (donvi == 1) result += "một ";
                else if (donvi == 5) result += "lăm ";
                else if (donvi > 0) result += $"{cs[donvi]} ";
            }
            else if (chuc == 0 && donvi > 0)
            {
                result += $"{cs[donvi]} ";
            }

            return result.Trim();
        }

        partial void OnSelectedNguoiThueChanged(NguoiThueTmp value)
        {
            RefreshSuggestedSavePath();
        }

        partial void OnSelectedPhongChanged(PhongTmp value)
        {
            RefreshSuggestedSavePath();
            HasSelectedRoom = value != null;

            // Khi chọn phòng, tự động set tiền cọc = giá thuê phòng
            if (value != null && !IsEditMode)
            {
                _ = UpdateTienCocFromRoomAsync();
                _ = LoadTenantsForRoomAsync(value.MaPhong);
            }
            else if (value == null)
            {
                // Xóa danh sách người thuê khi bỏ chọn phòng
                NguoiThueList.Clear();
                SelectedNguoiThue = null;
            }
        }

        private async Task LoadTenantsForRoomAsync(int maPhong)
        {
            try
            {
                NguoiThueList.Clear();
                SelectedNguoiThue = null;

                // Lấy danh sách người thuê trong phòng
                var roomTenants = await _tenantRepo.GetTenantsByRoomIdAsync(maPhong);
                
                foreach (var tenantInfo in roomTenants)
                {
                    NguoiThueList.Add(new NguoiThueTmp 
                    { 
                        MaNguoiThue = tenantInfo.MaNguoiThue, 
                        HoTen = tenantInfo.HoTen 
                    });
                }

                // Nếu đang chỉnh sửa, tự động chọn người thuê của hợp đồng
                if (IsEditMode && _editingContract != null)
                {
                    SelectedNguoiThue = NguoiThueList.FirstOrDefault(x => x.MaNguoiThue == _editingContract.MaNguoiThue);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi khi tải danh sách người thuê: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ĐÃ SỬA: Chỉ giữ lại 1 hàm UpdateTienCocFromRoomAsync duy nhất (phiên bản đầy đủ nhất)
        private async Task UpdateTienCocFromRoomAsync()
        {
            if (SelectedPhong == null)
            {
                TienCoc = "0";
                return;
            }

            try
            {
                var room = await _roomRepo.GetByIdAsync(SelectedPhong.MaPhong);
                if (room != null && room.GiaCoBan > 0)
                {
                    // Tiền cọc mặc định = giá thuê phòng
                    TienCoc = room.GiaCoBan.ToString("N0", System.Globalization.CultureInfo.InvariantCulture);
                }
                else
                {
                    TienCoc = "0";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi cập nhật tiền cọc: {ex.Message}");
                TienCoc = "0";
            }
        }

        private void RefreshSuggestedSavePath()
        {
            if (_hasCustomSavePath)
                return;

            ContractSavePath = GenerateDefaultSavePath();
        }

        private string GenerateDefaultSavePath()
        {
            var tenantPart = SanitizeFileSegment(SelectedNguoiThue?.HoTen, "NguoiThue");
            var roomPart = SanitizeFileSegment(SelectedPhong?.TenPhong, "Phong");
            var fileName = $"{tenantPart}_{roomPart}_{DateTime.Now:yyyyMMdd_HHmmss}.docx";
            return Path.Combine(_defaultContractFolder, fileName);
        }

        private static string SanitizeFileSegment(string value, string fallback)
        {
            var source = string.IsNullOrWhiteSpace(value) ? fallback : value;
            var invalidChars = Path.GetInvalidFileNameChars();
            var cleaned = new string(source.Where(c => !invalidChars.Contains(c)).ToArray());
            return string.IsNullOrWhiteSpace(cleaned) ? fallback : cleaned;
        }

        private static bool IsRoomCurrentlyRented(RentedRoom room)
        {
            if (room == null)
                return false;

            var status = room.TrangThai?.Trim();
            if (string.IsNullOrEmpty(status))
                return false;

            var normalized = status.ToLowerInvariant();
            return normalized == "đang thuê" || normalized == "dang thue" || normalized == "bảo trì" || normalized == "bao tri";
        }
    }
}