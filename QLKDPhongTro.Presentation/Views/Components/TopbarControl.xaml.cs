using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using QLKDPhongTro.Presentation.Views.Windows;
using QLKDPhongTro.Presentation.Utils;

namespace QLKDPhongTro.Presentation.Views.Components
{
    public partial class TopbarControl : UserControl
    {
        public event EventHandler MenuButtonClicked;
        public event EventHandler<string> SearchTextChanged;
        public event EventHandler SettingsButtonClicked;

        // Danh sách lệnh gợi ý cố định
        private readonly (string Key, string Display)[] _commands = new[]
        {
            ("room", "Thêm phòng"),
            ("tenant", "Thêm người thuê"),
            ("billing", "Thêm thanh toán / hóa đơn"),
            ("maintenance", "Danh sách bảo trì"),
            ("contract", "Quản lý hợp đồng"),
            ("house", "Thông tin nhà"),
            ("logout", "Đăng xuất"),

            // Lệnh quản lý / tổng quan
            ("room-manage", "Quản lý phòng"),
            ("tenant-manage", "Quản lý người thuê"),
            ("financial", "Quản lý tài chính"),
            ("payment-manage", "Quản lý thanh toán"),
            ("maintenance-manage", "Quản lý bảo trì"),
            ("report", "Báo cáo thống kê"),
            ("security", "Bảo mật người dùng")
        };

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

