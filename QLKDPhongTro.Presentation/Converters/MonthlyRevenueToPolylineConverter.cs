using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using QLKDPhongTro.BusinessLayer.DTOs;

namespace QLKDPhongTro.Presentation.Converters
{
    // Converts a list of MonthlyStatsDto into a PointCollection for a Polyline (fixed base size)
    public class MonthlyRevenueToPolylineConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var data = value as IEnumerable<MonthlyStatsDto>;
            if (data == null) return new PointCollection();

            var list = data.ToList();
            if (list.Count == 0) return new PointCollection();

            // Fixed base drawing size; will be scaled by Viewbox in XAML
            double width = 600;
            double height = 240;
            double padding = 24;
            double usableW = Math.Max(0, width - padding * 2);
            double usableH = Math.Max(0, height - padding * 2);
            int n = list.Count;
            double stepX = n > 1 ? usableW / (n - 1) : 0;

            decimal maxValDec = list.Max(m => m.ThuNhap);
            double maxVal = (double)maxValDec;
            if (maxVal <= 0) maxVal = 1;

            var points = new PointCollection();
            for (int i = 0; i < n; i++)
            {
                double x = padding + i * stepX;
                double yRatio = (double)list[i].ThuNhap / maxVal;
                double y = padding + (1 - yRatio) * usableH;
                points.Add(new Point(x, y));
            }
            return points;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
