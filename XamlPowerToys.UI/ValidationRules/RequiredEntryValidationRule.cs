namespace XamlPowerToys.UI.ValidationRules {
    using System;
    using System.Globalization;
    using System.Windows.Controls;

    public class RequiredEntryValidationRule : ValidationRule {

        public override ValidationResult Validate(Object value, CultureInfo cultureInfo) {
            if (String.IsNullOrWhiteSpace(value?.ToString())) {
                return new ValidationResult(false, "Required entry.");
            }
            return ValidationResult.ValidResult;
        }

    }
}
