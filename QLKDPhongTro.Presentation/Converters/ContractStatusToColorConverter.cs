using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace QLKDPhongTro.Presentation.Converters
{
    public class ContractStatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string status)
            {
                return status.ToLower() switch
                {
                    "hiệu lực" or "active" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#10D096")),
                    "hết hạn" or "expired" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF6B6B")),
                    "sắp hết hạn" or "expiring" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFA500")),
                    "tạm ngưng" or "suspended" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6B7280")),
                    "đang đàm phán" or "negotiating" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3B82F6")),
                    _ => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6B7280"))
                };
            }
            return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6B7280"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}