using System;
using System.Globalization;
using System.Windows.Data;

namespace QLKDPhongTro.Presentation.Converters
{
    public class IntToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString() ?? string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrEmpty(value?.ToString()))
                return 0; // Default value for empty input
            if (int.TryParse(value.ToString(), out int result))
                return result;
            return 0; // Fallback if conversion fails
        }
    }
}