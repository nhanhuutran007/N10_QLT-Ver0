using QLKDPhongTro.DataLayer.Models;
using QLKDPhongTro.DataLayer.Repositories;
using QLKDPhongTro.BusinessLayer.Services;
using QLKDPhongTro.Presentation.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace QLKDPhongTro.BusinessLayer.Controllers
{
    public class MaintenanceController
    {
        private readonly IMaintenanceRepository _repo;
        private readonly GoogleSheetsService _googleSheetsService;
        private readonly IRentedRoomRepository? _roomRepository;
        private readonly ITenantRepository? _tenantRepository;

        public MaintenanceController(IMaintenanceRepository repo, GoogleSheetsService? googleSheetsService = null, IRentedRoomRepository? roomRepository = null, ITenantRepository? tenantRepository = null)
        {
            _repo = repo;
            _googleSheetsService = googleSheetsService ?? new GoogleSheetsService(new System.Net.Http.HttpClient());
            _roomRepository = roomRepository;
            _tenantRepository = tenantRepository;
        }

        public Task<List<MaintenanceIncident>> GetAllAsync() => _repo.GetAllAsync();
        public async Task<List<MaintenanceIncident>> GetAllForCurrentUserAsync()
        {
            var current = AuthController.CurrentUser;
            if (current != null && current.MaNha > 0)
            {
                return await _repo.GetAllByMaNhaAsync(current.MaNha);
            }
            return await _repo.GetAllAsync();
        }
        public Task<List<MaintenanceIncident>> GetByRoomAsync(int maPhong) => _repo.GetByRoomAsync(maPhong);
        public Task<MaintenanceIncident?> GetByIdAsync(int id) => _repo.GetByIdAsync(id);
        public Task AddAsync(MaintenanceIncident incident) => _repo.AddAsync(incident);
        public Task UpdateAsync(MaintenanceIncident incident) => _repo.UpdateAsync(incident);
        public Task DeleteAsync(int id) => _repo.DeleteAsync(id);

        /// <summary>
        /// Đồng bộ dữ liệu từ Google Sheets vào database
        /// Chỉ thêm các bảo trì mới chưa tồn tại trong database và chưa bị xóa
        /// </summary>
        public async Task<int> SyncFromGoogleSheetsAsync()
        {
            try
            {
                // Đọc dữ liệu từ Google Sheets
                var sheetData = await _googleSheetsService.ReadMaintenanceDataAsync();
                
                if (sheetData == null || sheetData.Count == 0)
                    return 0;

                // Lấy tất cả dữ liệu hiện có trong database để so sánh
                var existingData = await _repo.GetAllAsync();

                int addedCount = 0;
                var skippedRooms = new System.Collections.Generic.List<int>();

                // Duyệt qua từng dòng từ Google Sheets
                foreach (var row in sheetData)
                {
                    // Kiểm tra xem bảo trì này đã tồn tại trong database chưa
                    var existsInDb = existingData.Any(existing =>
                        existing.MaPhong == row.MaPhong &&
                        existing.MoTaSuCo.Trim().Equals(row.MoTaSuCo.Trim(), System.StringComparison.OrdinalIgnoreCase) &&
                        existing.NgayBaoCao.Date == row.NgayBaoCao.Date);

                    if (existsInDb)
                        continue; // Đã tồn tại, bỏ qua

                    // Kiểm tra xem bảo trì này có bị đánh dấu là đã xóa không
                    var isDeleted = await _repo.IsDeletedFromSyncAsync(row.MaPhong, row.MoTaSuCo, row.NgayBaoCao);

                    if (isDeleted)
                        continue; // Đã bị xóa trước đó, không sync lại

                    // Kiểm tra xem MaPhong có tồn tại trong bảng Phong không (tránh lỗi foreign key constraint)
                    if (_roomRepository != null)
                    {
                        var roomExists = await _roomRepository.IsRoomExistsAsync(row.MaPhong);
                        if (!roomExists)
                        {
                            // Log cảnh báo và bỏ qua dòng này
                            if (!skippedRooms.Contains(row.MaPhong))
                            {
                                skippedRooms.Add(row.MaPhong);
                            }
                            System.Diagnostics.Debug.WriteLine($"Cảnh báo: Mã phòng {row.MaPhong} không tồn tại trong database. Bỏ qua bảo trì: {row.MoTaSuCo}");
                            continue;
                        }
                    }

                    // Tạo MaintenanceIncident mới
                    var newIncident = new MaintenanceIncident
                    {
                        MaPhong = row.MaPhong,
                        MoTaSuCo = row.MoTaSuCo,
                        NgayBaoCao = row.NgayBaoCao,
                        NgayCoTheSua = row.NgayCoTheSua, // Lấy từ cột D trong Google Sheets
                        TrangThai = "Chưa xử lý", // Mặc định trạng thái cho bảo trì mới
                        ChiPhi = 0 // Mặc định chi phí = 0
                    };

                    // Thêm vào database
                    await _repo.AddAsync(newIncident);
                    addedCount++;

                    // Gửi email thông báo đến tất cả khách hàng thuộc mã phòng
                    try
                    {
                        await SendMaintenanceNotificationEmailAsync(newIncident);
                    }
                    catch (Exception emailEx)
                    {
                        // Log lỗi nhưng không dừng quá trình đồng bộ
                        System.Diagnostics.Debug.WriteLine($"Lỗi khi gửi email thông báo sự cố (Phòng {newIncident.MaPhong}): {emailEx.Message}");
                    }
                }

                // Nếu có mã phòng không hợp lệ, thêm thông tin vào log
                if (skippedRooms.Count > 0)
                {
                    var skippedRoomsStr = string.Join(", ", skippedRooms);
                    System.Diagnostics.Debug.WriteLine($"Đã bỏ qua {skippedRooms.Count} mã phòng không tồn tại: {skippedRoomsStr}");
                }

                return addedCount;
            }
            catch (System.Exception ex)
            {
                // Log lỗi và ném lại exception với thông tin chi tiết hơn
                var errorMsg = $"Lỗi đồng bộ từ Google Sheets: {ex.Message}";
                if (ex.Message.Contains("foreign key constraint"))
                {
                    errorMsg += "\n\nNguyên nhân: Mã phòng trong Google Sheets không tồn tại trong database. Vui lòng kiểm tra lại dữ liệu trong Google Sheets.";
                }
                System.Diagnostics.Debug.WriteLine(errorMsg);
                throw new System.Exception(errorMsg, ex);
            }
        }

        /// <summary>
        /// Gửi email thông báo sự cố đến tất cả khách hàng thuộc mã phòng
        /// </summary>
        public async Task SendMaintenanceNotificationEmailAsync(MaintenanceIncident incident)
        {
            if (_tenantRepository == null)
            {
                System.Diagnostics.Debug.WriteLine("TenantRepository chưa được khởi tạo, không thể gửi email thông báo.");
                return;
            }

            try
            {
                // Lấy danh sách khách thuê theo mã phòng
                var roomTenants = await _tenantRepository.GetTenantsByRoomIdAsync(incident.MaPhong);

                if (roomTenants == null || roomTenants.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine($"Không tìm thấy khách thuê nào cho phòng {incident.MaPhong}");
                    return;
                }

                // Lấy thông tin phòng để hiển thị trong email
                string tenPhong = $"Phòng {incident.MaPhong}";
                if (_roomRepository != null)
                {
                    var room = await _roomRepository.GetByIdAsync(incident.MaPhong);
                    if (room != null && !string.IsNullOrEmpty(room.TenPhong))
                    {
                        tenPhong = room.TenPhong;
                    }
                }

                // Gửi email cho từng khách thuê
                foreach (var roomTenant in roomTenants)
                {
                    try
                    {
                        // Lấy thông tin đầy đủ của tenant để có email
                        var tenant = await _tenantRepository.GetByIdAsync(roomTenant.MaNguoiThue);
                        if (tenant == null || string.IsNullOrWhiteSpace(tenant.Email))
                        {
                            System.Diagnostics.Debug.WriteLine($"Khách thuê {roomTenant.HoTen} (Mã: {roomTenant.MaNguoiThue}) không có email, bỏ qua.");
                            continue;
                        }

                        // Tạo nội dung email
                        string subject = $"🔧 Thông báo: Đã nhận được báo cáo sự cố - {tenPhong}";
                        string body = GenerateMaintenanceEmailBody(incident, tenant, tenPhong);

                        // Gửi email
                        await EmailService.SendEmailAsync(tenant.Email, subject, body);
                        System.Diagnostics.Debug.WriteLine($"Đã gửi email thông báo sự cố đến {tenant.Email} (Phòng {incident.MaPhong})");
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Lỗi khi gửi email cho khách thuê {roomTenant.HoTen} (Phòng {incident.MaPhong}): {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi khi gửi email thông báo sự cố: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Tạo nội dung email HTML thông báo sự cố
        /// </summary>
        private string GenerateMaintenanceEmailBody(MaintenanceIncident incident, Tenant tenant, string tenPhong)
        {
            string ngayBaoCao = incident.NgayBaoCao.ToString("dd/MM/yyyy HH:mm");
            string trangThai = incident.TrangThai;
            string moTaSuCo = System.Net.WebUtility.HtmlEncode(incident.MoTaSuCo);

            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Thông báo sự cố</title>
</head>
<body style=""margin: 0; padding: 0; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #F5F5F5;"">
    <table role=""presentation"" style=""width: 100%; border-collapse: collapse; background-color: #F5F5F5;"">
        <tr>
            <td style=""padding: 40px 20px;"">
                <table role=""presentation"" style=""max-width: 600px; margin: 0 auto; background-color: #FFFFFF; border-radius: 12px; overflow: hidden; box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);"">
                    <!-- Header -->
                    <tr>
                        <td style=""background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 30px 20px; text-align: center;"">
                            <h1 style=""margin: 0; color: #FFFFFF; font-size: 24px; font-weight: 600;"">🔧 Thông Báo Sự Cố</h1>
                        </td>
                    </tr>
                    
                    <!-- Title -->
                    <tr>
                        <td style=""padding: 30px 20px 20px 20px; text-align: center; border-bottom: 2px solid #f3f4f6;"">
                            <h2 style=""margin: 0; color: #1F2937; font-size: 20px; font-weight: 600;"">Chủ nhà đã nhận được báo cáo sự cố</h2>
                            <p style=""margin: 10px 0 0 0; color: #6B7280; font-size: 16px;"">Thông báo từ hệ thống quản lý phòng trọ</p>
                        </td>
                    </tr>
                    
                    <!-- Content -->
                    <tr>
                        <td style=""padding: 30px 20px;"">
                            <p style=""margin: 0 0 20px 0; color: #374151; font-size: 15px; line-height: 1.6;"">
                                Kính gửi <strong style=""color: #1F2937;"">{System.Net.WebUtility.HtmlEncode(tenant.HoTen)}</strong>,
                            </p>
                            <p style=""margin: 0 0 25px 0; color: #374151; font-size: 15px; line-height: 1.6;"">
                                Chúng tôi xin thông báo rằng chủ nhà đã nhận được báo cáo sự cố từ phòng của bạn. 
                                Chúng tôi sẽ xử lý sự cố này trong thời gian sớm nhất.
                            </p>
                            
                            <!-- Incident Info Table -->
                            <table role=""presentation"" style=""width: 100%; border-collapse: collapse; margin: 25px 0; background-color: #F9FAFB; border-radius: 8px; overflow: hidden;"">
                                <tr>
                                    <td style=""padding: 20px; background-color: #3B82F6; color: #ffffff; font-weight: 600; font-size: 16px; text-align: center;"">
                                        📋 Thông Tin Sự Cố
                                    </td>
                                </tr>
                                <tr>
                                    <td style=""padding: 0;"">
                                        <table role=""presentation"" style=""width: 100%; border-collapse: collapse;"">
                                            <tr>
                                                <td style=""padding: 12px 20px; border-bottom: 1px solid #E5E7EB; color: #374151; font-size: 14px;"">
                                                    <strong style=""color: #1F2937; min-width: 140px; display: inline-block;"">Phòng:</strong>
                                                    <span style=""color: #6B7280;"">{System.Net.WebUtility.HtmlEncode(tenPhong)}</span>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style=""padding: 12px 20px; border-bottom: 1px solid #E5E7EB; color: #374151; font-size: 14px;"">
                                                    <strong style=""color: #1F2937; min-width: 140px; display: inline-block;"">Ngày báo cáo:</strong>
                                                    <span style=""color: #6B7280;"">{ngayBaoCao}</span>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style=""padding: 12px 20px; border-bottom: 1px solid #E5E7EB; color: #374151; font-size: 14px;"">
                                                    <strong style=""color: #1F2937; min-width: 140px; display: inline-block;"">Trạng thái:</strong>
                                                    <span style=""color: #6B7280;"">{System.Net.WebUtility.HtmlEncode(trangThai)}</span>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style=""padding: 12px 20px; color: #374151; font-size: 14px;"">
                                                    <strong style=""color: #1F2937; min-width: 140px; display: inline-block; vertical-align: top;"">Mô tả sự cố:</strong>
                                                    <span style=""color: #6B7280; display: inline-block; max-width: 400px;"">{moTaSuCo}</span>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>

                            <p style=""margin: 25px 0 0 0; color: #374151; font-size: 15px; line-height: 1.6;"">
                                Chúng tôi sẽ liên hệ với bạn sớm nhất có thể để xử lý sự cố này. 
                                Nếu có bất kỳ câu hỏi nào, vui lòng liên hệ với chúng tôi.
                            </p>
                        </td>
                    </tr>
                    
                    <!-- Footer -->
                    <tr>
                        <td style=""padding: 20px; background-color: #F9FAFB; border-top: 1px solid #E5E7EB; text-align: center;"">
                            <p style=""margin: 0; color: #6B7280; font-size: 12px; line-height: 1.6;"">
                                Email này được gửi tự động từ hệ thống quản lý phòng trọ.<br>
                                Vui lòng không trả lời email này.
                            </p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
        }

        /// <summary>
        /// Thêm sự cố mới và gửi email thông báo
        /// </summary>
        public async Task AddWithNotificationAsync(MaintenanceIncident incident)
        {
            await _repo.AddAsync(incident);
            
            // Gửi email thông báo
            try
            {
                await SendMaintenanceNotificationEmailAsync(incident);
            }
            catch (Exception emailEx)
            {
                // Log lỗi nhưng không dừng quá trình thêm sự cố
                System.Diagnostics.Debug.WriteLine($"Lỗi khi gửi email thông báo sự cố (Phòng {incident.MaPhong}): {emailEx.Message}");
            }
        }
    }
}
