using System;
using System.Reflection;
using System.Windows;
using QLKDPhongTro.Presentation.ViewModels;
using QLKDPhongTro.Presentation.Utils;
using QLKDPhongTro.Presentation.Views.Components;

namespace QLKDPhongTro.Presentation.Views.Windows_User
{
    public partial class UserHousingWindow : Window
    {
        private bool _isClosing = false;

        public UserHousingWindow()
        {
            InitializeComponent();

            Loaded += UserHousingWindow_Loaded;
            Closing += UserHousingWindow_Closing;
            Closed += UserHousingWindow_Closed;
        }

        private void UserHousingWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _isClosing = true;
        }

        private void UserHousingWindow_Closed(object? sender, EventArgs e)
        {
            WindowHelper.CheckAndShutdownIfNoWindows(this);
        }

        private void UserHousingWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Update sidebar selection
            SidebarControl.UpdateMenuSelection();
        }
    }
}
