using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace QLKDPhongTro.Presentation.Converters
{
    public class PaymentStatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string status)
            {
                return status.ToLower() switch
                {
                    "đã thanh toán" or "paid" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#10D096")),
                    "chưa thanh toán" or "unpaid" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF6B6B")),
                    "quá hạn" or "overdue" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFA500")),
                    "đang xử lý" or "processing" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3B82F6")),
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