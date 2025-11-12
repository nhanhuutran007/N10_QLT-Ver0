using System;
using System.Windows;
using System.Windows.Controls;
using QLKDPhongTro.Presentation.Views.Windows;

namespace QLKDPhongTro.Presentation.Views.Components
{
    public partial class TopbarControl : UserControl
    {
        public event EventHandler MenuButtonClicked;
        public event EventHandler<string> SearchTextChanged;
        public event EventHandler SettingsButtonClicked;

        public TopbarControl()
        {
            InitializeComponent();
        }

        // Centralized logic for TopbarControl actions
        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            // Toggle sidebar visibility - có thể implement logic toggle sidebar ở đây
            var currentWindow = Window.GetWindow(this);
            if (currentWindow != null)
            {
                // TODO: Implement sidebar toggle logic
                System.Diagnostics.Debug.WriteLine("Menu button clicked - Toggle sidebar");
            }
            
            // Vẫn fire event để các window có thể xử lý logic riêng nếu cần
            MenuButtonClicked?.Invoke(this, EventArgs.Empty);
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null)
            {
                var searchText = textBox.Text;
                
                // Centralized search logic
                if (!string.IsNullOrEmpty(searchText))
                {
                    System.Diagnostics.Debug.WriteLine($"Searching for: {searchText}");
                    // TODO: Implement global search logic
                }
                
                // Fire event để các window có thể xử lý search riêng
                SearchTextChanged?.Invoke(this, searchText);
            }
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            // Centralized settings logic
            var currentWindow = Window.GetWindow(this);
            if (currentWindow != null)
            {
                var chat = new ChatWindow
                {
                    Owner = currentWindow
                };
                chat.Show();
            }
            
            // Vẫn fire event để các window có thể xử lý logic riêng nếu cần
            SettingsButtonClicked?.Invoke(this, EventArgs.Empty);
        }

        private void ProfilePopup_Opened(object sender, EventArgs e)
        {
            // Refresh thông tin user mỗi khi popup mở để đảm bảo hiển thị thông tin mới nhất
            if (ProfileDropDownControl != null)
            {
                ProfileDropDownControl.RefreshUserInfo();
            }
        }
    }
}
