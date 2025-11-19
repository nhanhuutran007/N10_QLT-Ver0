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
                // === Lấy thông tin người thuê & phòng ===
                var tenant = await _tenantRepo.GetByIdAsync(SelectedNguoiThue.MaNguoiThue);
                var room = await _roomRepo.GetByIdAsync(SelectedPhong.MaPhong);

                if (tenant == null || room == null)
                {
                    MessageBox.Show("❌ Không tìm thấy thông tin người thuê hoặc phòng.");
                    return;
                }

                // === Giả định thông tin bên A (chủ nhà) ===
                string tenA = "Nguyễn Văn A";
                string cccdA = "012345678901";
                string noiCapA = "CA TP.HCM";
                DateTime ngaySinhA = new DateTime(1980, 1, 1);
                DateTime ngayCapA = new DateTime(2020, 1, 1);
                string diaChiA = "123 Đường ABC, Quận 1, TP.HCM";
                string dienThoaiA = "0909123456";

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
                string diaChiPhong = diaChiA ?? "";
                decimal dienTich = room.DienTich;
                string trangThietBi = room.TrangThietBi ?? "";

                // === Giá thuê & thời hạn ===
                decimal giaThue = room.GiaCoBan;
                string giaBangChu = NumberToVietnameseText((long)giaThue);
                string ngayTraTien = "Ngày 05 hàng tháng";
                int thoiHanNam = Math.Max(1, NgayKetThuc.Value.Year - NgayBatDau.Value.Year);
                DateTime ngayGiaoNha = NgayBatDau.Value;

                // === Tạo file hợp đồng DOCX + PDF ===
                var contractFiles = ContractTemplateService.CreateContractFile(
                    "TP.HCM", DateTime.Now,       // Nơi tạo + Ngày tạo
                    tenA, ngaySinhA, cccdA, ngayCapA, noiCapA, diaChiA, dienThoaiA,
                    tenB, ngaySinhB, cccdB, ngayCapB, noiCapB, diaChiB, dienThoaiB,
                    tenPhong, diaChiPhong, dienTich, trangThietBi,
                    giaThue, giaBangChu, ngayTraTien, thoiHanNam, ngayGiaoNha
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
                        TrangThai = "Hiệu lực"
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
    }
}
