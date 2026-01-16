using System;
using System.Windows;

namespace QLKDPhongTro.Presentation.Utils
{
    public static class NavigationHelper
    {
        public static void NavigateTo<T>(Window currentWindow) where T : Window, new()
        {
            var newWindow = new T();
            newWindow.Show();
            
            if (currentWindow != null)
            {
                currentWindow.Close();
            }
        }

        public static void NavigateTo(Window currentWindow, Window newWindow)
        {
            if (newWindow == null) throw new ArgumentNullException(nameof(newWindow));

            newWindow.Show();
            
            if (currentWindow != null)
            {
                currentWindow.Close();
            }
        }
    }
}
