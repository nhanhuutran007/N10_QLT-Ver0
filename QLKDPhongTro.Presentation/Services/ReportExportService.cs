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

		// ========== PROFIT (MONTHLY) ==========
		public static void ExportProfitMonthlyCsv(IEnumerable<PaymentDto> payments, string thangNam, string filePath)
		{
			var revenue = payments.Sum(p => p.TienThue ?? 0m);
			var expenses = CalcExpenseTotals(payments).Values.Sum();
			var profit = revenue - expenses;
			using var sw = new StreamWriter(filePath);
			sw.WriteLine($"Báo cáo Lợi nhuận tháng,{thangNam}");
			sw.WriteLine($"Doanh thu,{revenue:N0}");
			sw.WriteLine($"Chi phí,{expenses:N0}");
			sw.WriteLine($"Lợi nhuận,{profit:N0}");
		}

		public static void ExportProfitMonthlyPdf(IEnumerable<PaymentDto> payments, string thangNam, string filePath)
		{
			var revenue = payments.Sum(p => p.TienThue ?? 0m);
			var expenses = CalcExpenseTotals(payments).Values.Sum();
			var profit = revenue - expenses;

			var doc = new PdfDocument();
			var page = doc.AddPage();
			var gfx = XGraphics.FromPdfPage(page);

			var fontTitle = new XFont("Arial", 18);
			var fontBody = new XFont("Arial", 12);

			double y = 100;
			gfx.DrawString("BÁO CÁO LỢI NHUẬN THÁNG", fontTitle, XBrushes.Black, new XRect(0, 50, page.Width, 30), XStringFormats.Center);
			gfx.DrawString($"Tháng {thangNam}", fontBody, XBrushes.Black, new XRect(0, 75, page.Width, 20), XStringFormats.Center);

			gfx.DrawString($"Doanh thu: {revenue:N0} VNĐ", fontBody, XBrushes.Black, 80, y); y += 30;
			gfx.DrawString($"Chi phí: {expenses:N0} VNĐ", fontBody, XBrushes.Black, 80, y); y += 30;
			gfx.DrawString($"Lợi nhuận: {profit:N0} VNĐ", fontBody, XBrushes.Black, 80, y);

			doc.Save(filePath);
			doc.Dispose();
		}

		// ========== ROOM STATUS ==========
		public static void ExportRoomStatusCsv(IEnumerable<QLKDPhongTro.DataLayer.Models.RentedRoom> rooms, string thangNam, string filePath)
		{
			using var sw = new StreamWriter(filePath);
			sw.WriteLine($"Báo cáo Danh sách phòng,{thangNam}");
			sw.WriteLine("Phòng,Trạng thái,Giá");
			foreach (var r in rooms.OrderBy(r => r.TenPhong))
			{
				sw.WriteLine($"{r.TenPhong},{Safe(r.TrangThai)},{r.GiaCoBan:N0}");
			}
		}

		public static void ExportRoomStatusPdf(IEnumerable<QLKDPhongTro.DataLayer.Models.RentedRoom> rooms, string thangNam, string filePath)
		{
			var doc = new PdfDocument();
			var page = doc.AddPage();
			var gfx = XGraphics.FromPdfPage(page);

			var fontTitle = new XFont("Arial", 16);
			var fontHeader = new XFont("Arial", 10);
			var fontBody = new XFont("Arial", 9);

			double y = 40;
			gfx.DrawString("BÁO CÁO DANH SÁCH PHÒNG", fontTitle, XBrushes.Black, new XRect(0, y, page.Width, 20), XStringFormats.Center);
			y += 22;
			gfx.DrawString($"Tháng {thangNam}", fontHeader, XBrushes.Black, new XRect(0, y, page.Width, 20), XStringFormats.Center);
			y += 30;

			double x = 40;
			double[] widths = { 140, 140, 140 };
			string[] headers = { "Phòng", "Trạng thái", "Giá (VNĐ)" };

			DrawTableHeader(gfx, x, ref y, widths, headers, fontHeader);
			foreach (var r in rooms.OrderBy(r => r.TenPhong))
			{
				string[] row = new[] { r.TenPhong ?? "", r.TrangThai ?? "", (r.GiaCoBan).ToString("N0", CultureInfo.GetCultureInfo("vi-VN")) };
				DrawTableRow(gfx, x, ref y, widths, row, fontBody);
				if (y > page.Height - 60)
				{
					page = doc.AddPage();
					gfx = XGraphics.FromPdfPage(page);
					y = 40;
					DrawTableHeader(gfx, x, ref y, widths, headers, fontHeader);
				}
			}

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

		private static void DrawTableRow(XGraphics gfx, double x, ref double y, double[] widths, string[] cells, XFont font)
		{
			double rowHeight = 18;
			double cx = x;
			for (int i = 0; i < cells.Length; i++)
			{
				gfx.DrawRectangle(new XPen(XColors.LightGray), cx, y, widths[i], rowHeight);
				gfx.DrawString(cells[i] ?? "", font, XBrushes.Black, new XRect(cx, y, widths[i], rowHeight), XStringFormats.Center);
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

