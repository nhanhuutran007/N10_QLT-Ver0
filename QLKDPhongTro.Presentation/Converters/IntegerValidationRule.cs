using System.Globalization;
using System.Windows.Controls;

namespace QLKDPhongTro.Presentation.Converters
{
    public class IntegerValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (string.IsNullOrEmpty(value?.ToString()))
                return new ValidationResult(false, "Vui lòng nhập số nguyên.");
            if (!int.TryParse(value.ToString(), out int result) || result <= 0)
                return new ValidationResult(false, "Vui lòng nhập số nguyên dương.");
            return ValidationResult.ValidResult;
        }
    }
}