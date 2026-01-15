using System.Windows;
using System.Windows.Media.Imaging;
using QLKDPhongTro.BusinessLayer.DTOs;
using QLKDPhongTro.BusinessLayer.Controllers;
using QLKDPhongTro.BusinessLayer.Services;
using QLKDPhongTro.Presentation.Services;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Input;
using Microsoft.Win32;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using QLKDPhongTro.Presentation.Utils; // Chứa InvoiceMappingExtensions
using QLKDPhongTro.DataLayer.Repositories;

namespace QLKDPhongTro.Presentation.Views.Windows
{
    public partial class InvoiceDetailView : Window
    {
        private readonly FinancialController _financialController = FinancialController.CreateDefault();
        private readonly TenantController _tenantController = new TenantController(new DataLayer.Repositories.TenantRepository());

        // Helper property để cast DataContext sang DTO
        public InvoiceDetailDto? InvoiceData
        {
            get => DataContext as InvoiceDetailDto;
            set => DataContext = value;
        }

        public InvoiceDetailView()
        {
            InitializeComponent();
            // Đảm bảo kích thước cửa sổ
            this.MinWidth = 800;
            this.MinHeight = 600;
            this.Width = 1200;
            this.Height = 700;
        }

        public InvoiceDetailView(InvoiceDetailDto invoiceData) : this()
        {
            InvoiceData = invoiceData;
        }

        #region UI Event Handlers

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void InvoiceDetailView_Loaded(object sender, RoutedEventArgs e)
        {
            LoadQrCode();
        }

        private void QrImage_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            // Xử lý khi ảnh QR không tải được (ẩn hoặc hiện ảnh mặc định)
            try
            {
                // Có thể set source null hoặc một ảnh placeholder khác nếu muốn
                // QrImage.Source = null; 
            }
            catch { /* Ignore */ }
        }

