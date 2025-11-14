using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using QLKDPhongTro.BusinessLayer.DTOs;
using QLKDPhongTro.BusinessLayer.Services;
using QLKDPhongTro.DataLayer.Models;
using QLKDPhongTro.DataLayer.Repositories;
using QLKDPhongTro.Presentation.Services;

namespace QLKDPhongTro.Presentation.Services
{
    public class DebtProcessingService
    {
        private readonly GoogleFormService _googleFormService;
        private readonly OcrService _ocrService;
        private readonly PaymentRepository _paymentRepository;
        private readonly ContractRepository _contractRepository;
        private readonly RentedRoomRepository _roomRepository;

        private const decimal ELECTRICITY_RATE = 3500;
        private const decimal WATER_RATE = 100000;

        // Dictionary để lưu trữ thông tin sự cố tạm thời
        private readonly Dictionary<string, DebtDiscrepancyInfo> _discrepancyCache = new();

        public DebtProcessingService()
        {
            _googleFormService = new GoogleFormService();
            _ocrService = new OcrService();
            _paymentRepository = new PaymentRepository();
            _contractRepository = new ContractRepository();
            _roomRepository = new RentedRoomRepository();
        }

        // Phương thức hiện có giữ nguyên
        public async Task<List<DebtCalculationResult>> ProcessDebtsAsync()
        {
            var results = new List<DebtCalculationResult>();
            _discrepancyCache.Clear(); // Clear cache mỗi lần xử lý mới

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
                // 1. Lấy chỉ số cũ từ DB
                decimal dbOldElectricValue = 0;
                int? contractId = await GetContractIdByRoomNameAsync(formData.RoomName);

                if (contractId == null)
                {
                    return new DebtCalculationResult
                    {
                        RoomName = formData.RoomName,
                        IsProcessed = false,
                        ErrorMessage = $"Không tìm thấy hợp đồng cho phòng {formData.RoomName}"
                    };
                }

                var lastPayment = await GetLastPaymentByContractIdAsync(contractId.Value);
                if (lastPayment != null)
                {
                    dbOldElectricValue = lastPayment.ChiSoDienMoi ?? 0;
                }

                // 2. Xử lý so sánh: Form nhập tay vs OCR
                decimal finalElectricValue = 0;
                decimal manualValue = formData.CurrentElectricValue;
                decimal ocrValue = -1;
                string ocrStatus = "";
                bool isDiscrepancy = false;
                string warningNote = "";
                string originalImagePath = null;
                MeterReadingResult meterReadingResult = null;

                // Luôn chạy OCR để đối chiếu (nếu có ảnh)
                if (!string.IsNullOrEmpty(formData.ElectricImageUrl))
                {
                    // Tải ảnh về local
                    originalImagePath = await DownloadImageAsync(formData.ElectricImageUrl);
                    if (!string.IsNullOrEmpty(originalImagePath))
                    {
                        meterReadingResult = await ProcessElectricImageAsync(originalImagePath);
                        ocrValue = meterReadingResult.Value;
                        ocrStatus = meterReadingResult.IsValid ? "OCR OK" : "OCR Fail";
                    }
                }

                // Logic so sánh và phát hiện sự cố
                if (manualValue > 0 && ocrValue > 0)
                {
                    decimal difference = Math.Abs(manualValue - ocrValue);
                    if (difference > 1) // Ngưỡng chênh lệch
                    {
                        isDiscrepancy = true;
                        warningNote = $"CẢNH BÁO: Khách nhập {manualValue} nhưng Ảnh đọc được {ocrValue} (Chênh lệch: {difference})";
                        finalElectricValue = manualValue; // Tạm dùng giá trị nhập tay

                        // Lưu thông tin sự cố vào cache
                        _discrepancyCache[formData.RoomName] = new DebtDiscrepancyInfo
                        {
                            RoomName = formData.RoomName,
                            ManualValue = manualValue,
                            OcrValue = ocrValue,
                            // FIX: Đã xóa dòng "Difference = difference," vì 'Difference' là read-only
                            WarningNote = warningNote,
                            MeterReadingResult = meterReadingResult,
                            OriginalImagePath = originalImagePath,
                            DetectionTime = DateTime.Now
                        };
                    }
                    else
                    {
                        finalElectricValue = manualValue;
                        ocrStatus += " (Khớp)";
                    }
                }
                else if (manualValue > 0)
                {
                    finalElectricValue = manualValue;
                    warningNote = ocrValue == -1 ? "Không có ảnh kiểm chứng" : "Ảnh không đọc được số";
                }
                else if (ocrValue > 0)
                {
                    finalElectricValue = ocrValue;
                    warningNote = "Dùng số từ ảnh (Khách không nhập)";
                }
                else
                {
                    return new DebtCalculationResult
                    {
                        RoomName = formData.RoomName,
                        IsProcessed = false,
                        ErrorMessage = "Thiếu cả số nhập tay và ảnh không đọc được"
                    };
                }

                // 3. Validation logic cũ
                if (finalElectricValue < dbOldElectricValue)
                {
                    return new DebtCalculationResult
                    {
                        RoomName = formData.RoomName,
                        OldElectricValue = dbOldElectricValue,
                        CurrentElectricValue = finalElectricValue,
                        IsProcessed = false,
                        ErrorMessage = $"Số mới ({finalElectricValue}) < Số cũ DB ({dbOldElectricValue})"
                    };
                }

                // 4. Tính toán
                decimal electricityUsage = finalElectricValue - dbOldElectricValue;
                decimal electricityCost = electricityUsage * ELECTRICITY_RATE;
                decimal totalDebt = electricityCost + WATER_RATE;

                return new DebtCalculationResult
                {
                    RoomName = formData.RoomName,
                    Email = formData.Email,
                    Timestamp = formData.Timestamp,
                    OldElectricValue = dbOldElectricValue,
                    CurrentElectricValue = finalElectricValue,
                    ElectricityUsage = electricityUsage,
                    ElectricityCost = electricityCost,
                    WaterCost = WATER_RATE,
                    TotalDebt = totalDebt,
                    OcrStatus = ocrStatus,
                    ElectricImageUrl = formData.ElectricImageUrl,
                    IsProcessed = true,
                    ManualValue = manualValue,
                    OcrValue = ocrValue,
                    IsDiscrepancy = isDiscrepancy,
                    WarningNote = warningNote,
                    OriginalImagePath = originalImagePath
                };
            }
            catch (Exception ex)
            {
                return new DebtCalculationResult
                {
                    RoomName = formData.RoomName,
                    IsProcessed = false,
                    ErrorMessage = $"Lỗi xử lý: {ex.Message}"
                };
            }
        }

