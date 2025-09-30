using System;
using System.Windows;
using System.Windows.Input;

namespace QLKDPhongTro.Presentation.Views.Windows
{
    public partial class EditRoomWindow : Window
    {
        public string RoomId { get; set; }
        public string OriginalRoomName { get; set; }

        public EditRoomWindow()
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

        // Hủy chỉnh sửa
        private void CancelButton_Click(object sender, RoutedEventArgs e)
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

        // Cập nhật thông tin phòng
        private void UpdateRoomButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Lấy dữ liệu từ form
                string roomName = RoomNameTextBox.Text.Trim();
                string bedCount = BedCountTextBox.Text.Trim();
                string rentPrice = RentPriceTextBox.Text.Trim();
                string status = ((System.Windows.Controls.ComboBoxItem)StatusComboBox.SelectedItem)?.Content?.ToString() ?? "";
                string description = DescriptionTextBox.Text.Trim();

                // Validation
                if (string.IsNullOrEmpty(roomName))
                {
                    MessageBox.Show("Vui lòng nhập tên phòng!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    RoomNameTextBox.Focus();
                    return;
                }

                if (string.IsNullOrEmpty(bedCount) || !int.TryParse(bedCount, out int bedCountValue) || bedCountValue <= 0)
                {
                    MessageBox.Show("Vui lòng nhập số giường hợp lệ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    BedCountTextBox.Focus();
                    return;
                }

                if (string.IsNullOrEmpty(rentPrice))
                {
                    MessageBox.Show("Vui lòng nhập giá thuê!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    RentPriceTextBox.Focus();
                    return;
                }

                // TODO: Thêm logic cập nhật phòng vào database
                // Ở đây bạn có thể gọi service hoặc repository để cập nhật dữ liệu
                
                // Hiển thị thông báo thành công
                MessageBox.Show($"Đã cập nhật thông tin phòng '{roomName}' thành công!", "Thành công", 
                              MessageBoxButton.OK, MessageBoxImage.Information);

                // Đóng cửa sổ với animation
                this.DialogResult = true;
                CloseWithAnimation();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật phòng: {ex.Message}", "Lỗi", 
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Phương thức để set dữ liệu phòng cần chỉnh sửa
        public void SetRoomData(string roomId, string roomName, string bedCount, string rentPrice, string status, string description)
        {
            RoomId = roomId;
            OriginalRoomName = roomName;
            RoomNameTextBox.Text = roomName;
            BedCountTextBox.Text = bedCount;
            RentPriceTextBox.Text = rentPrice;
            DescriptionTextBox.Text = description;

            // Set trạng thái
            foreach (System.Windows.Controls.ComboBoxItem item in StatusComboBox.Items)
            {
                if (item.Content.ToString() == status)
                {
                    StatusComboBox.SelectedItem = item;
                    break;
                }
            }
        }
    }
}
