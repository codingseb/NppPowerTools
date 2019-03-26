using CodingSeb.ExpressionEvaluator;
using NppPowerTools.PluginInfrastructure;
using NppPowerTools.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace NppPowerTools
{
    class Main
    {
        internal const string PluginName = "Npp Power Tools";
        private static readonly Regex loremIspumVariableEvalRegex = new Regex(@"^(li|loremipsum|lorem|ipsum)(w(?<words>\d+)|wl(?<wordsPerLine>\d+)|(?<language>fr|en|la)|l?(?<lines>\d+))*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex loopVariableEvalRegex = new Regex(@"^lp|loop(f(?<from>\d+)|(t()?<to>\d+)|[nc]?(?<count>\d+))*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static void OnNotification(ScNotification notification)
        { }

        internal static void CommandMenuInit()
        {
            PluginBase.SetCommand(0, "Process", Process, new ShortcutKey(true, false, true, Keys.E));
        }

        internal static void Process()
        {
            ExpressionEvaluator evaluator = new ExpressionEvaluator();

            evaluator.EvaluateFunction += Evaluator_EvaluateFunction;
            evaluator.EvaluateVariable += Evaluator_EvaluateVariable;

            try
            {
                if (BNpp.SelectionLength <= 0)
                {
                    IScintillaGateway scintilla = new ScintillaGateway(PluginBase.GetCurrentScintilla());
                    int start = scintilla.GetCurrentPos();
                    int line = scintilla.GetCurrentLineNumber();
                    int lineStart = scintilla.PositionFromLine(line);
                    scintilla.SetSel(new Position(lineStart), new Position(start));
                }

                BNpp.SelectedText = evaluator.Evaluate(BNpp.SelectedText).ToString();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
            finally
            {
                evaluator.EvaluateFunction -= Evaluator_EvaluateFunction;
                evaluator.EvaluateVariable -= Evaluator_EvaluateVariable;
            }
        }

        private static void Evaluator_EvaluateVariable(object sender, VariableEvaluationEventArg e)
        {
            Match loremIspumVariableEvalMatch = loremIspumVariableEvalRegex.Match(e.Name);

            if(loremIspumVariableEvalMatch.Success)
            {
                LoremIpsum li = new LoremIpsum();

                if (loremIspumVariableEvalMatch.Groups["language"].Success)
                    li.CurrentLanguage = loremIspumVariableEvalMatch.Groups["language"].Value.ToLower();

                int wordPerLine = loremIspumVariableEvalMatch.Groups["wordsPerLine"].Success ? int.Parse(loremIspumVariableEvalMatch.Groups["wordsPerLine"].Value, CultureInfo.InvariantCulture) : 10;

                if (loremIspumVariableEvalMatch.Groups["words"].Success)
                    e.Value = li.GetWords(int.Parse(loremIspumVariableEvalMatch.Groups["words"].Value, CultureInfo.InvariantCulture), wordPerLine);
                else if (loremIspumVariableEvalMatch.Groups["lines"].Success)
                    e.Value = li.GetLines(int.Parse(loremIspumVariableEvalMatch.Groups["lines"].Value, CultureInfo.InvariantCulture), wordPerLine);
                else
                    e.Value = li.GetWords(100, wordPerLine);
            }
        }

        private static void Evaluator_EvaluateFunction(object sender, FunctionEvaluationEventArg e)
        {
            Match loopVariableEvalMatch = loopVariableEvalRegex.Match(e.Name);

            if(loopVariableEvalMatch.Success)
            {
                List<string> results = new List<string>();

                int from = loopVariableEvalMatch.Groups["from"].Success ? int.Parse(loopVariableEvalMatch.Groups["from"].Value, CultureInfo.InvariantCulture) : 0;

                if(loopVariableEvalMatch.Groups["to"].Success)
                {
                    for (int i = from; i <= int.Parse(loopVariableEvalMatch.Groups["to"].Value, CultureInfo.InvariantCulture); i++)
                    {
                        e.Evaluator.Variables["i"] = i;
                        results.Add(e.Evaluator.Evaluate(e.Args[0]).ToString());
                    }
                }
                else
                {
                    int count = loopVariableEvalMatch.Groups["count"].Success ? int.Parse(loopVariableEvalMatch.Groups["count"].Value, CultureInfo.InvariantCulture) : 10;

                    for (int i = 0; i < count; i++)
                    {
                        e.Evaluator.Variables["i"] = i + from;
                        results.Add(e.Evaluator.Evaluate(e.Args[0]).ToString());
                    }
                }

                e.Value = string.Join(e.Args.Count > 1 ? e.EvaluateArg(1).ToString() : "\r\n", results);
            }
        }
    }
}