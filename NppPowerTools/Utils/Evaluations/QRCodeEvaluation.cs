using CodingSeb.ExpressionEvaluator;
using QRCoder;
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

                if (qrMatch.Groups["size"].Success)
                    size = int.Parse(qrMatch.Groups["size"].Value);

                e.Value = qrCode.GetGraphic(size).BitmapToImageSource();

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
