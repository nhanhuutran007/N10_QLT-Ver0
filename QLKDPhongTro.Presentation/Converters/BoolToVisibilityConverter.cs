using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace QLKDPhongTro.Presentation.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                // If parameter is "Invert", invert the boolean value
                if (parameter?.ToString() == "Invert")
                {
                    boolValue = !boolValue;
                }
                
                return boolValue ? Visibility.Visible : Visibility.Collapsed;
            }
            
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
            {
                bool result = visibility == Visibility.Visible;
                
                // If parameter is "Invert", invert the boolean value
                if (parameter?.ToString() == "Invert")
                {
                    result = !result;
                }
                
                return result;
            }
            
            return false;
        }
    }
}
