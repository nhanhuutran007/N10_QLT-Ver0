using System;
using System.Collections.Generic;
using System.IO;
// THÊM 3 DÒNG NÀY:
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using System.Diagnostics; // Để ghi log [DEBUG]
using System.Linq;
using Google.Apis.Drive.v3;
using Google.Apis.Download;
using System.Threading.Tasks;
using QLKDPhongTro.BusinessLayer.DTOs;
using QLKDPhongTro.BusinessLayer.Services;
using QLKDPhongTro.DataLayer.Models;
using QLKDPhongTro.DataLayer.Repositories;
using QLKDPhongTro.Presentation.Services;
// Thêm thư viện HttpClient (hiện đại hơn WebClient)
using System.Net.Http;


namespace QLKDPhongTro.Presentation.Services
{
    public class DebtProcessingService
    {
        private readonly GoogleFormService _googleFormService;
        private readonly OcrService _ocrService;
        private readonly PaymentRepository _paymentRepository;
        private readonly ContractRepository _contractRepository;
        private readonly RentedRoomRepository _roomRepository;
        private readonly TenantRepository _tenantRepository;

        // === FIX: Khởi tạo HttpClient một lần để tái sử dụng ===
        // Tốt hơn cho hiệu năng so với việc tạo mới WebClient mỗi lần
        private readonly DriveService _driveService;
        private const string DEBUG_PATH = @"C:\TempOcrDebug";


        private const decimal ELECTRICITY_RATE = 3500;
        private const decimal WATER_RATE = 100000;

        // === FIX: THÊM DON_GIA_NUOC VÀO ĐÂY ===
        private const decimal DON_GIA_NUOC = 100000;     // ✅ Đã thêm (bằng với WATER_RATE)
        private const decimal DON_GIA_INTERNET = 100000; // Giá trị giả định
        private const decimal DON_GIA_VE_SINH = 60000;   // Giá trị giả định

        // Dictionary để lưu trữ thông tin sự cố tạm thời
        private readonly Dictionary<string, DebtDiscrepancyInfo> _discrepancyCache = new();

        public DebtProcessingService()
        {
            _googleFormService = new GoogleFormService();
            _ocrService = new OcrService();
            _paymentRepository = new PaymentRepository();
            _contractRepository = new ContractRepository();
            _roomRepository = new RentedRoomRepository();
            _tenantRepository = new TenantRepository();

            // Thêm User-Agent cho HttpClient, một số dịch vụ (như Google) cần
            Directory.CreateDirectory(DEBUG_PATH);

            _driveService = _googleFormService.DriveService;
        }

        private static DebtFeeSettings EnsureSettings(DebtFeeSettings? settings)
            => settings ?? new DebtFeeSettings();

        /// <summary>
        /// Tính tiền giữ xe: Free 1 xe đầu, các xe tiếp theo tính 100.000 VNĐ/xe
        /// Lấy tất cả xe của tất cả người thuê trong phòng
        /// </summary>
        private async Task<decimal> CalculateParkingFeeAsync(int contractId, DebtFeeSettings settings)
        {
            var contract = await _contractRepository.GetByIdAsync(contractId);
            if (contract == null)
            {
                return 0;
            }

            // Lấy tất cả người thuê trong phòng (bao gồm cả người đứng tên và người ở cùng)
            var roomTenants = await _tenantRepository.GetTenantsByRoomIdAsync(contract.MaPhong);
            if (roomTenants == null || roomTenants.Count == 0)
            {
                return 0;
            }

            // Lấy assets của tất cả người thuê trong phòng
            var allAssets = new List<DataLayer.Models.TenantAsset>();
            foreach (var tenant in roomTenants)
            {
                var tenantAssets = await _tenantRepository.GetAssetsAsync(tenant.MaNguoiThue);
                if (tenantAssets != null && tenantAssets.Count > 0)
                {
                    allAssets.AddRange(tenantAssets);
                }
            }

            if (allAssets.Count == 0)
            {
                return 0;
            }

            // Đếm tổng số xe của tất cả người thuê trong phòng
            var vehicleCount = allAssets.Count(a =>
                a.LoaiTaiSan?.IndexOf("xe", StringComparison.OrdinalIgnoreCase) >= 0);

            // Free 1 xe đầu, các xe tiếp theo tính phí
            var extraVehicleCount = Math.Max(0, vehicleCount - settings.FreeVehicleCount);
            var parkingFee = extraVehicleCount * settings.AdditionalVehicleFee;

            return parkingFee;
        }

