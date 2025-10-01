using System.Windows;
using QLKDPhongTro.Presentation.ViewModels;
using QLKDPhongTro.BusinessLayer.Controllers;
using QLKDPhongTro.DataLayer.Repositories;

namespace QLKDPhongTro.Presentation.Views.Windows
{
    public partial class OtpWindow : Window
    {
        public OtpWindow()
        {
            InitializeComponent();
            
            // Khởi tạo AuthController và ViewModel
            var userRepository = new UserRepository();
            var authController = new AuthController(userRepository);
            this.DataContext = new OtpViewModel(authController);
        }
    }
}
