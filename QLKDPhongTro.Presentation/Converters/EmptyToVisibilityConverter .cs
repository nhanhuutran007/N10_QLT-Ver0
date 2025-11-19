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
            bool isEmpty = value switch
            {
                null => true,
                int intVal => intVal == 0,
                string strVal => string.IsNullOrWhiteSpace(strVal),
                _ => false
            };

            var isInverse = string.Equals(parameter?.ToString(), "inverse", StringComparison.OrdinalIgnoreCase);
            if (isInverse)
            {
                return isEmpty ? Visibility.Collapsed : Visibility.Visible;
            }

            return isEmpty ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
