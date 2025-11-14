using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using QLKDPhongTro.BusinessLayer.DTOs;

namespace QLKDPhongTro.BusinessLayer.Services
{
    public class GoogleFormService
    {
        private readonly SheetsService _sheetsService;

        // ID file Google Sheet của bạn (giữ nguyên)
        private const string SPREADSHEET_ID = "1TXLdDe8aYi41_RJj8ZseOi12xcA4CvFuNmOoMSiw5vw";

        // === SỬA LỖI TẠI ĐÂY ===
        // 1. Thêm dấu nháy đơn '...' bao quanh tên sheet
        // 2. Thêm !A:F để lấy dữ liệu từ cột A đến F
        // Sửa lại đúng chính tả và thêm !A:F
        // Sửa lại đúng tên Sheet và thêm !A:F
        private const string SHEET_RANGE = "ElectricReport";

        public GoogleFormService()
        {
            var credential = GetCredential();
            _sheetsService = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "QLKDPhongTro"
            });
        }

        private GoogleCredential GetCredential()
        {
            // Code lấy credential (đã sửa ở bước trước để báo lỗi rõ ràng)
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var credentialPath = Path.Combine(baseDir, "credentials.json");

            if (!File.Exists(credentialPath))
            {
                throw new FileNotFoundException($"CHƯA CÓ FILE KEY! Hãy copy file 'credentials.json' vào thư mục: {credentialPath}");
            }

            try
            {
                using var stream = new FileStream(credentialPath, FileMode.Open, FileAccess.Read);
                return GoogleCredential.FromStream(stream)
                    .CreateScoped(SheetsService.Scope.SpreadsheetsReadonly);
            }
            catch (Exception ex)
            {
                throw new Exception($"File credentials.json lỗi: {ex.Message}");
            }
        }

        public async Task<List<DebtCreationDto>> ReadDebtDataFromGoogleSheetAsync(string spreadsheetId, string range = "")
        {
            var debtData = new List<DebtCreationDto>();

            try
            {
                // Nếu không truyền range, dùng mặc định đã sửa ở trên
                var rangeToUse = string.IsNullOrEmpty(range) ? SHEET_RANGE : range;

                // Nếu range truyền vào chưa có dấu nháy đơn mà có dấu cách, tự động sửa (Optional)
                if (!rangeToUse.StartsWith("'") && rangeToUse.Contains(" ") && rangeToUse.Contains("!"))
                {
                    var parts = rangeToUse.Split('!');
                    rangeToUse = $"'{parts[0]}'!{parts[1]}";
                }

                var request = _sheetsService.Spreadsheets.Values.Get(spreadsheetId, rangeToUse);
                var response = await request.ExecuteAsync();

                if (response.Values == null || response.Values.Count < 2)
                    return debtData;

                var headers = response.Values[0].Cast<string>().ToList();

                for (int i = 1; i < response.Values.Count; i++)
                {
                    var row = response.Values[i].Cast<string>().ToList();
                    if (row.Count == 0) continue;

                    var debtDto = ParseRowToDebtData(headers, row);
                    if (debtDto != null)
                    {
                        debtData.Add(debtDto);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi đọc Google Sheet: {ex.Message}");
            }

            return debtData;
        }

        private static DebtCreationDto ParseRowToDebtData(List<string> headers, List<string> row)
        {
            try
            {
                var debtDto = new DebtCreationDto
                {
                    // Các tên cột này phải khớp với dòng 1 trong file Sheet của bạn
                    Timestamp = GetCellValue(headers, row, "Dấu thời gian"), // Hoặc "Timestamp"
                    RoomName = GetCellValue(headers, row, "Tên phòng"),
                    CurrentElectricValue = ParseDecimal(GetCellValue(headers, row, "Chỉ số điện mới")),
                    ElectricImageUrl = GetCellValue(headers, row, "Ảnh đồng hồ điện"),
                    Email = GetCellValue(headers, row, "Địa chỉ email") // Hoặc "Email Address"
                };

                // Validate
                if (string.IsNullOrEmpty(debtDto.RoomName))
                    debtDto.ErrorMessage = "Thiếu tên phòng";
                else if (debtDto.CurrentElectricValue <= 0)
                    debtDto.ErrorMessage = "Chỉ số điện <= 0";

                return debtDto;
            }
            catch (Exception ex)
            {
                return new DebtCreationDto { ErrorMessage = $"Lỗi dòng: {ex.Message}" };
            }
        }

        private static string GetCellValue(List<string> headers, List<string> row, string headerName)
        {
            var index = headers.FindIndex(h => h.IndexOf(headerName, StringComparison.OrdinalIgnoreCase) >= 0);
            return index >= 0 && index < row.Count ? row[index] : string.Empty;
        }

        private static decimal ParseDecimal(string value)
        {
            if (string.IsNullOrEmpty(value)) return 0;
            value = value.Replace(",", ".");
            return decimal.TryParse(value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal result) ? result : 0;
        }

        public async Task<List<DebtFormData>> GetFormDataAsync()
        {
            // Gọi hàm đọc với Range mặc định đã sửa
            var rawData = await ReadDebtDataFromGoogleSheetAsync(SPREADSHEET_ID, SHEET_RANGE);

            return rawData.Select(item => new DebtFormData
            {
                Timestamp = item.Timestamp,
                Email = item.Email,
                RoomName = item.RoomName,
                ElectricImageUrl = item.ElectricImageUrl,
                OldElectricValue = item.OldElectricValue,
                CurrentElectricValue = item.CurrentElectricValue
            }).ToList();
        }
    }

    // Class DebtFormData cần thiết cho DebtProcessingService
    public class DebtFormData
    {
        public string Timestamp { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string RoomName { get; set; } = string.Empty;
        public string ElectricImageUrl { get; set; } = string.Empty;
        public decimal OldElectricValue { get; set; }
        public decimal CurrentElectricValue { get; set; }
        public bool HasManualValues => OldElectricValue > 0 && CurrentElectricValue > 0;
    }
}