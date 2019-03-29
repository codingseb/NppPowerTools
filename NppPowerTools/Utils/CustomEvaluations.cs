using CodingSeb.ExpressionEvaluator;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;

namespace NppPowerTools.Utils
{
    internal class CustomEvaluations
    {
        private static readonly Regex loremIspumVariableEvalRegex = new Regex(@"^(li|loremipsum|lorem|ipsum)(w(?<words>\d+)|wl(?<wordsPerLine>\d+)|(?<language>fr|en|la)|l?(?<lines>\d+))*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex loopVariableEvalRegex = new Regex(@"^(lp|loop)((?<join>j)|f(?<from>\d+)|(t()?<to>\d+)|[nc]?(?<count>\d+))*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex tabVarRegex = new Regex(@"tab((?<tabIndex>\d+)|(?<all>all))", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static Random random = new Random();

        public static void Evaluator_EvaluateVariable(object sender, VariableEvaluationEventArg e)
        {
            Match loremIspumVariableEvalMatch = loremIspumVariableEvalRegex.Match(e.Name);
            Match tabVarMatch = tabVarRegex.Match(e.Name);

            if (loremIspumVariableEvalMatch.Success)
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
            else if(tabVarMatch.Success)
            {
                string currentTab = BNpp.NotepadPP.CurrentFileName;

                if(tabVarMatch.Groups["all"].Success)
                {
                    List<string> texts = new List<string>();

                    BNpp.NotepadPP.GetAllOpenedDocuments.ForEach(tabName =>
                    {
                        BNpp.NotepadPP.ShowOpenedDocument(tabName);
                        texts.Add(BNpp.Text);
                    });

                    BNpp.NotepadPP.ShowOpenedDocument(currentTab);
                    e.Value = string.Join(BNpp.CurrentEOL, texts);
                }
                else if(tabVarMatch.Groups["tabIndex"].Success)
                {
                    BNpp.NotepadPP.ShowTab(int.Parse(tabVarMatch.Groups["tabIndex"].Value));
                    string text = BNpp.Text;
                    BNpp.NotepadPP.ShowOpenedDocument(currentTab);
                    string doNothingWithItText = BNpp.Text;
                    e.Value = text;
                }

            }
            else if ((e.Name.ToLower().Equals("sjoin") || e.Name.ToLower().Equals("sj") || e.Name.ToLower().Equals("j")) && e.This is IEnumerable<object> list)
            {
                e.Value = string.Join(BNpp.CurrentEOL, list);
            }
            else if(e.Name.ToLower().Equals("json") && e.This != null)
            {
                e.Value = JsonConvert.SerializeObject(e.This); 
            }
            else if(e.Name.Equals("random") || e.Name.ToLower().Equals("rand") || e.Name.ToLower().Equals("rnd"))
            {
                e.Value = random;
            }
            else if(e.Name.ToLower().Equals("hex") && e.This is int intHexValue)
            {
                e.Value = $"0x{intHexValue.ToString("X")}";
            }
            else if(e.Name.ToLower().Equals("bin") && e.This is int intBinValue)
            {
                e.Value = $"0b{Convert.ToString(intBinValue, 2)}";
            }
            else if(e.Name.Equals("guid"))
            {
                e.Value = Guid.NewGuid().ToString();
            }
            else if (e.Name.Equals("clipboard") || e.Name.ToLower().Equals("cb"))
            {
                e.Value =  Clipboard.GetText();
            }

        }

        public static void Evaluator_EvaluateFunction(object sender, FunctionEvaluationEventArg e)
        {
            Match loopVariableEvalMatch = loopVariableEvalRegex.Match(e.Name);

            if (loopVariableEvalMatch.Success)
            {
                List<object> results = new List<object>();

                int from = loopVariableEvalMatch.Groups["from"].Success ? int.Parse(loopVariableEvalMatch.Groups["from"].Value, CultureInfo.InvariantCulture) : 1;

                if (loopVariableEvalMatch.Groups["to"].Success)
                {
                    for (int i = from; i <= int.Parse(loopVariableEvalMatch.Groups["to"].Value, CultureInfo.InvariantCulture); i++)
                    {
                        e.Evaluator.Variables[e.Args.Count > 1 ? e.Args[1].Trim() : "i"] = i;
                        results.Add(e.Evaluator.Evaluate(e.Args[0]).ToString());
                    }
                }
                else
                {
                    int count = loopVariableEvalMatch.Groups["count"].Success ? int.Parse(loopVariableEvalMatch.Groups["count"].Value, CultureInfo.InvariantCulture) : 10;

                    for (int i = 0; i < count; i++)
                    {
                        e.Evaluator.Variables[e.Args.Count > 1 ? e.Args[1].Trim() : "i"] = i + from;
                        results.Add(e.Evaluator.Evaluate(e.Args[0]));
                    }
                }

                e.Value = results;
            }
            else if ((e.Name.ToLower().Equals("sjoin") || e.Name.ToLower().Equals("sj") || e.Name.ToLower().Equals("j")) && (e.This is IEnumerable<object> || e.Args.Count > 1 && e.EvaluateArg(1) is List<object>))
            {
                if (e.This is List<object> list)
                {
                    e.Value = string.Join(e.Args.Count > 0 ? e.EvaluateArg<string>(0) : "\r\n", list);
                }
                else if (e.Args.Count > 1 && e.EvaluateArg(1) is List<object> list2)
                {
                    e.Value = string.Join(e.EvaluateArg<string>(0), list2);
                }
            }
        }
    }
}
