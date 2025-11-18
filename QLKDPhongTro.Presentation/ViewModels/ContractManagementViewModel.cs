using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QLKDPhongTro.BusinessLayer.Controllers;
using QLKDPhongTro.BusinessLayer.DTOs;
using QLKDPhongTro.BusinessLayer.Services;
using QLKDPhongTro.DataLayer.Repositories;
using QLKDPhongTro.Presentation.Views.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using Google.Apis.Drive.v3;
using Google.Apis.Upload;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using PdfSharp.Fonts;

namespace QLKDPhongTro.Presentation.ViewModels
{
    // Kế thừa ObservableObject để sử dụng SetProperty
    public partial class ContractManagementViewModel : ObservableObject
    {
        private readonly ContractController _contractController;
        private readonly GoogleFormService _googleFormService;

        // Danh sách đầy đủ và danh sách trang hiện tại
        private List<ContractDto> _allContracts = new();
        private ObservableCollection<ContractDto> _contracts;
        public ObservableCollection<ContractDto> Contracts
        {
            get => _contracts;
            set => SetProperty(ref _contracts, value);
        }

        [ObservableProperty]
        private ContractDto _selectedContract;

        partial void OnSelectedContractChanged(ContractDto value)
        {
            EditContractCommand.NotifyCanExecuteChanged();
            DeleteContractCommand.NotifyCanExecuteChanged();
        }

        // Phân trang
        private int _pageSize = 10;
        public string PageSize
        {
            get => _pageSize.ToString();
            set 
            { 
                string stringValue = value?.ToString() ?? string.Empty;
                if (!string.IsNullOrEmpty(stringValue) && int.TryParse(stringValue, out int size) && size > 0)
                {
                    _pageSize = size;
                    OnPropertyChanged(nameof(PageSize));
                    ApplySortAndPage();
                }
            }
        }
        private int _pageIndex = 1; // 1-based
        public int PageIndex
        {
            get => _pageIndex;
            set { _pageIndex = value < 1 ? 1 : value; OnPropertyChanged(); ApplySortAndPage(); }
        }
        private int _totalPages = 1;
        public int TotalPages
        {
            get => _totalPages;
            set { _totalPages = value < 1 ? 1 : value; OnPropertyChanged(); }
        }

        // Sắp xếp: newest | oldest (bind từ ComboBox SelectedValue Tag)
        [ObservableProperty]
        private string _sortOrder = "newest";

        // Tìm kiếm
        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set { _searchText = value ?? string.Empty; OnPropertyChanged(); PageIndex = 1; ApplySortAndPage(); }
        }

        private string _paginationText = string.Empty;
        public string PaginationText
        {
            get => _paginationText;
            set { _paginationText = value; OnPropertyChanged(); }
        }

        public ContractManagementViewModel()
        {
            _contractController = new ContractController(new ContractRepository());
            _googleFormService = new GoogleFormService();
            _contracts = new ObservableCollection<ContractDto>();
            _ = LoadContractsAsync();
        }

