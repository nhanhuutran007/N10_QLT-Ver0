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
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;

namespace QLKDPhongTro.Presentation.ViewModels
{
    public partial class TenantViewModel : ObservableObject
    {
        private readonly TenantController _tenantController;
        private readonly RentedRoomController _roomController;
        private readonly DispatcherTimer _statusTimer;
        private ObservableCollection<TenantDto> _allTenants = new();
        private List<TenantDto> _filteredTenants = new();

        public TenantViewModel()
        {
            // !! Lưu ý: Đảm bảo bạn có file TenantRepository.cs
            // và TenantController.cs trong các thư mục tương ứng
            var repo = new TenantRepository();
            _tenantController = new TenantController(repo);
            _roomController = new RentedRoomController(new RentedRoomRepository());

            _statusTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(5)
            };
            _statusTimer.Tick += (s, e) =>
            {
                StatusMessage = string.Empty;
                _statusTimer.Stop();
            };

            _title = "Thêm khách thuê mới";
            _buttonContent = "Thêm khách thuê";
            _saveCommand = SaveTenantCommand;

            LoadTenantsCommand.Execute(null);
            LoadRoomsCommand.Execute(null);
        }

        [ObservableProperty] private ObservableCollection<TenantDto> _tenants = new();
        [ObservableProperty] private TenantDto? _selectedTenant;
        [ObservableProperty] private bool _isLoading;
        [ObservableProperty] private string _statusMessage = string.Empty;
        [ObservableProperty] private TenantDto _newTenant = new();
        [ObservableProperty] private string _searchText = string.Empty;

        partial void OnSearchTextChanged(string value)
        {
            SearchTenantsCommand.Execute(null);
        }

        [ObservableProperty] private string _title = "Thêm khách thuê mới";
        [ObservableProperty] private string _buttonContent = "Thêm khách thuê";
        [ObservableProperty] private IAsyncRelayCommand _saveCommand = null!;

        // Danh sách phòng để chọn
        [ObservableProperty] private ObservableCollection<RentedRoomDto> _rooms = new();
        [ObservableProperty] private RentedRoomDto? _selectedRoom;

        public string[] GenderOptions { get; } = new[] { "Nam", "Nữ", "Khác" };
        public string[] TenantStatuses { get; } = new[] { "Đang ở", "Đặt cọc", "Sắp trả phòng", "Đã trả phòng" };

        // === Pagination & Sorting ===
        [ObservableProperty] private string _sortOrder = "newest"; // newest | oldest
        [ObservableProperty] private string _pageSize = "5"; // bind từ ComboBox Tag (string)
        [ObservableProperty] private int _pageIndex = 1;
        [ObservableProperty] private int _totalPages = 1;
        [ObservableProperty] private string _paginationText = string.Empty;

        partial void OnSortOrderChanged(string value)
        {
            ApplySort();
            UpdatePagination(resetPageIndex: true);
        }

        partial void OnPageSizeChanged(string value)
        {
            UpdatePagination(resetPageIndex: true);
        }

        // Điều hướng trang
        [RelayCommand]
        private void NextPage()
        {
            if (PageIndex < TotalPages)
            {
                PageIndex++;
                UpdatePagination(resetPageIndex: false);
            }
        }

        [RelayCommand]
        private void PrevPage()
        {
            if (PageIndex > 1)
            {
                PageIndex--;
                UpdatePagination(resetPageIndex: false);
            }
        }

