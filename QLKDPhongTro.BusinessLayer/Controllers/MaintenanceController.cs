using QLKDPhongTro.DataLayer.Models;
using QLKDPhongTro.DataLayer.Repositories;
using QLKDPhongTro.BusinessLayer.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QLKDPhongTro.BusinessLayer.Controllers
{
    public class MaintenanceController
    {
        private readonly IMaintenanceRepository _repo;
        private readonly GoogleSheetsService _googleSheetsService;

        public MaintenanceController(IMaintenanceRepository repo, GoogleSheetsService? googleSheetsService = null)
        {
            _repo = repo;
            _googleSheetsService = googleSheetsService ?? new GoogleSheetsService(new System.Net.Http.HttpClient());
        }

        public Task<List<MaintenanceIncident>> GetAllAsync() => _repo.GetAllAsync();
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

                    // Tạo MaintenanceIncident mới
                    var newIncident = new MaintenanceIncident
                    {
                        MaPhong = row.MaPhong,
                        MoTaSuCo = row.MoTaSuCo,
                        NgayBaoCao = row.NgayBaoCao,
                        TrangThai = "Chưa xử lý", // Mặc định trạng thái cho bảo trì mới
                        ChiPhi = 0 // Mặc định chi phí = 0
                    };

                    // Thêm vào database
                    await _repo.AddAsync(newIncident);
                    addedCount++;
                }

                return addedCount;
            }
            catch (System.Exception ex)
            {
                // Log lỗi và ném lại exception
                System.Diagnostics.Debug.WriteLine($"Lỗi đồng bộ từ Google Sheets: {ex.Message}");
                throw;
            }
        }
    }
}
