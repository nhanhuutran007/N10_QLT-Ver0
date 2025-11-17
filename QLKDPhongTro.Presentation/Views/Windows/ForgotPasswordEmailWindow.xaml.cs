using System.Windows;
using QLKDPhongTro.Presentation.ViewModels;

namespace QLKDPhongTro.Presentation.Views.Windows
{
    public partial class ForgotPasswordEmailWindow : Window
    {
        public ForgotPasswordEmailWindow()
        {
            InitializeComponent();
            DataContext = new ForgotPasswordEmailViewModel();
        }
    }
}
