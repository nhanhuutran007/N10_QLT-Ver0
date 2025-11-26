using PdfSharp.Drawing;
using PdfSharp.Fonts;
using PdfSharp.Pdf;
using QLKDPhongTro.BusinessLayer.DTOs;
using System;
using System.Globalization;
using System.IO;
using System.Windows;

namespace QLKDPhongTro.Presentation.Services
{
    public static class PlumeriaInvoicePdfService
    {
        // Định nghĩa các font - sử dụng lazy initialization với FontResolver
        private static XFont GetFont(string familyName, double size, bool isBold = false)
        {
            // Đảm bảo FontResolver được đăng ký
            if (GlobalFontSettings.FontResolver == null)
            {
                GlobalFontSettings.FontResolver = new PdfFontResolver();
            }
            GlobalFontSettings.UseWindowsFontsUnderWindows = true;

            // Tạo font với tên font và size
            return new XFont(familyName, size);
        }

        private static XFont FontTitle => GetFont("Arial", 24);
        private static XFont FontSubtitle => GetFont("Arial", 16);
        private static XFont FontHeader => GetFont("Arial", 9);
        private static XFont FontBody => GetFont("Arial", 9);
        private static XFont FontBodyBold => GetFont("Arial", 9);
        private static XFont FontSmall => GetFont("Arial", 8);

        private static readonly XColor ColorPrimary = XColor.FromArgb(0, 176, 240); // Màu xanh dương
        private static readonly XColor ColorGrayText = XColor.FromArgb(128, 128, 128);
        private static readonly XColor ColorRedText = XColor.FromArgb(255, 0, 0);

        public static string CreateInvoicePdf(PlumeriaInvoiceDto invoice, string outputPath, string logoPath)
        {
            var document = new PdfDocument();

            // === TRANG 1: THÔNG BÁO PHÍ ===
            DrawMainInvoicePage(document, invoice, logoPath);

            // === TRANG 2: CHI TIẾT ĐIỆN ===
            DrawElectricityDetailPage(document, invoice, logoPath);

            // === TRANG 3: CHI TIẾT NƯỚC ===
            DrawWaterDetailPage(document, invoice, logoPath);

            document.Save(outputPath);
            document.Dispose();

            return outputPath;
        }

        private static void DrawMainInvoicePage(PdfDocument document, PlumeriaInvoiceDto invoice, string logoPath)
        {
            var page = document.AddPage();
            var gfx = XGraphics.FromPdfPage(page);

            DrawHeader(gfx, invoice, logoPath, "Thông báo phí");
            DrawCustomerInfo(gfx, invoice, 160);
            DrawInvoiceTable(gfx, invoice, 270);
            DrawBankInfo(gfx, invoice, page.Height.Point - 220);
            DrawFooter(gfx, invoice, page, 1, 3);
        }

        private static void DrawElectricityDetailPage(PdfDocument document, PlumeriaInvoiceDto invoice, string logoPath)
        {
            var page = document.AddPage();
            var gfx = XGraphics.FromPdfPage(page);

            DrawHeader(gfx, invoice, logoPath, "Bảng chi tiết chỉ số điện");
            DrawCustomerInfo(gfx, invoice, 160);

            // Vẽ phần chi tiết điện với căn chỉnh tốt hơn
            double y = 280;
            double labelX = 50;
            double valueX = 250;
            double valueWidth = 200;
            
            gfx.DrawString("Đơn giá:", FontBody, XBrushes.Black, labelX, y);
            gfx.DrawString($"{invoice.DonGiaDien:N0} VND/Kwh", FontBodyBold, XBrushes.Black, new XRect(valueX, y, valueWidth, 20), XStringFormats.TopRight);
            y += 30;

            gfx.DrawString("Chỉ số của tháng trước:", FontBodyBold, XBrushes.Black, labelX, y);
            y += 18;
            DrawDetailLine(gfx, "Ngày giờ ghi chỉ số:", invoice.NgayGhiDienCu.ToString("dd/MM/yyyy"), y);
            y += 18;
            DrawDetailLineRight(gfx, "Chỉ số điện:", $"{invoice.ChiSoDienCu} Kwh", y, valueX, valueWidth);
            y += 30;

            gfx.DrawString("Chỉ số của tháng này:", FontBodyBold, XBrushes.Black, labelX, y);
            y += 18;
            DrawDetailLine(gfx, "Ngày giờ ghi chỉ số:", invoice.NgayGhiDienMoi.ToString("dd/MM/yyyy"), y);
            y += 18;
            DrawDetailLineRight(gfx, "Chỉ số điện:", $"{invoice.ChiSoDienMoi} Kwh", y, valueX, valueWidth);
            y += 30;

            gfx.DrawString("Mức tiêu thụ:", FontBody, XBrushes.Black, labelX, y);
            gfx.DrawString($"{invoice.MucTieuThuDien} Kwh", FontBodyBold, XBrushes.Black, new XRect(valueX, y, valueWidth, 20), XStringFormats.TopRight);
            y += 18;
            gfx.DrawString("Thành tiền:", FontBody, XBrushes.Black, labelX, y);
            gfx.DrawString($"{invoice.ThanhTienDien:N0} VNĐ", FontBodyBold, XBrushes.Black, new XRect(valueX, y, valueWidth, 20), XStringFormats.TopRight);

            gfx.DrawString("-Kết thúc-", FontBody, XBrushes.Black, new XRect(0, page.Height.Point - 100, page.Width.Point, 20), XStringFormats.Center);

            DrawPageNumber(gfx, page, 2, 3);
        }