                // Cập nhật gợi ý lệnh
                var keyword = (searchText ?? string.Empty).Trim().ToLowerInvariant();
                if (string.IsNullOrEmpty(keyword))
                {
                    if (CommandSuggestionPopup != null)
                    {
                        CommandSuggestionPopup.IsOpen = false;
                    }
                }
                else if (CommandSuggestionListBox != null && CommandSuggestionPopup != null)
                {
                    var suggestions = _commands
                        .Where(c => c.Display.ToLowerInvariant().Contains(keyword)
                                    || c.Key.ToLowerInvariant().Contains(keyword)
                                    || keyword.Contains(c.Key.ToLowerInvariant()))
                        .Select(c => c.Display)
                        .ToList();

                    if (suggestions.Count > 0)
                    {
                        CommandSuggestionListBox.ItemsSource = suggestions;
                        CommandSuggestionListBox.SelectedIndex = 0;
                        CommandSuggestionPopup.IsOpen = true;
                    }
                    else
                    {
                        CommandSuggestionPopup.IsOpen = false;
                    }
                }
            }
        }

        private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
            {
                return;
            }

            var text = (sender as TextBox)?.Text ?? string.Empty;
            var commandText = text.Trim().ToLowerInvariant();
            if (string.IsNullOrEmpty(commandText))
            {
                return;
            }

            // Nếu có gợi ý đang mở và có item được chọn, dùng gợi ý đó
            if (CommandSuggestionPopup != null && CommandSuggestionPopup.IsOpen &&
                CommandSuggestionListBox != null && CommandSuggestionListBox.SelectedItem != null)
            {
                var selectedDisplay = CommandSuggestionListBox.SelectedItem.ToString() ?? string.Empty;
                ExecuteCommandFromText(selectedDisplay);
                CommandSuggestionPopup.IsOpen = false;
                return;
            }

            ExecuteCommandFromText(commandText);
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

        private void CommandSuggestionListBox_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (CommandSuggestionListBox?.SelectedItem == null)
            {
                return;
            }

            var display = CommandSuggestionListBox.SelectedItem.ToString() ?? string.Empty;
            if (!string.IsNullOrEmpty(display))
            {
                SearchTextBox.Text = display;
                ExecuteCommandFromText(display);
            }

            CommandSuggestionPopup.IsOpen = false;
        }

        private void ExecuteCommandFromText(string rawText)
        {
            var commandText = (rawText ?? string.Empty).Trim().ToLowerInvariant();
            if (string.IsNullOrEmpty(commandText)) return;

            try
            {
                while (commandText.Contains("  "))
                {
                    commandText = commandText.Replace("  ", " ");
                }

                // === Thêm phòng ===
                if (commandText.Contains("them phong") ||
                    commandText.Contains("thêm phòng") ||
                    commandText.Contains("add room") ||
                    commandText.Contains("room") ||
                    commandText.Contains("quan ly phong") ||
                    commandText.Contains("quản lý phòng"))
                {
                    var window = new RoomWindow();
                    window.Show();

                    if (window.DataContext is QLKDPhongTro.Presentation.ViewModels.RentedRoomViewModel vmRoom &&
                        vmRoom.ShowAddRoomPanelCommand != null)
                    {
                        vmRoom.ShowAddRoomPanelCommand.Execute(null);
                    }
                    return;
                }

                // === Thêm người thuê ===
                if (commandText.Contains("them khach") ||
                    commandText.Contains("them nguoi thue") ||
                    commandText.Contains("thêm khách") ||
                    commandText.Contains("thêm người thuê") ||
                    commandText.Contains("add tenant") ||
                    commandText.Contains("tenant") ||
                    commandText.Contains("quan ly nguoi thue") ||
                    commandText.Contains("quản lý người thuê"))
                {
                    var window = new TenantManagementWindow();
                    window.Show();

                    if (window.DataContext is QLKDPhongTro.Presentation.ViewModels.TenantViewModel vmTenant &&
                        vmTenant.ShowAddTenantPanelCommand != null)
                    {
                        vmTenant.ShowAddTenantPanelCommand.Execute(null);
                    }
                    return;
                }

                // === Thêm thanh toán / hóa đơn ===
                if (commandText.Contains("them thanh toan") ||
                    commandText.Contains("thêm thanh toán") ||
                    commandText.Contains("them hoa don") ||
                    commandText.Contains("thêm hóa đơn") ||
                    commandText.Contains("add billing") ||
                    commandText.Contains("add payment") ||
                    commandText.Contains("billing"))
                {
                    var currentWindow = Window.GetWindow(this);
                    var window = new AddBillingInfoWindow
                    {
                        Owner = currentWindow
                    };
                    window.ShowDialog();
                    return;
                }

                // === Quản lý tài chính ===
                if (commandText.Contains("quan ly tai chinh") ||
                    commandText.Contains("quản lý tài chính") ||
                    commandText.Contains("financial"))
                {
                    // Mở cửa sổ Quản lý tài chính hiện có
                    var window = new FinancialWindow();
                    window.Show();
                    return;
                }

                // === Quản lý thanh toán (danh sách) ===
                if (commandText.Contains("quan ly thanh toan") ||
                    commandText.Contains("quản lý thanh toán") ||
                    commandText.Contains("payment"))
                {
                    var window = new PaymentListView();
                    window.Show();
                    return;
                }

                // === Bảo trì / Quản lý bảo trì ===
                if (commandText.Contains("bao tri") ||
                    commandText.Contains("bảo trì") ||
                    commandText.Contains("maintenance"))
                {
                    var window = new MaintenanceListView
                    {
                        Owner = Window.GetWindow(this)
                    };
                    window.Show();
                    return;
                }

                // === Hợp đồng ===
                if (commandText.Contains("hop dong") ||
                    commandText.Contains("hợp đồng") ||
                    commandText.Contains("contract"))
                {
                    var window = new ContractManagementWindow
                    {
                        Owner = Window.GetWindow(this)
                    };
                    window.Show();
                    return;
                }

                // === Thông tin nhà ===
                if (commandText.Contains("thong tin nha") ||
                    commandText.Contains("thông tin nhà") ||
                    commandText.Contains("house info") ||
                    commandText.Contains("house"))
                {
                    var window = new HouseInfoWindow
                    {
                        Owner = Window.GetWindow(this)
                    };
                    window.ShowDialog();
                    return;
                }

                // === Báo cáo thống kê ===
                if (commandText.Contains("bao cao") ||
                    commandText.Contains("thong ke") ||
                    commandText.Contains("báo cáo") ||
                    commandText.Contains("thống kê") ||
                    commandText.Contains("report"))
                {
                    var window = new ReportWindow();
                    window.Show();
                    return;
                }

                // === Bảo mật người dùng ===
                if (commandText.Contains("bao mat") ||
                    commandText.Contains("bảo mật") ||
                    commandText.Contains("user security") ||
                    commandText.Contains("security") ||
                    commandText.Contains("bao mat nguoi dung") ||
                    commandText.Contains("bảo mật người dùng"))
                {
                    var window = new UserSecurityWindow();
                    window.Show();
                    return;
                }

                // === Đăng xuất ===
                if (commandText.Contains("logout") ||
                    commandText.Contains("log out") ||
                    commandText.Contains("dang xuat") ||
                    commandText.Contains("đăng xuất") ||
                    commandText.Contains("logout"))
                {
                    var result = MessageBox.Show("Bạn có chắc chắn muốn đăng xuất?", "Xác nhận đăng xuất",
                        MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        LogoutHelper.PerformLogout();
                    }
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể thực hiện lệnh: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