        /// <summary>
        /// Tính các chi phí khác (không bao gồm tiền giữ xe)
        /// Lấy tất cả assets của tất cả người thuê trong phòng
        /// </summary>
        private async Task<decimal> CalculateAdditionalCostsAsync(int contractId, DebtFeeSettings settings)
        {
            var contract = await _contractRepository.GetByIdAsync(contractId);
            if (contract == null)
            {
                return 0;
            }

            // Lấy tất cả người thuê trong phòng (bao gồm cả người đứng tên và người ở cùng)
            var roomTenants = await _tenantRepository.GetTenantsByRoomIdAsync(contract.MaPhong);
            if (roomTenants == null || roomTenants.Count == 0)
            {
                return 0;
            }

            // Lấy assets của tất cả người thuê trong phòng
            var allAssets = new List<DataLayer.Models.TenantAsset>();
            foreach (var tenant in roomTenants)
            {
                var tenantAssets = await _tenantRepository.GetAssetsAsync(tenant.MaNguoiThue);
                if (tenantAssets != null && tenantAssets.Count > 0)
                {
                    allAssets.AddRange(tenantAssets);
                }
            }

            if (allAssets.Count == 0)
            {
                return 0;
            }

            // Chỉ tính phí phụ thu từ tài sản (không tính xe vì đã tính riêng trong tiền giữ xe)
            var assetSurcharge = allAssets
                .Where(a => a.LoaiTaiSan?.IndexOf("xe", StringComparison.OrdinalIgnoreCase) < 0) // Loại bỏ xe
                .Sum(a => a.PhiPhuThu);

            return assetSurcharge;
        }