        private static void DrawWaterDetailPage(PdfDocument document, PlumeriaInvoiceDto invoice, string logoPath)
        {
            var page = document.AddPage();
            var gfx = XGraphics.FromPdfPage(page);

            DrawHeader(gfx, invoice, logoPath, "Bảng chi tiết chỉ số nước");
            DrawCustomerInfo(gfx, invoice, 160);

            // Vẽ phần chi tiết nước (tương tự điện) với căn chỉnh tốt hơn
            double y = 280;
            double labelX = 50;
            double valueX = 250;
            double valueWidth = 200;
            
            gfx.DrawString("Đơn giá:", FontBody, XBrushes.Black, labelX, y);
            gfx.DrawString($"{invoice.DonGiaNuoc:N0} VNĐ/m³", FontBodyBold, XBrushes.Black, new XRect(valueX, y, valueWidth, 20), XStringFormats.TopRight);
            y += 30;

            gfx.DrawString("Chỉ số của tháng trước:", FontBodyBold, XBrushes.Black, labelX, y);
            y += 18;
            DrawDetailLine(gfx, "Ngày giờ ghi chỉ số:", invoice.NgayGhiNuocCu.ToString("dd/MM/yyyy"), y);
            y += 18;
            DrawDetailLineRight(gfx, "Chỉ số đồng hồ:", $"{invoice.ChiSoNuocCu} m³", y, valueX, valueWidth);
            y += 30;

            gfx.DrawString("Chỉ số của tháng này:", FontBodyBold, XBrushes.Black, labelX, y);
            y += 18;
            DrawDetailLine(gfx, "Ngày giờ ghi chỉ số:", invoice.NgayGhiNuocMoi.ToString("dd/MM/yyyy"), y);
            y += 18;
            DrawDetailLineRight(gfx, "Chỉ số đồng hồ:", $"{invoice.ChiSoNuocMoi} m³", y, valueX, valueWidth);
            y += 30;

            gfx.DrawString("Mức tiêu thụ:", FontBody, XBrushes.Black, labelX, y);
            gfx.DrawString($"{invoice.MucTieuThuNuoc} m³", FontBodyBold, XBrushes.Black, new XRect(valueX, y, valueWidth, 20), XStringFormats.TopRight);
            y += 18;
            gfx.DrawString("Thành tiền:", FontBody, XBrushes.Black, labelX, y);
            gfx.DrawString($"{invoice.ThanhTienNuoc:N0} VNĐ", FontBodyBold, XBrushes.Black, new XRect(valueX, y, valueWidth, 20), XStringFormats.TopRight);

            gfx.DrawString("-Kết thúc-", FontBody, XBrushes.Black, new XRect(0, page.Height.Point - 100, page.Width.Point, 20), XStringFormats.Center);

            DrawPageNumber(gfx, page, 3, 3);
        }

        // --- CÁC PHƯƠNG THỨC VẼ PHỤ TRỢ ---

