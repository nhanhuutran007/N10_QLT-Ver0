using System.Windows;
using QLKDPhongTro.BusinessLayer.DTOs;

namespace QLKDPhongTro.Presentation.Views.Windows
{
    public partial class DeleteTenantConfirmWindow : Window
    {
        public bool IsConfirmed { get; private set; } = false;

        public DeleteTenantConfirmWindow(TenantDto tenant)
        {
            InitializeComponent();
            
            if (tenant != null)
            {
                TenantNameTextBlock.Text = $"Tên: {tenant.HoTen}";
                var infoParts = new System.Collections.Generic.List<string>();
                if (!string.IsNullOrEmpty(tenant.SoDienThoai))
                    infoParts.Add($"SĐT: {tenant.SoDienThoai}");
                if (tenant.MaPhong.HasValue)
                    infoParts.Add($"Phòng: {tenant.MaPhong}");
                if (!string.IsNullOrEmpty(tenant.TrangThai))
                    infoParts.Add($"Trạng thái: {tenant.TrangThai}");
                
                TenantInfoTextBlock.Text = string.Join(" | ", infoParts);
            }
        }

        private void ConfirmCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            ConfirmButton.IsEnabled = true;
        }

        private void ConfirmCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            ConfirmButton.IsEnabled = false;
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (ConfirmCheckBox.IsChecked == true)
            {
                IsConfirmed = true;
                DialogResult = true;
                Close();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            IsConfirmed = false;
            DialogResult = false;
            Close();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            IsConfirmed = false;
            DialogResult = false;
            Close();
        }
    }
}

