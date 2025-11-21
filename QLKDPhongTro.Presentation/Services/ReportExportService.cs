using PdfSharp.Drawing;
using PdfSharp.Fonts;
using PdfSharp.Pdf;
using QLKDPhongTro.BusinessLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace QLKDPhongTro.Presentation.Services
{
	public static class ReportExportService
	{
		// Public row model used for detailed expense export
		public class ExpenseRow
		{
			public string TenPhong { get; set; } = string.Empty;
			public DateTime? NgayThanhToan { get; set; }

			// Điện
			public decimal? ChiSoDienCu { get; set; }
			public decimal? ChiSoDienMoi { get; set; }
			public decimal? SoDien { get; set; }
			public decimal? DonGiaDien { get; set; }
			public decimal? TienDien { get; set; }

			// Nước
			public decimal? SoNuoc { get; set; }
			public decimal? DonGiaNuoc { get; set; }
			public decimal? TienNuoc { get; set; }

			// Khác/Sửa chữa
			public decimal? ChiPhiSuaChua { get; set; }

			// Dịch vụ cố định
			public decimal? TienInternet { get; set; }
			public decimal? TienVeSinh { get; set; }
			public decimal? TienGiuXe { get; set; }

			public decimal Tong => (TienDien ?? 0)
				+ (TienNuoc ?? 0)
				+ (TienInternet ?? 0)
				+ (TienVeSinh ?? 0)
				+ (TienGiuXe ?? 0)
				+ (ChiPhiSuaChua ?? 0);
		}

		// ========== REVENUE (MONTHLY) ==========
		public static void ExportRevenueMonthlyCsv(IEnumerable<PaymentDto> payments, string thangNam, string filePath)
		{
			var vi = CultureInfo.GetCultureInfo("vi-VN");
			using var sw = new StreamWriter(filePath);
			sw.WriteLine($"Báo cáo Doanh thu tháng,{thangNam}");
			sw.WriteLine("Phòng,Người thuê,Tiền thuê,Tình trạng thanh toán");
			foreach (var p in payments.OrderBy(p => p.TenPhong))
			{
				var tienThue = (p.TienThue ?? 0m).ToString("N0", vi);
				sw.WriteLine($"{p.TenPhong},{Safe(p.TenKhachHang)},{tienThue},{Safe(p.TrangThaiThanhToan)}");
			}
			sw.WriteLine();
			var total = payments.Sum(p => p.TienThue ?? 0m);
			sw.WriteLine($"Tổng doanh thu tháng,,{total:N0},");
		}

		public static void ExportRevenueMonthlyPdf(IEnumerable<PaymentDto> payments, string thangNam, string filePath)
		{
			// Đảm bảo có FontResolver để dùng font hệ thống
			if (GlobalFontSettings.FontResolver == null)
			{
				GlobalFontSettings.FontResolver = new PdfFontResolver();
			}

			// Chỉ bao gồm đúng 2 trạng thái: "Đã trả" hoặc "Trả một phần" (bỏ khoảng trắng và dấu tiếng Việt)
			var filtered = payments
				.Where(p =>
				{
					var status = NormalizeStatus(p.TrangThaiThanhToan);
					return status == "da tra" || status == "tra mot phan";
				})
				.ToList();

			var doc = new PdfDocument();
			var page = doc.AddPage();
			var gfx = XGraphics.FromPdfPage(page);

			// Dùng Times New Roman (ổn định hơn trên Windows)
			var fontTitle = new XFont("Times New Roman", 18);
			var fontHeader = new XFont("Times New Roman", 11);
			var fontBody = new XFont("Times New Roman", 10);
			var fontSmall = new XFont("Times New Roman", 9);

			// Lề trang
			double left = 50;
			double right = 50;
			double contentWidth = page.Width - left - right;

			// Header mô phỏng template doanh nghiệp nhỏ
			// Hàng 1: Tên công ty/địa chỉ (placeholder)
			double y = 40;
			gfx.DrawString("HỆ THỐNG QUẢN LÝ TRỌ HOMESTEAD", fontSmall, XBrushes.Black, new XRect(left, y, contentWidth / 2, 14), XStringFormats.TopLeft);
			y += 14;
			gfx.DrawString("ĐỊA CHỈ: 19, NGUYỄN HỮU THỌ, TÂN HƯNG, TP.HCM", fontSmall, XBrushes.Black, new XRect(left, y, contentWidth / 2, 14), XStringFormats.TopLeft);

			// Tiêu đề
			y = 90;
			gfx.DrawString($"BÁO CÁO DOANH THU THEO THÁNG", fontTitle, XBrushes.Black, new XRect(left, y, contentWidth, 24), XStringFormats.TopCenter);
			y += 26;

			// Dòng thời gian kỳ báo cáo + thời gian xuất
			DateTime startOfMonth, endOfMonth;
			try
			{
				var dt = DateTime.ParseExact(thangNam, "MM/yyyy", CultureInfo.InvariantCulture);
				startOfMonth = new DateTime(dt.Year, dt.Month, 1);
				endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);
			}
			catch
			{
				// Fallback nếu parse lỗi
				startOfMonth = DateTime.Now.Date;
				endOfMonth = DateTime.Now.Date;
			}

			var rangeText = $"TỪ NGÀY: {startOfMonth:dd/MM/yyyy} ĐẾN NGÀY: {endOfMonth:dd/MM/yyyy}";
			gfx.DrawString(rangeText, fontSmall, XBrushes.Black, new XRect(left, y, contentWidth, 16), XStringFormats.TopCenter);
			y += 20;
			gfx.DrawString($"Xuất lúc: {DateTime.Now:dd/MM/yyyy HH:mm}", fontSmall, XBrushes.Black, new XRect(left, y, contentWidth, 16), XStringFormats.TopRight);
			y += 14;

			// Bảng: STT | Ngày TT | Phòng | Người thuê | Tiền thuê | Tình trạng
			double x = left;
			double[] ratios = { 0.06, 0.14, 0.18, 0.30, 0.16, 0.16 };
			double[] widths = ratios.Select(r => Math.Round(contentWidth * r, 0)).ToArray();
			string[] headers = { "STT", "Ngày TT", "Phòng", "Người thuê", "Tiền thuê", "Tình trạng" };

			DrawTableHeader(gfx, x, ref y, widths, headers, fontHeader);
			int stt = 1;
			foreach (var p in filtered.OrderBy(p => p.TenPhong).ThenBy(p => p.NgayThanhToan))
			{
				var row = new string[]
				{
					stt.ToString(),
					p.NgayThanhToan?.ToString("dd/MM/yyyy") ?? "",
					p.TenPhong ?? "",
					p.TenKhachHang ?? "",
					(p.TienThue ?? 0m).ToString("N0", CultureInfo.GetCultureInfo("vi-VN")),
					p.TrangThaiThanhToan ?? ""
				};
				DrawTableRow(gfx, x, ref y, widths, row, fontBody);
				stt++;
				if (y > page.Height - 60)
				{
					page = doc.AddPage();
					gfx = XGraphics.FromPdfPage(page);
					y = 40;
					DrawTableHeader(gfx, x, ref y, widths, headers, fontHeader);
				}
			}

			y += 10;
			var totalRent = filtered.Sum(p => p.TienThue ?? 0m);

			// Hàng tổng cộng theo template
			// Vẽ ô "TỔNG CỘNG:" trải qua 4 cột đầu (STT + Ngày TT + Phòng + Người thuê)
			double totalLabelWidth = widths[0] + widths[1] + widths[2] + widths[3];
			double rowHeight = 18;
			gfx.DrawRectangle(new XPen(XColors.LightGray), x, y, totalLabelWidth, rowHeight);
			gfx.DrawString("TỔNG CỘNG:", fontHeader, XBrushes.Black, new XRect(x, y, totalLabelWidth, rowHeight), XStringFormats.Center);

			// Ô tổng tiền thuê (cột Tiền thuê)
			double totalRentWidth = widths[4];
			double totalStatusWidth = widths[5];
			double totalRentX = x + totalLabelWidth;
			gfx.DrawRectangle(new XPen(XColors.LightGray), totalRentX, y, totalRentWidth, rowHeight);
			gfx.DrawString($"{totalRent:N0}", fontHeader, XBrushes.Black, new XRect(totalRentX, y, totalRentWidth, rowHeight), XStringFormats.Center);

			// Ô trạng thái để trống
			double totalStatusX = totalRentX + totalRentWidth;
			gfx.DrawRectangle(new XPen(XColors.LightGray), totalStatusX, y, totalStatusWidth, rowHeight);
			y += rowHeight + 24;

			// Khu chữ ký: bám theo cuối bảng (không cố định cuối trang)
			// Nếu phần chữ ký gần tràn trang thì chuyển trang mới
			double estimatedSignatureHeight = 50;
			if (y + estimatedSignatureHeight > page.Height - 60)
			{
				page = doc.AddPage();
				gfx = XGraphics.FromPdfPage(page);
				y = 40;
			}

			// Chia 2 cột chữ ký
			double half = contentWidth / 2;
			double signY = y;

			// Ngày ... căn giữa phía trên khu bên phải
			gfx.DrawString($"Ngày.....tháng.....năm..........", fontSmall, XBrushes.Black, new XRect(left + half, signY, half, 14), XStringFormats.TopCenter);
			signY += 18;

			// Nhãn chữ ký
			gfx.DrawString("NGƯỜI LẬP BIỂU", fontSmall, XBrushes.Black, new XRect(left, signY, half, 14), XStringFormats.TopCenter);
			gfx.DrawString("KẾ TOÁN TRƯỞNG", fontSmall, XBrushes.Black, new XRect(left + half, signY, half, 14), XStringFormats.TopCenter);
			signY += 14;
			gfx.DrawString("(Ký, họ tên)", fontSmall, XBrushes.Black, new XRect(left, signY, half, 14), XStringFormats.TopCenter);
			gfx.DrawString("(Ký, họ tên)", fontSmall, XBrushes.Black, new XRect(left + half, signY, half, 14), XStringFormats.TopCenter);

			doc.Save(filePath);
			doc.Dispose();
		}

		// ========== EXPENSE (MONTHLY) ==========
		public static void ExportExpenseMonthlyCsv(IEnumerable<PaymentDto> payments, string thangNam, string filePath)
		{
			var totals = CalcExpenseTotals(payments);
			using var sw = new StreamWriter(filePath);
			sw.WriteLine($"Báo cáo Chi phí tháng,{thangNam}");
			sw.WriteLine("Khoản mục,Số tiền");
			foreach (var kv in totals)
			{
				sw.WriteLine($"{kv.Key},{kv.Value:N0}");
			}
			sw.WriteLine();
			sw.WriteLine($"Tổng chi phí,{totals.Values.Sum():N0}");
		}

		public static void ExportExpenseMonthlyPdf(IEnumerable<PaymentDto> payments, string thangNam, string filePath)
		{
			var totals = CalcExpenseTotals(payments);

			// Ensure font resolver for Arial
			if (GlobalFontSettings.FontResolver == null)
			{
				GlobalFontSettings.FontResolver = new PdfFontResolver();
			}
			GlobalFontSettings.UseWindowsFontsUnderWindows = true;

			var doc = new PdfDocument();
			var page = doc.AddPage();
			var gfx = XGraphics.FromPdfPage(page);

			var fontTitle = new XFont("Arial", 16);
			var fontHeader = new XFont("Arial", 10);
			var fontBody = new XFont("Arial", 9);

			double y = 40;
			gfx.DrawString("BÁO CÁO CHI PHÍ THÁNG", fontTitle, XBrushes.Black, new XRect(0, y, page.Width, 20), XStringFormats.Center);
			y += 22;
			gfx.DrawString($"Tháng {thangNam}", fontHeader, XBrushes.Black, new XRect(0, y, page.Width, 20), XStringFormats.Center);
			y += 30;

			double x = 60;
			double[] widths = { 220, 140 };
			string[] headers = { "Khoản mục", "Số tiền (VNĐ)" };

			DrawTableHeader(gfx, x, ref y, widths, headers, fontHeader);
			foreach (var kv in totals)
			{
				var row = new string[] { kv.Key, kv.Value.ToString("N0", CultureInfo.GetCultureInfo("vi-VN")) };
				DrawTableRow(gfx, x, ref y, widths, row, fontBody);
			}

			y += 10;
			gfx.DrawString($"Tổng chi phí: {totals.Values.Sum():N0} VNĐ", fontHeader, XBrushes.Black, x, y);

			doc.Save(filePath);
			doc.Dispose();
		}

		// Detailed EXPENSE (MONTHLY) with per-room rows and full columns from ThanhToan
		public static void ExportExpenseMonthlyCsv(IEnumerable<ExpenseRow> rows, string thangNam, string filePath)
		{
			var vi = CultureInfo.GetCultureInfo("vi-VN");
			using var sw = new StreamWriter(filePath);
			sw.WriteLine($"Báo cáo Chi phí tháng (chi tiết),{thangNam}");
			sw.WriteLine("STT,Phòng,CS điện cũ,CS điện mới,Số điện,ĐG điện,Tiền điện,Số nước,ĐG nước,Tiền nước,Internet,Vệ sinh,Giữ xe,Sửa chữa/T. khác,Tổng");

			int stt = 1;
			foreach (var r in rows.OrderBy(r => r.TenPhong))
			{
				sw.WriteLine(string.Join(",", new[]
				{
					stt.ToString(),
					Safe(r.TenPhong),
					(r.ChiSoDienCu ?? 0).ToString("0.##", vi),
					(r.ChiSoDienMoi ?? 0).ToString("0.##", vi),
					(r.SoDien ?? 0).ToString("0.##", vi),
					(r.DonGiaDien ?? 0).ToString("N0", vi),
					(r.TienDien ?? 0).ToString("N0", vi),
					(r.SoNuoc ?? 0).ToString("0.##", vi),
					(r.DonGiaNuoc ?? 0).ToString("N0", vi),
					(r.TienNuoc ?? 0).ToString("N0", vi),
					(r.TienInternet ?? 0).ToString("N0", vi),
					(r.TienVeSinh ?? 0).ToString("N0", vi),
					(r.TienGiuXe ?? 0).ToString("N0", vi),
					(r.ChiPhiSuaChua ?? 0).ToString("N0", vi),
					(r.Tong).ToString("N0", vi)
				}));
				stt++;
			}

			sw.WriteLine();
			sw.WriteLine(string.Join(",", new[]
			{
				"TỔNG","","","","","",
				rows.Sum(r => r.TienDien ?? 0).ToString("N0", vi),
				"","",
				rows.Sum(r => r.TienNuoc ?? 0).ToString("N0", vi),
				rows.Sum(r => r.TienInternet ?? 0).ToString("N0", vi),
				rows.Sum(r => r.TienVeSinh ?? 0).ToString("N0", vi),
				rows.Sum(r => r.TienGiuXe ?? 0).ToString("N0", vi),
				rows.Sum(r => r.ChiPhiSuaChua ?? 0).ToString("N0", vi),
				rows.Sum(r => r.Tong).ToString("N0", vi)
			}));
		}

		public static void ExportExpenseMonthlyPdf(IEnumerable<ExpenseRow> rows, string thangNam, string filePath)
		{
			// Ensure font resolver
			if (GlobalFontSettings.FontResolver == null)
			{
				GlobalFontSettings.FontResolver = new PdfFontResolver();
			}

			var doc = new PdfDocument();
			var page = doc.AddPage();
			page.Orientation = PdfSharp.PageOrientation.Landscape; // Present horizontally on A4
			var gfx = XGraphics.FromPdfPage(page);

			// Fonts
			var fontTitle = new XFont("Times New Roman", 18);
			var fontHeader = new XFont("Times New Roman", 10);
			var fontBody = new XFont("Times New Roman", 9);
			var fontSmall = new XFont("Times New Roman", 9);

			// Margins and width
			double left = 35;
			double right = 35;
			double contentWidth = page.Width - left - right;

			// Header
			double y = 40;
			gfx.DrawString("HỆ THỐNG QUẢN LÝ TRỌ HOMESTEAD", fontSmall, XBrushes.Black, new XRect(left, y, contentWidth / 2, 14), XStringFormats.TopLeft);
			y += 14;
			gfx.DrawString("ĐỊA CHỈ: 19, NGUYỄN HỮU THỌ, TÂN HƯNG, TP.HCM", fontSmall, XBrushes.Black, new XRect(left, y, contentWidth / 2, 14), XStringFormats.TopLeft);

			// Title
			y = 90;
			gfx.DrawString($"BÁO CÁO CHI PHÍ THEO THÁNG", fontTitle, XBrushes.Black, new XRect(left, y, contentWidth, 24), XStringFormats.TopCenter);
			y += 26;

			// Period and timestamp
			DateTime startOfMonth, endOfMonth;
			try
			{
				var dt = DateTime.ParseExact(thangNam, "MM/yyyy", CultureInfo.InvariantCulture);
				startOfMonth = new DateTime(dt.Year, dt.Month, 1);
				endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);
			}
			catch
			{
				startOfMonth = DateTime.Now.Date;
				endOfMonth = DateTime.Now.Date;
			}

			var rangeText = $"TỪ NGÀY: {startOfMonth:dd/MM/yyyy} ĐẾN NGÀY: {endOfMonth:dd/MM/yyyy}";
			gfx.DrawString(rangeText, fontSmall, XBrushes.Black, new XRect(left, y, contentWidth, 16), XStringFormats.TopCenter);
			y += 20;
			gfx.DrawString($"Xuất lúc: {DateTime.Now:dd/MM/yyyy HH:mm}", fontSmall, XBrushes.Black, new XRect(left, y, contentWidth, 16), XStringFormats.TopRight);
			y += 10;

			// Table columns (landscape): STT | Phòng | CSĐ cũ | CSĐ mới | Số điện | ĐG điện | Tiền điện | Số nước | ĐG nước | Tiền nước | Internet | Vệ sinh | Giữ xe | Sửa chữa | Tổng
			// Normalize ratios to ensure total width fits within content area, then center between margins
			double[] ratios = { 0.05, 0.12, 0.07, 0.07, 0.07, 0.08, 0.08, 0.06, 0.08, 0.08, 0.07, 0.07, 0.07, 0.07, 0.12 };
			double ratioSum = ratios.Sum();
			double[] widths = ratios.Select(r => Math.Round(contentWidth * (r / ratioSum), 0)).ToArray();
			double tableWidth = widths.Sum();
			double x = left + (contentWidth - tableWidth) / 2.0; // Center the table horizontally
			string[] headers = { "STT", "Phòng", "CSĐ cũ", "CSĐ mới", "Số điện", "ĐG điện", "Tiền điện", "Số nước", "ĐG nước", "Tiền nước", "Internet", "Vệ sinh", "Giữ xe", "Sửa chữa", "Tổng" };

			DrawTableHeader(gfx, x, ref y, widths, headers, fontHeader);

			int stt = 1;
			var vi = CultureInfo.GetCultureInfo("vi-VN");
			var ordered = rows.OrderBy(r => r.TenPhong).ThenBy(r => r.NgayThanhToan ?? DateTime.MaxValue);
			foreach (var r in ordered)
			{
				var row = new string[]
				{
					stt.ToString(),
					r.TenPhong ?? "",
					(r.ChiSoDienCu ?? 0).ToString("0.##", vi),
					(r.ChiSoDienMoi ?? 0).ToString("0.##", vi),
					(r.SoDien ?? 0).ToString("0.##", vi),
					(r.DonGiaDien ?? 0).ToString("N0", vi),
					(r.TienDien ?? 0).ToString("N0", vi),
					(r.SoNuoc ?? 0).ToString("0.##", vi),
					(r.DonGiaNuoc ?? 0).ToString("N0", vi),
					(r.TienNuoc ?? 0).ToString("N0", vi),
					(r.TienInternet ?? 0).ToString("N0", vi),
					(r.TienVeSinh ?? 0).ToString("N0", vi),
					(r.TienGiuXe ?? 0).ToString("N0", vi),
					(r.ChiPhiSuaChua ?? 0).ToString("N0", vi),
					(r.Tong).ToString("N0", vi)
				};

				DrawTableRow(gfx, x, ref y, widths, row, fontBody);
				stt++;

				// Pagination
				if (y > page.Height - 60)
				{
					page = doc.AddPage();
					page.Orientation = PdfSharp.PageOrientation.Landscape;
					gfx = XGraphics.FromPdfPage(page);
					y = 40;
					DrawTableHeader(gfx, x, ref y, widths, headers, fontHeader);
				}
			}

			// Totals row across columns
			y += 10;
			double rowHeight = 18;

			// Precompute X position of each column
			double[] colX = new double[widths.Length];
			colX[0] = x;
			for (int i = 1; i < widths.Length; i++)
				colX[i] = colX[i - 1] + widths[i - 1];

			// Merge first two columns for the label
			double labelWidth = widths[0] + widths[1];
			gfx.DrawRectangle(new XPen(XColors.LightGray), colX[0], y, labelWidth, rowHeight);
			gfx.DrawString("TỔNG CỘNG:", fontHeader, XBrushes.Black, new XRect(colX[0], y, labelWidth, rowHeight), XStringFormats.Center);

			// Draw empty cells for columns 2..5 and 7..8 to keep grid consistent
			for (int i = 2; i <= 5; i++)
				gfx.DrawRectangle(new XPen(XColors.LightGray), colX[i], y, widths[i], rowHeight);
			for (int i = 7; i <= 8; i++)
				gfx.DrawRectangle(new XPen(XColors.LightGray), colX[i], y, widths[i], rowHeight);

			// Totals for money columns
			var totalTienDien = rows.Sum(r => r.TienDien ?? 0);
			var totalTienNuoc = rows.Sum(r => r.TienNuoc ?? 0);
			var totalInternet = rows.Sum(r => r.TienInternet ?? 0);
			var totalVeSinh = rows.Sum(r => r.TienVeSinh ?? 0);
			var totalGiuXe = rows.Sum(r => r.TienGiuXe ?? 0);
			var totalSuaChua = rows.Sum(r => r.ChiPhiSuaChua ?? 0);
			var totalAll = rows.Sum(r => r.Tong);

			// Tiền điện (col 6)
			gfx.DrawRectangle(new XPen(XColors.LightGray), colX[6], y, widths[6], rowHeight);
			gfx.DrawString($"{totalTienDien:N0}", fontHeader, XBrushes.Black, new XRect(colX[6], y, widths[6], rowHeight), XStringFormats.Center);

			// Tiền nước (col 9)
			gfx.DrawRectangle(new XPen(XColors.LightGray), colX[9], y, widths[9], rowHeight);
			gfx.DrawString($"{totalTienNuoc:N0}", fontHeader, XBrushes.Black, new XRect(colX[9], y, widths[9], rowHeight), XStringFormats.Center);

			// Internet (col 10)
			gfx.DrawRectangle(new XPen(XColors.LightGray), colX[10], y, widths[10], rowHeight);
			gfx.DrawString($"{totalInternet:N0}", fontHeader, XBrushes.Black, new XRect(colX[10], y, widths[10], rowHeight), XStringFormats.Center);

			// Vệ sinh (col 11)
			gfx.DrawRectangle(new XPen(XColors.LightGray), colX[11], y, widths[11], rowHeight);
			gfx.DrawString($"{totalVeSinh:N0}", fontHeader, XBrushes.Black, new XRect(colX[11], y, widths[11], rowHeight), XStringFormats.Center);

			// Giữ xe (col 12)
			gfx.DrawRectangle(new XPen(XColors.LightGray), colX[12], y, widths[12], rowHeight);
			gfx.DrawString($"{totalGiuXe:N0}", fontHeader, XBrushes.Black, new XRect(colX[12], y, widths[12], rowHeight), XStringFormats.Center);

			// Sửa chữa (col 13)
			gfx.DrawRectangle(new XPen(XColors.LightGray), colX[13], y, widths[13], rowHeight);
			gfx.DrawString($"{totalSuaChua:N0}", fontHeader, XBrushes.Black, new XRect(colX[13], y, widths[13], rowHeight), XStringFormats.Center);

			// Tổng (col 14)
			gfx.DrawRectangle(new XPen(XColors.LightGray), colX[14], y, widths[14], rowHeight);
			gfx.DrawString($"{totalAll:N0}", fontHeader, XBrushes.Black, new XRect(colX[14], y, widths[14], rowHeight), XStringFormats.Center);

			// Signature section follows the table length (not fixed at page bottom)
			y += rowHeight + 24;

			double estimatedSignatureHeight = 50;
			if (y + estimatedSignatureHeight > page.Height - 60)
			{
				page = doc.AddPage();
				gfx = XGraphics.FromPdfPage(page);
				y = 40;
			}

			// Split area into two halves for signatures
			double half = contentWidth / 2;
			double signY = y;

			// Date line on the right half
			gfx.DrawString($"Ngày.....tháng.....năm..........", fontSmall, XBrushes.Black, new XRect(left + half, signY, half, 14), XStringFormats.TopCenter);
			signY += 18;

			// Signature labels
			gfx.DrawString("NGƯỜI LẬP BIỂU", fontSmall, XBrushes.Black, new XRect(left, signY, half, 14), XStringFormats.TopCenter);
			gfx.DrawString("KẾ TOÁN TRƯỞNG", fontSmall, XBrushes.Black, new XRect(left + half, signY, half, 14), XStringFormats.TopCenter);
			signY += 14;
			gfx.DrawString("(Ký, họ tên)", fontSmall, XBrushes.Black, new XRect(left, signY, half, 14), XStringFormats.TopCenter);
			gfx.DrawString("(Ký, họ tên)", fontSmall, XBrushes.Black, new XRect(left + half, signY, half, 14), XStringFormats.TopCenter);

			doc.Save(filePath);
			doc.Dispose();
		}

		// ========== PROFIT (MONTHLY) ==========
		public static void ExportProfitMonthlyCsv(IEnumerable<PaymentDto> payments, string thangNam, string filePath, string partialMode = "None")
		{
			var vi = CultureInfo.GetCultureInfo("vi-VN");

			// Lấy tất cả trạng thái.
			// Quy tắc:
			// - Đã trả: Doanh thu = SoTienDaTra; Chi phí = Điện + Nước + Internet + Vệ sinh + Chi phí khác; Lợi nhuận = Doanh thu - Chi phí.
			// - Trả một phần (và trạng thái khác): Lợi nhuận = SoTienDaTra - TongTien (có thể âm). Cột Chi phí vẫn hiển thị tổng chi phí (tham khảo).
			var filtered = payments
				.OrderBy(p => p.TenPhong)
				.ToList();

			using var sw = new StreamWriter(filePath);
			sw.WriteLine($"Báo cáo Lợi nhuận tháng,{thangNam}");
			sw.WriteLine("STT,Phòng,Tiền thuê,Chi phí,Tổng tiền,Doanh thu,Lợi nhuận");

			int stt = 1;
			decimal totalRent = 0, totalRevenue = 0, totalCost = 0, totalTongTien = 0, totalProfit = 0;
			foreach (var p in filtered)
			{
				var status = NormalizeStatus(p.TrangThaiThanhToan);
				decimal baseCosts = (p.TienDien ?? 0) + (p.TienNuoc ?? 0) + (p.TienInternet ?? 0) + (p.TienVeSinh ?? 0) + (p.ChiPhiKhac ?? 0);
				decimal doanhThu = (p.SoTienDaTra ?? 0);
				decimal chiPhi = baseCosts; // hiển thị chi phí loại tiền thuê và giữ xe
				decimal tienThue = (p.TienThue ?? 0);
				decimal tongTien = p.TongTien;

				decimal loiNhuan;
				if (status == "da tra")
				{
					// Đã trả: lợi nhuận = doanh thu - chi phí (bằng Tiền thuê + Giữ xe)
					loiNhuan = doanhThu - chiPhi;
				}
				else if (status == "tra mot phan")
				{
					// Trả một phần: thiếu so với tổng tiền
					loiNhuan = (p.SoTienDaTra ?? 0) - p.TongTien;
				}
				else
				{
					// Các trạng thái khác (vd: Chưa trả): coi như thiếu so với tổng tiền
					loiNhuan = (p.SoTienDaTra ?? 0) - p.TongTien;
				}

				totalRent += tienThue;
				totalRevenue += doanhThu;
				totalCost += chiPhi;
				totalTongTien += tongTien;
				totalProfit += loiNhuan;

				sw.WriteLine(string.Join(",", new[]
				{
					stt.ToString(),
					Safe(p.TenPhong),
					tienThue.ToString("N0", vi),
					chiPhi.ToString("N0", vi),
					tongTien.ToString("N0", vi),
					doanhThu.ToString("N0", vi),
					loiNhuan.ToString("N0", vi)
				}));
				stt++;
			}

			sw.WriteLine();
			sw.WriteLine($"Tổng,,{totalRent:N0},{totalCost:N0},{totalTongTien:N0},{totalRevenue:N0},{totalProfit:N0}");
		}

		public static void ExportProfitMonthlyPdf(IEnumerable<PaymentDto> payments, string thangNam, string filePath, string partialMode = "None")
		{
			// Đảm bảo có FontResolver để dùng font hệ thống
			if (GlobalFontSettings.FontResolver == null)
			{
				GlobalFontSettings.FontResolver = new PdfFontResolver();
			}

			// Lấy tất cả trạng thái; công thức như CSV (phân biệt Đã trả và Trả một phần)
			var filtered = payments.ToList();

			var doc = new PdfDocument();
			var page = doc.AddPage();
			page.Orientation = PdfSharp.PageOrientation.Landscape;
			var gfx = XGraphics.FromPdfPage(page);

			// Fonts giống layout doanh thu
			var fontTitle = new XFont("Times New Roman", 18);
			var fontHeader = new XFont("Times New Roman", 11);
			var fontBody = new XFont("Times New Roman", 10);
			var fontSmall = new XFont("Times New Roman", 9);

			// Lề trang
			double left = 35;
			double right = 35;
			double contentWidth = page.Width - left - right;

			// Header công ty
			double y = 40;
			gfx.DrawString("HỆ THỐNG QUẢN LÝ TRỌ HOMESTEAD", fontSmall, XBrushes.Black, new XRect(left, y, contentWidth / 2, 14), XStringFormats.TopLeft);
			y += 14;
			gfx.DrawString("ĐỊA CHỈ: 19, NGUYỄN HỮU THỌ, TÂN HƯNG, TP.HCM", fontSmall, XBrushes.Black, new XRect(left, y, contentWidth / 2, 14), XStringFormats.TopLeft);

			// Tiêu đề
			y = 90;
			gfx.DrawString($"BÁO CÁO LỢI NHUẬN THEO THÁNG", fontTitle, XBrushes.Black, new XRect(left, y, contentWidth, 24), XStringFormats.TopCenter);
			y += 26;

			// Kỳ báo cáo + thời gian xuất
			DateTime startOfMonth, endOfMonth;
			try
			{
				var dt = DateTime.ParseExact(thangNam, "MM/yyyy", CultureInfo.InvariantCulture);
				startOfMonth = new DateTime(dt.Year, dt.Month, 1);
				endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);
			}
			catch
			{
				startOfMonth = DateTime.Now.Date;
				endOfMonth = DateTime.Now.Date;
			}

			var rangeText = $"TỪ NGÀY: {startOfMonth:dd/MM/yyyy} ĐẾN NGÀY: {endOfMonth:dd/MM/yyyy}";
			gfx.DrawString(rangeText, fontSmall, XBrushes.Black, new XRect(left, y, contentWidth, 16), XStringFormats.TopCenter);
			y += 20;
			gfx.DrawString($"Xuất lúc: {DateTime.Now:dd/MM/yyyy HH:mm}", fontSmall, XBrushes.Black, new XRect(left, y, contentWidth, 16), XStringFormats.TopRight);
			y += 14;

			// Bảng: STT | Ngày TT | Phòng | Tiền thuê | Chi phí | Tổng tiền | Doanh thu | Lợi nhuận
			double[] ratios = { 0.07, 0.16, 0.20, 0.13, 0.14, 0.14, 0.08, 0.08 };
			double ratioSum = ratios.Sum();
			double[] widths = ratios.Select(r => Math.Round(contentWidth * (r / ratioSum), 0)).ToArray();
			double tableWidth = widths.Sum();
			double x = left + (contentWidth - tableWidth) / 2.0;
			string[] headers = { "STT", "Ngày TT", "Phòng", "Tiền thuê", "Chi phí", "Tổng tiền", "Doanh thu", "Lợi nhuận" };

			DrawTableHeader(gfx, x, ref y, widths, headers, fontHeader);

			int stt = 1;
			var vi = CultureInfo.GetCultureInfo("vi-VN");
			decimal totalRent = 0, totalRevenue = 0, totalCost = 0, totalTongTien = 0, totalProfit = 0;
			foreach (var p in filtered.OrderBy(p => p.TenPhong).ThenBy(p => p.NgayThanhToan))
			{
				var status = NormalizeStatus(p.TrangThaiThanhToan);
				decimal baseCosts = (p.TienDien ?? 0) + (p.TienNuoc ?? 0) + (p.TienInternet ?? 0) + (p.TienVeSinh ?? 0) + (p.ChiPhiKhac ?? 0);
				decimal doanhThu = (p.SoTienDaTra ?? 0);
				decimal chiPhi = baseCosts;
				decimal tienThue = (p.TienThue ?? 0);
				decimal tongTien = p.TongTien;

				decimal loiNhuan;
				if (status == "da tra")
				{
					loiNhuan = doanhThu - chiPhi;
				}
				else
				{
					loiNhuan = (p.SoTienDaTra ?? 0) - p.TongTien;
				}

				totalRent += tienThue;
				totalRevenue += doanhThu;
				totalCost += chiPhi;
				totalTongTien += tongTien;
				totalProfit += loiNhuan;

				var row = new string[]
				{
					stt.ToString(),
					p.NgayThanhToan?.ToString("dd/MM/yyyy") ?? "",
					p.TenPhong ?? "",
					tienThue.ToString("N0", vi),
					chiPhi.ToString("N0", vi),
					tongTien.ToString("N0", vi),
					doanhThu.ToString("N0", vi),
					loiNhuan.ToString("N0", vi)
				};
				DrawTableRow(gfx, x, ref y, widths, row, fontBody);
				stt++;

				if (y > page.Height - 60)
				{
					page = doc.AddPage();
					page.Orientation = PdfSharp.PageOrientation.Landscape;
					gfx = XGraphics.FromPdfPage(page);
					y = 40;
					DrawTableHeader(gfx, x, ref y, widths, headers, fontHeader);
				}
			}

			// Hàng tổng kết
			y += 10;
			double totalLabelWidth = widths[0] + widths[1] + widths[2];
			double rowHeight = 18;
			gfx.DrawRectangle(new XPen(XColors.LightGray), x, y, totalLabelWidth, rowHeight);
			gfx.DrawString("TỔNG CỘNG:", fontHeader, XBrushes.Black, new XRect(x, y, totalLabelWidth, rowHeight), XStringFormats.Center);

			// Tiền thuê
			double cx = x + totalLabelWidth;
			gfx.DrawRectangle(new XPen(XColors.LightGray), cx, y, widths[3], rowHeight);
			gfx.DrawString($"{totalRent:N0}", fontHeader, XBrushes.Black, new XRect(cx, y, widths[3], rowHeight), XStringFormats.Center);
			cx += widths[3];

			// Chi phí
			gfx.DrawRectangle(new XPen(XColors.LightGray), cx, y, widths[4], rowHeight);
			gfx.DrawString($"{totalCost:N0}", fontHeader, XBrushes.Black, new XRect(cx, y, widths[4], rowHeight), XStringFormats.Center);
			cx += widths[4];

			// Tổng tiền
			gfx.DrawRectangle(new XPen(XColors.LightGray), cx, y, widths[5], rowHeight);
			gfx.DrawString($"{totalTongTien:N0}", fontHeader, XBrushes.Black, new XRect(cx, y, widths[5], rowHeight), XStringFormats.Center);
			cx += widths[5];

			// Doanh thu
			gfx.DrawRectangle(new XPen(XColors.LightGray), cx, y, widths[6], rowHeight);
			gfx.DrawString($"{totalRevenue:N0}", fontHeader, XBrushes.Black, new XRect(cx, y, widths[6], rowHeight), XStringFormats.Center);
			cx += widths[6];

			// Lợi nhuận
			gfx.DrawRectangle(new XPen(XColors.LightGray), cx, y, widths[7], rowHeight);
			gfx.DrawString($"{totalProfit:N0}", fontHeader, XBrushes.Black, new XRect(cx, y, widths[7], rowHeight), XStringFormats.Center);

			// Notes about formulas
			y += rowHeight + 8;
			double noteNeeded = 4 * 14; // approx lines
			if (y + noteNeeded > page.Height - 60)
			{
				page = doc.AddPage();
				gfx = XGraphics.FromPdfPage(page);
				y = 40;
			}

			gfx.DrawString("Lưu ý:", fontSmall, XBrushes.Black, new XRect(left, y, contentWidth, 14), XStringFormats.TopLeft); y += 14;
			gfx.DrawString("• Đã trả: Doanh thu = Số tiền đã trả; Chi phí = Điện + Nước + Internet + Vệ sinh + Chi phí khác; Lợi nhuận = Doanh thu − Chi phí.", fontSmall, XBrushes.Black, new XRect(left, y, contentWidth, 14), XStringFormats.TopLeft); y += 14;
			gfx.DrawString("• Trả một phần: Lợi nhuận = Số tiền đã trả − Tổng tiền (có thể âm, thể hiện phần còn thiếu).", fontSmall, XBrushes.Black, new XRect(left, y, contentWidth, 14), XStringFormats.TopLeft); y += 14;
			gfx.DrawString("• Trạng thái khác: xử lý như Trả một phần (lợi nhuận có thể âm).", fontSmall, XBrushes.Black, new XRect(left, y, contentWidth, 14), XStringFormats.TopLeft); y += 10;

			// Chữ ký
			y += rowHeight + 24;
			double estimatedSignatureHeight = 50;
			if (y + estimatedSignatureHeight > page.Height - 60)
			{
				page = doc.AddPage();
				gfx = XGraphics.FromPdfPage(page);
				y = 40;
			}

			double half = contentWidth / 2;
			double signY = y;
			gfx.DrawString($"Ngày.....tháng.....năm..........", fontSmall, XBrushes.Black, new XRect(left + half, signY, half, 14), XStringFormats.TopCenter);
			signY += 18;
			gfx.DrawString("NGƯỜI LẬP BIỂU", fontSmall, XBrushes.Black, new XRect(left, signY, half, 14), XStringFormats.TopCenter);
			gfx.DrawString("KẾ TOÁN TRƯỞNG", fontSmall, XBrushes.Black, new XRect(left + half, signY, half, 14), XStringFormats.TopCenter);
			signY += 14;
			gfx.DrawString("(Ký, họ tên)", fontSmall, XBrushes.Black, new XRect(left, signY, half, 14), XStringFormats.TopCenter);
			gfx.DrawString("(Ký, họ tên)", fontSmall, XBrushes.Black, new XRect(left + half, signY, half, 14), XStringFormats.TopCenter);

			doc.Save(filePath);
			doc.Dispose();
		}

		// ========== ROOM STATUS ==========
		public static void ExportRoomStatusCsv(IEnumerable<QLKDPhongTro.DataLayer.Models.RentedRoom> rooms, string thangNam, string filePath)
		{
			using var sw = new StreamWriter(filePath);
			sw.WriteLine($"Báo cáo Danh sách phòng,{thangNam}");
			sw.WriteLine("STT,Phòng,Diện tích (m²),Giá cơ bản,Trạng thái,Trang thiết bị,Ghi chú");
			var vi = CultureInfo.GetCultureInfo("vi-VN");
			int stt = 1;
			foreach (var r in rooms.OrderBy(r => r.TenPhong))
			{
				int dienTichInt = (int)Math.Round(r.DienTich);
				string giaCoBan = (r.GiaCoBan).ToString("N0", vi);
				sw.WriteLine(string.Join(",", new[]
				{
					stt.ToString(),
					Safe(r.TenPhong),
					dienTichInt.ToString(vi),
					giaCoBan,
					Safe(r.TrangThai),
					Safe(r.TrangThietBi),
					Safe(r.GhiChu)
				}));
				stt++;
			}
		}

		public static void ExportRoomStatusPdf(IEnumerable<QLKDPhongTro.DataLayer.Models.RentedRoom> rooms, string thangNam, string filePath)
		{
			// Ensure font resolver for Arial
			if (GlobalFontSettings.FontResolver == null)
			{
				GlobalFontSettings.FontResolver = new PdfFontResolver();
			}
			GlobalFontSettings.UseWindowsFontsUnderWindows = true;

			var doc = new PdfDocument();
			var page = doc.AddPage();
			page.Orientation = PdfSharp.PageOrientation.Landscape;
			var gfx = XGraphics.FromPdfPage(page);

			// Use Times New Roman for consistency with other reports
			var fontTitle = new XFont("Times New Roman", 18);
			var fontHeader = new XFont("Times New Roman", 11);
			var fontBody = new XFont("Times New Roman", 10);
			var fontSmall = new XFont("Times New Roman", 9);

			// Margins and content width
			double left = 35;
			double right = 35;
			double contentWidth = page.Width - left - right;

			// Company header (match revenue layout)
			double y = 40;
			gfx.DrawString("HỆ THỐNG QUẢN LÝ TRỌ HOMESTEAD", fontSmall, XBrushes.Black, new XRect(left, y, contentWidth / 2, 14), XStringFormats.TopLeft);
			y += 14;
			gfx.DrawString("ĐỊA CHỈ: 19, NGUYỄN HỮU THỌ, TÂN HƯNG, TP.HCM", fontSmall, XBrushes.Black, new XRect(left, y, contentWidth / 2, 14), XStringFormats.TopLeft);

			// Title
			y = 90;
			gfx.DrawString("BÁO CÁO DANH SÁCH PHÒNG", fontTitle, XBrushes.Black, new XRect(left, y, contentWidth, 24), XStringFormats.TopCenter);
			y += 22;
			gfx.DrawString($"Tháng {thangNam}", fontHeader, XBrushes.Black, new XRect(left, y, contentWidth, 16), XStringFormats.TopCenter);
			y += 20;
			gfx.DrawString($"Xuất lúc: {DateTime.Now:dd/MM/yyyy HH:mm}", fontSmall, XBrushes.Black, new XRect(left, y, contentWidth, 16), XStringFormats.TopRight);
			y += 10;
 
			// Landscape layout, centered table like expense report
			double[] ratios = { 0.06, 0.14, 0.12, 0.14, 0.14, 0.23, 0.17 }; // STT, Phòng, Diện tích, Giá cơ bản, Trạng thái, Trang thiết bị, Ghi chú
			double ratioSum = ratios.Sum();
			double[] widths = ratios.Select(r => Math.Round(contentWidth * (r / ratioSum), 0)).ToArray();
			double tableWidth = widths.Sum();
			double x = left + (contentWidth - tableWidth) / 2.0;
			string[] headers = { "STT", "Phòng", "Diện tích (m²)", "Giá cơ bản", "Trạng thái", "Trang thiết bị", "Ghi chú" };

			DrawTableHeader(gfx, x, ref y, widths, headers, fontHeader);
			var vi = CultureInfo.GetCultureInfo("vi-VN");
			int stt = 1;
			foreach (var r in rooms.OrderBy(r => r.TenPhong))
			{
				string[] row = new[]
				{
					stt.ToString(),
					r.TenPhong ?? "",
					((int)Math.Round(r.DienTich)).ToString(vi),
					(r.GiaCoBan).ToString("N0", vi),
					r.TrangThai ?? "",
					Safe(r.TrangThietBi),
					Safe(r.GhiChu)
				};
				DrawTableRow(gfx, x, ref y, widths, row, fontBody);
				stt++;
				if (y > page.Height - 60)
				{
					page = doc.AddPage();
					page.Orientation = PdfSharp.PageOrientation.Landscape;
					gfx = XGraphics.FromPdfPage(page);
					y = 40;
					DrawTableHeader(gfx, x, ref y, widths, headers, fontHeader);
				}
			}

			// Signature block (match revenue layout)
			y += 24;
			double estimatedSignatureHeight = 50;
			if (y + estimatedSignatureHeight > page.Height - 60)
			{
				page = doc.AddPage();
				page.Orientation = PdfSharp.PageOrientation.Landscape;
				gfx = XGraphics.FromPdfPage(page);
				y = 40;
			}

			double half = contentWidth / 2;
			double signY = y;
			gfx.DrawString($"Ngày.....tháng.....năm..........", fontSmall, XBrushes.Black, new XRect(left + half, signY, half, 14), XStringFormats.TopCenter);
			signY += 18;
			gfx.DrawString("NGƯỜI LẬP BIỂU", fontSmall, XBrushes.Black, new XRect(left, signY, half, 14), XStringFormats.TopCenter);
			gfx.DrawString("KẾ TOÁN TRƯỞNG", fontSmall, XBrushes.Black, new XRect(left + half, signY, half, 14), XStringFormats.TopCenter);
			signY += 14;
			gfx.DrawString("(Ký, họ tên)", fontSmall, XBrushes.Black, new XRect(left, signY, half, 14), XStringFormats.TopCenter);
			gfx.DrawString("(Ký, họ tên)", fontSmall, XBrushes.Black, new XRect(left + half, signY, half, 14), XStringFormats.TopCenter);

			doc.Save(filePath);
			doc.Dispose();
		}

		// ========== DEBT (MONTHLY) ==========
		public static void ExportDebtMonthlyCsv(IEnumerable<DebtReportDto> debts, string thangNam, string filePath)
		{
			var vi = CultureInfo.GetCultureInfo("vi-VN");
			using var sw = new StreamWriter(filePath);
			sw.WriteLine($"Báo cáo Công nợ tháng,{thangNam}");
			sw.WriteLine("STT,Mã TT,Phòng,Khách hàng,Số tiền nợ (VNĐ)");
			int stt = 1;
			foreach (var d in debts.OrderBy(d => d.TenPhong).ThenBy(d => d.TenKhachHang))
			{
				sw.WriteLine(string.Join(",", new[]
				{
					stt.ToString(),
					d.MaThanhToan.ToString(),
					Safe(d.TenPhong),
					Safe(d.TenKhachHang),
					(d.TongTien).ToString("N0", vi)
				}));
				stt++;
			}
			sw.WriteLine();
			sw.WriteLine($"Tổng công nợ,,,,{debts.Sum(d => d.TongTien):N0}");
		}

		public static void ExportDebtMonthlyPdf(IEnumerable<DebtReportDto> debts, string thangNam, string filePath)
		{
			// Ensure font resolver
			if (GlobalFontSettings.FontResolver == null)
			{
				GlobalFontSettings.FontResolver = new PdfFontResolver();
			}
			GlobalFontSettings.UseWindowsFontsUnderWindows = true;

			var doc = new PdfDocument();
			var page = doc.AddPage();
			var gfx = XGraphics.FromPdfPage(page);

			var fontTitle = new XFont("Times New Roman", 18);
			var fontHeader = new XFont("Times New Roman", 11);
			var fontBody = new XFont("Times New Roman", 10);
			var fontSmall = new XFont("Times New Roman", 9);

			double left = 50;
			double right = 50;
			double contentWidth = page.Width - left - right;

			// Company header
			double y = 40;
			gfx.DrawString("HỆ THỐNG QUẢN LÝ TRỌ HOMESTEAD", fontSmall, XBrushes.Black, new XRect(left, y, contentWidth / 2, 14), XStringFormats.TopLeft);
			y += 14;
			gfx.DrawString("ĐỊA CHỈ: 19, NGUYỄN HỮU THỌ, TÂN HƯNG, TP.HCM", fontSmall, XBrushes.Black, new XRect(left, y, contentWidth / 2, 14), XStringFormats.TopLeft);

			// Title
			y = 90;
			gfx.DrawString("BÁO CÁO CÔNG NỢ", fontTitle, XBrushes.Black, new XRect(left, y, contentWidth, 24), XStringFormats.TopCenter);
			y += 22;
			gfx.DrawString($"Tháng {thangNam}", fontHeader, XBrushes.Black, new XRect(left, y, contentWidth, 16), XStringFormats.TopCenter);
			y += 20;
			gfx.DrawString($"Xuất lúc: {DateTime.Now:dd/MM/yyyy HH:mm}", fontSmall, XBrushes.Black, new XRect(left, y, contentWidth, 16), XStringFormats.TopRight);
			y += 10;

			// Table: STT | Mã TT | Phòng | Khách hàng | Số tiền nợ
			double x = left;
			double[] ratios = { 0.08, 0.14, 0.16, 0.38, 0.24 };
			double[] widths = ratios.Select(r => Math.Round(contentWidth * r, 0)).ToArray();
			string[] headers = { "STT", "Mã TT", "Phòng", "Khách hàng", "Số tiền nợ (VNĐ)" };

			DrawTableHeader(gfx, x, ref y, widths, headers, fontHeader);

			int stt = 1;
			var vi = CultureInfo.GetCultureInfo("vi-VN");
			foreach (var d in debts.OrderBy(d => d.TenPhong).ThenBy(d => d.TenKhachHang))
			{
				string[] row = new[]
				{
					stt.ToString(),
					d.MaThanhToan.ToString(),
					d.TenPhong ?? "",
					d.TenKhachHang ?? "",
					(d.TongTien).ToString("N0", vi)
				};
				DrawTableRow(gfx, x, ref y, widths, row, fontBody);
				stt++;
				if (y > page.Height - 60)
				{
					page = doc.AddPage();
					gfx = XGraphics.FromPdfPage(page);
					y = 40;
					DrawTableHeader(gfx, x, ref y, widths, headers, fontHeader);
				}
			}

			// Totals row
			y += 10;
			double totalLabelWidth = widths[0] + widths[1] + widths[2];
			double rowHeight = 18;
			gfx.DrawRectangle(new XPen(XColors.LightGray), x, y, totalLabelWidth, rowHeight);
			gfx.DrawString("TỔNG CỘNG:", fontHeader, XBrushes.Black, new XRect(x, y, totalLabelWidth, rowHeight), XStringFormats.Center);

			double cx = x + totalLabelWidth;
			gfx.DrawRectangle(new XPen(XColors.LightGray), cx, y, widths[4], rowHeight);
			gfx.DrawString($"{debts.Sum(d => d.TongTien):N0}", fontHeader, XBrushes.Black, new XRect(cx, y, widths[4], rowHeight), XStringFormats.Center);

			// Note under table
			y += rowHeight + 8;
			gfx.DrawString("Lưu ý: Danh sách công nợ gồm khách hàng chưa trả hoặc trả một phần nhưng vẫn còn thiếu tiền.", fontSmall, XBrushes.Black, new XRect(left, y, contentWidth, 14), XStringFormats.TopLeft);

			// Signatures
			y += 24;
			double estimatedSignatureHeight = 50;
			if (y + estimatedSignatureHeight > page.Height - 60)
			{
				page = doc.AddPage();
				gfx = XGraphics.FromPdfPage(page);
				y = 40;
			}

			double half = contentWidth / 2;
			double signY = y;
			gfx.DrawString($"Ngày.....tháng.....năm..........", fontSmall, XBrushes.Black, new XRect(left + half, signY, half, 14), XStringFormats.TopCenter);
			signY += 18;
			gfx.DrawString("NGƯỜI LẬP BIỂU", fontSmall, XBrushes.Black, new XRect(left, signY, half, 14), XStringFormats.TopCenter);
			gfx.DrawString("KẾ TOÁN TRƯỞNG", fontSmall, XBrushes.Black, new XRect(left + half, signY, half, 14), XStringFormats.TopCenter);
			signY += 14;
			gfx.DrawString("(Ký, họ tên)", fontSmall, XBrushes.Black, new XRect(left, signY, half, 14), XStringFormats.TopCenter);
			gfx.DrawString("(Ký, họ tên)", fontSmall, XBrushes.Black, new XRect(left + half, signY, half, 14), XStringFormats.TopCenter);

			doc.Save(filePath);
			doc.Dispose();
		}

		// ========== Helpers ==========
		private static Dictionary<string, decimal> CalcExpenseTotals(IEnumerable<PaymentDto> payments)
		{
			return new Dictionary<string, decimal>
			{
				{ "Điện", payments.Sum(p => p.TienDien ?? 0m) },
				{ "Nước", payments.Sum(p => p.TienNuoc ?? 0m) },
				{ "Internet", payments.Sum(p => p.TienInternet ?? 0m) },
				{ "Vệ sinh", payments.Sum(p => p.TienVeSinh ?? 0m) },
				{ "Giữ xe", payments.Sum(p => p.TienGiuXe ?? 0m) },
				{ "Khác", payments.Sum(p => p.ChiPhiKhac ?? 0m) }
			};
		}

		private static void DrawTableHeader(XGraphics gfx, double x, ref double y, double[] widths, string[] headers, XFont font)
		{
			double rowHeight = 20;
			double cx = x;
			for (int i = 0; i < headers.Length; i++)
			{
				gfx.DrawRectangle(XBrushes.LightGray, cx, y, widths[i], rowHeight);
				gfx.DrawString(headers[i], font, XBrushes.Black, new XRect(cx, y, widths[i], rowHeight), XStringFormats.Center);
				cx += widths[i];
			}
			y += rowHeight;
		}

		private static List<string> WrapText(XGraphics gfx, string text, XFont font, double maxWidth)
		{
			var result = new List<string>();
			if (string.IsNullOrEmpty(text)) { result.Add(string.Empty); return result; }

			string[] words = text.Split(new[] { ' ' }, StringSplitOptions.None);
			string line = string.Empty;
			foreach (var w in words)
			{
				string test = string.IsNullOrEmpty(line) ? w : line + " " + w;
				var size = gfx.MeasureString(test, font);
				if (size.Width <= maxWidth)
				{
					line = test;
				}
				else
				{
					if (!string.IsNullOrEmpty(line)) result.Add(line);
					// Nếu từ quá dài, chấp nhận xuống dòng ngay
					line = w;
					// Nếu vẫn quá dài, cắt cứng theo chiều rộng
					while (gfx.MeasureString(line, font).Width > maxWidth && line.Length > 1)
					{
						int cut = Math.Max(1, (int)(line.Length * maxWidth / gfx.MeasureString(line, font).Width));
						cut = Math.Max(1, Math.Min(cut, line.Length - 1));
						result.Add(line.Substring(0, cut));
						line = line.Substring(cut);
					}
				}
			}
			if (!string.IsNullOrEmpty(line)) result.Add(line);
			return result;
		}

		private static double GetLineHeight(XFont font)
		{
			return Math.Max(12, font.Size * 1.35);
		}

		private static void DrawTableRow(XGraphics gfx, double x, ref double y, double[] widths, string[] cells, XFont font)
		{
			double padding = 4;
			double cx = x;
			var linesPerCell = new List<List<string>>(cells.Length);
			double lineHeight = GetLineHeight(font);
			double rowHeight = 0;
			for (int i = 0; i < cells.Length; i++)
			{
				var wrapped = WrapText(gfx, cells[i] ?? string.Empty, font, Math.Max(0, widths[i] - padding * 2));
				linesPerCell.Add(wrapped);
				rowHeight = Math.Max(rowHeight, wrapped.Count * lineHeight + padding * 2);
			}

			for (int i = 0; i < cells.Length; i++)
			{
				gfx.DrawRectangle(new XPen(XColors.LightGray), cx, y, widths[i], rowHeight);
				var cellLines = linesPerCell[i];
				double contentHeight = cellLines.Count * lineHeight;
				double textY = y + Math.Max(padding, (rowHeight - contentHeight) / 2);
				foreach (var line in cellLines)
				{
					var size = gfx.MeasureString(line, font);
					double textX = cx + (widths[i] - size.Width) / 2; // center horizontally
					gfx.DrawString(line, font, XBrushes.Black, new XRect(textX, textY, size.Width, lineHeight), XStringFormats.TopLeft);
					textY += lineHeight;
				}
				cx += widths[i];
			}

			y += rowHeight;
		}

		private static string Safe(string? s) => s?.Replace(',', ' ') ?? "";

		private static string NormalizeStatus(string? s)
		{
			if (string.IsNullOrWhiteSpace(s)) return string.Empty;
			var trimmed = s.Trim();
			// Chuẩn hóa, bỏ dấu và quy về chữ thường, đổi 'đ/Đ' thành 'd/D'
			string normalized = trimmed.Normalize(System.Text.NormalizationForm.FormD);
			var sb = new System.Text.StringBuilder(normalized.Length);
			foreach (var ch in normalized)
			{
				var uc = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(ch);
				if (uc != System.Globalization.UnicodeCategory.NonSpacingMark)
				{
					if (ch == 'đ') sb.Append('d');
					else if (ch == 'Đ') sb.Append('D');
					else sb.Append(ch);
				}
			}
			return sb.ToString().Normalize(System.Text.NormalizationForm.FormC).ToLowerInvariant();
		}
	}
}

