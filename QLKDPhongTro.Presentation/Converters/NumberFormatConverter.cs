using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace QLKDPhongTro.Presentation.Converters
{
    /// <summary>
    /// Converter để format số tiền với dấu phẩy khi hiển thị và chuyển đổi ngược lại khi nhập
    /// </summary>
    public class NumberFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return string.Empty;

            // Chuyển đổi từ số sang chuỗi có format
            if (value is decimal decimalValue)
            {
                return decimalValue.ToString("N0", CultureInfo.InvariantCulture);
            }
            if (value is double doubleValue)
            {
                return ((decimal)doubleValue).ToString("N0", CultureInfo.InvariantCulture);
            }
            if (value is int intValue)
            {
                return intValue.ToString("N0", CultureInfo.InvariantCulture);
            }
            if (value is long longValue)
            {
                return longValue.ToString("N0", CultureInfo.InvariantCulture);
            }
            if (value is string stringValue)
            {
                // Nếu là chuỗi, thử parse và format lại
                if (decimal.TryParse(stringValue.Replace(",", ""), NumberStyles.Number, CultureInfo.InvariantCulture, out decimal parsed))
                {
                    return parsed.ToString("N0", CultureInfo.InvariantCulture);
                }
                return stringValue;
            }

            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return 0m;

            // Loại bỏ dấu phẩy và parse về số
            string cleanValue = value.ToString().Replace(",", "").Trim();
            
            if (targetType == typeof(decimal) || targetType == typeof(decimal?))
            {
                if (decimal.TryParse(cleanValue, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal result))
                    return result;
                return 0m;
            }
            if (targetType == typeof(double) || targetType == typeof(double?))
            {
                if (double.TryParse(cleanValue, NumberStyles.Number, CultureInfo.InvariantCulture, out double result))
                    return result;
                return 0.0;
            }
            if (targetType == typeof(int) || targetType == typeof(int?))
            {
                if (int.TryParse(cleanValue, NumberStyles.Number, CultureInfo.InvariantCulture, out int result))
                    return result;
                return 0;
            }
            if (targetType == typeof(long) || targetType == typeof(long?))
            {
                if (long.TryParse(cleanValue, NumberStyles.Number, CultureInfo.InvariantCulture, out long result))
                    return result;
                return 0L;
            }
            if (targetType == typeof(string))
            {
                return cleanValue;
            }

            return 0m;
        }
    }
}

