using System.Windows;
using System.Windows.Media.Imaging;
using QLKDPhongTro.BusinessLayer.DTOs;
using QLKDPhongTro.BusinessLayer.Controllers;
using QLKDPhongTro.Presentation.Services;
using System;
using System.IO;
using System.Windows.Input;
using Microsoft.Win32;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using QLKDPhongTro.Presentation.Utils; // Chứa InvoiceMappingExtensions

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
                var plumeriaInvoice = InvoiceData.ToPlumeriaInvoiceDto(
                    extendedInfo.contract,
                    InvoiceData.Email,
                    extendedInfo.tenPhong,
                    soNguoiLuuTru
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

                // Chuyển đổi DTO
                var plumeriaInvoice = InvoiceData.ToPlumeriaInvoiceDto(
                    extendedInfo.contract,
                    InvoiceData.Email,
                    extendedInfo.tenPhong,
                    soNguoiLuuTru
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

        private void LoadQrCode()
        {
            // Logic nạp ảnh QR: Ưu tiên Resource Stream -> Fallback file hệ thống
            if (QrImage?.Source != null) return;

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