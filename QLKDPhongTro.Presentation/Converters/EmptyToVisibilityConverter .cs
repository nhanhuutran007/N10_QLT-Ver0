using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace QLKDPhongTro.Presentation.Converters
{
    public class EmptyToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter?.ToString() == "inverse")
            {
                return value == null || (value is int intValue && intValue == 0) ? Visibility.Collapsed : Visibility.Visible;
            }
            return value == null || (value is int intValue && intValue == 0) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}