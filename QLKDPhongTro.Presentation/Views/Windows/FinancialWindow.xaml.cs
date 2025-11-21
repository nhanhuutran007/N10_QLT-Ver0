using System;
using System.Windows;
using System.Windows.Controls;
using QLKDPhongTro.Presentation.ViewModels;
using QLKDPhongTro.BusinessLayer.DTOs;
using System.Windows.Input; // Cần thiết cho MouseButtonEventArgs

namespace QLKDPhongTro.Presentation.Views.Windows
{
    public partial class FinancialWindow : Window
    {
        private readonly FinancialViewModel? _viewModel;

        public FinancialWindow()
        {
            try
            {
                // Khởi tạo ViewModel trước InitializeComponent
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

                // Subscribe to view changed event để cập nhật tab button khi CurrentView thay đổi từ code
                _viewModel.ViewChangedRequested += (s, viewName) =>
                {
                    try
                    {
                        Dispatcher?.Invoke(() =>
                        {
                            Button? activeButton = viewName switch
                            {
                                "AllRecords" => ViewAllRecordsButton,
                                "Debts" => ViewDebtsButton,
                                //"Reports" => ViewReportsButton,
                                _ => null
                            };
                            if (activeButton != null)
                            {
                                UpdateViewButtons(activeButton);
                            }
                        });
                    }
                    catch
                    {
                        // Ignore nếu dispatcher không sẵn sàng
                    }
                };

                // Load data sau khi window đã load xong
                Loaded += async (s, e) =>
                {
                    try
                    {
                        if (_viewModel != null)
                        {
                            await _viewModel.LoadDataAsync();
                            // Đặt view mặc định sau khi load
                            UpdateViewButtons(ViewAllRecordsButton);
                        }
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
                    // Ignore
                }
            }
        }

        private async void ManualInputButton_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel == null) return;

            try
            {
                // var dialog = new ManualInputView(); // Lỗi: 'ManualInputView' could not be found
                // dialog.Owner = this;
                // if (dialog.ShowDialog() == true)
                // {
                //     // Refresh data sau khi nhập liệu thành công
                //     await _viewModel.LoadDataAsync();
                // }
                MessageBox.Show("Chức năng 'Nhập tay' chưa được liên kết (thiếu file ManualInputView).");
                await Task.CompletedTask; // Giữ cho hàm async
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở cửa sổ nhập liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ViewDetail_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (_viewModel != null && sender is FrameworkElement element && element.DataContext is FinancialRecordDto record)
            {
                _viewModel.ViewDetailCommand?.Execute(record);
            }
        }

        private void Card_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (sender is Border border)
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
            if (sender is Border border)
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
            if (_viewModel != null && sender is Button button && button.Content != null)
            {
                if (int.TryParse(button.Content.ToString(), out int pageNumber))
                {
                    _viewModel.CurrentPage = pageNumber;
                }
            }
        }

        private void ViewAllRecords_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel != null)
            {
                _viewModel.CurrentView = "AllRecords";
                UpdateViewButtons(sender as Button);
            }
        }

        private void ViewDebts_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel != null)
            {
                _viewModel.CurrentView = "Debts";
                UpdateViewButtons(sender as Button);
            }
        }

        private void ViewReports_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel != null)
            {
                _viewModel.CurrentView = "Reports";
                UpdateViewButtons(sender as Button);
            }
        }

        private void UpdateViewButtons(Button? activeButton)
        {
            // Tìm tất cả các button trong StackPanel
            if (activeButton?.Parent is StackPanel panel)
            {
                foreach (var child in panel.Children)
                {
                    if (child is Button btn)
                    {
                        // SỬA LỖI: Cập nhật tên Style mới khớp với file XAML hiện tại
                        if (btn == activeButton)
                        {
                            // Style khi đang Active
                            var activeStyle = FindResource("ModernActiveSegmentButton") as Style;
                            if (activeStyle != null) btn.Style = activeStyle;
                        }
                        else
                        {
                            // Style khi bình thường
                            var normalStyle = FindResource("ModernSegmentButton") as Style;
                            if (normalStyle != null) btn.Style = normalStyle;
                        }
                    }
                }
            }
        }

        // UPDATE: Thêm trình xử lý sự kiện double-click
        private void DataGridRow_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (_viewModel == null) return;

            // Lấy hàng được click và DataContext (DebtReportDto) của nó
            if (sender is DataGridRow row && row.DataContext is DebtReportDto debt)
            {
                // Chỉ kích hoạt nếu công nợ đang ở trạng thái "Cảnh báo"
                if (debt.TrangThaiThanhToan == "Cảnh báo")
                {
                    // Thực thi Command kiểm tra sự cố từ ViewModel
                    if (_viewModel.InspectDiscrepancyCommand.CanExecute(debt))
                    {
                        _viewModel.InspectDiscrepancyCommand.Execute(debt);
                    }
                    // Đánh dấu sự kiện đã được xử lý
                    e.Handled = true;
                }
            }
        }
    }
}