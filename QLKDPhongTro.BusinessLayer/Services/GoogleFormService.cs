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
            try
            {
                var credentialPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "credentials.json");
                if (File.Exists(credentialPath))
                {
                    using var stream = new FileStream(credentialPath, FileMode.Open, FileAccess.Read);
                    return GoogleCredential.FromStream(stream)
                        .CreateScoped(SheetsService.Scope.SpreadsheetsReadonly);
                }

                // For development without credentials file
                return GoogleCredential.FromAccessToken("dummy-token");
            }
            catch (Exception ex)
            {
                throw new Exception($"Không thể khởi tạo Google Sheets credential: {ex.Message}");
            }
        }

        /// <summary>
        /// Read debt data from Google Sheet - phương thức bị thiếu
        /// </summary>
        public async Task<List<DebtCreationDto>> ReadDebtDataFromGoogleSheetAsync(string spreadsheetId, string range = "A:E")
        {
            var debtData = new List<DebtCreationDto>();

            try
            {
                if (_sheetsService == null)
                {
                    throw new InvalidOperationException("Sheets service is not initialized. Check credentials.");
                }

                var request = _sheetsService.Spreadsheets.Values.Get(spreadsheetId, range);
                var response = await request.ExecuteAsync();

                if (response.Values == null || response.Values.Count < 2)
                    return debtData;

                var headers = response.Values[0].Cast<string>().ToList();

                for (int i = 1; i < response.Values.Count; i++)
                {
                    var row = response.Values[i].Cast<string>().ToList();
                    var debtDto = ParseRowToDebtData(headers, row);
                    if (debtDto != null)
                    {
                        debtData.Add(debtDto);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi đọc dữ liệu từ Google Sheet: {ex.Message}");
            }

            return debtData;
        }

        private static DebtCreationDto ParseRowToDebtData(List<string> headers, List<string> row)
        {
            try
            {
                var debtDto = new DebtCreationDto
                {
                    Timestamp = GetCellValue(headers, row, "Timestamp"),
                    Email = GetCellValue(headers, row, "Email Address"),
                    RoomName = GetCellValue(headers, row, "Room Number"),
                    ElectricImageUrl = GetCellValue(headers, row, "Electric Meter Image"),
                    OldElectricValue = ParseDecimal(GetCellValue(headers, row, "Old Electric Value")),
                    CurrentElectricValue = ParseDecimal(GetCellValue(headers, row, "Current Electric Value"))
                };

                // Validate required fields
                if (string.IsNullOrEmpty(debtDto.RoomName))
                {
                    debtDto.ErrorMessage = "Thiếu tên phòng";
                    debtDto.IsProcessed = false;
                    debtDto.ProcessingStatus = "Lỗi";
                }
                else if (debtDto.CurrentElectricValue <= 0)
                {
                    debtDto.ErrorMessage = "Chỉ số điện không hợp lệ";
                    debtDto.IsProcessed = false;
                    debtDto.ProcessingStatus = "Lỗi";
                }
                else
                {
                    debtDto.IsProcessed = false;
                    debtDto.ProcessingStatus = "Chưa xử lý";
                }

                return debtDto;
            }
            catch (Exception ex)
            {
                return new DebtCreationDto
                {
                    ErrorMessage = $"Lỗi phân tích dữ liệu: {ex.Message}",
                    IsProcessed = false,
                    ProcessingStatus = "Lỗi"
                };
            }
        }

        private static string GetCellValue(List<string> headers, List<string> row, string headerName)
        {
            var index = headers.FindIndex(h =>
                h.Equals(headerName, StringComparison.OrdinalIgnoreCase));
            return index >= 0 && index < row.Count ? row[index] : string.Empty;
        }

        private static decimal ParseDecimal(string value)
        {
            if (string.IsNullOrEmpty(value)) return 0;
            return decimal.TryParse(value, out decimal result) ? result : 0;
        }

        // Original method kept for backward compatibility
        public async Task<List<DebtFormData>> GetFormDataAsync()
        {
            var formDataList = new List<DebtFormData>();
            // Implementation giả lập
            return await Task.FromResult(formDataList);
        }
    }

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