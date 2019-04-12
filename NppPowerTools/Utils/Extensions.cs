using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Media.Imaging;

namespace NppPowerTools.Utils
{
    public static class Extensions
    {
        private static Regex hexColorRegex = new Regex("^[#][0-9a-f]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static BitmapImage BitmapToImageSource(this Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        public static Color ToColor(this string sColor)
        {
            string tmp = sColor.Trim();

            if (hexColorRegex.IsMatch(tmp))
            {
                tmp = tmp.TrimStart('#').PadLeft(6, '0').PadLeft(8, 'F');

                return Color.FromArgb(int.Parse(tmp, System.Globalization.NumberStyles.HexNumber));
            }
            else
                return Color.FromName(tmp);
        }

        public static string ToStringOutput(this object result)
        {
            string sResult = result?.ToString() ?? Config.Instance.TextWhenResultIsNull;
            return CustomEvaluations.Print + sResult;
        }

        public static bool TabEquals(this string tab, string compareText)
        {
            return tab.Equals(compareText) || Path.GetFileName(tab).Equals(compareText);
        }
    }
}
