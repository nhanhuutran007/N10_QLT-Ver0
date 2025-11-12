using System.Windows;
using QLKDPhongTro.Presentation.ViewModels;

namespace QLKDPhongTro.Presentation.Views.Windows
{
    public partial class FinancialWindow : Window
    {
        private readonly FinancialViewModel? _viewModel;

        public FinancialWindow()
        {
            try
            {
                // Khởi tạo ViewModel trước InitializeComponent để tránh binding errors
                _viewModel = new FinancialViewModel();
                
                // Subscribe to error events
                _viewModel.ShowMessageRequested += (s, msg) =>
                {
                    try
                    {
                        Dispatcher?.Invoke(() =>
                        {
                            MessageBox.Show(msg, "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        });
                    }
                    catch
                    {
                        // Ignore nếu dispatcher không sẵn sàng
                    }
                };
                
                DataContext = _viewModel;
                
                InitializeComponent();
                
                // Load data sau khi window đã load xong để tránh blocking UI
                Loaded += async (s, e) =>
                {
                    try
                    {
                        await _viewModel.LoadDataAsync();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi khởi tạo cửa sổ Quản lý Tài chính:\n{ex.Message}\n\nChi tiết:\n{ex.StackTrace}", 
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                try
                {
                    Close();
                }
                catch
                {
                    // Ignore nếu không thể close
                }
            }
        }

        private async void ScanImageButton_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel == null) return;
            
            try
            {
                var dialog = new ScanImageView();
                dialog.Owner = this;
                if (dialog.ShowDialog() == true)
                {
                    // Refresh data sau khi quét ảnh thành công
                    await _viewModel.LoadDataAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở cửa sổ quét ảnh: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void ManualInputButton_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel == null) return;
            
            try
            {
                var dialog = new ManualInputView();
                dialog.Owner = this;
                if (dialog.ShowDialog() == true)
                {
                    // Refresh data sau khi nhập liệu thành công
                    await _viewModel.LoadDataAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở cửa sổ nhập liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ViewDetail_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (_viewModel != null && sender is System.Windows.FrameworkElement element && element.DataContext is BusinessLayer.DTOs.FinancialRecordDto record)
            {
                _viewModel.ViewDetailCommand?.Execute(record);
            }
        }

        private void Card_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (sender is System.Windows.Controls.Border border)
            {
                border.Effect = new System.Windows.Media.Effects.DropShadowEffect
                {
                    Color = System.Windows.Media.Color.FromArgb(0xFF, 0xA9, 0xA8, 0xB6),
                    Opacity = 0.2,
                    ShadowDepth = 8,
                    BlurRadius = 30
                };
            }
        }

        private void Card_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (sender is System.Windows.Controls.Border border)
            {
                border.Effect = new System.Windows.Media.Effects.DropShadowEffect
                {
                    Color = System.Windows.Media.Color.FromArgb(0xFF, 0xA9, 0xA8, 0xB6),
                    Opacity = 0.1,
                    ShadowDepth = 4,
                    BlurRadius = 20
                };
            }
        }

        private void PageNumber_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel != null && sender is System.Windows.Controls.Button button && button.Content is int pageNumber)
            {
                _viewModel.CurrentPage = pageNumber;
            }
        }

        private void ViewAllRecords_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel != null)
            {
                _viewModel.CurrentView = "AllRecords";
                UpdateViewButtons(sender as System.Windows.Controls.Button);
            }
        }

        private void ViewDebts_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel != null)
            {
                _viewModel.CurrentView = "Debts";
                UpdateViewButtons(sender as System.Windows.Controls.Button);
            }
        }

        private void ViewReports_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel != null)
            {
                _viewModel.CurrentView = "Reports";
                UpdateViewButtons(sender as System.Windows.Controls.Button);
            }
        }

        private void UpdateViewButtons(System.Windows.Controls.Button? activeButton)
        {
            // Tìm tất cả các button trong StackPanel
            if (activeButton?.Parent is System.Windows.Controls.StackPanel panel)
            {
                foreach (var child in panel.Children)
                {
                    if (child is System.Windows.Controls.Button btn)
                    {
                        if (btn == activeButton)
                        {
                            btn.Style = (System.Windows.Style)FindResource("ActiveSecondarySegmentStyle");
                        }
                        else
                        {
                            btn.Style = (System.Windows.Style)FindResource("SecondarySegmentStyle");
                        }
                    }
                }
            }
        }
    }
}


