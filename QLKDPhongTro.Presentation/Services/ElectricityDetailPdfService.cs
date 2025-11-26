using PdfSharp.Drawing;
using PdfSharp.Fonts;
using PdfSharp.Pdf;
using QLKDPhongTro.BusinessLayer.DTOs;
using System;
using System.IO;
using System.Windows;

namespace QLKDPhongTro.Presentation.Services
{
    /// <summary>
    /// Service tạo PDF cho bảng chi tiết chỉ số điện (standalone)
    /// </summary>
    public static class ElectricityDetailPdfService
    {
        private static XFont GetFont(string familyName, double size)
        {
            if (GlobalFontSettings.FontResolver == null)
            {
                GlobalFontSettings.FontResolver = new PdfFontResolver();
            }
            GlobalFontSettings.UseWindowsFontsUnderWindows = true;
            return new XFont(familyName, size);
        }

        private static XFont FontTitle => GetFont("Arial", 24);
        private static XFont FontSubtitle => GetFont("Arial", 16);
        private static XFont FontBody => GetFont("Arial", 9);
        private static XFont FontBodyBold => GetFont("Arial", 9);
        private static XFont FontSmall => GetFont("Arial", 8);

        private static readonly XColor ColorGrayText = XColor.FromArgb(128, 128, 128);

        /// <summary>
        /// Tạo PDF bảng chi tiết chỉ số điện
        /// </summary>
        public static string CreateElectricityDetailPdf(PlumeriaInvoiceDto invoice, string outputPath, string logoPath)
        {
            var document = new PdfDocument();
            var page = document.AddPage();
            var gfx = XGraphics.FromPdfPage(page);

            DrawHeader(gfx, invoice, logoPath);
            DrawCustomerInfo(gfx, invoice, 160);
            DrawElectricityDetails(gfx, invoice, 280);

            gfx.DrawString("-Kết thúc-", FontBody, XBrushes.Black, 
                new XRect(0, page.Height.Point - 100, page.Width.Point, 20), XStringFormats.Center);

            document.Save(outputPath);
            document.Dispose();

            return outputPath;
        }

        private static void DrawHeader(XGraphics gfx, PlumeriaInvoiceDto invoice, string logoPath)
        {
            var page = gfx.PdfPage;

            // Vẽ Logo
            double logoX = 50;
            double logoY = 20;
            double logoWidth = 120;
            double logoHeight = 75;

            XImage? logoImage = null;
            if (!string.IsNullOrEmpty(logoPath) && File.Exists(logoPath))
            {
                try
                {
                    logoImage = XImage.FromFile(logoPath);
                }
                catch { }
            }

            if (logoImage == null)
            {
                logoImage = LoadLogoFromResource();
            }

            if (logoImage != null)
            {
                try
                {
                    double imageAspectRatio = logoImage.PixelWidth / (double)logoImage.PixelHeight;
                    double targetAspectRatio = logoWidth / logoHeight;

                    double finalWidth = logoWidth;
                    double finalHeight = logoHeight;

                    if (imageAspectRatio > targetAspectRatio)
                    {
                        finalHeight = logoWidth / imageAspectRatio;
                    }
                    else
                    {
                        finalWidth = logoHeight * imageAspectRatio;
                    }

                    double logoOffsetX = (logoWidth - finalWidth) / 2;
                    double logoOffsetY = (logoHeight - finalHeight) / 2;

                    gfx.DrawImage(logoImage, logoX + logoOffsetX, logoY + logoOffsetY, finalWidth, finalHeight);
                }
                catch { }
            }

            // Thông tin liên hệ
            var ownerPhone = string.IsNullOrWhiteSpace(invoice.OwnerPhone) ? "+84 09 0808 3890" : invoice.OwnerPhone;
            var ownerEmail = string.IsNullOrWhiteSpace(invoice.OwnerEmail) ? "plumeriafamily@outlook.com" : invoice.OwnerEmail;
            gfx.DrawString($"[T. {ownerPhone}] [E. {ownerEmail}]", FontSmall, 
                new XSolidBrush(ColorGrayText), 
                new XRect(0, 30, page.Width.Point - 40, 20), XStringFormats.TopRight);

            // Tiêu đề
            gfx.DrawString("Bảng chi tiết chỉ số điện", FontTitle, XBrushes.Black, 
                new XRect(0, 70, page.Width.Point, 40), XStringFormats.Center);
            string thangNam = string.IsNullOrEmpty(invoice.ThangNam) ? DateTime.Now.ToString("MM/yyyy") : invoice.ThangNam;
            gfx.DrawString($"Tháng {thangNam}", FontSubtitle, XBrushes.Black, 
                new XRect(0, 105, page.Width.Point, 30), XStringFormats.Center);

            // Thông tin tham chiếu
            double rightColX = page.Width.Point - 200;
            gfx.DrawString("Ngày:", FontBody, XBrushes.Black, rightColX, 130);
            string ngayPhatHanh = invoice.NgayPhatHanh != default(DateTime) ? invoice.NgayPhatHanh.ToString("dd/MM/yyyy") : DateTime.Now.ToString("dd/MM/yyyy");
            gfx.DrawString(ngayPhatHanh, FontBody, XBrushes.Black, rightColX + 80, 130);
            gfx.DrawString("Số tham chiếu:", FontBody, XBrushes.Black, rightColX, 145);
            int chiSoDienMoi = invoice.ChiSoDienMoi > 0 ? invoice.ChiSoDienMoi : 0;
            gfx.DrawString($"PLUMERIA-{DateTime.Now.Year}-DIEN-{chiSoDienMoi:D2}", FontBody, XBrushes.Black, rightColX + 80, 145);
        }