        private static void DrawHeader(XGraphics gfx, PlumeriaInvoiceDto invoice, string logoPath, string title)
        {
            var page = gfx.PdfPage;
            
            // Vẽ Logo ở góc trái trên - vị trí hợp lý và cân đối, kích thước lớn hơn
            double logoX = 50;
            double logoY = 20;
            double logoWidth = 120;  // Tăng từ 80 lên 120
            double logoHeight = 75;  // Tăng từ 50 lên 75
            
            XImage? logoImage = null;
            
            // Thử load logo từ file path
            if (!string.IsNullOrEmpty(logoPath) && File.Exists(logoPath))
            {
                try
                {
                    logoImage = XImage.FromFile(logoPath);
                }
                catch
                {
                    // Nếu không load được từ file, thử load từ resource
                }
            }
            
            // Nếu chưa có logo, thử load từ resource
            if (logoImage == null)
            {
                logoImage = LoadLogoFromResource();
            }
            
            // Vẽ logo nếu có
            if (logoImage != null)
            {
                try
                {
                    // Tính toán tỷ lệ để giữ nguyên aspect ratio
                    double imageAspectRatio = logoImage.PixelWidth / (double)logoImage.PixelHeight;
                    double targetAspectRatio = logoWidth / logoHeight;
                    
                    double finalWidth = logoWidth;
                    double finalHeight = logoHeight;
                    
                    if (imageAspectRatio > targetAspectRatio)
                    {
                        // Image rộng hơn, điều chỉnh height
                        finalHeight = logoWidth / imageAspectRatio;
                    }
                    else
                    {
                        // Image cao hơn, điều chỉnh width
                        finalWidth = logoHeight * imageAspectRatio;
                    }
                    
                    // Căn giữa logo trong vùng đã định
                    double logoOffsetX = (logoWidth - finalWidth) / 2;
                    double logoOffsetY = (logoHeight - finalHeight) / 2;
                    
                    gfx.DrawImage(logoImage, logoX + logoOffsetX, logoY + logoOffsetY, finalWidth, finalHeight);
                }
                catch
                {
                    // Bỏ qua nếu không vẽ được logo
                }
            }

            // Thông tin liên hệ
            var ownerPhone = string.IsNullOrWhiteSpace(invoice.OwnerPhone) ? "+84 0869 089 999" : invoice.OwnerPhone;
            var ownerEmail = string.IsNullOrWhiteSpace(invoice.OwnerEmail) ? "nhanhuutran007@gmail.com" : invoice.OwnerEmail;
            gfx.DrawString($"[T. {ownerPhone}] [E. {ownerEmail}]", FontSmall, new XSolidBrush(ColorGrayText), new XRect(0, 30, page.Width.Point - 40, 20), XStringFormats.TopRight);

            // Tiêu đề
            gfx.DrawString(title, FontTitle, XBrushes.Black, new XRect(0, 70, page.Width.Point, 40), XStringFormats.Center);
            gfx.DrawString($"Tháng {invoice.ThangNam}", FontSubtitle, XBrushes.Black, new XRect(0, 105, page.Width.Point, 30), XStringFormats.Center);

            // Thông tin tham chiếu bên phải
            double rightColX = page.Width.Point - 200;
            gfx.DrawString("Ngày:", FontBody, XBrushes.Black, rightColX, 130);
            gfx.DrawString(invoice.NgayPhatHanh.ToString("dd/MM/yyyy"), FontBody, XBrushes.Black, rightColX + 80, 130);
            gfx.DrawString("Số tham chiếu:", FontBody, XBrushes.Black, rightColX, 145);
            gfx.DrawString(invoice.SoThamChieu, FontBody, XBrushes.Black, rightColX + 80, 145);
        }

