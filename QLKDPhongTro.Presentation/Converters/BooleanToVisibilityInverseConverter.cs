using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace QLKDPhongTro.Presentation.Converters
{
    /// <summary>
    /// Converts a boolean value to Visibility, inverting the logic.
    /// True -> Collapsed, False -> Visible
    /// </summary>
    public class BooleanToVisibilityInverseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                // Inverted: true becomes Collapsed, false becomes Visible
                return boolValue ? Visibility.Collapsed : Visibility.Visible;
            }
            
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
            {
                // Inverted: Visible becomes false, Collapsed becomes true
                return visibility != Visibility.Visible;
            }
            
            return true;
        }
    }
}
