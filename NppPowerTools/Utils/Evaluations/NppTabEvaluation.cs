﻿using CodingSeb.ExpressionEvaluator;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace NppPowerTools.Utils.Evaluations
{
    public class NppAccessEvaluation : IVariableEvaluation, IFunctionEvaluation
    {
        private static readonly Regex tabVarRegex = new Regex(@"^tab(?<pos>(?<tabIndex>\d+)|(?<all>all)|(?<count>count|c))?(?<fileName>filename|fn|f|n)?$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex tabFuncRegex = new Regex("^tab((?<tabIndex>index|i)|(?<fileName>filename|fn|f|n))?$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public bool TryEvaluate(object sender, VariableEvaluationEventArg e)
        {
            Match tabVarMatch = tabVarRegex.Match(e.Name);
            if (tabVarMatch.Success && (tabVarMatch.Groups["pos"].Success || tabVarMatch.Groups["fileName"].Success))
            {
                string currentTab = BNpp.NotepadPP.CurrentFileName;

                if (tabVarMatch.Groups["all"].Success)
                {
                    List<string> texts = new List<string>();

                    BNpp.NotepadPP.GetAllOpenedDocuments.ForEach(tabName =>
                    {
                        if (tabVarMatch.Groups["fileName"].Success)
                        {
                            texts.Add(tabName);
                        }
                        else
                        {
                            BNpp.NotepadPP.ShowOpenedDocument(tabName);
                            texts.Add(BNpp.Text);
                        }
                    });

                    BNpp.NotepadPP.ShowOpenedDocument(currentTab);
                    e.Value = texts;
                }
                else if(tabVarMatch.Groups["count"].Success)
                {
                    e.Value = BNpp.NotepadPP.TabCount;
                }
                else if (tabVarMatch.Groups["fileName"].Success)
                {
                    e.Value = tabVarMatch.Groups["tabIndex"].Success
                        ? BNpp.NotepadPP.GetAllOpenedDocuments[int.Parse(tabVarMatch.Groups["tabIndex"].Value)]
                        : currentTab;
                }
                else if (tabVarMatch.Groups["tabIndex"].Success)
                {
                    BNpp.NotepadPP.ShowTab(int.Parse(tabVarMatch.Groups["tabIndex"].Value));
                    string text = BNpp.Text;
                    BNpp.NotepadPP.ShowOpenedDocument(currentTab);
                    string doNothingWithItText = BNpp.Text;

                    e.Value = text;
                }

                return true;
            }

            return false;
        }

        public bool TryEvaluate(object sender, FunctionEvaluationEventArg e)
        {
            Match tabFuncMatch = tabFuncRegex.Match(e.Name);

            if(tabFuncMatch.Success)
            {
                string currentTab = BNpp.NotepadPP.CurrentFileName;

                if (tabFuncMatch.Groups["tabIndex"].Success)
                {
                    if (e.Args.Count > 0)
                        e.Value = BNpp.NotepadPP.GetAllOpenedDocuments.FindIndex(tab => tab.Equals(e.EvaluateArg<string>(0)));
                    else
                        e.Value = BNpp.NotepadPP.GetAllOpenedDocuments.FindIndex(tab => tab.Equals(currentTab));
                }
                else if (tabFuncMatch.Groups["fileName"].Success)
                {
                    if (e.Args.Count > 0)
                        e.Value = BNpp.NotepadPP.GetAllOpenedDocuments[e.EvaluateArg<int>(0)];
                    else
                        e.Value = currentTab;
                }
                else
                {
                    if (e.Args.Count > 0)
                    {
                        object arg = e.EvaluateArg(0);

                        if (arg is int iArg)
                            BNpp.NotepadPP.ShowTab(iArg);
                        else
                            BNpp.NotepadPP.ShowOpenedDocument(arg.ToStringOutput());
                    }

                    string text = BNpp.Text;
                    BNpp.NotepadPP.ShowOpenedDocument(currentTab);
                    string doNothingWithItText = BNpp.Text;

                    e.Value = text;
                }

                return true;
            }

            return false;
        }

        #region singleton

        private static NppAccessEvaluation instance = null;

        public static NppAccessEvaluation Instance
        {
            get
            {
                return instance ?? (instance = new NppAccessEvaluation());
            }
        }

        private NppAccessEvaluation()
        { }

        #endregion
    }
}
