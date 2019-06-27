using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media.Imaging;

namespace NppPowerTools.Utils
{
    public static class Extensions
    {
        private static readonly Regex hexColorRegex = new Regex("^[#][0-9a-f]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static BitmapImage BitmapToImageSource(this Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        public static Bitmap GetBitmap(this BitmapSource source)
        {
            Bitmap bmp = new Bitmap(
              source.PixelWidth,
              source.PixelHeight,
              PixelFormat.Format32bppPArgb);
            BitmapData data = bmp.LockBits(
              new Rectangle(System.Drawing.Point.Empty, bmp.Size),
              ImageLockMode.WriteOnly,
              PixelFormat.Format32bppPArgb);
            source.CopyPixels(
              Int32Rect.Empty,
              data.Scan0,
              data.Height * data.Stride,
              data.Stride);
            bmp.UnlockBits(data);
            return bmp;
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
            {
                return Color.FromName(tmp);
            }
        }

        public static string GetNameOrHexString(this System.Windows.Media.Color color)
        {
            string result = "";

            // Récupération de toutes les couleurs connue par réflection
            Type ColorType = typeof(System.Windows.Media.Colors);
            PropertyInfo[] arrPiColors = ColorType.GetProperties(BindingFlags.Public | BindingFlags.Static);

            try
            {
                result = arrPiColors.ToList().Find(pi => ((Color)pi.GetValue(null, null)).Equals(color)).Name;
            }
            catch { }

            // Si c'est pas une couleur connue alors on donne la version hexa
            return ("#" + color.A.ToString("x2") + color.R.ToString("x2") + color.G.ToString("x2") + color.B.ToString("x2")).ToUpper();
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
