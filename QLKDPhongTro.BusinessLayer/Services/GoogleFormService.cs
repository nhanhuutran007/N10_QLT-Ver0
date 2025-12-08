using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Drive.v3; // <-- THÊM DÒNG NÀY
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
        public DriveService DriveService { get; }
        // ID file Google Sheet của bạn (giữ nguyên)
        private const string SPREADSHEET_ID = "1uEHAOTSRmXTNINYWiZNonPt3TO7GbubmGKZyBremQxQ";

        // === FIX 1 ===
        // Sửa lại range theo đúng cú pháp A1 (Tên Sheet + Dải ô)
        // Thêm dấu nháy đơn '...' để hỗ trợ tên sheet có dấu cách
        // Mở rộng tới cột Z để tránh thiếu cột khi form thêm trường mới
        private const string SHEET_RANGE = "'Form Responses 1'!A:Z";

        public GoogleFormService()
        {
            var credential = GetCredential();
            _sheetsService = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "QLKDPhongTro"
            });

            // <-- THÊM KHỐI NÀY ĐỂ KHỞI TẠO DRIVE SERVICE -->
            DriveService = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "QLKDPhongTro"
            });
            // ------------------------------------------------
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
                    // === FIX: THÊM QUYỀN DRIVE VÀO ĐÂY ===
                    .CreateScoped(
                        SheetsService.Scope.SpreadsheetsReadonly,
                        DriveService.Scope.DriveReadonly
                    );
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
                // Logic này đã OK vì range mặc định (SHEET_RANGE) đã có dấu nháy đơn
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
                // Báo lỗi chi tiết hơn nếu range sai
                if (ex.Message.Contains("Unable to parse range"))
                {
                    throw new Exception($"Lỗi đọc Google Sheet: Dải ô (Range) '{SHEET_RANGE}' không hợp lệ. Vui lòng kiểm tra lại tên Sheet.", ex);
                }
                throw new Exception($"Lỗi đọc Google Sheet: {ex.Message}", ex);
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
            // Xử lý cả dấu phẩy (từ Google Form) và dấu chấm
            value = value.Replace(",", ".");
            return decimal.TryParse(value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal result) ? result : 0;
        }

        public async Task<List<DebtFormData>> GetFormDataAsync()
        {
            // Gọi hàm đọc với Range mặc định đã sửa
            var rawData = await ReadDebtDataFromGoogleSheetAsync(SPREADSHEET_ID, SHEET_RANGE);

            // === FIX 2 ===
            // 'item' là một DebtCreationDto, nó không chứa 'OldElectricValue'.
            // OldElectricValue sẽ được gán giá trị 0 (mặc định)
            // và sẽ được dịch vụ DebtProcessingService cập nhật sau từ DB.
            return rawData.Select(item => new DebtFormData
            {
                Timestamp = item.Timestamp,
                Email = item.Email,
                RoomName = item.RoomName,
                ElectricImageUrl = item.ElectricImageUrl,
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