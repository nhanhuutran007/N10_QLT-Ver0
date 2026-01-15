using System;
using System.Globalization;
using System.Windows.Data;

namespace QLKDPhongTro.Presentation.Converters
{
    // IMultiValueConverter: [0] = current value (decimal), [1] = max value (decimal)
    // ConverterParameter = max pixel height (double), default 180
    public class BarHeightConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (values == null || values.Length < 2)
                    return 0d;

                var cur = System.Convert.ToDecimal(values[0]);
                var max = System.Convert.ToDecimal(values[1]);
                double maxPx = 180;
                if (parameter != null && double.TryParse(parameter.ToString(), out double p))
                    maxPx = p;

                if (max <= 0) return 0d;
                var ratio = (double)(cur / max);
                if (ratio < 0) ratio = 0;
                if (ratio > 1) ratio = 1;
                return ratio * maxPx;
            }
            catch
            {
                return 0d;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