        private static XImage? LoadLogoFromResource()
        {
            try
            {
                // 1) Thử load từ Resource stream (giống như QR.jpg)
                try
                {
                    var resourceUri = new Uri("/QLKDPhongTro.Presentation;component/Resources/Images/Logo.png", UriKind.Relative);
                    var streamInfo = Application.GetResourceStream(resourceUri);
                    if (streamInfo != null)
                    {
                        using (var stream = streamInfo.Stream)
                        {
                            // Đọc toàn bộ stream vào byte array
                            byte[] buffer = new byte[stream.Length];
                            stream.Read(buffer, 0, buffer.Length);
                            
                            // Lưu vào file tạm để PdfSharp có thể load
                            string tempFile = Path.Combine(Path.GetTempPath(), $"logo_temp_{Guid.NewGuid()}.png");
                            File.WriteAllBytes(tempFile, buffer);
                            
                            try
                            {
                                var image = XImage.FromFile(tempFile);
                                // Xóa file tạm sau khi load xong (sẽ được dispose khi không dùng nữa)
                                // File sẽ được xóa sau khi PDF được tạo xong
                                return image;
                            }
                            catch
                            {
                                // Nếu không load được, xóa file tạm
                                try { File.Delete(tempFile); } catch { }
                                throw;
                            }
                        }
                    }
                }
                catch
                {
                    // Bỏ qua, thử bước 2
                }

                // 2) Thử load từ file trong output directory
                try
                {
                    var baseDir = AppDomain.CurrentDomain.BaseDirectory;
                    var logoFile = Path.Combine(baseDir, "Resources", "Images", "Logo.png");
                    if (File.Exists(logoFile))
                    {
                        return XImage.FromFile(logoFile);
                    }
                }
                catch
                {
                    // Bỏ qua
                }

                // 3) Thử các đường dẫn khác
                var otherPaths = new[]
                {
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Images", "logo.png"),
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Images", "Logo.jpg"),
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Images", "logo.jpg")
                };

                foreach (var path in otherPaths)
                {
                    if (File.Exists(path))
                    {
                        return XImage.FromFile(path);
                    }
                }
            }
            catch
            {
                // Bỏ qua nếu không load được
            }
            
            return null;
        }

        private static void DrawCustomerInfo(XGraphics gfx, PlumeriaInvoiceDto invoice, double yStart)
        {
            var page = gfx.PdfPage;
            double y = yStart;
            gfx.DrawString("Kính gửi:", FontBody, XBrushes.Black, 50, y);

            // Khối thông tin khách hàng
            double labelX = 80;
            double valueX = 180;
            gfx.DrawString("[Khách hàng]", FontBody, XBrushes.Black, labelX, y += 20);
            gfx.DrawString(invoice.TenKhachHang, FontBodyBold, XBrushes.Black, valueX, y);
            gfx.DrawString("[Điện thoại]", FontBody, XBrushes.Black, labelX, y += 15);
            gfx.DrawString(invoice.DienThoai, FontBodyBold, XBrushes.Black, valueX, y);
            gfx.DrawString("[Email]", FontBody, XBrushes.Black, labelX, y += 15);
            gfx.DrawString(invoice.Email, FontBodyBold, XBrushes.Black, valueX, y);
            gfx.DrawString("[Số hợp đồng]", FontBody, XBrushes.Black, labelX, y += 15);
            gfx.DrawString(invoice.SoHopDong, FontBodyBold, XBrushes.Black, valueX, y);
            gfx.DrawString("[Ngày bắt đầu]", FontBody, XBrushes.Black, labelX, y += 15);
            gfx.DrawString(invoice.NgayBatDauHopDong.ToString("dd/MM/yyyy"), FontBodyBold, XBrushes.Black, valueX, y);

            // Khối thông tin phòng
            double rightColX = page.Width.Point - 200;
            gfx.DrawString("Phòng:", FontBody, XBrushes.Black, rightColX, yStart + 20);
            gfx.DrawString(invoice.TenPhong, FontBodyBold, XBrushes.Black, rightColX + 80, yStart + 20);
            gfx.DrawString("Số người lưu trú", FontBody, XBrushes.Black, rightColX, yStart + 35);
            gfx.DrawString(invoice.SoNguoiLuuTru.ToString(), FontBodyBold, XBrushes.Black, rightColX + 80, yStart + 35);
        }

