﻿using CodingSeb.ExpressionEvaluator;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace NppPowerTools.Utils.Evaluations
{
    public class NppTabEvaluation : IVariableEvaluation
    {
        private static readonly Regex tabVarRegex = new Regex(@"tab((?<tabIndex>\d+)|(?<all>all))", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private Match tabVarMatch = null;

        public bool CanEvaluate(object sender, VariableEvaluationEventArg e)
        {
            tabVarMatch = tabVarRegex.Match(e.Name);

            return tabVarMatch.Success;
        }

        public void Evaluate(object sender, VariableEvaluationEventArg e)
        {
            string currentTab = BNpp.NotepadPP.CurrentFileName;

            if (tabVarMatch.Groups["all"].Success)
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
            else if (tabVarMatch.Groups["tabIndex"].Success)
            {
                BNpp.NotepadPP.ShowTab(int.Parse(tabVarMatch.Groups["tabIndex"].Value));
                string text = BNpp.Text;
                BNpp.NotepadPP.ShowOpenedDocument(currentTab);
                string doNothingWithItText = BNpp.Text;
                e.Value = text;
            }
        }
    }
}
