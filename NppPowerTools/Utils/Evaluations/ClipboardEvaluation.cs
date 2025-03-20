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
                if (e.This != null)
                {
                    if (e.This is Bitmap bitmap)
                        Clipboard.SetDataObject(bitmap);
                    else
                        Clipboard.SetText(e.This?.ToString() ?? string.Empty);

                    e.Value = e.This;
                }
                else
                {
                    e.Value = Clipboard.ContainsImage() ? Clipboard.GetImage().GetBitmap() : (object)Clipboard.GetText();
                }
            }

            return e.HasValue;
        }

        public bool TryEvaluate(object sender, FunctionEvaluationEventArg e)
        {
            if (e.Name.Equals("clipboard", StringComparison.OrdinalIgnoreCase) || e.Name.Equals("cb", StringComparison.OrdinalIgnoreCase))
            {
                object arg =  null;

                if (e.This != null)
                    arg = e.This;
                if (e.Args.Count > 0)
                    arg = e.EvaluateArg(0);

                if (arg is Bitmap bitmap)
                    Clipboard.SetDataObject(bitmap);
                else
                    Clipboard.SetText(arg?.ToString() ?? string.Empty);

                e.Value = arg;
            }

            return e.FunctionReturnedValue;
        }

        #region singleton
        private static ClipboardEvaluation instance;

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
