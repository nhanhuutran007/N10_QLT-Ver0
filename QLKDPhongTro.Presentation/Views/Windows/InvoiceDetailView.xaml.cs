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
using QLKDPhongTro.Presentation.Utils;

namespace QLKDPhongTro.Presentation.Views.Windows
{
    public partial class InvoiceDetailView : Window
    {
        private readonly FinancialController _financialController = FinancialController.CreateDefault();
        public InvoiceDetailDto? InvoiceData
        {
            get => (InvoiceDetailDto?)DataContext;
            set => DataContext = value;
        }

        public InvoiceDetailView()
        {
            InitializeComponent();
            // Đảm bảo kích thước được áp dụng đúng
            this.MinWidth = 800;
            this.MinHeight = 600;
            this.Width = 1200;
            this.Height = 700;
        }

        public InvoiceDetailView(InvoiceDetailDto invoiceData) : this()
        {
            InvoiceData = invoiceData;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void QrImage_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            // Fallback sang đường dẫn Content nếu Resource không tải được
            try
            {
                var fallbackUri = new Uri("Resources/Images/QR.jpg", UriKind.Relative);
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = fallbackUri;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                QrImage.Source = bitmap;
            }
            catch
            {
                // Không ném lỗi ra ngoài UI; giữ nguyên để tránh crash cửa sổ
            }
        }

