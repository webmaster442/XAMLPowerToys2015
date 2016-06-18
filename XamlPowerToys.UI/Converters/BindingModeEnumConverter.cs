namespace XamlPowerToys.UI.Converters {
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public class BindingModeEnumConverter : IValueConverter {

        public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture) {
            return ((XamlPowerToys.Model.BindingMode)value).ToString();
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture) {
            // the null check is required.  when the data template selector is being changed dynamically,
            //   this method gets called with the value set to null.
            return value == null ? null : Enum.Parse(typeof(XamlPowerToys.Model.BindingMode), value.ToString());
        }

    }
}
