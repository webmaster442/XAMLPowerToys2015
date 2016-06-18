namespace XamlPowerToys.UI.Converters {
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    public class BooleanToBoldFontWeightConverter : IValueConverter {

        public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture) {
            if (value is Boolean) {
                if ((Boolean)value) {
                    return FontWeights.Bold;
                }
            }
            return FontWeights.Normal;
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }

    }
}