        // Phương thức hiện có giữ nguyên
        public async Task<List<DebtCalculationResult>> ProcessDebtsAsync(DebtFeeSettings? settings = null)
        {
            settings = EnsureSettings(settings);
            var results = new List<DebtCalculationResult>();
            _discrepancyCache.Clear(); // Clear cache mỗi lần xử lý mới

            try
            {
                var formDataList = await _googleFormService.GetFormDataAsync();

                foreach (var formData in formDataList)
                {
                    var result = await ProcessSingleDebtAsync(formData, settings);
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
        /// <summary>
        /// Thay đổi kích thước ảnh bằng ImageSharp (hiện đại, tránh lỗi OutOfMemory).
        /// </summary>
        private string ResizeImage(string originalImagePath, int maxWidth = 1024)
        {
            try
            {
                var tempPath = Path.GetTempPath();
                var fileName = $"meter_resized_{Guid.NewGuid()}.jpg";
                var resizedPath = Path.Combine(tempPath, fileName);

                // 1. Tải ảnh bằng ImageSharp
                using (Image image = Image.Load(originalImagePath))
                {
                    // Nếu ảnh đã nhỏ rồi thì thôi
                    if (image.Width <= maxWidth)
                    {
                        // Chỉ cần lưu lại với bộ mã hóa chuẩn
                        image.Save(resizedPath, new JpegEncoder());
                        Debug.WriteLine($"[DEBUG] Ảnh gốc đã nhỏ, chỉ lưu lại: {resizedPath}");
                        return resizedPath;
                    }

                    // 2. Tính toán tỷ lệ
                    var newHeight = (int)Math.Round((double)image.Height * maxWidth / image.Width);

                    // 3. Resize ảnh
                    image.Mutate(x => x
                         .Resize(new ResizeOptions
                         {
                             Size = new Size(maxWidth, newHeight),
                             Mode = ResizeMode.Stretch // Đã tính toán tỷ lệ, chỉ cần kéo
                         }));

                    // 4. Lưu ảnh đã resize
                    image.Save(resizedPath, new JpegEncoder());
                }

                Debug.WriteLine($"[DEBUG] Đã resize ảnh (ImageSharp) sang: {resizedPath}");
                return resizedPath;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[DEBUG] LỖI khi resize (ImageSharp) {originalImagePath}: {ex.Message}");
                // Nếu resize lỗi, trả về ảnh gốc và chấp nhận rủi ro
                return originalImagePath;
            }
        }
        private async Task<DebtCalculationResult> ProcessSingleDebtAsync(DebtFormData formData, DebtFeeSettings settings)
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

                var normalizedSettings = EnsureSettings(settings);
                var waterCost = normalizedSettings.WaterFee;
                var internetCost = normalizedSettings.InternetFee;
                var sanitationCost = normalizedSettings.SanitationFee;
                var parkingFee = await CalculateParkingFeeAsync(contractId.Value, normalizedSettings);
                var otherCost = await CalculateAdditionalCostsAsync(contractId.Value, normalizedSettings);

                // 2. Xử lý so sánh: Form nhập tay vs OCR
                decimal finalElectricValue = 0;
                decimal manualValue = formData.CurrentElectricValue;
                // Khởi tạo ocrValue = -1 (để phân biệt với 0 - lỗi)
                decimal ocrValue = -1;
                string ocrStatus = "";
                bool isDiscrepancy = false;
                string warningNote = "";
                string originalImagePath = null;
                MeterReadingResult meterReadingResult = null;

                // Luôn chạy OCR để đối chiếu (nếu có ảnh)
                if (!string.IsNullOrEmpty(formData.ElectricImageUrl))
                {
                    // Tải ảnh về local (Đã sửa hàm này)
                    originalImagePath = await DownloadImageAsync(formData.ElectricImageUrl);

                    if (!string.IsNullOrEmpty(originalImagePath))
                    {
                        // === FIX QUAN TRỌNG: RESIZE ẢNH TRƯỚC KHI OCR ===
                        //string imageToProcess = ResizeImage(originalImagePath);
                        string imageToProcess = originalImagePath;
                        // ===============================================

                        // Truyền ảnh đã resize vào OCR
                        meterReadingResult = await ProcessElectricImageAsync(imageToProcess);

                        // (Tùy chọn) Xóa file ảnh đã resize
                        // File.Delete(imageToProcess);

                        if (meterReadingResult.IsValid)
                        {
                            ocrValue = meterReadingResult.Value;
                            ocrStatus = "OCR OK";
                        }
                        else
                        {
                            ocrValue = 0; // Gán 0 nếu OCR thất bại
                            ocrStatus = $"OCR Fail: {meterReadingResult.ErrorMessage}";
                        }
                    }
                    else
                    {
                        ocrStatus = "Tải ảnh thất bại";
                    }
                }

                // Logic so sánh và phát hiện sự cố
                // ocrValue > 0 (đảm bảo OCR đọc được số dương)
                if (manualValue > 0 && ocrValue > 0)
                {
                    decimal difference = Math.Abs(manualValue - ocrValue);
                    if (difference >= 1) // Ngưỡng chênh lệch
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
                decimal totalDebt = electricityCost + waterCost + internetCost + sanitationCost + parkingFee + otherCost;

                return new DebtCalculationResult
                {
                    RoomName = formData.RoomName,
                    Email = formData.Email,
                    Timestamp = ParseTimestamp(formData.Timestamp),
                    OldElectricValue = dbOldElectricValue,
                    CurrentElectricValue = finalElectricValue,
                    ElectricityUsage = electricityUsage,
                    ElectricityCost = electricityCost,
                    WaterCost = waterCost,
                    InternetCost = internetCost,
                    SanitationCost = sanitationCost,
                    OtherCost = otherCost, // Chỉ tính các chi phí khác (không bao gồm tiền giữ xe)
                    TotalDebt = totalDebt, // TotalDebt đã bao gồm parkingFee
                    OcrStatus = ocrStatus,
                    ElectricImageUrl = formData.ElectricImageUrl,
                    IsProcessed = true,
                    ManualValue = manualValue,
                    // Gán ocrValue (sẽ là -1, 0 hoặc giá trị detect được)
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
        public async Task<ProcessingResult> ResolveDiscrepancyAsync(string roomName, decimal confirmedValue, DebtFeeSettings? settings = null)
        {
            try
            {
                var normalizedSettings = EnsureSettings(settings);
                // 1. Tìm contractId
                var contractId = await GetContractIdByRoomNameAsync(roomName);
                if (!contractId.HasValue)
                {
                    return new ProcessingResult { IsSuccess = false, ErrorMessage = $"Không tìm thấy hợp đồng cho phòng {roomName}" };
                }

                // 2. Lấy chỉ số cũ
                var lastPayment = await GetLastPaymentByContractIdAsync(contractId.Value);
                decimal oldElectricValue = lastPayment?.ChiSoDienMoi ?? 0;

                // 3. Validation
                if (confirmedValue < oldElectricValue)
                {
                    return new ProcessingResult { IsSuccess = false, ErrorMessage = $"Số mới ({confirmedValue}) không thể nhỏ hơn số cũ ({oldElectricValue})" };
                }

                // 4. Tính toán
                decimal electricityUsage = confirmedValue - oldElectricValue;
                decimal electricityCost = electricityUsage * ELECTRICITY_RATE;
                var parkingFee = await CalculateParkingFeeAsync(contractId.Value, normalizedSettings);
                var additionalCosts = await CalculateAdditionalCostsAsync(contractId.Value, normalizedSettings);

                // 5. Lấy tháng từ Google Form data (nếu có) hoặc dùng tháng hiện tại
                string monthYear;
                try
                {
                    var formDataList = await _googleFormService.GetFormDataAsync();
                    var formData = formDataList.FirstOrDefault(f => 
                        string.Equals(f.RoomName?.Trim().Replace(".", "").Replace(" ", ""), 
                                     roomName?.Trim().Replace(".", "").Replace(" ", ""), 
                                     StringComparison.OrdinalIgnoreCase));
                    if (formData != null && !string.IsNullOrEmpty(formData.Timestamp))
                    {
                        var timestamp = ParseTimestamp(formData.Timestamp);
                        monthYear = timestamp.ToString("MM/yyyy");
                    }
                    else
                    {
                        monthYear = DateTime.Now.ToString("MM/yyyy");
                    }
                }
                catch
                {
                    monthYear = DateTime.Now.ToString("MM/yyyy");
                }

                // 6. Tạo payment record mới
                var existing = await GetPaymentByMonthAsync(contractId.Value, monthYear);
                if (existing != null)
                {
                    return new ProcessingResult { IsSuccess = false, ErrorMessage = $"Đã tồn tại thanh toán cho tháng {monthYear}" };
                }

                var payment = new Payment
                {
                    MaHopDong = contractId.Value,
                    ThangNam = monthYear,
                    TienThue = await GetRentPriceAsync(contractId.Value),
                    ChiSoDienCu = oldElectricValue,
                    ChiSoDienMoi = confirmedValue,
                    SoDien = electricityUsage,
                    DonGiaDien = ELECTRICITY_RATE,
                    TienDien = electricityCost,
                    SoNuoc = 1,
                    DonGiaNuoc = normalizedSettings.WaterFee,
                    TienNuoc = normalizedSettings.WaterFee,
                    // Gán giá trị cố định
                    TienInternet = normalizedSettings.InternetFee,
                    TienVeSinh = normalizedSettings.SanitationFee,
                    TienGiuXe = parkingFee, // Tính dựa trên số lượng xe (free 1 xe, các xe sau tính 100.000/xe)
                    ChiPhiKhac = additionalCosts,
                    TrangThaiThanhToan = "Chưa trả",
                    GhiChu = $"Đã xác nhận sau kiểm tra sự cố. Giá trị: {confirmedValue}"
                };

                // === FIX LỖI CONVERT TYPE TẠI ĐÂY ===
                // Sử dụng (?? 0) cho TẤT CẢ các trường nullable
                payment.TongTien = (payment.TienThue ?? 0)
                                 + (payment.TienDien ?? 0)
                                 + (payment.TienNuoc ?? 0)
                                 + (payment.TienInternet ?? 0) // Thêm ?? 0
                                 + (payment.TienVeSinh ?? 0)    // Thêm ?? 0
                                 + (payment.TienGiuXe ?? 0)
                                 + (payment.ChiPhiKhac ?? 0);    // Thêm ?? 0

                // 6. Lưu vào database
                var success = await _paymentRepository.CreateAsync(payment);
                if (success)
                {
                    _discrepancyCache.Remove(roomName);
                    return new ProcessingResult { IsSuccess = true };
                }
                else
                {
                    return new ProcessingResult { IsSuccess = false, ErrorMessage = "Lỗi khi lưu vào database" };
                }
            }
            catch (Exception ex)
            {
                return new ProcessingResult { IsSuccess = false, ErrorMessage = $"Lỗi khi giải quyết sự cố: {ex.Message}" };
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
                // FIX: Sửa 'Electric' thành 'Electricity' (Giữ nguyên fix của bạn)
                return await _ocrService.AnalyzeImageAsync(imagePath, MeterType.Electricity);
            }
            catch (Exception ex)
            {
                return new MeterReadingResult { ErrorMessage = ex.Message };
            }
        }

        // === CRITICAL FIX: Sửa hàm tải ảnh Google Drive ===
        // === FIX 3: THAY THẾ TOÀN BỘ HÀM NÀY ===
        // === FIX: THAY THẾ TOÀN BỘ HÀM NÀY ===
        private async Task<string> DownloadImageAsync(string imageUrl)
        {
            Debug.WriteLine($"[DEBUG] Bắt đầu tải API: {imageUrl}");
            try
            {
                if (string.IsNullOrEmpty(imageUrl))
                {
                    Debug.WriteLine("[DEBUG] LỖI: ImageUrl rỗng.");
                    return null;
                }

                // 1. Parse File ID từ URL
                string fileId = null;
                if (imageUrl.Contains("id="))
                {
                    fileId = imageUrl.Split(new[] { "id=" }, StringSplitOptions.None)[1];
                    fileId = fileId.Split('&')[0];
                }
                else if (imageUrl.Contains("/d/"))
                {
                    var parts = imageUrl.Split(new[] { "/d/" }, StringSplitOptions.None);
                    if (parts.Length > 1)
                    {
                        fileId = parts[1].Split('/')[0];
                    }
                }

                if (string.IsNullOrEmpty(fileId))
                {
                    Debug.WriteLine($"[DEBUG] LỖI: Không thể parse File ID từ {imageUrl}");
                    return null;
                }

                Debug.WriteLine($"[DEBUG] File ID: {fileId}");

                // 2. Chuẩn bị đường dẫn lưu file
                var tempPath = Path.GetTempPath(); // Dùng thư mục temp chuẩn
                var fileName = $"meter_api_{Guid.NewGuid()}.jpg";
                var localPath = Path.Combine(tempPath, fileName);

                // 3. Dùng Drive API để tải
                var request = _driveService.Files.Get(fileId);
                using (var fileStream = new FileStream(localPath, FileMode.Create, FileAccess.Write))
                {
                    // Download file
                    var result = await request.DownloadAsync(fileStream);

                    if (result.Status == DownloadStatus.Failed)
                    {
                        Debug.WriteLine($"[DEBUG] LỖI API Drive: {result.Exception.Message}");
                        return null;
                    }
                }

                var fileInfo = new FileInfo(localPath);
                Debug.WriteLine($"[DEBUG] Tải API thành công. Kích thước: {fileInfo.Length} bytes.");
                return localPath;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[DEBUG] LỖI KHI TẢI API {imageUrl}: {ex.Message}");
                return null;
            }
        }

        public async Task<int> CreatePaymentRecordsFromDebtsAsync(List<DebtCalculationResult> debts, DebtFeeSettings? settings = null)
        {
            var normalizedSettings = EnsureSettings(settings);
            int createdCount = 0;
            foreach (var debt in debts.Where(d => d.IsProcessed && !d.IsDiscrepancy))
            {
                try
                {
                    var contractId = await GetContractIdByRoomNameAsync(debt.RoomName);
                    if (contractId.HasValue)
                    {
                        // Sử dụng tháng từ timestamp của Google Form thay vì tháng hiện tại
                        var monthYear = debt.Timestamp.ToString("MM/yyyy");
                        var existing = await GetPaymentByMonthAsync(contractId.Value, monthYear);
                        if (existing != null) continue;

                        // Tính tiền giữ xe dựa trên số lượng xe (free 1 xe, các xe sau tính 100.000/xe)
                        var parkingFee = await CalculateParkingFeeAsync(contractId.Value, normalizedSettings);

                        var payment = new Payment
                        {
                            MaHopDong = contractId.Value,
                            ThangNam = monthYear,
                            TienThue = await GetRentPriceAsync(contractId.Value),
                            ChiSoDienCu = debt.OldElectricValue,
                            ChiSoDienMoi = debt.CurrentElectricValue,
                            SoDien = debt.ElectricityUsage,
                            DonGiaDien = ELECTRICITY_RATE,
                            TienDien = debt.ElectricityCost,
                            SoNuoc = 1,
                            DonGiaNuoc = normalizedSettings.WaterFee,
                            TienNuoc = debt.WaterCost > 0 ? debt.WaterCost : normalizedSettings.WaterFee,
                            // Gán giá trị cố định
                            TienInternet = debt.InternetCost > 0 ? debt.InternetCost : normalizedSettings.InternetFee,
                            TienVeSinh = debt.SanitationCost > 0 ? debt.SanitationCost : normalizedSettings.SanitationFee,
                            TienGiuXe = parkingFee, // Tính dựa trên số lượng xe (free 1 xe, các xe sau tính 100.000/xe)
                            ChiPhiKhac = debt.OtherCost, // Các chi phí khác (không bao gồm tiền giữ xe)
                            TrangThaiThanhToan = "Chưa trả",
                            GhiChu = $"Auto Google Form. {debt.OcrStatus}"
                        };

                        // === FIX LỖI CONVERT TYPE TẠI ĐÂY ===
                        // Sử dụng (?? 0) cho TẤT CẢ các trường nullable
                        payment.TongTien = (payment.TienThue ?? 0)
                                         + (payment.TienDien ?? 0)
                                         + (payment.TienNuoc ?? 0)
                                         + (payment.TienInternet ?? 0) // Thêm ?? 0
                                         + (payment.TienVeSinh ?? 0)    // Thêm ?? 0
                                         + (payment.TienGiuXe ?? 0)
                                         + (payment.ChiPhiKhac ?? 0);    // Thêm ?? 0

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
            
            // Chuẩn hóa tên phòng: loại bỏ dấu chấm, khoảng trắng và chuyển thành chữ hoa
            string NormalizeRoomName(string name)
            {
                if (string.IsNullOrWhiteSpace(name)) return string.Empty;
                return name.Trim()
                    .Replace(".", "")
                    .Replace(" ", "")
                    .Replace("-", "")
                    .ToUpperInvariant();
            }

            var normalizedRoomName = NormalizeRoomName(roomName);
            var room = rooms.FirstOrDefault(r => NormalizeRoomName(r.TenPhong) == normalizedRoomName);
            
            if (room == null) return null;
            
            var contracts = await _contractRepository.GetActiveContractsAsync();
            return contracts.FirstOrDefault(c => c.MaPhong == room.MaPhong)?.MaHopDong;
        }

        private async Task<decimal> GetRentPriceAsync(int contractId)
        {
            var contract = await _contractRepository.GetByIdAsync(contractId);

            if (contract == null) return 0;

            // FIX 2: Bỏ '?? 0'. Nếu 'GiaThue > 0', chỉ cần trả về nó.
            if (contract.GiaThue > 0) return contract.GiaThue;

            // FIX 1: Bỏ '?? 0'. Chỉ cần truyền 'contract.MaPhong'.
            var room = await _roomRepository.GetByIdAsync(contract.MaPhong);

            // Dòng này đã đúng, vì 'room' có thể null ('?')
            return room?.GiaCoBan ?? 0;
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

        /// <summary>
        /// Parse timestamp từ Google Form với format dd/MM/yyyy (ví dụ: 06/12/2025 = ngày 6 tháng 12 năm 2025)
        /// </summary>
        private DateTime ParseTimestamp(string timestamp)
        {
            if (string.IsNullOrWhiteSpace(timestamp))
            {
                return DateTime.Now;
            }

            // Thử các format phổ biến của Google Form
            string[] formats = {
                "dd/MM/yyyy",           // 06/12/2025
                "dd/MM/yyyy HH:mm:ss",  // 06/12/2025 10:30:00
                "d/M/yyyy",             // 6/12/2025
                "d/M/yyyy HH:mm:ss",     // 6/12/2025 10:30:00
                "MM/dd/yyyy",            // Fallback cho format Mỹ
                "yyyy-MM-dd",            // ISO format
                "yyyy-MM-dd HH:mm:ss"    // ISO format với time
            };

            // Thử parse với các format trên
            foreach (var format in formats)
            {
                if (DateTime.TryParseExact(timestamp.Trim(), format, 
                    System.Globalization.CultureInfo.InvariantCulture, 
                    System.Globalization.DateTimeStyles.None, out DateTime result))
                {
                    return result;
                }
            }

            // Nếu không parse được, thử parse với culture mặc định
            if (DateTime.TryParse(timestamp, out DateTime defaultResult))
            {
                return defaultResult;
            }

            // Fallback: trả về thời gian hiện tại
            return DateTime.Now;
        }
    }

}