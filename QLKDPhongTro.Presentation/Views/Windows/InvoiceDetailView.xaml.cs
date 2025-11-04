using System.Windows;
using System.Windows.Media.Imaging;
using QLKDPhongTro.BusinessLayer.DTOs;
using QLKDPhongTro.BusinessLayer.Controllers;
using System;
using System.IO;
using System.Windows.Input;

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
            this.Height = 850;
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
    }
}