        // CÁC PHƯƠNG THỨC MỚI CHO XỬ LÝ SỰ CỐ

        /// <summary>
        /// Lấy thông tin chi tiết sự cố cho phòng cụ thể
        /// </summary>
        public async Task<DebtDiscrepancyInfo?> GetDiscrepancyInfoAsync(string roomName)
        {
            // Kiểm tra trong cache trước
            if (_discrepancyCache.TryGetValue(roomName, out var cachedInfo))
            {
                return cachedInfo;
            }

            // Nếu không có trong cache, thử tái tạo từ dữ liệu hiện có
            try
            {
                var formDataList = await _googleFormService.GetFormDataAsync();
                var formData = formDataList.FirstOrDefault(f => f.RoomName == roomName);

                if (formData != null && !string.IsNullOrEmpty(formData.ElectricImageUrl))
                {
                    var originalImagePath = await DownloadImageAsync(formData.ElectricImageUrl);
                    if (!string.IsNullOrEmpty(originalImagePath))
                    {
                        var meterReadingResult = await ProcessElectricImageAsync(originalImagePath);

                        return new DebtDiscrepancyInfo
                        {
                            RoomName = roomName,
                            ManualValue = formData.CurrentElectricValue,
                            OcrValue = meterReadingResult.Value,
                            // FIX: Đã xóa dòng gán 'Difference' vì nó là read-only
                            WarningNote = $"Chênh lệch: {Math.Abs(formData.CurrentElectricValue - meterReadingResult.Value)}",
                            MeterReadingResult = meterReadingResult,
                            OriginalImagePath = originalImagePath,
                            DetectionTime = DateTime.Now
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi khi tái tạo thông tin sự cố: {ex.Message}");
            }

            return null;
        }

        /// <summary>
        /// Giải quyết sự cố với giá trị đã xác nhận
        /// </summary>
        public async Task<ProcessingResult> ResolveDiscrepancyAsync(string roomName, decimal confirmedValue)
        {
            try
            {
                // 1. Tìm contractId
                var contractId = await GetContractIdByRoomNameAsync(roomName);
                if (!contractId.HasValue)
                {
                    return new ProcessingResult
                    {
                        IsSuccess = false,
                        ErrorMessage = $"Không tìm thấy hợp đồng cho phòng {roomName}"
                    };
                }

                // 2. Lấy chỉ số cũ
                var lastPayment = await GetLastPaymentByContractIdAsync(contractId.Value);
                decimal oldElectricValue = lastPayment?.ChiSoDienMoi ?? 0;

                // 3. Validation
                if (confirmedValue < oldElectricValue)
                {
                    return new ProcessingResult
                    {
                        IsSuccess = false,
                        ErrorMessage = $"Số mới ({confirmedValue}) không thể nhỏ hơn số cũ ({oldElectricValue})"
                    };
                }

                // 4. Tính toán
                decimal electricityUsage = confirmedValue - oldElectricValue;
                decimal electricityCost = electricityUsage * ELECTRICITY_RATE;

                // 5. Tạo payment record mới
                var currentMonth = DateTime.Now.ToString("MM/yyyy");
                var existing = await GetPaymentByMonthAsync(contractId.Value, currentMonth);
                if (existing != null)
                {
                    return new ProcessingResult
                    {
                        IsSuccess = false,
                        ErrorMessage = $"Đã tồn tại thanh toán cho tháng {currentMonth}"
                    };
                }

                var payment = new Payment
                {
                    MaHopDong = contractId.Value,
                    ThangNam = currentMonth,
                    TienThue = await GetRentPriceAsync(contractId.Value),
                    ChiSoDienCu = oldElectricValue,
                    ChiSoDienMoi = confirmedValue,
                    SoDien = electricityUsage,
                    DonGiaDien = ELECTRICITY_RATE,
                    TienDien = electricityCost,
                    SoNuoc = 1,
                    DonGiaNuoc = WATER_RATE,
                    TienNuoc = WATER_RATE,
                    TrangThaiThanhToan = "Chưa trả",
                    NgayTao = DateTime.Now,
                    GhiChu = $"Đã xác nhận sau kiểm tra sự cố. Giá trị: {confirmedValue}"
                };
                payment.TongTien = (payment.TienThue ?? 0) + (payment.TienDien ?? 0) + (payment.TienNuoc ?? 0);

                // 6. Lưu vào database
                var success = await _paymentRepository.CreateAsync(payment);
                if (success)
                {
                    // Xóa khỏi cache sau khi giải quyết thành công
                    _discrepancyCache.Remove(roomName);
                    return new ProcessingResult { IsSuccess = true };
                }
                else
                {
                    return new ProcessingResult
                    {
                        IsSuccess = false,
                        ErrorMessage = "Lỗi khi lưu vào database"
                    };
                }
            }
            catch (Exception ex)
            {
                return new ProcessingResult
                {
                    IsSuccess = false,
                    ErrorMessage = $"Lỗi khi giải quyết sự cố: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Lấy danh sách tất cả các phòng có sự cố
        /// </summary>
        public List<string> GetRoomsWithDiscrepancies()
        {
            return _discrepancyCache.Keys.ToList();
        }

        // CÁC PHƯƠNG THỨC HIỆN CÓ GIỮ NGUYÊN
        private async Task<MeterReadingResult> ProcessElectricImageAsync(string imagePath)
        {
            try
            {
                // FIX: Sửa 'Electric' thành 'Electricity'
                return await _ocrService.AnalyzeImageAsync(imagePath, MeterType.Electricity);
            }
            catch (Exception ex)
            {
                return new MeterReadingResult { ErrorMessage = ex.Message };
            }
        }

        private static async Task<string> DownloadImageAsync(string imageUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(imageUrl)) return null;
                var tempPath = Path.GetTempPath();
                var fileName = $"meter_{Guid.NewGuid()}.jpg";
                var localPath = Path.Combine(tempPath, fileName);
                using var webClient = new WebClient();
                await webClient.DownloadFileTaskAsync(new Uri(imageUrl), localPath);
                return File.Exists(localPath) ? localPath : null;
            }
            catch { return null; }
        }

        public async Task<int> CreatePaymentRecordsFromDebtsAsync(List<DebtCalculationResult> debts)
        {
            int createdCount = 0;
            foreach (var debt in debts.Where(d => d.IsProcessed && !d.IsDiscrepancy)) // Chỉ tạo cho các bản ghi không có sự cố
            {
                try
                {
                    var contractId = await GetContractIdByRoomNameAsync(debt.RoomName);
                    if (contractId.HasValue)
                    {
                        var currentMonth = DateTime.Now.ToString("MM/yyyy");
                        var existing = await GetPaymentByMonthAsync(contractId.Value, currentMonth);
                        if (existing != null) continue;

                        var payment = new Payment
                        {
                            MaHopDong = contractId.Value,
                            ThangNam = currentMonth,
                            TienThue = await GetRentPriceAsync(contractId.Value),
                            ChiSoDienCu = debt.OldElectricValue,
                            ChiSoDienMoi = debt.CurrentElectricValue,
                            SoDien = debt.ElectricityUsage,
                            DonGiaDien = ELECTRICITY_RATE,
                            TienDien = debt.ElectricityCost,
                            SoNuoc = 1,
                            DonGiaNuoc = WATER_RATE,
                            TienNuoc = debt.WaterCost,
                            TrangThaiThanhToan = "Chưa trả",
                            NgayTao = DateTime.Now,
                            GhiChu = $"Auto Google Form. {debt.OcrStatus}"
                        };
                        payment.TongTien = (payment.TienThue ?? 0) + (payment.TienDien ?? 0) + (payment.TienNuoc ?? 0);

                        var success = await _paymentRepository.CreateAsync(payment);
                        if (success) createdCount++;
                    }
                }
                catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"Lỗi lưu DB: {ex.Message}"); }
            }
            return createdCount;
        }

        private async Task<int?> GetContractIdByRoomNameAsync(string roomName)
        {
            var rooms = await _roomRepository.GetAllAsync();
            var room = rooms.FirstOrDefault(r => r.TenPhong.Trim().Equals(roomName.Trim(), StringComparison.OrdinalIgnoreCase));
            if (room == null) return null;
            var contracts = await _contractRepository.GetActiveContractsAsync();
            return contracts.FirstOrDefault(c => c.MaPhong == room.MaPhong)?.MaHopDong;
        }

        private async Task<decimal> GetRentPriceAsync(int contractId)
        {
            var contract = await _contractRepository.GetByIdAsync(contractId);
            return contract?.GiaThue ?? 0;
        }

        private async Task<Payment?> GetLastPaymentByContractIdAsync(int contractId)
        {
            var payments = await _paymentRepository.GetAllAsync();
            return payments.Where(p => p.MaHopDong == contractId).OrderByDescending(p => p.MaThanhToan).FirstOrDefault();
        }

        private async Task<Payment?> GetPaymentByMonthAsync(int contractId, string monthYear)
        {
            var payments = await _paymentRepository.GetAllAsync();
            return payments.FirstOrDefault(p => p.MaHopDong == contractId && p.ThangNam == monthYear);
        }
    }

    // CÁC LỚP DTO BỔ SUNG

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
        public decimal ManualValue { get; set; }
        public decimal OcrValue { get; set; }
        public bool IsDiscrepancy { get; set; }
        public string WarningNote { get; set; } = string.Empty;
        public string OriginalImagePath { get; set; } = string.Empty;
    }

    public class DebtDiscrepancyInfo
    {
        public string RoomName { get; set; } = string.Empty;
        public decimal ManualValue { get; set; }
        public decimal OcrValue { get; set; }
        public decimal ConfirmedValue { get; set; }
        public decimal Difference => Math.Abs(ManualValue - OcrValue);
        public string WarningNote { get; set; } = string.Empty;
        public MeterReadingResult MeterReadingResult { get; set; } = new();
        public string? OriginalImagePath { get; set; }
        public DateTime DetectionTime { get; set; } = DateTime.Now;
    }

    public class ProcessingResult
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }
}