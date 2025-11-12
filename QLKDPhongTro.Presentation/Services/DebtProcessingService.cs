using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using QLKDPhongTro.BusinessLayer.DTOs;
using QLKDPhongTro.DataLayer.Models;
using QLKDPhongTro.DataLayer.Repositories;

namespace QLKDPhongTro.Presentation.Services
{
    public class DebtProcessingService
    {
        private readonly GoogleFormService _googleFormService;
        private readonly OcrService _ocrService;
        private readonly PaymentRepository _paymentRepository;
        private readonly ContractRepository _contractRepository;
        private readonly RentedRoomRepository _roomRepository;

        private const decimal ELECTRICITY_RATE = 3500; // 3.500 VND/kWh
        private const decimal WATER_RATE = 100000; // 100.000 VND/tháng

        public DebtProcessingService()
        {
            _googleFormService = new GoogleFormService();
            _ocrService = new OcrService();
            _paymentRepository = new PaymentRepository();
            _contractRepository = new ContractRepository();
            _roomRepository = new RentedRoomRepository();
        }

        public async Task<List<DebtCalculationResult>> ProcessDebtsAsync()
        {
            var results = new List<DebtCalculationResult>();

            try
            {
                var formDataList = await _googleFormService.GetFormDataAsync();

                foreach (var formData in formDataList)
                {
                    var result = await ProcessSingleDebtAsync(formData);
                    if (result != null)
                    {
                        results.Add(result);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xử lý công nợ: {ex.Message}");
            }

            return results;
        }

        private async Task<DebtCalculationResult> ProcessSingleDebtAsync(DebtFormData formData)
        {
            try
            {
                decimal currentElectricValue = 0;
                decimal electricityCost = 0;
                string ocrStatus = "Manual";

                if (formData.HasManualValues)
                {
                    currentElectricValue = formData.CurrentElectricValue;
                    electricityCost = CalculateElectricityCost(formData.OldElectricValue, currentElectricValue);
                }
                else
                {
                    var ocrResult = await ProcessElectricImageAsync(formData.ElectricImageUrl);
                    if (ocrResult.IsValid)
                    {
                        currentElectricValue = ocrResult.Value;
                        electricityCost = CalculateElectricityCost(formData.OldElectricValue, currentElectricValue);
                        ocrStatus = "OCR Success";
                    }
                    else
                    {
                        ocrStatus = $"OCR Failed: {ocrResult.ErrorMessage}";
                    }
                }

                decimal totalDebt = electricityCost + WATER_RATE;

                return new DebtCalculationResult
                {
                    RoomName = formData.RoomName,
                    Email = formData.Email,
                    Timestamp = formData.Timestamp,
                    OldElectricValue = formData.OldElectricValue,
                    CurrentElectricValue = currentElectricValue,
                    ElectricityUsage = currentElectricValue - formData.OldElectricValue,
                    ElectricityCost = electricityCost,
                    WaterCost = WATER_RATE,
                    TotalDebt = totalDebt,
                    OcrStatus = ocrStatus,
                    ElectricImageUrl = formData.ElectricImageUrl,
                    IsProcessed = true
                };
            }
            catch (Exception ex)
            {
                return new DebtCalculationResult
                {
                    RoomName = formData.RoomName,
                    Email = formData.Email,
                    Timestamp = formData.Timestamp,
                    IsProcessed = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        private async Task<MeterReadingResult> ProcessElectricImageAsync(string imageUrl)
        {
            try
            {
                var localImagePath = await DownloadImageAsync(imageUrl);
                if (string.IsNullOrEmpty(localImagePath))
                {
                    return new MeterReadingResult
                    {
                        Type = MeterType.Electricity,
                        Value = 0,
                        Confidence = 0f,
                        ErrorMessage = "Không thể tải ảnh từ URL"
                    };
                }

                return await _ocrService.AnalyzeImageAsync(localImagePath, MeterType.Electricity);
            }
            catch (Exception ex)
            {
                return new MeterReadingResult
                {
                    Type = MeterType.Electricity,
                    Value = 0,
                    Confidence = 0f,
                    ErrorMessage = $"Lỗi xử lý ảnh: {ex.Message}"
                };
            }
        }

        private static async Task<string> DownloadImageAsync(string imageUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(imageUrl))
                    return null;

                var tempPath = Path.GetTempPath();
                var fileName = $"electric_meter_{Guid.NewGuid()}.jpg";
                var localPath = Path.Combine(tempPath, fileName);

                using var webClient = new WebClient();
                await webClient.DownloadFileTaskAsync(new Uri(imageUrl), localPath);

                return File.Exists(localPath) ? localPath : null;
            }
            catch
            {
                return null;
            }
        }

        private static decimal CalculateElectricityCost(decimal oldValue, decimal currentValue)
        {
            if (currentValue <= oldValue)
                return 0;

            var usage = currentValue - oldValue;
            return usage * ELECTRICITY_RATE;
        }

        public async Task<int> CreatePaymentRecordsFromDebtsAsync(List<DebtCalculationResult> debts)
        {
            int createdCount = 0;

            foreach (var debt in debts.Where(d => d.IsProcessed))
            {
                try
                {
                    var contractId = await GetContractIdByRoomNameAsync(debt.RoomName);
                    if (contractId.HasValue)
                    {
                        var createPaymentDto = new CreatePaymentDto
                        {
                            MaHopDong = contractId.Value,
                            ThangNam = DateTime.Now.ToString("MM/yyyy"),
                            TienThue = 0, // Chỉ tính điện nước
                            TienDien = debt.ElectricityCost,
                            TienNuoc = debt.WaterCost,
                            TienInternet = 0,
                            TienVeSinh = 0,
                            TienGiuXe = 0,
                            ChiPhiKhac = 0,
                            SoDien = debt.ElectricityUsage,
                            SoNuoc = 1, // Mặc định 1 tháng
                            DonGiaDien = ELECTRICITY_RATE,
                            DonGiaNuoc = WATER_RATE
                        };

                        var payment = new Payment
                        {
                            MaHopDong = createPaymentDto.MaHopDong,
                            ThangNam = createPaymentDto.ThangNam,
                            TienThue = createPaymentDto.TienThue,
                            TienDien = createPaymentDto.TienDien,
                            TienNuoc = createPaymentDto.TienNuoc,
                            TienInternet = createPaymentDto.TienInternet,
                            TienVeSinh = createPaymentDto.TienVeSinh,
                            TienGiuXe = createPaymentDto.TienGiuXe,
                            ChiPhiKhac = createPaymentDto.ChiPhiKhac,
                            SoDien = createPaymentDto.SoDien,
                            SoNuoc = createPaymentDto.SoNuoc,
                            DonGiaDien = createPaymentDto.DonGiaDien,
                            DonGiaNuoc = createPaymentDto.DonGiaNuoc,
                            TrangThaiThanhToan = "Chưa trả",
                            NgayThanhToan = null,
                            TongTien = createPaymentDto.TienThue + createPaymentDto.TienDien + createPaymentDto.TienNuoc +
                                      createPaymentDto.TienInternet + createPaymentDto.TienVeSinh +
                                      createPaymentDto.TienGiuXe + createPaymentDto.ChiPhiKhac
                        };

                        var success = await _paymentRepository.CreateAsync(payment);
                        if (success)
                        {
                            createdCount++;
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Lỗi tạo payment cho phòng {debt.RoomName}: {ex.Message}");
                }
            }

            return createdCount;
        }

        private async Task<int?> GetContractIdByRoomNameAsync(string roomName)
        {
            try
            {
                var rooms = await _roomRepository.GetAllAsync();
                var room = rooms.FirstOrDefault(r =>
                    r.TenPhong.Equals(roomName, StringComparison.OrdinalIgnoreCase));

                if (room != null)
                {
                    var contracts = await _contractRepository.GetActiveContractsAsync();
                    var contract = contracts.FirstOrDefault(c => c.MaPhong == room.MaPhong);
                    return contract?.MaHopDong;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi lấy contract ID cho phòng {roomName}: {ex.Message}");
            }

            return null;
        }
    }

    public class DebtCalculationResult
    {
        public string RoomName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Timestamp { get; set; } = string.Empty;
        public decimal OldElectricValue { get; set; }
        public decimal CurrentElectricValue { get; set; }
        public decimal ElectricityUsage { get; set; }
        public decimal ElectricityCost { get; set; }
        public decimal WaterCost { get; set; }
        public decimal TotalDebt { get; set; }
        public string OcrStatus { get; set; } = string.Empty;
        public string ElectricImageUrl { get; set; } = string.Empty;
        public bool IsProcessed { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }
}