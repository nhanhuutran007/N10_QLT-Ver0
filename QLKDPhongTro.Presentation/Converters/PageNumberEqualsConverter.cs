using System;
using System.Globalization;
using System.Windows.Data;

namespace QLKDPhongTro.Presentation.Converters
{
    /// <summary>
    /// Converter này so sánh hai giá trị (CurrentPage từ VM và PageNumber từ Button).
    /// Trả về True nếu chúng bằng nhau, False nếu không.
    /// </summary>
    public class PageNumberEqualsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // value: Là giá trị từ Binding chính (DataContext.CurrentPage)
            // parameter: Là giá trị từ ConverterParameter (Page number của Button)

            // Chuyển đổi cả hai giá trị sang string để so sánh an toàn
            string? currentPage = value?.ToString();
            string? buttonPage = parameter?.ToString();

            return currentPage == buttonPage;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}