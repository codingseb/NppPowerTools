using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace NppPowerTools
{
    public class BoolToStringConverter: MarkupExtension, IValueConverter
    {
        public string FalseValue { get; set; } = "false";
        public string TrueValue { get; set; } = "true";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString().Equals(TrueValue);
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
