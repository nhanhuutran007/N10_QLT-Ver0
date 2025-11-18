using QLKDPhongTro.BusinessLayer.Controllers;
using QLKDPhongTro.DataLayer.Repositories;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace QLKDPhongTro.Presentation.Services
{
    /// <summary>
    /// Service tự động gửi email cảnh báo khi hợp đồng còn đúng 30 ngày
    /// </summary>
    public class ContractExpiryEmailService
    {
        private readonly ContractController _contractController;
        private DispatcherTimer _timer;
        private bool _isRunning = false;
        private DateTime _lastCheckDate = DateTime.MinValue;

        public ContractExpiryEmailService()
        {
            var repository = new ContractRepository();
            _contractController = new ContractController(repository);
        }

        /// <summary>
        /// Bắt đầu service tự động kiểm tra và gửi email
        /// </summary>
        public void Start()
        {
            if (_isRunning)
                return;

            _isRunning = true;

            // Tạo timer kiểm tra mỗi ngày một lần (24 giờ)
            // DispatcherTimer phải được tạo trên UI thread (sẽ được gọi từ OnStartup)
            _timer = new DispatcherTimer(DispatcherPriority.Background)
            {
                Interval = TimeSpan.FromHours(24) // Kiểm tra mỗi 24 giờ
            };
            _timer.Tick += async (sender, e) => 
            {
                // Chạy async trên background thread để không block UI
                await Task.Run(async () => await CheckAndSendEmailsAsync());
            };
            _timer.Start();

            // Kiểm tra ngay lập tức khi khởi động (chạy trên background thread)
            _ = Task.Run(async () => await CheckAndSendEmailsAsync());
        }

        /// <summary>
        /// Dừng service
        /// </summary>
        public void Stop()
        {
            if (!_isRunning)
                return;

            _isRunning = false;
            _timer?.Stop();
            _timer = null;
        }

        /// <summary>
        /// Kiểm tra và gửi email cho hợp đồng còn đúng 30 ngày
        /// </summary>
        private async Task CheckAndSendEmailsAsync()
        {
            try
            {
                // Chỉ kiểm tra một lần mỗi ngày (tránh gửi trùng lặp)
                DateTime today = DateTime.Now.Date;
                if (_lastCheckDate == today)
                    return;

                _lastCheckDate = today;

                // Gửi email cho hợp đồng còn đúng 30 ngày
                var result = await _contractController.SendExpiryWarningEmailsForExactDaysAsync(30);

                // Log kết quả (có thể mở rộng để ghi vào file log)
                if (result.Success > 0 || result.Failed > 0)
                {
                    System.Diagnostics.Debug.WriteLine(
                        $"[ContractExpiryEmailService] Đã gửi email cảnh báo: " +
                        $"Thành công: {result.Success}, Thất bại: {result.Failed}");
                }
            }
            catch (Exception ex)
            {
                // Log lỗi (có thể mở rộng để ghi vào file log)
                System.Diagnostics.Debug.WriteLine(
                    $"[ContractExpiryEmailService] Lỗi khi gửi email cảnh báo: {ex.Message}");
            }
        }

        /// <summary>
        /// Kiểm tra thủ công (có thể gọi từ UI để test)
        /// </summary>
        public async Task<(int Success, int Failed, List<string> Errors)> CheckAndSendEmailsManuallyAsync()
        {
            return await _contractController.SendExpiryWarningEmailsForExactDaysAsync(30);
        }
    }
}

