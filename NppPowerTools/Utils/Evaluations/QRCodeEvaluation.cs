using CodingSeb.ExpressionEvaluator;
using QRCoder;
using System.Drawing;
using System.Text.RegularExpressions;

namespace NppPowerTools.Utils.Evaluations
{
    public class QRCodeEvaluation : IFunctionEvaluation
    {
        private static readonly Regex qrRegex = new Regex(@"^(qr|qrcode)(?<size>\d+)?$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public bool TryEvaluate(object sender, FunctionEvaluationEventArg e)
        {
            Match qrMatch = qrRegex.Match(e.Name);

            if (qrMatch.Success)
            {
                string text = string.Empty;

                if (e.Args.Count > 0)
                    text = e.EvaluateArg<string>(0);

                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrCodeData);

                int size = Config.Instance.QrCodeDefaultSize;
                Color darkColor = Config.Instance.QRCodeDarkColor;
                Color lightColor = Config.Instance.QRCodeLightColor;

                if (qrMatch.Groups["size"].Success)
                    size = int.Parse(qrMatch.Groups["size"].Value);

                Color GetColor(int i, Color defaultColor)
                {
                    Color temp = e.Args[i].ToColor();
                    if (temp.A == 0 && temp.R == 0 && temp.G == 0 && temp.B == 0)
                    {
                        object result = e.EvaluateArg(i);

                        if (result is Color color)
                            temp = color;
                        else
                            temp = e.EvaluateArg(i).ToString().ToColor();
                    }

                    if (temp.A == 0 && temp.R == 0 && temp.G == 0 && temp.B == 0)
                        return defaultColor;

                    return temp;
                }

                if (e.Args.Count > 1)
                    darkColor = GetColor(1, Config.Instance.QRCodeDarkColor);

                if(e.Args.Count > 2)
                    lightColor = GetColor(2, Config.Instance.QRCodeLightColor);

                e.Value = qrCode.GetGraphic(size, darkColor, lightColor, true);

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