        private static void DrawInvoiceTable(XGraphics gfx, PlumeriaInvoiceDto invoice, double yStart)
        {
            double y = yStart;
            double tableWidth = 500;
            double x = (gfx.PdfPage.Width.Point - tableWidth) / 2;

            // Định nghĩa các cột - căn chỉnh đều với cột "Đơn vị" (làm chuẩn)
            // Điều chỉnh vị trí để cột "Nội dung chi tiết" không bị lệch
            double[] colX = { x, x + 35, x + 170, x + 230, x + 300, x + 370 };
            double[] colWidths = { 30, 135, 60, 70, 70, 130 }; // Độ rộng các cột - điều chỉnh để cân đối hơn
            string[] headers = { "STT", "Nội dung chi tiết", "Đơn vị", "Đơn giá", "Số lượng", "Thành tiền" };

            // Vẽ Header của bảng
            gfx.DrawRectangle(XBrushes.LightGray, colX[0], y, tableWidth, 20);
            
            // Vẽ header với căn chỉnh đúng
            gfx.DrawString(headers[0], FontHeader, XBrushes.Black, new XRect(colX[0], y, colWidths[0], 20), XStringFormats.Center); // STT: center
            gfx.DrawString(headers[1], FontHeader, XBrushes.Black, new XRect(colX[1], y, colWidths[1], 20), XStringFormats.Center); // Nội dung: center để đều với các cột
            gfx.DrawString(headers[2], FontHeader, XBrushes.Black, new XRect(colX[2], y, colWidths[2], 20), XStringFormats.Center); // Đơn vị: center (chuẩn)
            gfx.DrawString(headers[3], FontHeader, XBrushes.Black, new XRect(colX[3], y, colWidths[3], 20), XStringFormats.Center); // Đơn giá: center (căn với Đơn vị)
            gfx.DrawString(headers[4], FontHeader, XBrushes.Black, new XRect(colX[4], y, colWidths[4], 20), XStringFormats.Center); // Số lượng: center (căn với Đơn vị)
            gfx.DrawString(headers[5], FontHeader, XBrushes.Black, new XRect(colX[5], y, colWidths[5], 20), XStringFormats.Center); // Thành tiền: center (căn với Đơn vị)
            
            y += 20;

            // Vẽ các dòng dữ liệu
            foreach (var item in invoice.CacKhoanMuc)
            {
                gfx.DrawRectangle(new XPen(XColors.LightGray), colX[0], y, tableWidth, 20);
                
                // STT: center
                gfx.DrawString(item.STT.ToString(), FontBody, XBrushes.Black, new XRect(colX[0], y, colWidths[0], 20), XStringFormats.Center);
                
                // Nội dung: center để đều với các cột khác
                gfx.DrawString(item.NoiDung, FontBody, XBrushes.Black, new XRect(colX[1], y, colWidths[1], 20), XStringFormats.Center);
                
                // Đơn vị: center (chuẩn)
                gfx.DrawString(item.DonVi, FontBody, XBrushes.Black, new XRect(colX[2], y, colWidths[2], 20), XStringFormats.Center);
                
                // Đơn giá: center (căn với Đơn vị)
                gfx.DrawString(item.DonGia.ToString("N0"), FontBody, XBrushes.Black, new XRect(colX[3], y, colWidths[3], 20), XStringFormats.Center);
                
                // Số lượng: center (căn với Đơn vị)
                string soLuongText = item.SoLuong == (int)item.SoLuong ? ((int)item.SoLuong).ToString() : item.SoLuong.ToString("0.00");
                gfx.DrawString(soLuongText, FontBody, XBrushes.Black, new XRect(colX[4], y, colWidths[4], 20), XStringFormats.Center);
                
                // Thành tiền: center (căn với Đơn vị)
                string thanhTien = item.ThanhTien > 0 ? item.ThanhTien.ToString("N0") : "-";
                gfx.DrawString(thanhTien, FontBody, XBrushes.Black, new XRect(colX[5], y, colWidths[5], 20), XStringFormats.Center);
                
                y += 20;
            }

            // Vẽ dòng tổng cộng - căn giữa với các cột
            y += 5;
            gfx.DrawString("TỔNG CỘNG", FontHeader, XBrushes.Black, new XRect(colX[4], y, colWidths[4], 20), XStringFormats.Center);
            gfx.DrawString(invoice.TongCong.ToString("N0"), FontHeader, XBrushes.Black, new XRect(colX[5], y, colWidths[5], 20), XStringFormats.Center);
        }

        private static void DrawBankInfo(XGraphics gfx, PlumeriaInvoiceDto invoice, double yStart)
        {
            double y = yStart;
            double rectWidth = 250;
            double x = 50;
            double labelWidth = 100; // Độ rộng cố định cho label
            double valueX = x + labelWidth + 10; // Vị trí bắt đầu của value
            double contentHeight = 80; // Tăng chiều cao để bao quanh đầy đủ phần "Chi nhánh"

            gfx.DrawRectangle(XBrushes.LightGray, x, y, rectWidth, 20);
            gfx.DrawString("Tài khoản thụ hưởng", FontHeader, XBrushes.Black, new XRect(x, y, rectWidth, 20), XStringFormats.Center);
            y += 20;

            gfx.DrawRectangle(new XPen(XColors.LightGray), x, y, rectWidth, contentHeight);
            
            // Căn chỉnh đều các dòng với label width cố định
            DrawBankDetailRowAligned(gfx, "Tên tài khoản", invoice.TenTaiKhoanNH, x, y, labelWidth, valueX);
            DrawBankDetailRowAligned(gfx, "Số tài khoản", invoice.SoTaiKhoanNH, x, y + 20, labelWidth, valueX);
            DrawBankDetailRowAligned(gfx, "Ngân hàng", invoice.NganHang, x, y + 40, labelWidth, valueX);
            DrawBankDetailRowAligned(gfx, "Chi nhánh", invoice.ChiNhanh, x, y + 60, labelWidth, valueX);
        }

