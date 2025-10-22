using System;
using System.Globalization;
using System.Windows.Data;

namespace QLKDPhongTro.Presentation.Converters
{
    public class PercentageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal decimalValue)
            {
                return decimalValue.ToString("N1") + "%";
            }
            if (value is double doubleValue)
            {
                return doubleValue.ToString("N1") + "%";
            }
            if (value is float floatValue)
            {
                return floatValue.ToString("N1") + "%";
            }
            return "0%";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}