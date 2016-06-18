namespace XamlPowerToys.UI.Converters {
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public class LabelPositionEnumConverter : IValueConverter {

        public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture) {
            return ((XamlPowerToys.Model.LabelPosition)value).ToString();
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture) {
            return Enum.Parse(typeof(XamlPowerToys.Model.LabelPosition), value.ToString());
        }

    }
}
