using QLKDPhongTro.Presentation.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace QLKDPhongTro.Presentation.Views.Windows
{
    public partial class ContractManagementWindow : Window
    {
        public ContractManagementWindow()
        {
            InitializeComponent();
            this.DataContext = new ContractManagementViewModel();
            
            // Set window properties
            this.MinHeight = 600;
            this.MinWidth = 800;
            this.Height = 700;
            this.Width = 1200;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            
            // Load contract data
            LoadContractData();
            
            // Add event handlers
            this.SizeChanged += ContractManagementWindow_SizeChanged;
        }

        private void LoadContractData()
        {
            // Load contract data logic here
            if (DataContext is ContractManagementViewModel viewModel)
            {
                viewModel.LoadContracts();
            }
        }

        private void ContractManagementWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Handle responsive layout updates
            UpdateContractCardsLayout();
        }

        private void UpdateContractCardsLayout()
        {
            // Update contract cards layout for responsive design
            var contractsView = FindName("ContractsView") as Grid;
            if (contractsView != null)
            {
                var itemsControl = FindVisualChild<ItemsControl>(contractsView);
                if (itemsControl?.ItemsPanel != null)
                {
                    var itemsPanelTemplate = new ItemsPanelTemplate();
                    var stackPanelFactory = new FrameworkElementFactory(typeof(StackPanel));
                    stackPanelFactory.SetValue(StackPanel.OrientationProperty, Orientation.Vertical);
                    stackPanelFactory.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
                    stackPanelFactory.SetValue(MarginProperty, new Thickness(0));
                    
                    itemsPanelTemplate.VisualTree = stackPanelFactory;
                    itemsControl.ItemsPanel = itemsPanelTemplate;
                }
            }
        }

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

        // Sidebar event handlers
        private void SidebarControl_MenuItemClicked(object sender, string menuItem)
        {
            switch (menuItem)
            {
                case "Dashboard":
                    var dashWindow = new DashWindow();
                    dashWindow.Show();
                    this.Close();
                    break;
                case "Rooms":
                    var roomWindow = new RoomWindow();
                    roomWindow.Show();
                    this.Close();
                    break;
                case "Tenants":
                    var tenantWindow = new TenantManagementWindow();
                    tenantWindow.Show();
                    this.Close();
                    break;
                case "Contracts":
                    // Already in contracts window
                    break;
                case "Reports":
                    // TODO: Navigate to reports
                    break;
                case "Settings":
                    // TODO: Navigate to settings
                    break;
            }
        }

        // Topbar event handlers
        private void TopbarControl_MenuButtonClicked(object sender, System.EventArgs e)
        {
            // Toggle sidebar visibility
            var sidebar = FindName("SidebarControl") as Grid;
            if (sidebar != null)
            {
                sidebar.Visibility = sidebar.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        private void TopbarControl_SearchTextChanged(object sender, string searchText)
        {
            if (DataContext is ContractManagementViewModel viewModel)
            {
                viewModel.SearchText = searchText;
                viewModel.FilterContracts();
            }
        }

        private void TopbarControl_SettingsButtonClicked(object sender, System.EventArgs e)
        {
            // TODO: Open settings dialog
            MessageBox.Show("Chức năng cài đặt đang được phát triển", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Contract item double-click handler
        private void ContractItem_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2) // Double-click
            {
                var border = sender as Border;
                if (border?.DataContext is ContractViewModel contract)
                {
                    // Open edit contract dialog
                    var editContractWindow = new EditContractWindow(contract);
                    if (editContractWindow.ShowDialog() == true)
                    {
                        LoadContractData(); // Refresh the list
                    }
                }
            }
        }

        private void TopbarControl_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
