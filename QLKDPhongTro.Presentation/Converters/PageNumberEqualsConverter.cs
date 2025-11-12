using System;
using System.Globalization;
using System.Windows.Data;

namespace QLKDPhongTro.Presentation.Converters
{
    /// <summary>
    /// Converter để so sánh CurrentPage với PageNumber trong pagination
    /// </summary>
    public class PageNumberEqualsConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 2)
                return false;

            if (values[0] is int currentPage && values[1] is int pageNumber)
            {
                return currentPage == pageNumber;
            }

            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

