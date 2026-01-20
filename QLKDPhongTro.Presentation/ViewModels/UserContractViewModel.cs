using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using QLKDPhongTro.BusinessLayer.Controllers;
using QLKDPhongTro.BusinessLayer.Services;
using QLKDPhongTro.DataLayer.Repositories;
using QLKDPhongTro.Presentation.Models;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace QLKDPhongTro.Presentation.ViewModels
{
    public partial class UserContractViewModel : ObservableObject
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly IContractRepository _contractRepository;
        private readonly UserRepository _userRepo;
        private readonly HouseRepository _houseRepo;
        private readonly RentedRoomRepository _roomRepo;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private bool _hasData;

        [ObservableProperty]
        private ObservableCollection<ContractDisplayModel> _contracts = new();

        [ObservableProperty]
        private ContractDisplayModel? _selectedContract;

        public UserContractViewModel()
        {
            _tenantRepository = new TenantRepository();
            _contractRepository = new ContractRepository();
            _userRepo = new UserRepository();
            _houseRepo = new HouseRepository();
            _roomRepo = new RentedRoomRepository();
            _ = InitializeAsync();
        }

        [RelayCommand]
        private async Task DownloadContractAsync(ContractDisplayModel? contract)
        {
            if (contract == null)
            {
                MessageBox.Show("Vui lòng chọn hợp đồng để tải xuống.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                // Lấy thông tin chi tiết hợp đồng từ database
                var contractData = await _contractRepository.GetByIdAsync(contract.MaHopDong);
                if (contractData == null)
                {
                    MessageBox.Show("Không tìm thấy thông tin hợp đồng.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Lấy thông tin người thuê
                var tenant = await _tenantRepository.GetByIdAsync(contractData.MaNguoiThue);
                if (tenant == null)
                {
                    MessageBox.Show("Không tìm thấy thông tin người thuê.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Lấy thông tin phòng
                var room = await _roomRepo.GetByIdAsync(contractData.MaPhong);
                if (room == null)
                {
                    MessageBox.Show("Không tìm thấy thông tin phòng.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Lấy thông tin admin/chủ nhà
                var currentUser = AuthController.CurrentUser;
                var admin = currentUser != null ? await _userRepo.GetByMaAdminAsync(currentUser.MaAdmin) ?? currentUser : null;
                var house = room.MaNha != 0 ? await _houseRepo.GetByIdAsync(room.MaNha) : null;

                // Chuẩn bị thông tin bên A (chủ nhà)
                string tenA = admin?.HoTen ?? "Chủ nhà";
                DateTime ngaySinhA = (admin != null && admin.NgaySinh.HasValue) ? admin.NgaySinh.Value : new DateTime(1980, 1, 1);
                string cccdA = admin?.CCCD ?? "Đang cập nhật";
                DateTime ngayCapA = (admin != null && admin.NgayCap.HasValue) ? admin.NgayCap.Value : DateTime.Today;
                string noiCapA = admin?.NoiCap ?? "Đang cập nhật";
                string diaChiA = admin?.DiaChi ?? house?.DiaChi ?? "Đang cập nhật";
                string dienThoaiA = admin?.SoDienThoai ?? "Đang cập nhật";
                string noiTaoHopDong = house?.TinhThanh ?? house?.DiaChi ?? "TP.HCM";

                // Thông tin bên B (người thuê)
                string tenB = tenant.HoTen;
                DateTime ngaySinhB = tenant.NgaySinh.HasValue ? tenant.NgaySinh.Value : DateTime.Now;
                string cccdB = tenant.CCCD ?? "";
                DateTime ngayCapB = tenant.NgayCap.HasValue ? tenant.NgayCap.Value : DateTime.Now;
                string noiCapB = tenant.NoiCap ?? "";
                string diaChiB = tenant.DiaChi ?? "";
                string dienThoaiB = tenant.SoDienThoai ?? "";

                // Thông tin phòng
                string tenPhong = room.TenPhong;
                string diaChiPhong = house?.DiaChi ?? diaChiA;
                decimal dienTich = room.DienTich;
                string trangThietBi = room.TrangThietBi ?? "";

                // Thông tin tài chính
                decimal giaThue = room.GiaCoBan;
                string giaBangChu = NumberToVietnameseText((long)giaThue);
                string ngayTraTien = "Ngày 05 hàng tháng";
                int thoiHanNam = contractData.NgayKetThuc.Year - contractData.NgayBatDau.Year;
                thoiHanNam = thoiHanNam > 0 ? thoiHanNam : 1;
                DateTime ngayGiaoNha = contractData.NgayBatDau;
                string dieuKhoanRieng = contractData.GhiChu;

                // Mở SaveFileDialog để user chọn nơi lưu
                var saveFileDialog = new SaveFileDialog
                {
                    Title = "Lưu file hợp đồng",
                    FileName = $"HopDong_{contract.MaHopDong}_{contract.TenPhong}_{DateTime.Now:yyyyMMdd}.docx",
                    Filter = "Word Document (*.docx)|*.docx|PDF Document (*.pdf)|*.pdf",
                    DefaultExt = ".docx"
                };

                if (saveFileDialog.ShowDialog() != true)
                    return;

                // Tạo file hợp đồng
                var contractFiles = ContractTemplateService.CreateContractFile(
                    noiTaoHopDong, DateTime.Now,
                    tenA, ngaySinhA, cccdA, ngayCapA, noiCapA, diaChiA, dienThoaiA,
                    tenB, ngaySinhB, cccdB, ngayCapB, noiCapB, diaChiB, dienThoaiB,
                    tenPhong, diaChiPhong, dienTich, trangThietBi,
                    giaThue, giaBangChu, ngayTraTien, thoiHanNam, ngayGiaoNha, dieuKhoanRieng,
                    saveFileDialog.FileName
                );

                MessageBox.Show($"Đã tạo và lưu file hợp đồng thành công!\nVị trí: {saveFileDialog.FileName}", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tạo file hợp đồng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Hàm chuyển số thành chữ tiếng Việt
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

        private async Task InitializeAsync()
        {
            try
            {
                IsLoading = true;
                HasData = false;
                Contracts.Clear();

                // 1. Get current user's email
                var currentUser = AuthController.CurrentUser;
                if (currentUser == null || string.IsNullOrEmpty(currentUser.Email))
                {
                    IsLoading = false;
                    return;
                }

                // 2. Find Tenant by Email
                var tenant = await _tenantRepository.GetByEmailAsync(currentUser.Email);
                if (tenant == null || tenant.MaKhachThue == 0)
                {
                    IsLoading = false;
                    return;
                }

                // 3. Get ALL Contracts for this Tenant (not just active ones)
                var contracts = await _contractRepository.GetActiveContractsByTenantAsync(tenant.MaKhachThue);
                
                // Also try to get all contracts if the method exists, otherwise use active only
                // For now, we'll use GetActiveContractsByTenantAsync and order by date
                
                if (contracts == null || !contracts.Any())
                {
                    IsLoading = false;
                    return;
                }

                // 4. Convert to display models and add to collection
                var contractDisplayModels = contracts
                    .OrderByDescending(c => c.NgayBatDau) // Most recent first
                    .Select(contract => CreateDisplayModel(contract, tenant.HoTen))
                    .ToList();

                // Mark the most recent active contract
                var mostRecentActive = contractDisplayModels
                    .FirstOrDefault(c => c.TrangThai == "Hiệu lực");
                if (mostRecentActive != null)
                {
                    mostRecentActive.IsActive = true;
                }

                foreach (var contract in contractDisplayModels)
                {
                    Contracts.Add(contract);
                }

                // Auto-select the first (most recent) contract
                if (Contracts.Any())
                {
                    SelectedContract = Contracts.First();
                    HasData = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải thông tin hợp đồng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private ContractDisplayModel CreateDisplayModel(DataLayer.Models.Contract contract, string tenantName)
        {
            // Luôn cho phép tải xuống vì chúng ta sẽ tạo file động
            var model = new ContractDisplayModel
            {
                MaHopDong = contract.MaHopDong,
                MaHopDongDisplay = $"#{contract.MaHopDong}",
                TenPhong = contract.TenPhong ?? "N/A",
                TenNguoiThue = contract.TenNguoiThue ?? tenantName ?? "N/A",
                NgayBatDauRaw = contract.NgayBatDau,
                NgayKetThucRaw = contract.NgayKetThuc,
                NgayBatDau = contract.NgayBatDau.ToString("dd/MM/yyyy"),
                NgayKetThuc = contract.NgayKetThuc.ToString("dd/MM/yyyy"),
                TienCoc = $"{contract.TienCoc:N0} VNĐ",
                GiaThue = $"{contract.GiaThue:N0} VNĐ",
                TrangThai = contract.TrangThai ?? "N/A",
                GhiChu = string.IsNullOrWhiteSpace(contract.GhiChu) ? "Không có ghi chú" : contract.GhiChu,
                FileHopDong = "Có thể tải xuống", // Luôn có thể tải vì tạo động
                FileHopDongPath = string.Empty,
                HasFile = true // Luôn true vì có thể tạo file bất cứ lúc nào
            };

            // Calculate remaining days
            var remainingDays = (contract.NgayKetThuc - DateTime.Now).Days;
            if (remainingDays > 0)
            {
                model.SoNgayConLai = $"{remainingDays} ngày";
            }
            else if (remainingDays == 0)
            {
                model.SoNgayConLai = "Hết hạn hôm nay";
            }
            else
            {
                model.SoNgayConLai = $"Đã hết hạn {Math.Abs(remainingDays)} ngày";
            }

            // Set color based on status
            model.TrangThaiColor = contract.TrangThai switch
            {
                "Hiệu lực" => "#2E7D32",      // Green
                "Sắp hết hạn" => "#F57C00",   // Orange
                "Hết hạn" => "#C62828",       // Red
                "Hủy" => "#757575",           // Gray
                _ => "#3B4758"                // Default
            };

            return model;
        }
    }
}
