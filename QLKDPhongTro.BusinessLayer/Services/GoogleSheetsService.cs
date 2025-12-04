using QLKDPhongTro.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace QLKDPhongTro.BusinessLayer.Services
{
    public class GoogleSheetsService
    {
        private readonly HttpClient _httpClient;
        private const string SpreadsheetId = "1uyYHI2eWTK7volip0lCX3LkE1LtRdflbfZrFOtMyqcU";
        private const string SheetName = "Form_Responses"; // Tên sheet tab

        public GoogleSheetsService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Đọc dữ liệu từ Google Sheets và chuyển đổi thành danh sách MaintenanceIncident
        /// </summary>
        public async Task<List<GoogleSheetMaintenanceRow>> ReadMaintenanceDataAsync()
        {
            var result = new List<GoogleSheetMaintenanceRow>();

            try
            {
                // URL để export Google Sheet dưới dạng CSV
                // Format: https://docs.google.com/spreadsheets/d/{SPREADSHEET_ID}/gviz/tq?tqx=out:csv&sheet={SHEET_NAME}
                var csvUrl = $"https://docs.google.com/spreadsheets/d/{SpreadsheetId}/gviz/tq?tqx=out:csv&sheet={SheetName}";

                var response = await _httpClient.GetAsync(csvUrl);
                response.EnsureSuccessStatusCode();

                var csvContent = await response.Content.ReadAsStringAsync();

                // Parse CSV
                var lines = csvContent.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
                
                if (lines.Length < 2) // Ít nhất phải có header và 1 dòng dữ liệu
                    return result;

                // Bỏ qua dòng header (dòng đầu tiên)
                for (int i = 1; i < lines.Length; i++)
                {
                    var line = lines[i].Trim();
                    if (string.IsNullOrEmpty(line))
                        continue;

                    var row = ParseCsvLine(line);
                    if (row != null)
                    {
                        result.Add(row);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log lỗi nếu cần
                System.Diagnostics.Debug.WriteLine($"Lỗi đọc Google Sheets: {ex.Message}");
                throw;
            }

            return result;
        }

        private GoogleSheetMaintenanceRow? ParseCsvLine(string line)
        {
            // Parse CSV line, xử lý các trường hợp có dấu phẩy trong giá trị
            var fields = new List<string>();
            var currentField = "";
            var insideQuotes = false;

            foreach (var c in line)
            {
                if (c == '"')
                {
                    insideQuotes = !insideQuotes;
                }
                else if (c == ',' && !insideQuotes)
                {
                    fields.Add(currentField.Trim());
                    currentField = "";
                }
                else
                {
                    currentField += c;
                }
            }
            fields.Add(currentField.Trim()); // Thêm field cuối cùng

            // Cấu trúc: Dấu thời gian (cột A), Mã phòng (cột B), Mô tả sự cố (cột C), Ngày có thể sửa (cột D)
            // Chỉ cần ít nhất 3 cột: Timestamp, Mã phòng, Mô tả
            if (fields.Count < 3)
                return null;

            try
            {
                // Lấy ngày báo cáo từ cột A (Dấu thời gian)
                var timestamp = fields[0].Trim('"');
                var ngayBaoCao = ParseDateOrDefault(timestamp);

                // Lấy ngày có thể sửa từ cột D (nếu có)
                DateTime? ngayCoTheSua = null;
                if (fields.Count >= 4 && !string.IsNullOrWhiteSpace(fields[3]))
                {
                    var ngayCoTheSuaStr = fields[3].Trim('"');
                    if (!string.IsNullOrWhiteSpace(ngayCoTheSuaStr))
                    {
                        ngayCoTheSua = ParseDateOrDefault(ngayCoTheSuaStr);
                    }
                }

                var row = new GoogleSheetMaintenanceRow
                {
                    Timestamp = timestamp,
                    MaPhong = ParseIntOrDefault(fields[1]),
                    MoTaSuCo = fields[2].Trim('"'), // Loại bỏ dấu ngoặc kép nếu có
                    NgayBaoCao = ngayBaoCao, // Sử dụng ngày từ cột A (Timestamp)
                    NgayCoTheSua = ngayCoTheSua // Sử dụng ngày từ cột D (nếu có)
                };

                // Chỉ trả về nếu có đủ thông tin cần thiết
                if (row.MaPhong > 0 && !string.IsNullOrWhiteSpace(row.MoTaSuCo))
                {
                    return row;
                }
            }
            catch
            {
                // Bỏ qua dòng không hợp lệ
            }

            return null;
        }

        private int ParseIntOrDefault(string value)
        {
            if (int.TryParse(value.Trim(), out int result))
                return result;
            return 0;
        }

        private DateTime ParseDateOrDefault(string value)
        {
            // Thử các format ngày thường gặp, bao gồm cả timestamp từ Google Sheets
            var formats = new[]
            {
                // Google Sheets timestamp formats
                "M/d/yyyy H:mm:ss",
                "M/d/yyyy HH:mm:ss",
                "MM/dd/yyyy H:mm:ss",
                "MM/dd/yyyy HH:mm:ss",
                "d/M/yyyy H:mm:ss",
                "d/M/yyyy HH:mm:ss",
                "dd/MM/yyyy H:mm:ss",
                "dd/MM/yyyy HH:mm:ss",
                // Date only formats
                "dd/MM/yyyy",
                "d/M/yyyy",
                "dd-MM-yyyy",
                "d-M-yyyy",
                "yyyy-MM-dd",
                "MM/dd/yyyy",
                "M/d/yyyy"
            };

            foreach (var format in formats)
            {
                if (DateTime.TryParseExact(value.Trim(), format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
                {
                    return result;
                }
            }

            // Nếu không parse được, thử parse thông thường
            if (DateTime.TryParse(value.Trim(), CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime defaultResult))
            {
                return defaultResult;
            }

            // Mặc định là ngày hôm nay
            return DateTime.Now;
        }
    }

    /// <summary>
    /// Class để lưu dữ liệu từ Google Sheets
    /// </summary>
    public class GoogleSheetMaintenanceRow
    {
        public string Timestamp { get; set; } = string.Empty;
        public int MaPhong { get; set; }
        public string MoTaSuCo { get; set; } = string.Empty;
        public DateTime NgayBaoCao { get; set; }
        public DateTime? NgayCoTheSua { get; set; }
    }
}
