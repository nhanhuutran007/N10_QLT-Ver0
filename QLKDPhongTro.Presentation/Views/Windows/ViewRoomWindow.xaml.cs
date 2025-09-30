using System;
using System.Windows;
using System.Windows.Input;

namespace QLKDPhongTro.Presentation.Views.Windows
{
    public partial class ViewRoomWindow : Window
    {
        public string RoomId { get; set; }

        public ViewRoomWindow()
        {
            InitializeComponent();
            
            // Tạo hiệu ứng fade-in mượt mà khi mở popup
            this.Opacity = 0;
            this.Loaded += (s, e) => {
                var fadeIn = new System.Windows.Media.Animation.DoubleAnimation
                {
                    From = 0,
                    To = 1,
                    Duration = TimeSpan.FromMilliseconds(300),
                    EasingFunction = new System.Windows.Media.Animation.CubicEase { EasingMode = System.Windows.Media.Animation.EasingMode.EaseOut }
                };
                this.BeginAnimation(OpacityProperty, fadeIn);
            };
        }

        // Sự kiện kéo cửa sổ
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        // Đóng cửa sổ với animation
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            CloseWithAnimation();
        }

        // Phương thức đóng với hiệu ứng fade-out
        private void CloseWithAnimation()
        {
            var fadeOut = new System.Windows.Media.Animation.DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(200),
                EasingFunction = new System.Windows.Media.Animation.CubicEase { EasingMode = System.Windows.Media.Animation.EasingMode.EaseIn }
            };
            
            fadeOut.Completed += (s, e) => this.Close();
            this.BeginAnimation(OpacityProperty, fadeOut);
        }

        // Phương thức để set dữ liệu phòng cần xem
        public void SetRoomData(string roomId, string roomName, string bedCount, string rentPrice, 
                               string status, string description, string area, string floor, 
                               string direction, string maxPeople, string qualityRating)
        {
            RoomId = roomId;
            
            // TODO: Cập nhật UI với dữ liệu thực từ database
            // Hiện tại sử dụng dữ liệu mẫu để demo
            
            // Có thể thêm logic để cập nhật các TextBlock trong XAML
            // hoặc sử dụng Data Binding với ViewModel
        }
    }
}
