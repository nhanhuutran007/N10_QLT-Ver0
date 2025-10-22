using System;
using System.Windows;
using System.Windows.Input;
using QLKDPhongTro.Presentation.ViewModels;

namespace QLKDPhongTro.Presentation.Views.Windows
{
    public partial class AddTenantWindow : Window
    {
        public AddTenantWindow()
        {
            InitializeComponent();
        }

        public AddTenantWindow(TenantViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
