using QLKDPhongTro.BusinessLayer.DTOs;
using System;
using System.Windows;
using System.Windows.Controls;

namespace QLKDPhongTro.Presentation.Views.Windows
{
    public partial class AddEditAssetWindow : Window
    {
        public TenantAssetDto Asset { get; private set; }

        public AddEditAssetWindow(TenantAssetDto asset, int maNguoiThue)
        {
            InitializeComponent();
            Asset = asset;
            var titleText = asset.MaTaiSan > 0 ? "Sửa tài sản" : "Thêm tài sản";
            Title = titleText;
            TitleTextBlock.Text = titleText;
            
            // Load asset types
            LoaiTaiSanComboBox.ItemsSource = new[] { "Xe", "Thú cưng", "Khác" };
            
            // Set default if empty
            if (string.IsNullOrEmpty(Asset.LoaiTaiSan))
            {
                Asset.LoaiTaiSan = "Xe";
            }
            
            // Select current value
            LoaiTaiSanComboBox.SelectedItem = Asset.LoaiTaiSan;
            
            // Bind to asset
            DataContext = Asset;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Validate
            if (LoaiTaiSanComboBox.SelectedItem == null || string.IsNullOrWhiteSpace(LoaiTaiSanComboBox.SelectedItem.ToString()))
            {
                MessageBox.Show("Vui lòng chọn loại tài sản!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Update asset from UI
            Asset.LoaiTaiSan = LoaiTaiSanComboBox.SelectedItem.ToString() ?? "Xe";
            Asset.MoTa = MoTaTextBox.Text ?? string.Empty;

            // Parse phí phụ thu
            if (!decimal.TryParse(PhiPhuThuTextBox.Text, out decimal phiPhuThu))
            {
                phiPhuThu = 0;
            }
            Asset.PhiPhuThu = phiPhuThu;

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}

