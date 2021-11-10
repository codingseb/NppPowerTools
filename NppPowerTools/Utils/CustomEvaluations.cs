using CodingSeb.ExpressionEvaluator;
using NppPowerTools.PluginInfrastructure;
using NppPowerTools.Utils.Evaluations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NppPowerTools.Utils
{
    internal static class CustomEvaluations
    {
        private static readonly Random random = new Random();

        private static readonly List<IVariableEvaluation> variablesEvaluations = new List<IVariableEvaluation>
        {
            new LoremIspumEvaluation(),
            NppAccessEvaluation.Instance,
            JsonEvaluation.Instance,
            new StringEvaluation(),
            ExcelEvaluation.Instance,
            IniEvaluation.Instance,
            ClipboardEvaluation.Instance,
            new IpConfigCommands(),
            ChartPlotEvaluation.Instance,
            PDFEvaluation.Instance,
            DBEvaluation.Instance,
            ImageEvaluation.Instance,
            HTMLEvaluation.Instance,
        };

        private static readonly List<IFunctionEvaluation> functionsEvaluations = new List<IFunctionEvaluation>
        {
            new LoopEvaluation(),
            NppAccessEvaluation.Instance,
            JsonEvaluation.Instance,
            new HttpEvaluation(),
            new StringEvaluation(),
            ExcelEvaluation.Instance,
            new QRCodeEvaluation(),
            IniEvaluation.Instance,
            ClipboardEvaluation.Instance,
            ChartPlotEvaluation.Instance,
            PDFEvaluation.Instance,
            DBEvaluation.Instance,
            ImageEvaluation.Instance,
            HTMLEvaluation.Instance,
            TextToSpeechEvaluation.Instance
        };

        private static readonly List<IEvaluatorInitializator> evaluatorInitializators = new List<IEvaluatorInitializator>
        {
            ExcelEvaluation.Instance,
            PDFEvaluation.Instance,
            ImageEvaluation.Instance,
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
            else if ((e.Name.Equals("hex", StringComparison.OrdinalIgnoreCase) || e.Name.Equals("hex_d", StringComparison.OrdinalIgnoreCase)) && e.This is int intHexValue)
            {
                e.Value = (e.Name.EndsWith("d", StringComparison.OrdinalIgnoreCase) ? string.Empty : "0x") + intHexValue.ToString("X");
            }
            else if ((e.Name.Equals("hex", StringComparison.OrdinalIgnoreCase) || e.Name.Equals("hex_d", StringComparison.OrdinalIgnoreCase)) && e.This is double doubleHexValue)
            {
                e.Value = (e.Name.EndsWith("d", StringComparison.OrdinalIgnoreCase) ? string.Empty : "0x") + ((int)doubleHexValue).ToString("X");
            }
            else if ((e.Name.Equals("bin", StringComparison.OrdinalIgnoreCase) || e.Name.Equals("bin_d", StringComparison.OrdinalIgnoreCase)) && e.This is int intBinValue)
            {
                e.Value = (e.Name.EndsWith("d", StringComparison.OrdinalIgnoreCase) ? string.Empty : "0b") + Convert.ToString(intBinValue, 2);
            }
            else if ((e.Name.Equals("bin", StringComparison.OrdinalIgnoreCase) || e.Name.Equals("bin_d", StringComparison.OrdinalIgnoreCase)) && e.This is double doubleBinValue)
            {
                e.Value = (e.Name.EndsWith("d", StringComparison.OrdinalIgnoreCase) ? string.Empty : "0b") + Convert.ToString((int)doubleBinValue, 2);
            }
            else if (e.Name.Equals("guid"))
            {
                e.Value = Guid.NewGuid().ToString();
            }
            else if (e.Name.Equals("text", StringComparison.OrdinalIgnoreCase) && e.This == null)
            {
                e.Value = BNpp.Text;
            }
            else if (e.Name.Equals("selectedtext", StringComparison.OrdinalIgnoreCase) && e.This == null)
            {
                e.Value = BNpp.SelectedText;
            }
            else if (e.Name.Equals("props", StringComparison.OrdinalIgnoreCase) && e.This != null)
            {
                ShowPropertiesViewModel.Instance.ShowPropertiesWindow(e.This);
                e.Value = e.This;
            }
            else if (new string[] { "getcurrentlanguage","currentlanguage", "currentlang", "curlang", "clang", "cl" }.Contains(e.Name.ToLower()))
            {
                e.Value = BNpp.NotepadPP.GetCurrentLanguage();
            }
            else if (e.This is LangType langType && e.Name.Equals("name", StringComparison.OrdinalIgnoreCase))
            {
                e.Value = NPTCommands.Languages.Find(c => c.ResultOrInfoSup is LangType tmplangType && tmplangType == langType)?.Name ?? "No Name";
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
            else if (e.This is string text && e.Name.Equals("Split"))
            {
                object[] args = e.EvaluateArgs();

                if(args.All(o => o is char))
                {
                    e.Value = text.Split(args.Cast<char>().ToArray());
                }
                else if(args.All(o => o is string))
                {
                    e.Value = text.Split(args.Cast<string>().ToArray(), StringSplitOptions.None);
                }
            }
            else
            {
                functionsEvaluations.Find(eval => eval.TryEvaluate(sender, e));
            }
        }
    }
}