        private static void DrawCustomerInfo(XGraphics gfx, PlumeriaInvoiceDto invoice, double startY)
        {
            double x = 50;
            double y = startY;

            gfx.DrawString("Kính gửi:", FontBodyBold, XBrushes.Black, x, y);
            y += 15;

            gfx.DrawString("[Khách hàng]:", FontBody, XBrushes.Black, x, y);
            gfx.DrawString(string.IsNullOrEmpty(invoice.TenKhachHang) ? "N/A" : invoice.TenKhachHang, FontBody, XBrushes.Black, x + 100, y);
            y += 12;

            gfx.DrawString("[Điện thoại]:", FontBody, XBrushes.Black, x, y);
            gfx.DrawString(string.IsNullOrEmpty(invoice.DienThoai) ? "N/A" : invoice.DienThoai, FontBody, XBrushes.Black, x + 100, y);
            y += 12;

            gfx.DrawString("[Email]:", FontBody, XBrushes.Black, x, y);
            gfx.DrawString(string.IsNullOrEmpty(invoice.Email) ? "N/A" : invoice.Email, FontBody, XBrushes.Black, x + 100, y);
            y += 12;

            gfx.DrawString("[Số hợp đồng]:", FontBody, XBrushes.Black, x, y);
            gfx.DrawString(string.IsNullOrEmpty(invoice.SoHopDong) ? "N/A" : invoice.SoHopDong, FontBody, XBrushes.Black, x + 100, y);
            y += 12;

            gfx.DrawString("[Ngày bắt đầu]:", FontBody, XBrushes.Black, x, y);
            string ngayBatDau = invoice.NgayBatDauHopDong != default(DateTime) ? invoice.NgayBatDauHopDong.ToString("dd/MM/yyyy") : "N/A";
            gfx.DrawString(ngayBatDau, FontBody, XBrushes.Black, x + 100, y);
            y += 15;

            gfx.DrawString("Phòng:", FontBodyBold, XBrushes.Black, x, y);
            gfx.DrawString(string.IsNullOrEmpty(invoice.TenPhong) ? "N/A" : invoice.TenPhong, FontBody, XBrushes.Black, x + 100, y);
            y += 12;

            gfx.DrawString("Số người lưu trú:", FontBodyBold, XBrushes.Black, x, y);
            gfx.DrawString(invoice.SoNguoiLuuTru.ToString(), FontBody, XBrushes.Black, x + 100, y);
        }

