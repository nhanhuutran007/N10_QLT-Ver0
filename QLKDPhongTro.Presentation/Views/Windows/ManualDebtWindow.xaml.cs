using QLKDPhongTro.Presentation.ViewModels;
using System.Windows;

namespace QLKDPhongTro.Presentation.Views.Windows
{
    public partial class ManualDebtWindow : Window
    {
        public ManualDebtWindow(ManualDebtViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}


