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
            if (value is string status)
            {
                return status.ToLower() switch
                {
                    "trống" or "available" => new SolidColorBrush(Colors.Green),
                    "đã thuê" or "rented" => new SolidColorBrush(Colors.Orange),
                    "bảo trì" or "maintenance" => new SolidColorBrush(Colors.Red),
                    _ => new SolidColorBrush(Colors.Gray)
                };
            }
            return new SolidColorBrush(Colors.Gray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}