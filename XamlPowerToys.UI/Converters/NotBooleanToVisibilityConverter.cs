namespace XamlPowerToys.UI.Converters {
    using System;
    using System.Windows;
    using System.Windows.Data;

    public class NotBooleanToVisibilityConverter : IValueConverter {

        public Object Convert(Object value, Type targetType, Object parameter, System.Globalization.CultureInfo culture) {
            if (value == null) {
                return Visibility.Visible;
            }

            Boolean testValue;
            if (Boolean.TryParse(value.ToString(), out testValue)) {
                return testValue ? Visibility.Collapsed : Visibility.Visible;
            }
            return Visibility.Visible;
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, System.Globalization.CultureInfo culture) {
            throw new NotSupportedException();
        }

    }
}
