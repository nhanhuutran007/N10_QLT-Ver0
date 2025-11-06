using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using QLKDPhongTro.Presentation.ViewModels;

namespace QLKDPhongTro.Presentation.Views.Windows
{
    public partial class DashWindow : Window
    {
        private DispatcherTimer _carouselTimer;
        private int _currentImageIndex = 0;
        private readonly string[] _imageSources = { "/Resources/Images/Anh1.jpg", "/Resources/Images/Anh2.jpg" };

        public DashWindow()
        {
            InitializeComponent();
            if (this.DataContext == null)
            {
                this.DataContext = new DashboardViewModel();
            }
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.MinHeight = 600;
            this.MinWidth = 800;
            
            // Load ảnh ban đầu
            LoadInitialImages();
            
            // Khởi tạo carousel timer
            InitializeCarousel();
            
            // Dừng timer khi window đóng
            this.Closing += (s, e) => 
            {
                if (_carouselTimer != null)
                {
                    _carouselTimer.Stop();
                    _carouselTimer = null;
                }
            };
        }

        private void LoadInitialImages()
        {
            // Load ảnh đầu tiên
            LoadImage(Image1, _imageSources[0]);
            
            // Load ảnh thứ hai
            LoadImage(Image2, _imageSources[1]);
        }

        private void LoadImage(Image imageControl, string imagePath)
        {
            try
            {
                // Thử cách 1: Load từ Resource stream (giống InvoiceDetailView)
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
                System.Diagnostics.Debug.WriteLine($"Lỗi load ảnh từ Resource stream ({imagePath}): {ex1.Message}");
            }

            try
            {
                // Thử cách 2: Load từ relative URI
                var uri = new Uri(imagePath, UriKind.Relative);
                var bitmap = new System.Windows.Media.Imaging.BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = uri;
                bitmap.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                imageControl.Source = bitmap;
            }
            catch (Exception ex2)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi load ảnh từ Relative URI ({imagePath}): {ex2.Message}");
            }
        }

        private void InitializeCarousel()
        {
            // Tạo timer để tự động chuyển ảnh mỗi 5 giây
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
            // Xác định ảnh hiện tại và ảnh tiếp theo
            Image currentImage = _currentImageIndex == 0 ? Image1 : Image2;
            Image nextImage = _currentImageIndex == 0 ? Image2 : Image1;
            
            // Cập nhật source cho ảnh tiếp theo
            var nextImageIndex = (_currentImageIndex + 1) % _imageSources.Length;
            LoadImage(nextImage, _imageSources[nextImageIndex]);
            
            // Tạo animation fade out cho ảnh hiện tại
            DoubleAnimation fadeOut = new DoubleAnimation
            {
                From = 1.0,
                To = 0.0,
                Duration = TimeSpan.FromSeconds(1)
            };
            
            // Tạo animation fade in cho ảnh tiếp theo
            DoubleAnimation fadeIn = new DoubleAnimation
            {
                From = 0.0,
                To = 1.0,
                Duration = TimeSpan.FromSeconds(1)
            };
            
            // Bắt đầu animation
            currentImage.BeginAnimation(UIElement.OpacityProperty, fadeOut);
            nextImage.BeginAnimation(UIElement.OpacityProperty, fadeIn);
            
            // Chuyển sang ảnh tiếp theo
            _currentImageIndex = (_currentImageIndex + 1) % _imageSources.Length;
        }


        // Handler cho TopbarControl events - giờ đây TopbarControl tự xử lý logic chung
        private void TopbarControl_MenuButtonClicked(object sender, EventArgs e)
        {
            // Logic chung đã được chuyển vào TopbarControl.xaml.cs
            // Chỉ xử lý logic riêng của DashWindow nếu cần
        }

        private void TopbarControl_SearchTextChanged(object sender, string searchText)
        {
            // Logic search chung đã được chuyển vào TopbarControl.xaml.cs
            // Chỉ xử lý search logic riêng của DashWindow nếu cần
        }

        private void TopbarControl_SettingsButtonClicked(object sender, EventArgs e)
        {
            // Logic settings chung đã được chuyển vào TopbarControl.xaml.cs
            // Chỉ xử lý settings logic riêng của DashWindow nếu cần
        }

        // Handler cho SidebarControl events - giờ đây SidebarControl tự xử lý navigation
        private void SidebarControl_MenuItemClicked(object sender, string menuItem)
        {
            // Logic navigation đã được chuyển vào SidebarControl.xaml.cs
            // Chỉ xử lý logic riêng của DashWindow nếu cần
        }


    }
}
