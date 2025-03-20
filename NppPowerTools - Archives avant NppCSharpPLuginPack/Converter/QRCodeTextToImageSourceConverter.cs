using NppPowerTools.Utils;
using QRCoder;
using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace NppPowerTools
{
    public class QRCodeTextToImageSourceConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string text = value.ToString();

            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);

            int size = Config.Instance.QrCodeDefaultSize;
            Color darkColor = Config.Instance.QRCodeDarkColor;
            Color lightColor = Config.Instance.QRCodeLightColor;

            return qrCode.GetGraphic(Math.Max(Math.Min(size, 500),1), darkColor, lightColor, true).BitmapToImageSource();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