        // Cho phép double click để sửa textbox (nếu cần chỉnh sửa nhanh)
        private void UnitPriceTextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is System.Windows.Controls.TextBox tb)
            {
                tb.IsReadOnly = false;
                tb.Focus();
                tb.SelectAll();
            }
        }

        // Xử lý khi nhấn Enter trong TextBox để lưu giá trị mới
        private async void UnitPriceTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            if (sender is not System.Windows.Controls.TextBox tb) return;

            // Lưu giá trị khi nhấn Enter
            await SaveUnitPriceAsync(tb);
        }

        // Xử lý khi mất focus (thoát khỏi ô nhập) để tự động lưu
        private async void UnitPriceTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is not System.Windows.Controls.TextBox tb) return;
            
            // Chỉ lưu nếu đang ở chế độ chỉnh sửa (IsReadOnly = false)
            if (tb.IsReadOnly) return;

            // Lưu giá trị khi mất focus
            await SaveUnitPriceAsync(tb);
        }

        // Method chung để lưu đơn giá
        private async Task SaveUnitPriceAsync(System.Windows.Controls.TextBox textBox)
        {
            // 1. Khóa lại ô nhập liệu
            textBox.IsReadOnly = true;

            if (InvoiceData == null) return;

            try
            {
                // Sử dụng NumberFormatConverter để parse giá trị từ TextBox
                var converter = new QLKDPhongTro.Presentation.Converters.NumberFormatConverter();
                var parsedValue = converter.ConvertBack(textBox.Text, typeof(decimal), null, System.Globalization.CultureInfo.InvariantCulture);

                // Xác định đây là đơn giá điện hay nước
                decimal? donGiaDien = null;
                decimal? donGiaNuoc = null;
                string unitPriceType = "";

                if (textBox.Name == "TxtDonGiaDien" && parsedValue is decimal giaDien && giaDien > 0)
                {
                    donGiaDien = giaDien;
                    unitPriceType = "điện";
                }
                else if (textBox.Name == "TxtDonGiaNuoc" && parsedValue is decimal giaNuoc && giaNuoc > 0)
                {
                    donGiaNuoc = giaNuoc;
                    unitPriceType = "nước";
                }

                // Nếu không có giá trị nào được cập nhật, không làm gì
                if (!donGiaDien.HasValue && !donGiaNuoc.HasValue)
                {
                    return;
                }

                // 2. Cập nhật đơn giá cho thanh toán hiện tại và tất cả thanh toán "Chưa trả" trong hệ thống
                var success = await _financialController.UpdateUnitPricesForCurrentAndUnpaidPaymentsAsync(
                    InvoiceData.MaThanhToan,
                    donGiaDien,
                    donGiaNuoc
                );

                if (!success)
                {
                    MessageBox.Show("Không thể lưu đơn giá. Vui lòng thử lại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // 3. Cập nhật lại InvoiceData để UI hiển thị đúng
                // Cần reload lại dữ liệu từ database
                var updatedPayment = await _financialController.GetInvoiceDetailAsync(InvoiceData.MaThanhToan);
                if (updatedPayment != null)
                {
                    InvoiceData = updatedPayment;
                }

                MessageBox.Show($"Đã cập nhật đơn giá {unitPriceType} cho thanh toán này và tất cả các thanh toán chưa trả trong hệ thống!", 
                    "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region Export & Email Logic

        private async void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            if (InvoiceData == null) return;

            try
            {
                // Hiển thị menu chọn loại PDF
                var contextMenu = new System.Windows.Controls.ContextMenu();
                
                var menuItem1 = new System.Windows.Controls.MenuItem
                {
                    Header = "Hóa đơn thanh toán",
                    Tag = "invoice"
                };
                menuItem1.Click += async (s, args) => await DownloadPdfAsync("invoice");
                
                var menuItem2 = new System.Windows.Controls.MenuItem
                {
                    Header = "Bảng chi tiết chỉ số điện",
                    Tag = "electricity"
                };
                menuItem2.Click += async (s, args) => await DownloadPdfAsync("electricity");
                
                contextMenu.Items.Add(menuItem1);
                contextMenu.Items.Add(menuItem2);
                
                if (sender is System.Windows.Controls.Button button)
                {
                    contextMenu.PlacementTarget = button;
                    contextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                    contextMenu.IsOpen = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tạo file PDF: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task DownloadPdfAsync(string pdfType)
        {
            if (InvoiceData == null) return;

            try
            {
                var extendedInfo = await GetInvoiceExtendedInfoAsync();
                int soNguoiLuuTru = await GetSoNguoiLuuTruAsync(extendedInfo.contract);
                var ownerInfo = await GetOwnerInfoAsync();
                var plumeriaInvoice = InvoiceData.ToPlumeriaInvoiceDto(
                    extendedInfo.contract,
                    InvoiceData.Email,
                    extendedInfo.tenPhong,
                    soNguoiLuuTru,
                    ownerInfo.TenTaiKhoan,
                    ownerInfo.SoTaiKhoan,
                    ownerInfo.SoDienThoai,
                    ownerInfo.Email
                );
                string logoPath = FindLogoPath();

                string defaultFileName;
                string dialogTitle;

                if (pdfType == "electricity")
                {
                    defaultFileName = $"BangChiTietDien_{InvoiceData.MaThanhToan}_{InvoiceData.ThangNam.Replace("/", "_")}.pdf";
                    dialogTitle = "Lưu bảng chi tiết chỉ số điện PDF";
                }
                else
                {
                    defaultFileName = $"HoaDon_{InvoiceData.MaThanhToan}_{InvoiceData.ThangNam.Replace("/", "_")}.pdf";
                    dialogTitle = "Lưu hóa đơn PDF";
                }

                var saveDialog = new SaveFileDialog
                {
                    Filter = "PDF files (*.pdf)|*.pdf",
                    FileName = defaultFileName,
                    Title = dialogTitle
                };

                if (saveDialog.ShowDialog() == true)
                {
                    if (pdfType == "electricity")
                    {
                        ElectricityDetailPdfService.CreateElectricityDetailPdf(plumeriaInvoice, saveDialog.FileName, logoPath);
                    }
                    else
                    {
                        PlumeriaInvoicePdfService.CreateInvoicePdf(plumeriaInvoice, saveDialog.FileName, logoPath);
                    }

                    MessageBox.Show($"Đã tải file PDF thành công!\nĐường dẫn: {saveDialog.FileName}",
                        "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                    try
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = saveDialog.FileName,
                            UseShellExecute = true
                        });
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tạo file PDF: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void SendToCustomerButton_Click(object sender, RoutedEventArgs e)
        {
            if (InvoiceData == null) return;

            if (string.IsNullOrWhiteSpace(InvoiceData.Email))
            {
                MessageBox.Show("Khách hàng này chưa có email.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Lấy thông tin mở rộng
                var extendedInfo = await GetInvoiceExtendedInfoAsync();
                int soNguoiLuuTru = await GetSoNguoiLuuTruAsync(extendedInfo.contract);

                var ownerInfo = await GetOwnerInfoAsync();

                // Chuyển đổi DTO
                var plumeriaInvoice = InvoiceData.ToPlumeriaInvoiceDto(
                    extendedInfo.contract,
                    InvoiceData.Email,
                    extendedInfo.tenPhong,
                    soNguoiLuuTru,
                    ownerInfo.TenTaiKhoan,
                    ownerInfo.SoTaiKhoan,
                    ownerInfo.SoDienThoai,
                    ownerInfo.Email
                );

                string logoPath = FindLogoPath();

                // Tạo cả 2 file PDF tạm
                string tempInvoiceFileName = $"HoaDon_Temp_{Guid.NewGuid()}.pdf";
                string tempInvoiceFilePath = Path.Combine(Path.GetTempPath(), tempInvoiceFileName);
                PlumeriaInvoicePdfService.CreateInvoicePdf(plumeriaInvoice, tempInvoiceFilePath, logoPath);

                string tempElectricityFileName = $"BangChiTietDien_Temp_{Guid.NewGuid()}.pdf";
                string tempElectricityFilePath = Path.Combine(Path.GetTempPath(), tempElectricityFileName);
                ElectricityDetailPdfService.CreateElectricityDetailPdf(plumeriaInvoice, tempElectricityFilePath, logoPath);

                // Tạo nội dung Email
                string subject = $"Thông báo thanh toán tháng {InvoiceData.ThangNam} - #{InvoiceData.MaThanhToan}";
                string body = GenerateEmailBody();

                // Gửi mail với cả 2 file đính kèm
                var attachments = new List<string> { tempInvoiceFilePath, tempElectricityFilePath };
                await EmailService.SendEmailWithAttachmentsAsync(InvoiceData.Email, subject, body, attachments);

                // Xóa file tạm sau khi gửi
                try
                {
                    if (File.Exists(tempInvoiceFilePath)) File.Delete(tempInvoiceFilePath);
                    if (File.Exists(tempElectricityFilePath)) File.Delete(tempElectricityFilePath);
                }
                catch { }

                MessageBox.Show("Đã gửi hóa đơn và bảng chi tiết chỉ số điện đến khách hàng thành công!", 
                    "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi gửi email: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Lấy thông tin bổ sung từ DB (Hợp đồng, Tên phòng) để in hóa đơn
        /// </summary>
        private async Task<(ContractDto? contract, string tenPhong)> GetInvoiceExtendedInfoAsync()
        {
            ContractDto? contract = null;
            string tenPhong = "N/A";

            if (InvoiceData != null && InvoiceData.MaHopDong > 0)
            {
                var contractController = ContractController.CreateDefault();
                contract = await contractController.GetByIdAsync(InvoiceData.MaHopDong);

                if (contract != null)
                {
                    // Nếu hợp đồng có mã phòng, lấy tên phòng từ bảng Room
                    if (contract.MaPhong > 0)
                    {
                        var room = await _financialController.GetRoomByIdAsync(contract.MaPhong);
                        if (room != null)
                        {
                            tenPhong = room.TenPhong ?? "N/A";
                        }
                    }
                    else
                    {
                        // Fallback nếu không join được bảng Room
                        tenPhong = contract.TenPhong ?? "N/A";
                    }
                }
            }
            return (contract, tenPhong);
        }

        /// <summary>
        /// Lấy số lượng người lưu trú trong phòng từ hợp đồng
        /// </summary>
        private async Task<int> GetSoNguoiLuuTruAsync(ContractDto? contract)
        {
            if (contract == null || contract.MaPhong <= 0)
            {
                return 1; // Mặc định 1 người nếu không có hợp đồng hoặc mã phòng
            }

            try
            {
                // Lấy danh sách người thuê trong phòng
                var roomTenants = await _tenantController.GetTenantsByRoomIdAsync(contract.MaPhong);
                
                // Đếm số người có trạng thái "Đang ở"
                int soNguoi = roomTenants?.Count(t => 
                    string.Equals(t.TrangThaiNguoiThue, "Đang ở", StringComparison.OrdinalIgnoreCase)) ?? 1;
                
                // Đảm bảo ít nhất 1 người
                return soNguoi < 1 ? 1 : soNguoi;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi khi lấy số người lưu trú: {ex.Message}");
                return 1; // Fallback về 1 nếu có lỗi
            }
        }

        private async void LoadQrCode()
        {
            // Logic nạp ảnh QR: Ưu tiên LinkQr từ Admin -> Fallback mặc định
            if (QrImage?.Source != null) return;

            // 1. Lấy LinkQr từ database (reload để đảm bảo có dữ liệu mới nhất)
            string? qrPath = null;
            try
            {
                var currentUser = AuthController.CurrentUser;
                if (currentUser != null)
                {
                    // Reload user từ database để lấy LinkQr mới nhất
                    var userRepository = new DataLayer.Repositories.UserRepository();
                    var userFromDb = await userRepository.GetByMaAdminAsync(currentUser.MaAdmin);
                    
                    if (userFromDb != null && !string.IsNullOrWhiteSpace(userFromDb.LinkQr))
                    {
                        qrPath = userFromDb.LinkQr.Trim();
                        // Cập nhật lại AuthController.CurrentUser để đồng bộ
                        AuthController.CurrentUser.LinkQr = qrPath;
                        System.Diagnostics.Debug.WriteLine($"[LoadQrCode] Loaded LinkQr from database: {qrPath}");
                    }
                    else
                    {
                        // Fallback: thử lấy từ CurrentUser (nếu có)
                        if (!string.IsNullOrWhiteSpace(currentUser.LinkQr))
                        {
                            qrPath = currentUser.LinkQr.Trim();
                            System.Diagnostics.Debug.WriteLine($"[LoadQrCode] Using LinkQr from CurrentUser: {qrPath}");
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"[LoadQrCode] No LinkQr found in database or CurrentUser");
                        }
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"[LoadQrCode] CurrentUser is null");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[LoadQrCode] Error loading LinkQr from database: {ex.Message}");
                // Fallback: thử lấy từ CurrentUser
                var currentUser = AuthController.CurrentUser;
                if (currentUser != null && !string.IsNullOrWhiteSpace(currentUser.LinkQr))
                {
                    qrPath = currentUser.LinkQr.Trim();
                }
            }

            // 2. Nếu có LinkQr, thử load từ đó
            if (!string.IsNullOrWhiteSpace(qrPath))
            {
                try
                {
                    // Xử lý đường dẫn tương đối (bắt đầu với /)
                    if (qrPath.StartsWith("/"))
                    {
                        // Đường dẫn tương đối: /Resources/Images/filename.jpg
                        // Thử load từ Resource Stream trước
                        var resourceUri = new Uri($"/QLKDPhongTro.Presentation;component{qrPath}", UriKind.Relative);
                        var streamInfo = Application.GetResourceStream(resourceUri);
                        if (streamInfo != null)
                        {
                            using var s = streamInfo.Stream;
                            var bitmap = new BitmapImage();
                            bitmap.BeginInit();
                            bitmap.CacheOption = BitmapCacheOption.OnLoad;
                            bitmap.StreamSource = s;
                            bitmap.EndInit();
                            QrImage.Source = bitmap;
                            return;
                        }

                        // Nếu không load được từ Resource, thử load từ file system
                        // Ưu tiên tìm trong thư mục source code trước
                        var projectDirectory = FindProjectDirectory();
                        var sourceFilePath = Path.Combine(projectDirectory, qrPath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                        
                        if (File.Exists(sourceFilePath))
                        {
                            var bitmap = new BitmapImage();
                            bitmap.BeginInit();
                            bitmap.CacheOption = BitmapCacheOption.OnLoad;
                            bitmap.UriSource = new Uri(sourceFilePath, UriKind.Absolute);
                            bitmap.EndInit();
                            QrImage.Source = bitmap;
                            System.Diagnostics.Debug.WriteLine($"[LoadQrCode] Loaded from source directory: {sourceFilePath}");
                            return;
                        }
                        
                        // Fallback: thử tìm trong output directory
                        var outputFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, qrPath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                        if (File.Exists(outputFilePath))
                        {
                            var bitmap = new BitmapImage();
                            bitmap.BeginInit();
                            bitmap.CacheOption = BitmapCacheOption.OnLoad;
                            bitmap.UriSource = new Uri(outputFilePath, UriKind.Absolute);
                            bitmap.EndInit();
                            QrImage.Source = bitmap;
                            System.Diagnostics.Debug.WriteLine($"[LoadQrCode] Loaded from output directory: {outputFilePath}");
                            return;
                        }
                    }
                    else
                    {
                        // Đường dẫn tuyệt đối hoặc tên file
                        // Thử load trực tiếp
                        if (File.Exists(qrPath))
                        {
                            var bitmap = new BitmapImage();
                            bitmap.BeginInit();
                            bitmap.CacheOption = BitmapCacheOption.OnLoad;
                            bitmap.UriSource = new Uri(qrPath, UriKind.Absolute);
                            bitmap.EndInit();
                            QrImage.Source = bitmap;
                            return;
                        }
                        else
                        {
                            // Nếu không phải đường dẫn tuyệt đối, thử tìm trong Resources/Images
                            var fileName = Path.GetFileName(qrPath);
                            
                            // Ưu tiên tìm trong thư mục source code trước
                            var projectDirectory = FindProjectDirectory();
                            var sourceFilePath = Path.Combine(projectDirectory, "Resources", "Images", fileName);
                            
                            if (File.Exists(sourceFilePath))
                            {
                                var bitmap = new BitmapImage();
                                bitmap.BeginInit();
                                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                                bitmap.UriSource = new Uri(sourceFilePath, UriKind.Absolute);
                                bitmap.EndInit();
                                QrImage.Source = bitmap;
                                System.Diagnostics.Debug.WriteLine($"[LoadQrCode] Loaded from source directory: {sourceFilePath}");
                                return;
                            }
                            
                            // Fallback: thử tìm trong output directory
                            var outputFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Images", fileName);
                            if (File.Exists(outputFilePath))
                            {
                                var bitmap = new BitmapImage();
                                bitmap.BeginInit();
                                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                                bitmap.UriSource = new Uri(outputFilePath, UriKind.Absolute);
                                bitmap.EndInit();
                                QrImage.Source = bitmap;
                                System.Diagnostics.Debug.WriteLine($"[LoadQrCode] Loaded from output directory: {outputFilePath}");
                                return;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Lỗi khi load QR từ LinkQr ({qrPath}): {ex.Message}");
                }
            }

            // 3. Fallback về đường dẫn mặc định nếu không load được từ LinkQr
            try
            {
                var resourceUri = new Uri("/QLKDPhongTro.Presentation;component/Resources/Images/QR.jpg", UriKind.Relative);
                var streamInfo = Application.GetResourceStream(resourceUri);
                if (streamInfo != null)
                {
                    using var s = streamInfo.Stream;
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.StreamSource = s;
                    bitmap.EndInit();
                    QrImage.Source = bitmap;
                    return;
                }
            }
            catch { }

            try
            {
                var relPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Images", "QR.jpg");
                if (File.Exists(relPath))
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.UriSource = new Uri(relPath, UriKind.Absolute);
                    bitmap.EndInit();
                    QrImage.Source = bitmap;
                }
            }
            catch { }
        }

        private string FindLogoPath()
        {
            var paths = new[]
            {
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Images", "Logo.png"),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Images", "logo.png"),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Images", "Logo.jpg")
            };

            foreach (var path in paths)
            {
                if (File.Exists(path)) return path;
            }
            return string.Empty;
        }

        /// <summary>
        /// Tìm thư mục source code của project (QLKDPhongTro.Presentation)
        /// </summary>
        private string FindProjectDirectory()
        {
            try
            {
                // Lấy đường dẫn của assembly hiện tại
                var assemblyLocation = Assembly.GetExecutingAssembly().Location;
                var assemblyDirectory = Path.GetDirectoryName(assemblyLocation);
                
                if (string.IsNullOrEmpty(assemblyDirectory))
                {
                    // Fallback: sử dụng AppDomain.CurrentDomain.BaseDirectory
                    assemblyDirectory = AppDomain.CurrentDomain.BaseDirectory;
                }
                
                System.Diagnostics.Debug.WriteLine($"[FindProjectDirectory] Assembly Location: {assemblyLocation}");
                System.Diagnostics.Debug.WriteLine($"[FindProjectDirectory] Assembly Directory: {assemblyDirectory}");
                
                // Đi lên các cấp thư mục để tìm thư mục chứa file .csproj
                var currentDir = new DirectoryInfo(assemblyDirectory);
                
                while (currentDir != null)
                {
                    // Kiểm tra xem có file .csproj trong thư mục này không
                    var csprojFiles = currentDir.GetFiles("*.csproj", SearchOption.TopDirectoryOnly);
                    if (csprojFiles.Length > 0)
                    {
                        // Kiểm tra xem có thư mục Resources/Images không
                        var resourcesPath = Path.Combine(currentDir.FullName, "Resources", "Images");
                        if (Directory.Exists(resourcesPath) || currentDir.Name == "QLKDPhongTro.Presentation")
                        {
                            System.Diagnostics.Debug.WriteLine($"[FindProjectDirectory] Found project directory: {currentDir.FullName}");
                            return currentDir.FullName;
                        }
                    }
                    
                    // Đi lên một cấp
                    currentDir = currentDir.Parent;
                }
                
                // Nếu không tìm thấy, thử tìm thư mục có tên "QLKDPhongTro.Presentation"
                currentDir = new DirectoryInfo(assemblyDirectory);
                while (currentDir != null)
                {
                    if (currentDir.Name == "QLKDPhongTro.Presentation")
                    {
                        System.Diagnostics.Debug.WriteLine($"[FindProjectDirectory] Found by name: {currentDir.FullName}");
                        return currentDir.FullName;
                    }
                    currentDir = currentDir.Parent;
                }
                
                // Fallback cuối cùng: sử dụng thư mục hiện tại và đi lên 3 cấp (bin/Debug/net8.0-windows -> QLKDPhongTro.Presentation)
                var fallbackDir = new DirectoryInfo(assemblyDirectory);
                for (int i = 0; i < 3 && fallbackDir != null; i++)
                {
                    fallbackDir = fallbackDir.Parent;
                }
                
                if (fallbackDir != null)
                {
                    System.Diagnostics.Debug.WriteLine($"[FindProjectDirectory] Using fallback: {fallbackDir.FullName}");
                    return fallbackDir.FullName;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[FindProjectDirectory] Error: {ex.Message}");
            }
            
            // Fallback cuối cùng: sử dụng AppDomain.CurrentDomain.BaseDirectory
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            System.Diagnostics.Debug.WriteLine($"[FindProjectDirectory] Using AppDomain.BaseDirectory: {baseDir}");
            return baseDir;
        }

        private async Task<OwnerInfo> GetOwnerInfoAsync()
        {
            var info = new OwnerInfo();

            try
            {
                var currentUser = AuthController.CurrentUser;
                if (currentUser == null)
                {
                    return info;
                }

                info.TenTaiKhoan = currentUser.TenTK?.Trim() ?? string.Empty;
                info.SoTaiKhoan = currentUser.SoTK?.Trim() ?? string.Empty;
                info.SoDienThoai = currentUser.SoDienThoai?.Trim() ?? string.Empty;
                info.Email = currentUser.Email?.Trim() ?? string.Empty;

                var userRepository = new UserRepository();
                var userFromDb = await userRepository.GetByMaAdminAsync(currentUser.MaAdmin);
                if (userFromDb != null)
                {
                    if (!string.IsNullOrWhiteSpace(userFromDb.TenTK))
                    {
                        info.TenTaiKhoan = userFromDb.TenTK.Trim();
                    }

                    if (!string.IsNullOrWhiteSpace(userFromDb.SoTK))
                    {
                        info.SoTaiKhoan = userFromDb.SoTK.Trim();
                    }

                    if (!string.IsNullOrWhiteSpace(userFromDb.SoDienThoai))
                    {
                        info.SoDienThoai = userFromDb.SoDienThoai.Trim();
                    }

                    if (!string.IsNullOrWhiteSpace(userFromDb.Email))
                    {
                        info.Email = userFromDb.Email.Trim();
                    }

                    // Đồng bộ lại CurrentUser để các nơi khác sử dụng
                    AuthController.CurrentUser.TenTK = info.TenTaiKhoan;
                    AuthController.CurrentUser.SoTK = info.SoTaiKhoan;
                    AuthController.CurrentUser.SoDienThoai = info.SoDienThoai;
                    AuthController.CurrentUser.Email = info.Email;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[GetOwnerInfoAsync] Error: {ex.Message}");
            }

            return info;
        }

        private sealed class OwnerInfo
        {
            public string TenTaiKhoan { get; set; } = string.Empty;
            public string SoTaiKhoan { get; set; } = string.Empty;
            public string SoDienThoai { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
        }

        private string GenerateEmailBody()
        {
            if (InvoiceData == null) return string.Empty;

            // HTML Template
            return $@"<!DOCTYPE html>
            <html lang=""vi"">
            <head>
                <meta charset=""UTF-8"" />
                <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
                <title>Thông báo thanh toán</title>
                <style>
                    body {{ margin: 0; padding: 0; background-color: #f3f4f6; font-family: 'Segoe UI', Roboto, sans-serif; color: #111827; }}
                    .wrapper {{ width: 100%; background-color: #f3f4f6; padding: 24px 0; }}
                    .container {{ max-width: 640px; margin: 0 auto; background-color: #ffffff; border-radius: 8px; overflow: hidden; box-shadow: 0 10px 25px rgba(15, 23, 42, 0.08); }}
                    .header {{ background: linear-gradient(135deg, #0ea5e9, #6366f1); padding: 20px 28px; color: #ffffff; }}
                    .brand-name {{ font-size: 20px; font-weight: 700; }}
                    .content {{ padding: 24px 28px; }}
                    .summary-card {{ border-radius: 10px; border: 1px solid #e5e7eb; padding: 16px 18px; background: #f9fafb; margin-bottom: 18px; }}
                    .summary-row {{ display: flex; justify-content: space-between; font-size: 14px; margin-top: 6px; }}
                    .summary-value.total {{ font-size: 15px; color: #16a34a; font-weight: 600; }}
                    .footer {{ padding: 16px 28px; font-size: 11px; color: #9ca3af; text-align: center; }}
                </style>
            </head>
            <body>
                <div class=""wrapper"">
                    <div class=""container"">
                        <div class=""header"">
                            <div class=""brand-name"">QUẢN LÝ PHÒNG TRỌ</div>
                            <div style=""opacity: 0.9; font-size: 13px;"">Thông báo hóa đơn tháng {InvoiceData.ThangNam}</div>
                        </div>
                        <div class=""content"">
                            <p>Kính gửi {InvoiceData.HoTen},</p>
                            <p>Đây là email nhắc thanh toán cho hóa đơn tiền phòng. Chi tiết vui lòng xem file đính kèm.</p>
                            
                            <div class=""summary-card"">
                                <div style=""font-weight: 600; color: #6b7280; margin-bottom: 8px; text-transform: uppercase; font-size: 12px;"">Tóm tắt</div>
                                <div class=""summary-row"">
                                    <span>Mã hóa đơn:</span>
                                    <strong>#{InvoiceData.MaThanhToan}</strong>
                                </div>
                                <div class=""summary-row"">
                                    <span>Tháng:</span>
                                    <strong>{InvoiceData.ThangNam}</strong>
                                </div>
                                <div class=""summary-row"">
                                    <span>Tạm tính:</span>
                                    <span>{InvoiceData.TamTinh:N0} đ</span>
                                </div>
                                <div class=""summary-row"">
                                    <span>Đã trả trước:</span>
                                    <span>{InvoiceData.SoTienDaTra:N0} đ</span>
                                </div>
                                <div style=""height: 1px; background: #e5e7eb; margin: 8px 0;""></div>
                                <div class=""summary-row"">
                                    <span>Còn lại:</span>
                                    <span class=""summary-value total"">{InvoiceData.TongTienTinhToan:N0} đ</span>
                                </div>
                            </div>

                            <p style=""font-size: 12px; color: #6b7280;"">
                                <strong>Lưu ý:</strong><br/>
                                · Nếu bạn đã thanh toán, vui lòng bỏ qua email này.
                            </p>
                        </div>
                        <div class=""footer"">
                            Email tự động từ hệ thống quản lý.
                        </div>
                    </div>
                </div>
            </body>
            </html>";
        }

        #endregion
    }
}