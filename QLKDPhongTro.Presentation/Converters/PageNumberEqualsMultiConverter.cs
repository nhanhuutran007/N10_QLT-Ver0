using System;
using System.Globalization;
using System.Windows.Data;

namespace QLKDPhongTro.Presentation.Converters
{
    /// <summary>
    /// So sánh hai giá trị (CurrentPage từ VM và PageNumber của Button).
    /// Trả về True nếu chúng bằng nhau.
    /// </summary>
    public class PageNumberEqualsMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (values == null || values.Length < 2)
                    return false;

                // values[0] là CurrentPage (từ ViewModel)
                // values[1] là DataContext của Button (số trang của nút đó)
                if (values[0] == null || values[1] == null)
                    return false;

                string val1 = values[0].ToString() ?? string.Empty;
                string val2 = values[1].ToString() ?? string.Empty;

                return val1.Equals(val2);
            }
            catch
            {
                return false;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}