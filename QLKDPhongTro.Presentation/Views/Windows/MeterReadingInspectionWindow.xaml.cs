using System.Windows;
using QLKDPhongTro.Presentation.ViewModels;

namespace QLKDPhongTro.Presentation.Views.Windows
{
    public partial class MeterReadingInspectionWindow : Window
    {
        public MeterReadingInspectionWindow(MeterReadingInspectionViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            if (viewModel != null)
            {
                viewModel.RequestClose += (s, e) => this.Close();
            }
        }
    }
}