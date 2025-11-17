using System.Configuration;
using System.Data;
using System.Windows;
using QLKDPhongTro.Presentation.Services;

namespace QLKDPhongTro.Presentation
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ContractExpiryEmailService _contractExpiryEmailService;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Khởi động service tự động gửi email cảnh báo hết hạn hợp đồng
            _contractExpiryEmailService = new ContractExpiryEmailService();
            _contractExpiryEmailService.Start();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // Dừng service khi ứng dụng đóng
            _contractExpiryEmailService?.Stop();
            base.OnExit(e);
        }
    }

}
