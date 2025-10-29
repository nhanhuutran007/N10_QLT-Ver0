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
                // Đổi tên biến intValue thành intVal để tránh trùng lặp
                if (value is int intVal && intVal == 0 || value == null)
                    return Visibility.Collapsed;
                return Visibility.Visible;
            }

            // Biến khác tên
            if (value is int intVal2 && intVal2 == 0 || value == null)
                return Visibility.Visible;
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
