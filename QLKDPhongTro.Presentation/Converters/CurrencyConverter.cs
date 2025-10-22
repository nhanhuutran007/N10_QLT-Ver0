using System;
using System.Globalization;
using System.Windows.Data;

namespace QLKDPhongTro.Presentation.Converters
{
    public class CurrencyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal decimalValue)
            {
                return decimalValue.ToString("N0") + " VNĐ";
            }
            if (value is double doubleValue)
            {
                return ((decimal)doubleValue).ToString("N0") + " VNĐ";
            }
            if (value is int intValue)
            {
                return intValue.ToString("N0") + " VNĐ";
            }
            return "0 VNĐ";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}