        private static void DrawElectricityDetails(XGraphics gfx, PlumeriaInvoiceDto invoice, double startY)
        {
            double y = startY;
            double labelX = 50;
            double valueX = 250;
            double valueWidth = 200;

            gfx.DrawString("Đơn giá:", FontBody, XBrushes.Black, labelX, y);
            decimal donGiaDien = invoice.DonGiaDien > 0 ? invoice.DonGiaDien : 0;
            gfx.DrawString($"{donGiaDien:N0} VND/Kwh", FontBodyBold, XBrushes.Black, 
                new XRect(valueX, y, valueWidth, 20), XStringFormats.TopRight);
            y += 30;

            gfx.DrawString("Chỉ số của tháng trước:", FontBodyBold, XBrushes.Black, labelX, y);
            y += 18;
            gfx.DrawString("Ngày giờ ghi chỉ số:", FontBody, XBrushes.Black, labelX, y);
            string ngayGhiDienCu = invoice.NgayGhiDienCu != default(DateTime) ? invoice.NgayGhiDienCu.ToString("dd/MM/yyyy") : DateTime.Now.AddMonths(-1).ToString("dd/MM/yyyy");
            gfx.DrawString(ngayGhiDienCu, FontBody, XBrushes.Black, valueX, y);
            y += 18;
            gfx.DrawString("Chỉ số điện:", FontBody, XBrushes.Black, labelX, y);
            int chiSoDienCu = invoice.ChiSoDienCu > 0 ? invoice.ChiSoDienCu : 0;
            gfx.DrawString($"{chiSoDienCu} Kwh", FontBodyBold, XBrushes.Black, 
                new XRect(valueX, y, valueWidth, 20), XStringFormats.TopRight);
            y += 30;

            gfx.DrawString("Chỉ số của tháng này:", FontBodyBold, XBrushes.Black, labelX, y);
            y += 18;
            gfx.DrawString("Ngày giờ ghi chỉ số:", FontBody, XBrushes.Black, labelX, y);
            string ngayGhiDienMoi = invoice.NgayGhiDienMoi != default(DateTime) ? invoice.NgayGhiDienMoi.ToString("dd/MM/yyyy") : DateTime.Now.ToString("dd/MM/yyyy");
            gfx.DrawString(ngayGhiDienMoi, FontBody, XBrushes.Black, valueX, y);
            y += 18;
            gfx.DrawString("Chỉ số điện:", FontBody, XBrushes.Black, labelX, y);
            gfx.DrawString($"{invoice.ChiSoDienMoi} Kwh", FontBodyBold, XBrushes.Black, 
                new XRect(valueX, y, valueWidth, 20), XStringFormats.TopRight);
            y += 30;

            gfx.DrawString("Mức tiêu thụ:", FontBody, XBrushes.Black, labelX, y);
            int mucTieuThuDien = invoice.MucTieuThuDien > 0 ? invoice.MucTieuThuDien : 0;
            gfx.DrawString($"{mucTieuThuDien} Kwh", FontBodyBold, XBrushes.Black, 
                new XRect(valueX, y, valueWidth, 20), XStringFormats.TopRight);
            y += 18;
            gfx.DrawString("Thành tiền:", FontBody, XBrushes.Black, labelX, y);
            string thanhTien = mucTieuThuDien > 0 && invoice.ThanhTienDien > 0 ? $"{invoice.ThanhTienDien:N0} VNĐ" : "- VNĐ";
            gfx.DrawString(thanhTien, FontBodyBold, XBrushes.Black, 
                new XRect(valueX, y, valueWidth, 20), XStringFormats.TopRight);
        }

        private static XImage? LoadLogoFromResource()
        {
            try
            {
                var resourceUri = new Uri("/QLKDPhongTro.Presentation;component/Resources/Images/Logo.png", UriKind.Relative);
                var streamInfo = Application.GetResourceStream(resourceUri);
                if (streamInfo != null)
                {
                    using (var stream = streamInfo.Stream)
                    {
                        byte[] buffer = new byte[stream.Length];
                        stream.Read(buffer, 0, buffer.Length);
                        string tempFile = Path.Combine(Path.GetTempPath(), $"logo_temp_{Guid.NewGuid()}.png");
                        File.WriteAllBytes(tempFile, buffer);
                        try
                        {
                            return XImage.FromFile(tempFile);
                        }
                        catch
                        {
                            try { File.Delete(tempFile); } catch { }
                            throw;
                        }
                    }
                }
            }
            catch { }

            try
            {
                var baseDir = AppDomain.CurrentDomain.BaseDirectory;
                var logoFile = Path.Combine(baseDir, "Resources", "Images", "Logo.png");
                if (File.Exists(logoFile))
                {
                    return XImage.FromFile(logoFile);
                }
            }
            catch { }

            return null;
        }
    }
}

