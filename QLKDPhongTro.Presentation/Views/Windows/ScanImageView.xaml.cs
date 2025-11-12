using Microsoft.Win32;
using QLKDPhongTro.Presentation.Services;
using QLKDPhongTro.Presentation.ViewModels;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace QLKDPhongTro.Presentation.Views.Windows
{
    public partial class ScanImageView : Window
    {
        private readonly ScanImageViewModel _viewModel;

        public decimal? ExtractedValue => _viewModel.ExtractedValue;
        public MeterType SelectedMeterType => _viewModel.SelectedMeterType;

        public ScanImageView()
        {
            // Khởi tạo ViewModel TRƯỚC InitializeComponent để tránh NullReferenceException
            _viewModel = new ScanImageViewModel();
            DataContext = _viewModel;

            _viewModel.ShowMessageRequested += (s, msg) =>
            {
                MessageBox.Show(msg, "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            };

            _viewModel.ValueExtracted += (s, value) =>
            {
                if (value.HasValue)
                {
                    DialogResult = true;
                }
            };

            InitializeComponent();
        }

        private void ElectricityRadio_Checked(object sender, RoutedEventArgs e)
        {
            if (_viewModel != null)
            {
                _viewModel.SelectedMeterType = MeterType.Electricity;
            }
        }

        private void WaterRadio_Checked(object sender, RoutedEventArgs e)
        {
            if (_viewModel != null)
            {
                _viewModel.SelectedMeterType = MeterType.Water;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void DropZone_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void DropZone_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var paths = (string[])e.Data.GetData(DataFormats.FileDrop);
                var allowedExt = new HashSet<string>(new[] { ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".webp" }, System.StringComparer.OrdinalIgnoreCase);
                var files = paths.Where(p => File.Exists(p) && allowedExt.Contains(Path.GetExtension(p))).ToList();
                if (files.Count > 0 && _viewModel != null)
                {
                    _viewModel.SetImagePaths(files);
                }
            }
        }

        private void DropZone_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Title = "Chọn ảnh để quét",
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif;*.webp",
                Multiselect = true
            };

            if (dlg.ShowDialog(this) == true && _viewModel != null)
            {
                _viewModel.SetImagePaths(dlg.FileNames);
            }
        }

        private void ImagePreview_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is System.Windows.FrameworkElement element && element.DataContext is ImageScanResult result)
            {
                // Mở ảnh full size trong window mới
                var imageWindow = new Window
                {
                    Title = $"Xem ảnh: {result.ImageName}",
                    Width = 800,
                    Height = 600,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                };

                var scrollViewer = new System.Windows.Controls.ScrollViewer
                {
                    HorizontalScrollBarVisibility = System.Windows.Controls.ScrollBarVisibility.Auto,
                    VerticalScrollBarVisibility = System.Windows.Controls.ScrollBarVisibility.Auto
                };

                var image = new System.Windows.Controls.Image
                {
                    Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(result.ImagePath, UriKind.Absolute)),
                    Stretch = System.Windows.Media.Stretch.Uniform
                };

                scrollViewer.Content = image;
                imageWindow.Content = scrollViewer;
                imageWindow.ShowDialog();
            }
        }
    }
}