        private async void SendToCustomerButton_Click(object sender, RoutedEventArgs e)
        {
            if (InvoiceData == null)
            {
                MessageBox.Show("Không có dữ liệu hóa đơn để gửi.", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(InvoiceData.Email))
            {
                MessageBox.Show("Không tìm thấy email của khách hàng để gửi hóa đơn.", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Lấy thông tin contract và room giống như khi tạo PDF để tải xuống
                ContractDto? contract = null;
                string tenPhong = "N/A";
                int soNguoiLuuTru = 1;
                string email = InvoiceData.Email;

                if (InvoiceData.MaHopDong > 0)
                {
                    var contractController = ContractController.CreateDefault();
                    contract = await contractController.GetByIdAsync(InvoiceData.MaHopDong);
                    if (contract != null)
                    {
                        if (contract.MaPhong > 0)
                        {
                            var financialController = FinancialController.CreateDefault();
                            var room = await financialController.GetRoomByIdAsync(contract.MaPhong);
                            if (room != null)
                            {
                                tenPhong = room.TenPhong ?? "N/A";
                            }
                        }
                        else
                        {
                            tenPhong = contract.TenPhong ?? "N/A";
                        }
                    }
                }

                var plumeriaInvoice = InvoiceData.ToPlumeriaInvoiceDto(contract, email, tenPhong, soNguoiLuuTru);

                // Tạo file PDF tạm thời
                string tempFileName = $"HoaDon_{InvoiceData.MaThanhToan}_{InvoiceData.ThangNam.Replace("/", "_")}.pdf";
                string tempFilePath = Path.Combine(Path.GetTempPath(), tempFileName);

                string logoPath = FindLogoPath();
                PlumeriaInvoicePdfService.CreateInvoicePdf(plumeriaInvoice, tempFilePath, logoPath);

                // Nội dung email nhắc khách thanh toán (HTML)
                string subject = $"Thông báo thanh toán tháng {InvoiceData.ThangNam} - #{InvoiceData.MaThanhToan}";
                string body = $@"<!DOCTYPE html>
<html lang=""vi"">
<head>
    <meta charset=""UTF-8"" />
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
    <title>Thông báo thanh toán</title>
    <style>
        body {{
            margin: 0;
            padding: 0;
            background-color: #f3f4f6;
            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
            color: #111827;
        }}
        .wrapper {{
            width: 100%;
            background-color: #f3f4f6;
            padding: 24px 0;
        }}
        .container {{
            max-width: 640px;
            margin: 0 auto;
            background-color: #ffffff;
            border-radius: 8px;
            overflow: hidden;
            box-shadow: 0 10px 25px rgba(15, 23, 42, 0.08);
        }}
        .header {{
            background: linear-gradient(135deg, #0ea5e9, #6366f1);
            padding: 20px 28px;
            color: #ffffff;
        }}
        .brand-name {{
            font-size: 20px;
            font-weight: 700;
            letter-spacing: 0.03em;
        }}
        .subtitle {{
            font-size: 13px;
            opacity: 0.9;
            margin-top: 4px;
        }}
        .content {{
            padding: 24px 28px 28px 28px;
        }}
        .greeting {{
            font-size: 15px;
            margin-bottom: 12px;
        }}
        .lead {{
            font-size: 14px;
            color: #374151;
            margin-bottom: 18px;
        }}
        .summary-card {{
            border-radius: 10px;
            border: 1px solid #e5e7eb;
            padding: 16px 18px;
            background: linear-gradient(135deg, #eff6ff 0%, #f9fafb 60%, #fefce8 100%);
            margin-bottom: 18px;
        }}
        .summary-title {{
            font-size: 13px;
            font-weight: 600;
            text-transform: uppercase;
            letter-spacing: 0.08em;
            color: #6b7280;
            margin-bottom: 8px;
        }}
        .summary-row {{
            display: flex;
            justify-content: space-between;
            align-items: baseline;
            font-size: 14px;
            margin-top: 6px;
        }}
        .summary-label {{
            color: #4b5563;
        }}
        .summary-value {{
            font-weight: 600;
            color: #111827;
        }}
        .summary-value.total {{
            font-size: 15px;
            color: #16a34a;
        }}
        .badge {{
            display: inline-block;
            padding: 2px 8px;
            border-radius: 999px;
            font-size: 11px;
            font-weight: 600;
            text-transform: uppercase;
            letter-spacing: 0.08em;
            background-color: #dcfce7;
            color: #15803d;
            margin-left: 8px;
        }}
        .cta {{
            margin: 22px 0 16px 0;
        }}
        .cta-button {{
            display: inline-block;
            padding: 10px 20px;
            border-radius: 999px;
            background: linear-gradient(135deg, #0ea5e9, #6366f1);
            color: #ffffff !important;
            font-size: 14px;
            font-weight: 600;
            text-decoration: none;
            box-shadow: 0 8px 18px rgba(59, 130, 246, 0.35);
        }}
        .note {{
            font-size: 12px;
            color: #6b7280;
            line-height: 1.6;
        }}
        .footer {{
            padding: 16px 28px 22px 28px;
            font-size: 11px;
            color: #9ca3af;
            text-align: center;
        }}
        .divider {{
            height: 1px;
            background: linear-gradient(to right, transparent, #e5e7eb, transparent);
            margin: 18px 0 14px 0;
        }}
        @media (max-width: 640px) {{
            .container {{
                border-radius: 0;
            }}
            .content {{
                padding: 20px 18px 22px 18px;
            }}
            .header {{
                padding: 18px 18px;
            }}
        }}
    </style>
    <meta name=""color-scheme"" content=""light"" />
    <meta name=""supported-color-schemes"" content=""light"" />
</head>
<body>
    <div class=""wrapper"">
        <div class=""container"">
            <div class=""header"">
                <div class=""brand-name"">QUẢN LÝ PHÒNG TRỌ</div>
                <div class=""subtitle"">Thông báo hóa đơn cần thanh toán tháng {InvoiceData.ThangNam}</div>
            </div>
            <div class=""content"">
                <p class=""greeting"">Kính gửi {InvoiceData.HoTen},</p>
                <p class=""lead"">
                    Đây là email nhắc thanh toán cho hóa đơn tiền phòng của bạn. Dưới đây là tóm tắt các thông tin chính.
                    Chi tiết đầy đủ về từng khoản phí đã được đính kèm trong file PDF đi kèm email này.
                </p>

                <div class=""summary-card"">
                    <div class=""summary-title"">Thông tin hóa đơn</div>
                    <div class=""summary-row"">
                        <span class=""summary-label"">Mã thanh toán:&nbsp;</span>
                        <span class=""summary-value"">#{InvoiceData.MaThanhToan}</span>
                    </div>
                    <div class=""summary-row"">
                        <span class=""summary-label"">Tháng:&nbsp;</span>
                        <span class=""summary-value"">{InvoiceData.ThangNam}</span>
                    </div>
                    <div class=""summary-row"">
                        <span class=""summary-label"">Tạm tính các khoản phí:&nbsp;</span>
                        <span class=""summary-value"">{InvoiceData.TamTinh:N0} đ</span>
                    </div>
                    <div class=""summary-row"">
                        <span class=""summary-label"">Đã khấu trừ từ tiền cọc:&nbsp;</span>
                        <span class=""summary-value"">- {InvoiceData.KhauTru:N0} đ</span>
                    </div>
                    <div class=""summary-row"">
                        <span class=""summary-label"">Số tiền cần thanh toán:&nbsp;</span>
                        <span class=""summary-value total"">{InvoiceData.TongTienTinhToan:N0} đ</span>
                    </div>
                </div>

                <div class=""cta"">
                    <a class=""cta-button"" href=""#"" target=""_blank"">Xem chi tiết hóa đơn (PDF đính kèm)</a>
                </div>

                <div class=""note"">
                    <p><strong>Lưu ý:</strong></p>
                    <p>
                        · Vui lòng kiểm tra kỹ thông tin và thực hiện thanh toán theo hướng dẫn trong file PDF đính kèm.<br />
                        · Tiền cọc hiện có: {InvoiceData.TienCocHienCo:N0} đ, tiền cọc còn dư sau khi khấu trừ (nếu có): {InvoiceData.TienCocConDu:N0} đ.<br />
                        · Nếu bạn đã thanh toán, vui lòng bỏ qua email này.<br />
                        · Nếu có bất kỳ thắc mắc nào, hãy phản hồi lại email để được hỗ trợ.
                    </p>
                </div>

                <div class=""divider""></div>

                <p class=""note"" style=""margin-top: 0;"">
                    Trân trọng,<br />
                    <strong>Hệ thống Quản lý Phòng Trọ</strong>
                </p>
            </div>
            <div class=""footer"">
                Email được gửi tự động từ hệ thống. Vui lòng không trả lời trực tiếp qua địa chỉ này nếu không cần thiết.
            </div>
        </div>
    </div>
</body>
</html>";

                await EmailService.SendEmailWithAttachmentAsync(email, subject, body, tempFilePath);

                MessageBox.Show("Đã gửi hóa đơn đến khách hàng thành công!", "Thành công",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi gửi email hóa đơn: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void InvoiceDetailView_Loaded(object sender, RoutedEventArgs e)
        {
            // Cố gắng nạp theo đúng cơ chế như Icon/Sidebar: lấy từ Resource trước,
            // nếu không có thì lấy Content ngoài output folder.
            if (QrImage?.Source != null)
            {
                return;
            }

            // 1) Thử lấy từ Resource stream
            try
            {
                var resourceUri = new Uri("/QLKDPhongTro.Presentation;component/Resources/Images/QR.jpg", UriKind.Relative);
                var streamInfo = Application.GetResourceStream(resourceUri);
                if (streamInfo != null)
                {
                    using (var s = streamInfo.Stream)
                    {
                        var bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.StreamSource = s;
                        bitmap.EndInit();
                        QrImage.Source = bitmap;
                        return;
                    }
                }
            }
            catch
            {
                // bỏ qua, thử bước 2
            }

            // 2) Thử nạp từ file copy ra Output (Content)
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
            catch
            {
                // im lặng nếu thất bại
            }
        }

        private void UnitPriceTextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is System.Windows.Controls.TextBox tb)
            {
                tb.IsReadOnly = false;
                tb.Focus();
                // Xử lý null hoặc empty string
                if (string.IsNullOrEmpty(tb.Text))
                {
                    tb.Text = "";
                    tb.CaretIndex = 0;
                }
                else
                {
                    tb.CaretIndex = tb.Text.Length;
                }
            }
        }

        private async void UnitPriceTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            if (sender is not System.Windows.Controls.TextBox tb) return;

            tb.IsReadOnly = true;

            if (InvoiceData == null) return;

            // Chuẩn hóa giá trị rỗng
            decimal? ParseNullable(string s)
            {
                if (decimal.TryParse(s, out var v)) return v;
                return null;
            }

            // Trường đã bind TwoWay nên InvoiceData đã có giá trị mới
            var success = await _financialController.UpdateInvoiceUnitPricesAsync(
                InvoiceData.MaThanhToan,
                InvoiceData.DonGiaDien,
                InvoiceData.SoDien,
                InvoiceData.DonGiaNuoc,
                InvoiceData.SoNuoc,
                InvoiceData.TienThue,
                InvoiceData.TienInternet,
                InvoiceData.TienVeSinh,
                InvoiceData.TienGiuXe,
                InvoiceData.ChiPhiKhac,
                InvoiceData.KhauTru
            );

            if (!success)
            {
                MessageBox.Show("Không thể lưu đơn giá. Vui lòng thử lại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Làm mới binding các giá trị tính toán bằng cách set lại DataContext
            var current = InvoiceData;
            InvoiceData = null;
            InvoiceData = current;
        }

        private async void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            if (InvoiceData == null)
            {
                MessageBox.Show("Không có dữ liệu hóa đơn để tải xuống.", "Thông báo", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Hiển thị dialog để chọn nơi lưu file
                var saveDialog = new SaveFileDialog
                {
                    Filter = "PDF files (*.pdf)|*.pdf",
                    FileName = $"HoaDon_{InvoiceData.MaThanhToan}_{InvoiceData.ThangNam.Replace("/", "_")}.pdf",
                    DefaultExt = "pdf",
                    Title = "Lưu hóa đơn PDF"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    // Lấy thông tin contract và room
                    ContractDto? contract = null;
                    string tenPhong = "N/A";
                    int soNguoiLuuTru = 1;
                    string email = "";

                    if (InvoiceData.MaHopDong > 0)
                    {
                        var contractController = ContractController.CreateDefault();
                        contract = await contractController.GetByIdAsync(InvoiceData.MaHopDong);
                        if (contract != null)
                        {
                            // Lấy tên phòng từ database dựa trên MaPhong từ contract
                            if (contract.MaPhong > 0)
                            {
                                var financialController = FinancialController.CreateDefault();
                                var room = await financialController.GetRoomByIdAsync(contract.MaPhong);
                                if (room != null)
                                {
                                    tenPhong = room.TenPhong ?? "N/A";
                                }
                            }
                            else
                            {
                                tenPhong = contract.TenPhong ?? "N/A";
                            }
                        }
                    }

                    // Chuyển đổi sang PlumeriaInvoiceDto
                    var plumeriaInvoice = InvoiceData.ToPlumeriaInvoiceDto(contract, email, tenPhong, soNguoiLuuTru);

                    // Tìm logo path
                    string logoPath = FindLogoPath();

                    // Tạo file PDF
                    string filePath = saveDialog.FileName;
                    PlumeriaInvoicePdfService.CreateInvoicePdf(plumeriaInvoice, filePath, logoPath);

                    MessageBox.Show($"Đã tải hóa đơn thành công!\nĐường dẫn: {filePath}", 
                        "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Mở file PDF sau khi tạo (tùy chọn)
                    try
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = filePath,
                            UseShellExecute = true
                        });
                    }
                    catch
                    {
                        // Bỏ qua nếu không thể mở file
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tạo file PDF: {ex.Message}", "Lỗi", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string FindLogoPath()
        {
            // Tìm logo trong Resources hoặc Output folder
            var paths = new[]
            {
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Images", "Logo.png"),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Images", "logo.png"),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Images", "Logo.jpg"),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Images", "logo.jpg")
            };

            foreach (var path in paths)
            {
                if (File.Exists(path))
                    return path;
            }

            // Nếu không tìm thấy, trả về empty string
            return string.Empty;
        }
    }
}
