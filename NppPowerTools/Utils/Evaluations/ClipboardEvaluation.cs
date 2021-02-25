using CodingSeb.ExpressionEvaluator;
using System;
using System.Drawing;
using System.Windows;

namespace NppPowerTools.Utils.Evaluations
{
    public sealed class ClipboardEvaluation : IVariableEvaluation, IFunctionEvaluation
    {
        public bool TryEvaluate(object sender, VariableEvaluationEventArg e)
        {
            if (e.Name.Equals("clipboard", StringComparison.OrdinalIgnoreCase) || e.Name.Equals("cb", StringComparison.OrdinalIgnoreCase))
            {
                e.Value = Clipboard.ContainsImage() ? Clipboard.GetImage().GetBitmap() : (object)Clipboard.GetText();
            }

            return e.HasValue;
        }

        public bool TryEvaluate(object sender, FunctionEvaluationEventArg e)
        {
            if (e.Name.Equals("clipboard", StringComparison.OrdinalIgnoreCase) || e.Name.Equals("cb", StringComparison.OrdinalIgnoreCase))
            {
                if (e.Args.Count > 0)
                {
                    object arg = e.EvaluateArg(0);

                    if (arg is Bitmap bitmap)
                        Clipboard.SetDataObject(bitmap);
                    else
                        Clipboard.SetText(arg.ToString());

                    e.Value = arg;
                }
            }

            return e.FunctionReturnedValue;
        }

        #region singleton
        private static ClipboardEvaluation instance = null;

        public static ClipboardEvaluation Instance
        {
            get
            {
                return instance ??= new ClipboardEvaluation();
            }
        }

        private ClipboardEvaluation()
        { }
        #endregion

    }
}
