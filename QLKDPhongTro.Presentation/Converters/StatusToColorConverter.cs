using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace QLKDPhongTro.Presentation.Converters
{
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var status = value?.ToString();
            return status switch
            {
                "Đặt trước" => new SolidColorBrush(Color.FromRgb(40, 167, 69)), // Green
                "Đang cho thuê" => new SolidColorBrush(Color.FromRgb(255, 193, 7)), // Yellow
                "Đã hủy" => new SolidColorBrush(Color.FromRgb(220, 53, 69)), // Red
                _ => new SolidColorBrush(Color.FromRgb(108, 117, 125)) // Gray (default)
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}