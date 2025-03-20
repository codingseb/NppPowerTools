
using Kbg.NppPluginNET;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace NppPowerTools.Utils.Evaluations
{
    public sealed class NppAccessEvaluation : IVariableEvaluation, IFunctionEvaluation
    {
        private static readonly Regex tabVarRegex = new(@"^tab(?<pos>(?<tabIndex>\d+)|(?<all>all)|(?<count>count|c))?(?<fileName>filename|fn|f|n)?$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex tabFuncRegex = new("^tab((?<tabIndex>index|i)|(?<fileName>filename|fn|f|n))?$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public bool TryEvaluate(object sender, VariableEvaluationEventArg e)
        {
            Match tabVarMatch = tabVarRegex.Match(e.Name);
            if (tabVarMatch.Success && (tabVarMatch.Groups["pos"].Success || tabVarMatch.Groups["fileName"].Success))
            {
                string currentTab = Npp.NotepadPP.CurrentFileName;
                
                if (tabVarMatch.Groups["all"].Success)
                {
                    List<string> texts = new();

                    Npp.NotepadPP.GetAllOpenedDocuments.ForEach(tabName =>
                    {
                        if (tabVarMatch.Groups["fileName"].Success)
                        {
                            texts.Add(tabName);
                        }
                        else
                        {
                            Npp.NotepadPP.ShowTab(tabName);
                            texts.Add(Npp.Text);
                        }
                    });

                    Npp.NotepadPP.ShowTab(currentTab);
                    e.Value = texts;
                }
                else if(tabVarMatch.Groups["count"].Success)
                {
                    e.Value = Npp.NotepadPP.TabCount;
                }
                else if (tabVarMatch.Groups["fileName"].Success)
                {
                    e.Value = tabVarMatch.Groups["tabIndex"].Success
                        ? Npp.NotepadPP.GetAllOpenedDocuments[int.Parse(tabVarMatch.Groups["tabIndex"].Value)]
                        : currentTab;
                }
                else if (tabVarMatch.Groups["tabIndex"].Success)
                {
                    Npp.NotepadPP.ShowTab(int.Parse(tabVarMatch.Groups["tabIndex"].Value));
                    string text = Npp.Text;
                    Npp.NotepadPP.ShowTab(currentTab);
                    string doNothingWithItText = Npp.Text;

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
                string currentTab = Npp.NotepadPP.CurrentFileName;

                if (tabFuncMatch.Groups["tabIndex"].Success)
                {
                    e.Value = e.Args.Count > 0 ? Npp.NotepadPP.GetTabIndex(e.EvaluateArg<string>(0), true) : (object)Npp.NotepadPP.GetTabIndex(currentTab);
                }
                else if (tabFuncMatch.Groups["fileName"].Success)
                {
                    e.Value = e.Args.Count > 0 ? Npp.NotepadPP.GetAllOpenedDocuments[e.EvaluateArg<int>(0)] : currentTab;
                }
                else
                {
                    if (e.Args.Count > 0)
                    {
                        object arg = e.EvaluateArg(0);

                        if (arg is int iArg)
                            Npp.NotepadPP.ShowTab(iArg);
                        else
                            Npp.NotepadPP.ShowTab(arg.ToStringOutput(), true);
                    }

                    string text = Npp.Text;
                    Npp.NotepadPP.ShowTab(currentTab);

                    e.Value = text;
                }

                return true;
            }

            return false;
        }

        #region singleton

        private static NppAccessEvaluation instance;

        public static NppAccessEvaluation Instance => instance ??= new NppAccessEvaluation();

        private NppAccessEvaluation()
        { }

        #endregion
    }
}
