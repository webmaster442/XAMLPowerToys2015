namespace XamlPowerToys.UI.ValidationRules {
    using System;
    using System.Globalization;
    using System.Windows.Controls;

    public class PositiveIntegerValidationRule : ValidationRule {

        public override ValidationResult Validate(Object value, CultureInfo cultureInfo) {
            if (value == null || String.IsNullOrWhiteSpace(value.ToString())) {
                return ValidationResult.ValidResult;
            }

            Int32 test;

            if (!Int32.TryParse(value.ToString(), out test) || test < 0) {
                return new ValidationResult(false, "Value must be a positive integer.");
            }

            return ValidationResult.ValidResult;
        }

    }
}