        [RelayCommand]
        private async Task LoadTenants()
        {
            try
            {
                IsLoading = true;
                var tenants = await _tenantController.GetAllTenantsAsync();
                _allTenants.Clear();
                Tenants.Clear();
                if (tenants != null)
                {
                    foreach (var tenant in tenants)
                    {
                        _allTenants.Add(tenant);
                    }
                }

                // Đồng bộ filtered + áp dụng sắp xếp & phân trang
                _filteredTenants = _allTenants.ToList();
                ApplySort();
                PageIndex = 1;
                UpdatePagination(resetPageIndex: false);
            }
            catch (Exception ex)
            {
                StatusMessage = $"Lỗi khi tải danh sách khách thuê: {ex.Message}";
                MessageBox.Show(StatusMessage, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private void ShowAddTenantPanel()
        {
            NewTenant = new TenantDto
            {
                GioiTinh = "Nam",
                NgaySinh = DateTime.Today,
                NgayCap = DateTime.Today,
                TrangThai = TenantStatuses.First()
            };
            SelectedRoom = null; // Reset phòng được chọn

            Title = "Thêm khách thuê mới";
            ButtonContent = "Thêm khách thuê";
            SaveCommand = SaveTenantCommand;

            var window = new AddTenantWindow(this)
            {
                Owner = Application.Current.Windows.OfType<Window>().FirstOrDefault(x => x.IsActive) ?? Application.Current.MainWindow
            };
            window.ShowDialog();
        }

        [RelayCommand]
        private async Task LoadRooms()
        {
            try
            {
                var rooms = await _roomController.GetAllRoomsAsync();
                Rooms.Clear();
                foreach (var room in rooms)
                {
                    Rooms.Add(room);
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Lỗi khi tải danh sách phòng: {ex.Message}";
            }
        }

        [RelayCommand]
        private void ShowEditTenantPanel(TenantDto tenant)
        {
            if (tenant == null)
            {
                MessageBox.Show("Không tìm thấy thông tin khách thuê để chỉnh sửa.",
                                "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // ⭐⭐⭐ BẮT ĐẦU SỬA LỖI ⭐⭐⭐
            NewTenant = new TenantDto
            {
                MaKhachThue = tenant.MaKhachThue,
                HoTen = tenant.HoTen,
                CCCD = tenant.CCCD,
                SoDienThoai = tenant.SoDienThoai,
                Email = tenant.Email,
                DiaChi = tenant.DiaChi,
                NgaySinh = tenant.NgaySinh,
                GioiTinh = tenant.GioiTinh,
                NgheNghiep = tenant.NgheNghiep,
                GhiChu = tenant.GhiChu,
                MaPhong = tenant.MaPhong,
                TrangThai = tenant.TrangThai,

                // ✅ ĐÃ BỔ SUNG 2 DÒNG CÒN THIẾU
                NoiCap = tenant.NoiCap,
                NgayCap = tenant.NgayCap
            };
            // ⭐⭐⭐ KẾT THÚC SỬA LỖI ⭐⭐⭐

            // Chọn phòng tương ứng với MaPhong của tenant
            if (tenant.MaPhong.HasValue)
            {
                SelectedRoom = Rooms.FirstOrDefault(r => r.MaPhong == tenant.MaPhong.Value);
            }
            else
            {
                SelectedRoom = null;
            }

            Title = "Chỉnh sửa khách thuê";
            ButtonContent = "Cập nhật";
            SaveCommand = UpdateTenantCommand;

            var window = new AddTenantWindow(this)
            {
                Owner = Application.Current.Windows.OfType<Window>().FirstOrDefault(x => x.IsActive)
                         ?? Application.Current.MainWindow
            };
            window.ShowDialog();
        }


        [RelayCommand]
        private async Task SaveTenant()
        {
            try
            {
                IsLoading = true;

                // Validate số điện thoại trước khi gửi lên backend
                if (!ValidatePhoneNumber(NewTenant.SoDienThoai))
                {
                    IsLoading = false;
                    return;
                }

                // Gán MaPhong từ phòng được chọn
                if (SelectedRoom != null)
                {
                    NewTenant.MaPhong = SelectedRoom.MaPhong;
                }
                if (string.IsNullOrWhiteSpace(NewTenant.TrangThai))
                {
                    NewTenant.TrangThai = TenantStatuses.First();
                }

                var result = await _tenantController.CreateTenantAsync(NewTenant);

                if (result.IsValid)
                {
                    StatusMessage = result.Message;
                    await LoadTenants();

                    var addWindow = Application.Current.Windows.OfType<AddTenantWindow>().FirstOrDefault();
                    addWindow?.Close();
                }
                else
                {
                    StatusMessage = result.Message;
                    MessageBox.Show(result.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Lỗi khi thêm khách thuê: {ex.Message}";
                MessageBox.Show(StatusMessage, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task UpdateTenant()
        {
            try
            {
                IsLoading = true;

                // Validate số điện thoại trước khi gửi lên backend
                if (!ValidatePhoneNumber(NewTenant.SoDienThoai))
                {
                    IsLoading = false;
                    return;
                }

                // Gán MaPhong từ phòng được chọn
                if (SelectedRoom != null)
                {
                    NewTenant.MaPhong = SelectedRoom.MaPhong;
                }
                if (string.IsNullOrWhiteSpace(NewTenant.TrangThai))
                {
                    NewTenant.TrangThai = TenantStatuses.First();
                }

                var result = await _tenantController.UpdateTenantAsync(NewTenant);

                if (result.IsValid)
                {
                    StatusMessage = result.Message;
                    await LoadTenants();

                    var addWindow = Application.Current.Windows.OfType<AddTenantWindow>().FirstOrDefault();
                    addWindow?.Close();
                }
                else
                {
                    StatusMessage = result.Message;
                    MessageBox.Show(result.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Lỗi khi cập nhật khách thuê: {ex.Message}";
                MessageBox.Show(StatusMessage, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task DeleteTenant(TenantDto tenant)
        {
            if (tenant == null)
            {
                MessageBox.Show("Không tìm thấy khách thuê để xóa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Hiển thị dialog xác nhận với thông báo nhắc nhở về việc xóa dấu vân tay
            var confirmWindow = new DeleteTenantConfirmWindow(tenant)
            {
                Owner = Application.Current?.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive) ?? Application.Current.MainWindow
            };

            bool? dialogResult = confirmWindow.ShowDialog();

            // Chỉ xóa khi admin đã tích checkbox xác nhận và bấm "Xác nhận xóa"
            if (dialogResult != true || !confirmWindow.IsConfirmed)
            {
                return;
            }

            try
            {
                IsLoading = true;
                var result = await _tenantController.DeleteTenantAsync(tenant.MaKhachThue);

                if (result.IsValid)
                {
                    // Nếu cần tạo hợp đồng mới
                    if (result.RequiresNewContract && result.RemainingTenants.Any() && result.OldContract != null)
                    {
                        // Hiển thị dialog chọn người thuê mới
                        var selectWindow = new SelectNewContractHolderWindow(result.RemainingTenants)
                        {
                            Owner = Application.Current?.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive) ?? Application.Current.MainWindow
                        };

                        bool? selectResult = selectWindow.ShowDialog();

                        if (selectResult == true && selectWindow.SelectedTenant != null)
                        {
                            // Tạo hợp đồng mới với người thuê được chọn
                            await CreateNewContractForRoomAsync(
                                selectWindow.SelectedTenant.MaKhachThue,
                                result.MaPhong.Value,
                                result.OldContract);

                            StatusMessage = result.Message + " Đã tạo hợp đồng mới với người thuê được chọn.";
                        }
                        else
                        {
                            StatusMessage = result.Message + " Lưu ý: Cần tạo hợp đồng mới cho phòng này.";
                        }
                    }
                    else
                    {
                        StatusMessage = result.Message;
                    }

                    await LoadTenants();
                }
                else
                {
                    MessageBox.Show(result.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xóa khách thuê: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Kiểm tra số điện thoại: phải có 10 chữ số và bắt đầu bằng '0'
        /// </summary>
        private bool ValidatePhoneNumber(string? phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
            {
                MessageBox.Show("Số điện thoại không được để trống.", "Thông tin bắt buộc", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            var normalized = phone.Trim();
            if (normalized.Length != 10 || !normalized.All(char.IsDigit))
            {
                MessageBox.Show("Số điện thoại phải gồm đúng 10 chữ số.", "Thông tin không hợp lệ", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!normalized.StartsWith("0"))
            {
                MessageBox.Show("Số điện thoại phải bắt đầu từ số 0.", "Thông tin không hợp lệ", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            NewTenant.SoDienThoai = normalized; // chuẩn hóa
            return true;
        }

        private async Task CreateNewContractForRoomAsync(int maNguoiThue, int maPhong, ContractDto oldContract)
        {
            try
            {
                // Lấy thông tin người thuê mới
                var tenant = await _tenantController.GetTenantByIdAsync(maNguoiThue);
                if (tenant == null)
                {
                    MessageBox.Show("Không tìm thấy thông tin người thuê.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Lấy thông tin phòng
                var rooms = await _roomController.GetAllRoomsAsync();
                var roomInfo = rooms.FirstOrDefault(r => r.MaPhong == maPhong);
                if (roomInfo == null)
                {
                    MessageBox.Show("Không tìm thấy thông tin phòng.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Lấy thông tin admin và nhà
                var currentUser = BusinessLayer.Controllers.AuthController.CurrentUser;
                if (currentUser == null)
                {
                    MessageBox.Show("Không xác định được tài khoản quản trị.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var userRepo = new DataLayer.Repositories.UserRepository();
                var houseRepo = new DataLayer.Repositories.HouseRepository();
                var admin = await userRepo.GetByMaAdminAsync(currentUser.MaAdmin) ?? currentUser;
                var house = roomInfo.MaNha > 0 ? await houseRepo.GetByIdAsync(roomInfo.MaNha) : null;

                // Thông tin bên A (chủ nhà)
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

                // Thông tin bên B (người thuê mới)
                string tenB = tenant.HoTen;
                DateTime ngaySinhB = tenant.NgaySinh ?? DateTime.Now;
                string cccdB = tenant.CCCD ?? "";
                DateTime ngayCapB = tenant.NgayCap ?? DateTime.Now;
                string noiCapB = tenant.NoiCap ?? "";
                string diaChiB = tenant.DiaChi ?? "";
                string dienThoaiB = tenant.SoDienThoai ?? "";

                // Thông tin phòng
                string tenPhong = roomInfo.TenPhong;
                string diaChiPhong = house?.DiaChi ?? diaChiA;
                decimal dienTich = (decimal)roomInfo.DienTich;
                string trangThietBi = roomInfo.TrangThietBi ?? "";

                // Giá thuê & thời hạn
                decimal giaThue = oldContract.GiaThue > 0 ? oldContract.GiaThue : roomInfo.GiaCoBan;
                string giaBangChu = NumberToVietnameseText((long)giaThue);
                string ngayTraTien = "Ngày 05 hàng tháng";
                DateTime ngayBatDau = DateTime.Today;
                DateTime ngayKetThuc = oldContract.NgayKetThuc > DateTime.Today
                    ? oldContract.NgayKetThuc
                    : DateTime.Today.AddYears(1);
                int thoiHanNam = Math.Max(1, ngayKetThuc.Year - ngayBatDau.Year);
                DateTime ngayGiaoNha = ngayBatDau;
                string dieuKhoanRieng = $"Hợp đồng mới được tạo sau khi xóa người thuê cũ (HD#{oldContract.MaHopDong})";

                // Tạo đường dẫn file hợp đồng mới - sử dụng thư mục của hợp đồng cũ
                string contractFilePath;
                if (!string.IsNullOrWhiteSpace(oldContract.FileHopDong) && File.Exists(oldContract.FileHopDong))
                {
                    // Lấy thư mục từ file hợp đồng cũ
                    string oldContractFolder = Path.GetDirectoryName(oldContract.FileHopDong);
                    if (!string.IsNullOrWhiteSpace(oldContractFolder) && Directory.Exists(oldContractFolder))
                    {
                        // Tạo file mới trong cùng thư mục với file cũ
                        string safeTenant = new string(tenB.Where(c => !Path.GetInvalidFileNameChars().Contains(c)).ToArray());
                        string safeRoom = new string(tenPhong.Where(c => !Path.GetInvalidFileNameChars().Contains(c)).ToArray());
                        string fileName = $"{safeTenant}_{safeRoom}_{DateTime.Now:yyyyMMdd_HHmmss}.docx";
                        contractFilePath = Path.Combine(oldContractFolder, fileName);
                    }
                    else
                    {
                        // Nếu thư mục cũ không tồn tại, dùng thư mục mặc định
                        string defaultFolder = Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                            "HopDongPhongTro");
                        Directory.CreateDirectory(defaultFolder);
                        string safeTenant = new string(tenB.Where(c => !Path.GetInvalidFileNameChars().Contains(c)).ToArray());
                        string safeRoom = new string(tenPhong.Where(c => !Path.GetInvalidFileNameChars().Contains(c)).ToArray());
                        string fileName = $"{safeTenant}_{safeRoom}_{DateTime.Now:yyyyMMdd_HHmmss}.docx";
                        contractFilePath = Path.Combine(defaultFolder, fileName);
                    }
                }
                else
                {
                    // Nếu không có file hợp đồng cũ, dùng thư mục mặc định
                    string defaultFolder = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                        "HopDongPhongTro");
                    Directory.CreateDirectory(defaultFolder);
                    string safeTenant = new string(tenB.Where(c => !Path.GetInvalidFileNameChars().Contains(c)).ToArray());
                    string safeRoom = new string(tenPhong.Where(c => !Path.GetInvalidFileNameChars().Contains(c)).ToArray());
                    string fileName = $"{safeTenant}_{safeRoom}_{DateTime.Now:yyyyMMdd_HHmmss}.docx";
                    contractFilePath = Path.Combine(defaultFolder, fileName);
                }

                // Tạo file hợp đồng DOCX + PDF
                var contractFiles = BusinessLayer.Services.ContractTemplateService.CreateContractFile(
                    noiTaoHopDong, DateTime.Now,
                    tenA, ngaySinhA, cccdA, ngayCapA, noiCapA, diaChiA, dienThoaiA,
                    tenB, ngaySinhB, cccdB, ngayCapB, noiCapB, diaChiB, dienThoaiB,
                    tenPhong, diaChiPhong, dienTich, trangThietBi,
                    giaThue, giaBangChu, ngayTraTien, thoiHanNam, ngayGiaoNha, dieuKhoanRieng,
                    contractFilePath
                );
                string filePath = contractFiles.PdfPath ?? contractFiles.DocxPath;

                // Tạo hợp đồng mới trong database
                var contractController = new BusinessLayer.Controllers.ContractController(
                    new DataLayer.Repositories.ContractRepository());

                var newContract = new ContractDto
                {
                    MaNguoiThue = maNguoiThue,
                    MaPhong = maPhong,
                    NgayBatDau = ngayBatDau,
                    NgayKetThuc = ngayKetThuc,
                    TienCoc = oldContract.TienCoc,
                    GiaThue = giaThue,
                    FileHopDong = filePath,
                    TrangThai = "Hiệu lực",
                    GhiChu = dieuKhoanRieng
                };

                await contractController.CreateHopDongAsync(newContract);

                // Tự động mở thư mục chứa file hợp đồng
                try
                {
                    string folderPath = Path.GetDirectoryName(filePath);
                    if (!string.IsNullOrWhiteSpace(folderPath) && Directory.Exists(folderPath))
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = folderPath,
                            UseShellExecute = true
                        });

                        MessageBox.Show(
                            $"✅ Đã tạo hợp đồng mới và file hợp đồng thành công!\n\nĐã mở thư mục chứa file.\n\nĐường dẫn: {filePath}",
                            "Thông báo",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information
                        );
                    }
                    else
                    {
                        MessageBox.Show(
                            $"✅ Đã tạo hợp đồng mới và file hợp đồng thành công!\n\nĐường dẫn: {filePath}",
                            "Thông báo",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information
                        );
                    }
                }
                catch
                {
                    MessageBox.Show(
                        $"✅ Đã tạo hợp đồng mới và file hợp đồng thành công!\n\nĐường dẫn: {filePath}",
                        "Thông báo",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tạo hợp đồng mới: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Hàm phụ: Chuyển số thành chữ tiếng Việt
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


        [RelayCommand]
        private async Task SearchTenants()
        {
            try
            {
                IsLoading = true;
                if (string.IsNullOrWhiteSpace(SearchText))
                {
                    _filteredTenants = _allTenants.ToList();
                }
                else
                {
                    var searchResults = await _tenantController.SearchTenantsByNameAsync(SearchText);
                    _filteredTenants = searchResults.ToList();
                }

                ApplySort();
                PageIndex = 1;
                UpdatePagination(resetPageIndex: false);
            }
            catch (Exception ex)
            {
                StatusMessage = $"Lỗi khi tìm kiếm: {ex.Message}";
                MessageBox.Show(StatusMessage, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        public void ShowTenantDetailsWindow()
        {
            if (SelectedTenant == null)
            {
                MessageBox.Show("Vui lòng chọn khách thuê để xem chi tiết", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var detailVm = new TenantDetailViewModel(_tenantController);
            var detailWindow = new TenantDetailWindow(SelectedTenant.MaKhachThue, detailVm)
            {
                Owner = Application.Current.MainWindow
            };
            detailWindow.ShowDialog();
        }

        // === Row-level Commands khớp với XAML ===
        [RelayCommand]
        private void ViewTenantRow(TenantDto tenant)
        {
            if (tenant == null) return;
            SelectedTenant = tenant;
            ShowTenantDetailsWindow();
        }

        [RelayCommand]
        private void EditTenantRow(TenantDto tenant)
        {
            if (tenant == null) return;
            ShowEditTenantPanel(tenant);
        }

        [RelayCommand]
        private async Task DeleteTenantRow(TenantDto tenant)
        {
            if (tenant == null) return;
            await DeleteTenant(tenant);
        }

        // === Helpers: sort + paginate ===
        private void ApplySort()
        {
            IEnumerable<TenantDto> src = _filteredTenants;
            if (string.Equals(SortOrder, "oldest", StringComparison.OrdinalIgnoreCase))
            {
                src = src.OrderBy(t => t.NgayTao);
            }
            else
            {
                src = src.OrderByDescending(t => t.NgayTao);
            }
            _filteredTenants = src.ToList();
        }

        private void UpdatePagination(bool resetPageIndex)
        {
            int size = 5;
            if (!int.TryParse(PageSize, out size) || size <= 0) size = 5;

            TotalPages = Math.Max(1, (int)Math.Ceiling((_filteredTenants.Count) / (double)size));
            if (resetPageIndex) PageIndex = 1;
            if (PageIndex > TotalPages) PageIndex = TotalPages;
            if (PageIndex < 1) PageIndex = 1;

            Tenants.Clear();
            if (_filteredTenants.Count > 0)
            {
                int start = (PageIndex - 1) * size;
                var pageItems = _filteredTenants.Skip(start).Take(size);
                foreach (var item in pageItems)
                {
                    Tenants.Add(item);
                }
                int end = Math.Min(start + size, _filteredTenants.Count);
                PaginationText = $"Hiển thị {start + 1}-{end} của {_filteredTenants.Count}";
            }
            else
            {
                PaginationText = "Không có dữ liệu";
            }
        }
    }
}