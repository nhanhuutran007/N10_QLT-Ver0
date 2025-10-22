using QLKDPhongTro.BusinessLayer.DTOs;
using QLKDPhongTro.Presentation.ViewModels;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace QLKDPhongTro.Presentation.Views.Windows
{
    public partial class RoomWindow : Window
    {
        private bool _isHousesView = true;
        private DateTime _lastClickTime;
        private object _lastClickedItem;

        public RoomWindow()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.MinHeight = 600;
            this.MinWidth = 800;
            
            // Khởi tạo DataContext với ViewModel
            this.DataContext = new RentedRoomViewModel();
            
            // Load dữ liệu từ database
            LoadRoomData();
            
            // Thêm event handler cho việc thay đổi kích thước cửa sổ
            this.SizeChanged += RoomWindow_SizeChanged;
        }

        // Handler cho TopbarControl events - giờ đây TopbarControl tự xử lý logic chung
        private void TopbarControl_MenuButtonClicked(object sender, EventArgs e)
        {
            // Logic chung đã được chuyển vào TopbarControl.xaml.cs
            // Chỉ xử lý logic riêng của RoomWindow nếu cần
        }

        private void TopbarControl_SearchTextChanged(object sender, string searchText)
        {
            // Logic search chung đã được chuyển vào TopbarControl.xaml.cs
            // Chỉ xử lý search logic riêng của RoomWindow nếu cần
            if (DataContext is RentedRoomViewModel viewModel)
            {
                // TODO: Implement search logic
                System.Diagnostics.Debug.WriteLine($"Searching for: {searchText}");
            }
        }

        private void TopbarControl_SettingsButtonClicked(object sender, EventArgs e)
        {
            // Logic settings chung đã được chuyển vào TopbarControl.xaml.cs
            // Chỉ xử lý settings logic riêng của RoomWindow nếu cần
        }

        // Handler cho SidebarControl events - giờ đây SidebarControl tự xử lý navigation
        private void SidebarControl_MenuItemClicked(object sender, string menuItem)
        {
            // Logic navigation đã được chuyển vào SidebarControl.xaml.cs
            // Chỉ xử lý logic riêng của RoomWindow nếu cần
        }

        // Handler cho RadioButton tab switching
        private void Tab_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton radioButton)
            {
                if (radioButton.Content.ToString() == "Nhà ở")
                {
                    _isHousesView = true;
                    // TODO: Implement houses view logic
                    System.Diagnostics.Debug.WriteLine("Switched to Houses view");
                }
                else if (radioButton.Content.ToString() == "Phòng ở")
                {
                    _isHousesView = false;
                    // TODO: Implement rooms view logic
                    System.Diagnostics.Debug.WriteLine("Switched to Rooms view");
                }
            }
        }

        // Load dữ liệu từ database
        private void LoadRoomData()
        {
            if (DataContext is RentedRoomViewModel viewModel)
            {
                // Load rooms data từ database
                viewModel.LoadRoomsCommand?.Execute(null);
            }
        }

        // Event handler cho RoomCard click
        private void RoomCard_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is RentedRoomDto room)
            {
                if (DataContext is RentedRoomViewModel viewModel)
                {
                    if (room == null)
                    {
                        MessageBox.Show("Không thể chọn phòng: Dữ liệu phòng không hợp lệ.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    viewModel.SelectedRoom = room;

                    // Handle double click to view room details
                    var currentTime = DateTime.Now;
                    if (_lastClickedItem == room && (currentTime - _lastClickTime).TotalMilliseconds < 500)
                    {
                        viewModel.ShowRoomDetailsWindow();
                        _lastClickedItem = null;
                        _lastClickTime = DateTime.MinValue;
                    }
                    else
                    {
                        _lastClickedItem = room;
                        _lastClickTime = currentTime;
                    }
                }
                else
                {
                    MessageBox.Show("Không thể truy cập ViewModel.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Không thể chọn phòng: Dữ liệu không hợp lệ.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Event handler cho Button click (thêm phòng)
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is RentedRoomViewModel viewModel)
            {
                // Trigger add room command
                viewModel.ShowAddRoomPanelCommand?.Execute(null);
            }
        }

        // Event handler cho SizeChanged
        private void RoomWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Cập nhật layout khi kích thước cửa sổ thay đổi
            UpdateRoomCardsLayout();
        }

        // Cập nhật layout của room cards
        private void UpdateRoomCardsLayout()
        {
            // Tìm ItemsControl trong XAML
            var roomsView = FindName("RoomsView") as Grid;
            if (roomsView != null)
            {
                var itemsControl = FindVisualChild<ItemsControl>(roomsView);
                if (itemsControl?.ItemsPanel != null)
                {
                    // Sử dụng WrapPanel để tự động wrap các card
                    var itemsPanelTemplate = new ItemsPanelTemplate();
                    var wrapPanelFactory = new FrameworkElementFactory(typeof(WrapPanel));
                    wrapPanelFactory.SetValue(WrapPanel.OrientationProperty, Orientation.Horizontal);
                    wrapPanelFactory.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
                    wrapPanelFactory.SetValue(MarginProperty, new Thickness(0));
                    
                    itemsPanelTemplate.VisualTree = wrapPanelFactory;
                    itemsControl.ItemsPanel = itemsPanelTemplate;
                }
            }
        }

        // Helper method để tìm child control
        private T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T result)
                    return result;
                
                var childOfChild = FindVisualChild<T>(child);
                if (childOfChild != null)
                    return childOfChild;
            }
            return null;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}

