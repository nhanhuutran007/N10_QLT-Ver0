using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

namespace QLKDPhongTro.Presentation.Services
{
    public class GoogleFormService
    {
        private readonly SheetsService _sheetsService;
        private const string SPREADSHEET_ID = "YOUR_SPREADSHEET_ID";
        private const string RANGE = "Form Responses 1!A:Z";

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

                return GoogleCredential.GetApplicationDefault()
                    .CreateScoped(SheetsService.Scope.SpreadsheetsReadonly);
            }
            catch (Exception ex)
            {
                throw new Exception($"Không thể khởi tạo Google Sheets credential: {ex.Message}");
            }
        }

        public async Task<List<DebtFormData>> GetFormDataAsync()
        {
            var formDataList = new List<DebtFormData>();

            try
            {
                var request = _sheetsService.Spreadsheets.Values.Get(SPREADSHEET_ID, RANGE);
                var response = await request.ExecuteAsync();

                if (response.Values == null || response.Values.Count < 2)
                    return formDataList;

                var headers = response.Values[0].Cast<string>().ToList();

                for (int i = 1; i < response.Values.Count; i++)
                {
                    var row = response.Values[i].Cast<string>().ToList();
                    var formData = ParseRowToDebtFormData(headers, row);
                    if (formData != null)
                    {
                        formDataList.Add(formData);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi đọc dữ liệu từ Google Form: {ex.Message}");
            }

            return formDataList;
        }

        private static DebtFormData ParseRowToDebtFormData(List<string> headers, List<string> row)
        {
            try
            {
                var formData = new DebtFormData
                {
                    Timestamp = GetCellValue(headers, row, "Timestamp"),
                    Email = GetCellValue(headers, row, "Email Address"),
                    RoomName = GetCellValue(headers, row, "Room Number"),
                    ElectricImageUrl = GetCellValue(headers, row, "Electric Meter Image"),
                    OldElectricValue = ParseDecimal(GetCellValue(headers, row, "Old Electric Value")),
                    CurrentElectricValue = ParseDecimal(GetCellValue(headers, row, "Current Electric Value"))
                };

                if (string.IsNullOrEmpty(formData.RoomName) || string.IsNullOrEmpty(formData.ElectricImageUrl))
                {
                    return null;
                }

                return formData;
            }
            catch
            {
                return null;
            }
        }

        private static string GetCellValue(List<string> headers, List<string> row, string headerName)
        {
            var index = headers.FindIndex(h => h.Equals(headerName, StringComparison.OrdinalIgnoreCase));
            return index >= 0 && index < row.Count ? row[index] : string.Empty;
        }

        private static decimal ParseDecimal(string value)
        {
            if (string.IsNullOrEmpty(value)) return 0;
            return decimal.TryParse(value, out decimal result) ? result : 0;
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