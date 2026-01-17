using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using QLKDPhongTro.Presentation.Utils;
using QLKDPhongTro.Presentation.ViewModels;

namespace QLKDPhongTro.Presentation.Views.Windows_User
{
    public partial class UserDashboardWindow : Window
    {
        private DispatcherTimer? _carouselTimer;
        private int _currentImageIndex = 0;
        private readonly string[] _imageSources = { "/Resources/Images/Anh1.jpg", "/Resources/Images/Anh2.jpg" };

        public UserDashboardWindow()
        {
            InitializeComponent();
            
            // Có thể dùng chung DashboardViewModel hoặc tạo UserDashboardViewModel
            if (this.DataContext == null)
            {
                this.DataContext = new DashboardViewModel();
            }

            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.MinHeight = 600;
            this.MinWidth = 800;
            
            LoadInitialImages();
            InitializeCarousel();
            
            this.Closing += DashWindow_Closing;
            this.Closed += DashWindow_Closed;
        }

        private void LoadInitialImages()
        {
            LoadImage(Image1, _imageSources[0]);
            LoadImage(Image2, _imageSources[1]);
        }

        private void LoadImage(Image imageControl, string imagePath)
        {
            try
            {
                var resourceUri = new Uri($"/QLKDPhongTro.Presentation;component{imagePath}", UriKind.Relative);
                var streamInfo = Application.GetResourceStream(resourceUri);
                if (streamInfo != null)
                {
                    using (var stream = streamInfo.Stream)
                    {
                        var bitmap = new System.Windows.Media.Imaging.BitmapImage();
                        bitmap.BeginInit();
                        bitmap.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
                        bitmap.StreamSource = stream;
                        bitmap.EndInit();
                        imageControl.Source = bitmap;
                        return;
                    }
                }
            }
            catch (Exception ex1)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi load ảnh: {ex1.Message}");
            }

            try
            {
                var uri = new Uri(imagePath, UriKind.Relative);
                var bitmap = new System.Windows.Media.Imaging.BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = uri;
                bitmap.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                imageControl.Source = bitmap;
            }
            catch { }
        }

        private void InitializeCarousel()
        {
            _carouselTimer = new DispatcherTimer();
            _carouselTimer.Interval = TimeSpan.FromSeconds(5);
            _carouselTimer.Tick += CarouselTimer_Tick;
            _carouselTimer.Start();
        }

        private void CarouselTimer_Tick(object? sender, EventArgs e)
        {
            SwitchToNextImage();
        }

        private void SwitchToNextImage()
        {
            Image currentImage = _currentImageIndex == 0 ? Image1 : Image2;
            Image nextImage = _currentImageIndex == 0 ? Image2 : Image1;
            
            var nextImageIndex = (_currentImageIndex + 1) % _imageSources.Length;
            LoadImage(nextImage, _imageSources[nextImageIndex]);
            
            DoubleAnimation fadeOut = new DoubleAnimation { From = 1.0, To = 0.0, Duration = TimeSpan.FromSeconds(1) };
            DoubleAnimation fadeIn = new DoubleAnimation { From = 0.0, To = 1.0, Duration = TimeSpan.FromSeconds(1) };
            
            currentImage.BeginAnimation(UIElement.OpacityProperty, fadeOut);
            nextImage.BeginAnimation(UIElement.OpacityProperty, fadeIn);
            
            _currentImageIndex = (_currentImageIndex + 1) % _imageSources.Length;
        }

        private void DashWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_carouselTimer != null)
            {
                _carouselTimer.Stop();
                _carouselTimer = null;
            }
        }

        private void DashWindow_Closed(object? sender, EventArgs e)
        {
            WindowHelper.CheckAndShutdownIfNoWindows(this);
        }
    }
}