        private static void DrawFooter(XGraphics gfx, PlumeriaInvoiceDto invoice, PdfPage page, int currentPage, int totalPages)
        {
            double y = page.Height.Point - 150;

            // Thời hạn thanh toán - căn chỉnh hợp lý hơn
            double rightX = page.Width.Point - 250;
            double boxWidth = 200;
            
            // Vẽ box đỏ
            gfx.DrawRectangle(new XSolidBrush(ColorRedText), rightX - 5, y - 5, boxWidth, 18);
            gfx.DrawString("[Thời hạn thanh toán]", FontBody, XBrushes.White, new XRect(rightX - 5, y - 5, boxWidth, 18), XStringFormats.Center);
            
            // Căn chỉnh "Trước ngày" và ngày tháng
            double labelDateX = rightX + (boxWidth - 120) / 2; // Căn giữa trong khoảng boxWidth
            gfx.DrawString("Trước ngày", FontBody, XBrushes.Black, new XRect(rightX - 5, y + 20, boxWidth, 20), XStringFormats.Center);
            gfx.DrawString(invoice.ThoiHanThanhToan.ToString("dd/MM/yyyy"), FontBodyBold, new XSolidBrush(ColorRedText), new XRect(rightX - 5, y + 38, boxWidth, 20), XStringFormats.Center);

            // Ghi chú
            gfx.DrawString("* Có thể thanh toán bằng Tiền mặt.", FontSmall, XBrushes.Black, 50, y + 70);

            // Lời cảm ơn
            gfx.DrawString("Trân trọng cảm ơn Quý khách.", FontBodyBold, XBrushes.Black, new XRect(0, page.Height.Point - 100, page.Width.Point, 20), XStringFormats.Center);

            DrawPageNumber(gfx, page, currentPage, totalPages);
        }

        private static void DrawPageNumber(XGraphics gfx, PdfPage page, int currentPage, int totalPages)
        {
            gfx.DrawString($"{currentPage}/{totalPages}", FontBody, XBrushes.Black, new XRect(0, page.Height.Point - 40, page.Width.Point, 20), XStringFormats.Center);
        }

        private static void DrawRightAlignedText(XGraphics gfx, string text, XFont font, double x, double y)
        {
            var size = gfx.MeasureString(text, font);
            gfx.DrawString(text, font, XBrushes.Black, x - size.Width, y);
        }

        private static void DrawBankDetailRow(XGraphics gfx, string label, string value, double x, double y)
        {
            gfx.DrawString(label, FontBody, XBrushes.Black, x + 5, y + 3);
            gfx.DrawString(value, FontBodyBold, XBrushes.Black, x + 90, y + 3);
        }

        private static void DrawBankDetailRowAligned(XGraphics gfx, string label, string value, double x, double y, double labelWidth, double valueX)
        {
            // Vẽ label với độ rộng cố định, right-aligned để căn đều
            gfx.DrawString(label + ":", FontBody, XBrushes.Black, new XRect(x + 5, y + 3, labelWidth - 5, 20), XStringFormats.TopRight);
            // Vẽ value căn đều
            gfx.DrawString(value, FontBodyBold, XBrushes.Black, new XRect(valueX, y + 3, 140, 20), XStringFormats.TopLeft);
        }

        private static void DrawDetailLine(XGraphics gfx, string label, string value, double y)
        {
            gfx.DrawString(label, FontBody, XBrushes.Black, 80, y);
            gfx.DrawString(value, FontBodyBold, XBrushes.Black, 250, y);
        }

        private static void DrawDetailLineRight(XGraphics gfx, string label, string value, double y, double valueX, double valueWidth)
        {
            gfx.DrawString(label, FontBody, XBrushes.Black, 80, y);
            gfx.DrawString(value, FontBodyBold, XBrushes.Black, new XRect(valueX, y, valueWidth, 20), XStringFormats.TopRight);
        }
    }
}