using CodingSeb.ExpressionEvaluator;
using NppPowerTools.Utils.Evaluations;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;

namespace NppPowerTools.Utils
{
    internal static class CustomEvaluations
    {
        private static readonly Random random = new Random();

        private static readonly List<IVariableEvaluation> variablesEvaluations = new List<IVariableEvaluation>
        {
            new LoremIspumEvaluation(),
            NppAccessEvaluation.Instance,
            new JsonEvaluation(),
            new StringJoinEvaluation(),
            ExcelEvaluation.Instance,
        };

        private static readonly List<IFunctionEvaluation> functionsEvaluations = new List<IFunctionEvaluation>
        {
            new LoopEvaluation(),
            NppAccessEvaluation.Instance,
            new HttpEvaluation(),
            new StringJoinEvaluation(),
            ExcelEvaluation.Instance,
            new QRCodeEvaluation(),
            new IniEvaluation(),
        };

        private static readonly List<IEvaluatorInitializator> evaluatorInitializators = new List<IEvaluatorInitializator>
        {
            ExcelEvaluation.Instance,
        };


        public static string Print { get; set; }

        public static void EvaluatorInit(ExpressionEvaluator evaluator)
        {
            evaluatorInitializators.ForEach(ini => ini.Init(evaluator));
        }

        public static void Evaluator_EvaluateVariable(object sender, VariableEvaluationEventArg e)
        {
            if (e.Name.Equals("hw", StringComparison.OrdinalIgnoreCase) && e.This == null)
            {
                e.Value = "!!! Hello World !!!";
            }
            else if (e.Name.Equals("random") || e.Name.Equals("rand", StringComparison.OrdinalIgnoreCase) || e.Name.Equals("rnd", StringComparison.OrdinalIgnoreCase))
            {
                e.Value = random;
            }
            else if ((e.Name.Equals("hex", StringComparison.OrdinalIgnoreCase) || e.Name.Equals("hexd", StringComparison.OrdinalIgnoreCase)) && e.This is int intHexValue)
            {
                e.Value = $"{(e.Name.EndsWith("d", StringComparison.OrdinalIgnoreCase) ? string.Empty : "0x")}{intHexValue.ToString("X")}";
            }
            else if ((e.Name.Equals("hex", StringComparison.OrdinalIgnoreCase) || e.Name.Equals("hexd", StringComparison.OrdinalIgnoreCase)) && e.This is double doubleHexValue)
            {
                e.Value = $"{(e.Name.EndsWith("d", StringComparison.OrdinalIgnoreCase) ? string.Empty : "0x")}{((int)doubleHexValue).ToString("X")}";
            }
            else if ((e.Name.Equals("bin", StringComparison.OrdinalIgnoreCase) || e.Name.Equals("bind", StringComparison.OrdinalIgnoreCase)) && e.This is int intBinValue)
            {
                e.Value = $"{(e.Name.EndsWith("d", StringComparison.OrdinalIgnoreCase) ? string.Empty : "0b")}{Convert.ToString(intBinValue, 2)}";
            }
            else if ((e.Name.Equals("bin", StringComparison.OrdinalIgnoreCase) || e.Name.Equals("bind", StringComparison.OrdinalIgnoreCase)) && e.This is double doubleBinValue)
            {
                e.Value = $"{(e.Name.EndsWith("d", StringComparison.OrdinalIgnoreCase) ? string.Empty : "0b")}{Convert.ToString((int)doubleBinValue, 2)}";
            }
            else if (e.Name.Equals("guid"))
            {
                e.Value = Guid.NewGuid().ToString();
            }
            else if (e.Name.Equals("clipboard", StringComparison.OrdinalIgnoreCase) || e.Name.Equals("cb", StringComparison.OrdinalIgnoreCase))
            {
                if (Clipboard.ContainsImage())
                    e.Value = Clipboard.GetImage().GetBitmap();
                else
                    e.Value = Clipboard.GetText();
            }
            else
            {
                variablesEvaluations.Find(eval => eval.TryEvaluate(sender, e));
            }
        }

        public static void Evaluator_EvaluateFunction(object sender, FunctionEvaluationEventArg e)
        {
            if (e.Name.Equals("print", StringComparison.OrdinalIgnoreCase) && e.This == null)
            {
                if (e.Args.Count > 1)
                    Print += string.Format(e.EvaluateArg(0).ToString(), e.Args.Skip(1).ToArray()) + BNpp.CurrentEOL;
                else
                    Print += e.EvaluateArg(0).ToString() + BNpp.CurrentEOL;

                e.Value = null;
            }
            else if (e.Name.Equals("range", StringComparison.OrdinalIgnoreCase) && e.This == null)
            {
                if (e.Args.Count == 2)
                    e.Value = Enumerable.Range((int)e.EvaluateArg(0), (int)e.EvaluateArg(1)).Cast<object>();
                else if (e.Args.Count == 1)
                    e.Value = Enumerable.Range(1, (int)e.EvaluateArg(1)).Cast<object>();
            }
            else if (e.Name.Equals("repeat", StringComparison.OrdinalIgnoreCase))
            {
                if (e.Args.Count == 2 && e.This == null)
                    e.Value = Enumerable.Repeat(e.EvaluateArg(0), (int)e.EvaluateArg(1));
                else if (e.Args.Count == 1 && e.This != null)
                    e.Value = Enumerable.Repeat(e.This, (int)e.EvaluateArg(1));
            }
            else if (e.Name.Equals("clipboard", StringComparison.OrdinalIgnoreCase) || e.Name.Equals("cb", StringComparison.OrdinalIgnoreCase))
            {
                if(e.Args.Count > 0)
                {
                    object arg = e.EvaluateArg(0);

                    if (arg is Bitmap bitmap)
                        Clipboard.SetDataObject(bitmap);
                    else
                        Clipboard.SetText(arg.ToString());

                    e.Value = arg;
                }
            }
            else
            {
                functionsEvaluations.Find(eval => eval.TryEvaluate(sender, e));
            }
        }
    }
}
