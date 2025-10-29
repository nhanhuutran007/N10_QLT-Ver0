using System;

namespace QLKDPhongTro.DataLayer.Utils
{
    /// <summary>
    /// Các hàm tiện ích tính toán tài chính
    /// </summary>
    public static class FinancialCalculator
    {
        /// <summary>
        /// Tính số tháng nợ dựa trên tháng năm
        /// </summary>
        public static int CalculateMonthsOverdue(string thangNam)
        {
            if (string.IsNullOrEmpty(thangNam)) return 0;

            var parts = thangNam.Split('/');
            if (parts.Length != 2) return 0;

            if (int.TryParse(parts[0], out int month) && int.TryParse(parts[1], out int year))
            {
                var paymentDate = new DateTime(year, month, 1);
                var currentDate = DateTime.Now;
                var monthsDifference = (currentDate.Year - paymentDate.Year) * 12 + currentDate.Month - paymentDate.Month;
                return Math.Max(0, monthsDifference);
            }

            return 0;
        }

        /// <summary>
        /// Tính tổng tiền từ các khoản chi phí
        /// </summary>
        public static decimal CalculateTotalAmount(decimal? tienThue, decimal? tienDien, decimal? tienNuoc,
                                                  decimal? tienInternet, decimal? tienVeSinh, decimal? tienGiuXe,
                                                  decimal? chiPhiKhac)
        {
            return (tienThue ?? 0) + (tienDien ?? 0) + (tienNuoc ?? 0) +
                   (tienInternet ?? 0) + (tienVeSinh ?? 0) + (tienGiuXe ?? 0) +
                   (chiPhiKhac ?? 0);
        }

        /// <summary>
        /// Tính tỷ lệ tăng trưởng
        /// </summary>
        public static decimal CalculateGrowthRate(decimal currentValue, decimal previousValue)
        {
            if (previousValue == 0) return 0;
            return ((currentValue - previousValue) / previousValue) * 100;
        }

        /// <summary>
        /// Kiểm tra định dạng tháng năm
        /// </summary>
        public static bool IsValidMonthYear(string thangNam)
        {
            if (string.IsNullOrEmpty(thangNam) || thangNam.Length != 7) return false;

            var parts = thangNam.Split('/');
            if (parts.Length != 2) return false;

            return int.TryParse(parts[0], out int month) && month >= 1 && month <= 12 &&
                   int.TryParse(parts[1], out int year) && year >= 2000 && year <= 2100;
        }
    }
}