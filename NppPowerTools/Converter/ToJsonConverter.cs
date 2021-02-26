using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace NppPowerTools
{
    public class ToJsonConverter : MarkupExtension, IValueConverter
    {
        public bool ShowBaseType { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return (ShowBaseType ? value.GetType().Name + " : " : string.Empty) + JsonConvert.SerializeObject(value);
            }
            catch
            {
                return "- Not Json Serializable -";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
