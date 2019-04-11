using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace NppPowerTools
{
    public class DrawingColorToWPFBrush : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is System.Drawing.Color dColor)
                return new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(dColor.A, dColor.R, dColor.G, dColor.B));
            else
                return System.Windows.Media.Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is System.Windows.Media.SolidColorBrush mColor)
                return  System.Drawing.Color.FromArgb(mColor.Color.A, mColor.Color.R, mColor.Color.G, mColor.Color.B);
            else
                return System.Drawing.Color.Black;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
