namespace XamlPowerToys.UI.Converters {
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public class EnumToBooleanConverter : IValueConverter {

        /// <summary>
        /// Compares the bound value with an enum param. Returns true when they match.
        /// </summary>
        public Object Convert(Object value, Type targetType, Object param, CultureInfo culture) {
            return value.Equals(param);
        }

        /// <summary>
        /// Updates the bound value if it's different from the parameter.
        /// </summary>
        public Object ConvertBack(Object value, Type targetType, Object param, CultureInfo culture) {
            return (bool)value ? param : Binding.DoNothing;
        }

    }
}