        // 🔹 Load toàn bộ danh sách hợp đồng
        private async Task LoadContractsAsync()
        {
            try
            {
                var contractList = await _contractController.GetAllHopDongAsync();
                _allContracts = contractList.ToList();
                
                // Clear selection nếu item đã bị xóa
                if (SelectedContract != null && !_allContracts.Any(c => c.MaHopDong == SelectedContract.MaHopDong))
                {
                    SelectedContract = null;
                }
                
                // Giữ nguyên trang hiện tại nếu có thể, nếu không thì về trang 1
                var currentPage = PageIndex;
                PageIndex = 1;
                ApplySortAndPage();
                
                // Nếu trang hiện tại vẫn hợp lệ, quay lại trang đó
                if (currentPage <= TotalPages && currentPage > 1)
                {
                    PageIndex = currentPage;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplySortAndPage()
        {
            // Lọc theo tìm kiếm
            IEnumerable<ContractDto> query = _allContracts;
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var keyword = SearchText.Trim().ToLowerInvariant();
                query = query.Where(x =>
                    x.MaHopDong.ToString().ToLowerInvariant().Contains(keyword)
                    || (!string.IsNullOrEmpty(x.TenPhong) && x.TenPhong.ToLowerInvariant().Contains(keyword))
                    || (!string.IsNullOrEmpty(x.TenNguoiThue) && x.TenNguoiThue.ToLowerInvariant().Contains(keyword))
                    || (!string.IsNullOrEmpty(x.TrangThai) && x.TrangThai.ToLowerInvariant().Contains(keyword))
                );
            }

            // Sắp xếp
            if (SortOrder == "newest")
                query = query.OrderByDescending(x => x.NgayBatDau);
            else
                query = query.OrderBy(x => x.NgayBatDau);

            // Tính tổng trang
            var total = query.Count();
            TotalPages = (int)Math.Ceiling(total / (double)_pageSize);
            if (TotalPages == 0) TotalPages = 1;
            if (PageIndex > TotalPages) PageIndex = TotalPages;

            // Lấy trang hiện tại
            var skip = (PageIndex - 1) * _pageSize;
            var pageItems = query.Skip(skip).Take(_pageSize).ToList();

            Contracts.Clear();
            foreach (var item in pageItems) Contracts.Add(item);

            // Cập nhật text phân trang
            var start = total == 0 ? 0 : (PageIndex - 1) * _pageSize + 1;
            var end = Math.Min(PageIndex * _pageSize, total);
            PaginationText = $"Hiển thị {start}-{end} trong tổng {total} hợp đồng";
        }

        partial void OnSortOrderChanged(string value)
        {
            ApplySortAndPage();
        }

        [RelayCommand]
        private void AddContract()
        {
            try
            {
                var vm = new AddContractViewModel(_contractController);
                var win = new AddContractWindow(vm)
                {
                    Owner = Application.Current?.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive)
                };

                bool? result = win.ShowDialog();
                if (result == true)
                {
                    _ = LoadContractsAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi khi mở form tạo hợp đồng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand(CanExecute = nameof(CanEditOrDelete))]
        private void EditContract()
        {
            if (SelectedContract == null)
            {
                MessageBox.Show("⚠️ Vui lòng chọn hợp đồng để chỉnh sửa.");
                return;
            }

            try
            {
                var vm = new AddContractViewModel(_contractController, SelectedContract);
                var win = new AddContractWindow(vm)
                {
                    Owner = Application.Current?.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive)
                };

                bool? result = win.ShowDialog();
                if (result == true)
                {
                    _ = LoadContractsAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi khi mở form sửa hợp đồng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // 🔹 Lệnh: Xóa hợp đồng
        [RelayCommand(CanExecute = nameof(CanEditOrDelete))]
        private async Task DeleteContract()
        {
            if (SelectedContract == null) return;

            var confirm = MessageBox.Show(
                $"Bạn có chắc chắn muốn xóa hợp đồng của '{SelectedContract.TenNguoiThue}'?",
                "Xác nhận xóa",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (confirm == MessageBoxResult.Yes)
            {
                try
                {
                    await _contractController.DeleteHopDongAsync(SelectedContract.MaHopDong);
                    MessageBox.Show("✅ Hợp đồng đã được xóa thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    await LoadContractsAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"❌ Lỗi khi xóa: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private bool CanEditOrDelete() => SelectedContract != null;

        [RelayCommand]
        private async Task LoadExpiringContractsAsync()
        {
            try
            {
                int days = 30;
                var expiringContracts = await _contractController.GetExpiringContractsAsync(days);

                if (expiringContracts.Count == 0)
                {
                    MessageBox.Show($"Không có hợp đồng nào sắp hết hạn trong {days} ngày tới.",
                                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                _allContracts = expiringContracts.ToList();
                PageIndex = 1;
                ApplySortAndPage();

                MessageBox.Show($"Đã tải {expiringContracts.Count} hợp đồng sắp hết hạn trong {days} ngày tới.",
                                "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi khi tải hợp đồng sắp hết hạn: {ex.Message}",
                                "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async Task SendExpiryWarningEmailsAsync()
        {
            // Đảm bảo method được gọi - hiển thị thông báo ngay
            MessageBox.Show("⏳ Bắt đầu gửi email cảnh báo...",
                            "Đang xử lý",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);

            try
            {
                int days = 30; // Hợp đồng sắp hết hạn trong 30 ngày

                // Gọi method async
                var result = await _contractController.SendExpiryWarningEmailsAsync(days);
                var (success, failed, errors) = result;

                // Xử lý kết quả
                if (success == 0 && failed == 0)
                {
                    if (errors != null && errors.Count > 0 && errors[0].Contains("Không có hợp đồng"))
                    {
                        MessageBox.Show("ℹ️ " + errors[0],
                                        "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("ℹ️ Không có hợp đồng nào sắp hết hạn trong 30 ngày tới.",
                                        "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    return;
                }

                string message = $"📧 Đã gửi email cảnh báo:\n\n✅ Thành công: {success} email\n❌ Thất bại: {failed} email";

                if (errors != null && errors.Count > 0)
                {
                    message += "\n\nChi tiết lỗi:\n" + string.Join("\n", errors.Take(5));
                    if (errors.Count > 5)
                    {
                        message += $"\n... và {errors.Count - 5} lỗi khác.";
                    }
                }

                MessageBox.Show(message,
                                success > 0 ? "Thành công" : "Có lỗi xảy ra",
                                MessageBoxButton.OK,
                                success > 0 ? MessageBoxImage.Information : MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                string errorMessage = $"❌ Lỗi khi gửi email cảnh báo:\n\n{ex.Message}";
                if (ex.InnerException != null)
                {
                    errorMessage += $"\n\nChi tiết: {ex.InnerException.Message}";
                }

                MessageBox.Show(errorMessage,
                                "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void PrevPage()
        {
            if (PageIndex > 1) PageIndex--;
        }

        [RelayCommand]
        private void NextPage()
        {
            if (PageIndex < TotalPages) PageIndex++;
        }

        [RelayCommand]
        private async Task ExportToPdfAsync()
        {
            try
            {
                if (Contracts == null || Contracts.Count == 0)
                {
                    MessageBox.Show("Không có hợp đồng nào để xuất PDF.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var saveDialog = new SaveFileDialog
                {
                    Filter = "PDF files (*.pdf)|*.pdf",
                    FileName = $"DanhSachHopDong_{DateTime.Now:yyyyMMdd_HHmmss}.pdf",
                    Title = "Lưu danh sách hợp đồng PDF"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    // Tạo PDF danh sách hợp đồng
                    await CreateContractsListPdfAsync(saveDialog.FileName);
                    MessageBox.Show("✅ Xuất PDF thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi khi xuất PDF: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async Task ExportToPdfAndDriveAsync()
        {
            try
            {
                if (Contracts == null || Contracts.Count == 0)
                {
                    MessageBox.Show("Không có hợp đồng nào để xuất PDF.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // Tạo file PDF tạm
                var tempPath = Path.Combine(Path.GetTempPath(), $"DanhSachHopDong_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");
                await CreateContractsListPdfAsync(tempPath);

                // Upload lên Google Drive
                await UploadToGoogleDriveAsync(tempPath);

                // Xóa file tạm
                if (File.Exists(tempPath))
                {
                    File.Delete(tempPath);
                }

                MessageBox.Show("✅ Xuất PDF và upload lên Google Drive thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi khi xuất PDF và upload Drive: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task CreateContractsListPdfAsync(string filePath)
        {
            // Sử dụng PdfSharp để tạo PDF
            // Đảm bảo FontResolver được đăng ký
            if (GlobalFontSettings.FontResolver == null)
            {
                GlobalFontSettings.FontResolver = new QLKDPhongTro.Presentation.Services.PdfFontResolver();
            }
            GlobalFontSettings.UseWindowsFontsUnderWindows = true;

            using (var document = new PdfDocument())
            {
                var page = document.AddPage();
                var gfx = XGraphics.FromPdfPage(page);
                var font = new XFont("Arial", 12);
                // Sử dụng font lớn hơn cho tiêu đề để làm nổi bật (thay vì bold)
                var boldFont = new XFont("Arial", 14);

                double yPos = 50;
                double xPos = 50;
                double lineHeight = 20;

                // Tiêu đề
                gfx.DrawString("DANH SÁCH HỢP ĐỒNG", boldFont, XBrushes.Black, 
                    new XRect(xPos, yPos, page.Width, lineHeight), 
                    XStringFormats.TopLeft);
                yPos += lineHeight * 2;

                // Header
                gfx.DrawString("Mã HĐ", boldFont, XBrushes.Black, xPos, yPos);
                gfx.DrawString("Phòng", boldFont, XBrushes.Black, xPos + 80, yPos);
                gfx.DrawString("Người thuê", boldFont, XBrushes.Black, xPos + 200, yPos);
                gfx.DrawString("Ngày BĐ", boldFont, XBrushes.Black, xPos + 350, yPos);
                gfx.DrawString("Ngày KT", boldFont, XBrushes.Black, xPos + 450, yPos);
                gfx.DrawString("Tiền cọc", boldFont, XBrushes.Black, xPos + 550, yPos);
                yPos += lineHeight;

                // Dữ liệu
                foreach (var contract in Contracts)
                {
                    if (yPos > page.Height - 50)
                    {
                        page = document.AddPage();
                        gfx = XGraphics.FromPdfPage(page);
                        yPos = 50;
                    }

                    gfx.DrawString(contract.MaHopDong.ToString(), font, XBrushes.Black, xPos, yPos);
                    gfx.DrawString(contract.TenPhong ?? "", font, XBrushes.Black, xPos + 80, yPos);
                    gfx.DrawString(contract.TenNguoiThue ?? "", font, XBrushes.Black, xPos + 200, yPos);
                    gfx.DrawString(contract.NgayBatDau.ToString("dd/MM/yyyy"), font, XBrushes.Black, xPos + 350, yPos);
                    gfx.DrawString(contract.NgayKetThuc.ToString("dd/MM/yyyy"), font, XBrushes.Black, xPos + 450, yPos);
                    gfx.DrawString(contract.TienCoc.ToString("N0"), font, XBrushes.Black, xPos + 550, yPos);
                    yPos += lineHeight;
                }

                document.Save(filePath);
            }
        }

        private async Task UploadToGoogleDriveAsync(string filePath)
        {
            var driveService = _googleFormService.DriveService;
            var fileName = Path.GetFileName(filePath);

            var fileMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = fileName,
                Parents = new List<string>() // Có thể thêm folder ID nếu cần
            };

            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                var request = driveService.Files.Create(fileMetadata, stream, "application/pdf");
                request.Fields = "id";
                await request.UploadAsync();
            }
        }
    }
}