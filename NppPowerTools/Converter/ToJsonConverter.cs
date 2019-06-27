using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace NppPowerTools
{
    public class ToJsonConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return JsonConvert.SerializeObject(value);
            }
            catch
            {
                return "- Not Json Serializable -";
            }
        }

#pragma warning disable RCS1079 // Throwing of new NotImplementedException.
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
        #pragma warning restore RCS1079 // Throwing of new NotImplementedException.

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
