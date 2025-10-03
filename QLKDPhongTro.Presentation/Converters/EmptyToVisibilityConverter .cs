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
            if (value is int count)
            {
                // Nếu có parameter "inverse" thì đảo ngược logic
                bool isInverse = parameter?.ToString()?.ToLower() == "inverse";

                if (isInverse)
                {
                    // Hiển thị khi có phần tử (count > 0)
                    return count > 0 ? Visibility.Visible : Visibility.Collapsed;
                }
                else
                {
                    // Hiển thị khi không có phần tử (count == 0)
                    return count == 0 ? Visibility.Visible : Visibility.Collapsed;
                }
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}