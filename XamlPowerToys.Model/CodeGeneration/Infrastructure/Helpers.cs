namespace XamlPowerToys.Model.CodeGeneration.Infrastructure {
    using System;
    using System.Globalization;
    using System.Windows;

    public static class Helpers {

        public static String ConstructBinding(String path, BindingMode bindingMode, String stringFormatText, IncludeValidationAttributes includeValidationAttributes = IncludeValidationAttributes.No) {
            var validationAttributes = String.Empty;
            if (includeValidationAttributes == IncludeValidationAttributes.Yes) {
                validationAttributes = ", ValidatesOnDataErrors=True, ValidatesOnNotifyDataErrors=True, ValidatesOnExceptions=True";
            }
            return $"{{Binding Path={path}, Mode={bindingMode}{stringFormatText}{validationAttributes}}}";
        }

        public static String ParseGridLength(GridLength gridLength) {
            if (gridLength.IsAbsolute) {
                return gridLength.Value.ToString(CultureInfo.InvariantCulture);
            }
            if (gridLength.IsAuto) {
                return "Auto";
            }
            var temp = gridLength.ToString().Replace("Star", "*").Replace(".*", "*");
            if (temp.StartsWith("0")) {
                return temp.Substring(1);
            }

            return temp;
        }

    }
}
