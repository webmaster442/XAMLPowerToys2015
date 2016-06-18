namespace XamlPowerToys.UI.ValidationRules {
    using System;
    using System.Globalization;
    using System.Windows.Controls;

    public class PositiveDoubleValidationRule : ValidationRule {

        public override ValidationResult Validate(Object value, CultureInfo cultureInfo) {
            if (value == null || String.IsNullOrWhiteSpace(value.ToString())) {
                return ValidationResult.ValidResult;
            }

            Double test;

            if (!Double.TryParse(value.ToString(), out test) || test < 0) {
                return new ValidationResult(false, "Value must be greater or equal to 0.");
            }

            return ValidationResult.ValidResult;
        }

    }
}
