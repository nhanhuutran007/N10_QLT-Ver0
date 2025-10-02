using System.Windows;
using QLKDPhongTro.Presentation.ViewModels;
using QLKDPhongTro.BusinessLayer.Controllers;
using QLKDPhongTro.DataLayer.Repositories;

namespace QLKDPhongTro.Presentation.Views.Windows
{
    public partial class OtpWindow : Window
    {
        public OtpWindow(string username, string email, string password)
        {
            InitializeComponent();
            
            // Khởi tạo UserRepository và ViewModel
            var userRepository = new UserRepository();
            this.DataContext = new OtpViewModel(userRepository, username, email, password);
        }
    }
}
