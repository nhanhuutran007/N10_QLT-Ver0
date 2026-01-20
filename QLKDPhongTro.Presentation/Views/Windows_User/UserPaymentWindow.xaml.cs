using QLKDPhongTro.Presentation.Utils;
using System.Windows;

namespace QLKDPhongTro.Presentation.Views.Windows_User
{
    /// <summary>
    /// Interaction logic for UserPaymentWindow.xaml
    /// </summary>
    public partial class UserPaymentWindow : Window
    {
        public UserPaymentWindow()
        {
            InitializeComponent();
            Loaded += UserPaymentWindow_Loaded;
        }

        private void UserPaymentWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Initialize sidebar with current window reference
            if (SidebarControl != null)
            {
                SidebarControl.SetCurrentWindow(this);
            }
        }
    }
}
