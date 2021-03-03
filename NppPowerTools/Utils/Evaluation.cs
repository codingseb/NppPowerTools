using CodingSeb.ExpressionEvaluator;
using NppPowerTools.PluginInfrastructure;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Diagnostics;

namespace NppPowerTools.Utils
{
    public static class Evaluation
    {
        internal static IDictionary<string, object> LastVariables { get; set; } = new Dictionary<string, object>();

        internal static void ResetVariables()
        {
            LastVariables.Clear();
            MessageBox.Show("Variables reseted");
        }

        internal static void Process(bool isScript, string script = null,  Action<object> setResult = null, bool forceErrorInOutput = false)
        {
            if (!Config.Instance.KeepVariablesBetweenEvaluations)
            {
                LastVariables = new Dictionary<string, object>();
            }
            else if(LastVariables != null)
            {
                LastVariables
                    .ToList()
                    .FindAll(kvp => kvp.Value is StronglyTypedVariable)
                    .ForEach(kvp => LastVariables.Remove(kvp.Key));
            }

            CustomEvaluations.Print = null;

            ExpressionEvaluator evaluator = new XEval(LastVariables)
            {
                OptionForceIntegerNumbersEvaluationsAsDoubleByDefault = Config.Instance.OptionForceIntegerNumbersEvaluationsAsDoubleByDefault,
                OptionCaseSensitiveEvaluationActive = Config.Instance.CaseSensitive,
                OptionScriptNeedSemicolonAtTheEndOfLastExpression = false,
            };

            evaluator.Namespaces.Add("NppPowerTools");
            evaluator.Namespaces.Add("System.Windows");
            evaluator.Namespaces.Add("System.Diagnostics");
            evaluator.Types.Add(typeof(IniFile));
            evaluator.Types.Add(typeof(PDFFile));
            evaluator.StaticTypesForExtensionsMethods.Add(typeof(Extensions));

            CustomEvaluations.EvaluatorInit(evaluator);

            evaluator.EvaluateFunction += CustomEvaluations.Evaluator_EvaluateFunction;
            evaluator.EvaluateVariable += CustomEvaluations.Evaluator_EvaluateVariable;

            try
            {
                if (BNpp.SelectionLength <= 0)
                {
                    IScintillaGateway
                        scintilla = new ScintillaGateway(PluginBase.GetCurrentScintilla());
                    int line = scintilla.GetCurrentLineNumber();
                    int end = scintilla.GetLineEndPosition(line);
                    int start = 0;

                    if (isScript)
                    {
                        // TODO special start script tag
                    }
                    else
                    {
                        int i;
                        for (i = line; i > 0 && scintilla.GetLine(line).TrimStart().StartsWith("."); i--) ;

                        start = scintilla.PositionFromLine(i);

                        for (i = line; i < scintilla.GetLineCount() && scintilla.GetLine(line).TrimStart().StartsWith("."); i++) ;

                        end = scintilla.GetLineEndPosition(i);
                    }

                    if(setResult == null)
                        scintilla.SetSel(new Position(start), new Position(end));
                }

                setResult ??= Config.Instance.CurrentResultOut.SetResult;

                script ??= BNpp.SelectedText;

                Config.Instance.LastScripts.Insert(0, script);
                while (Config.Instance.LastScripts.Count > Config.Instance.NbrOfLastScriptToKeep)
                    Config.Instance.LastScripts.RemoveAt(Config.Instance.LastScripts.Count -1);

                Config.Instance.LastScripts = Config.Instance.LastScripts.Distinct().ToList();

                object result = isScript ? evaluator.ScriptEvaluate(evaluator.RemoveComments(script)) : evaluator.Evaluate(evaluator.RemoveComments(script.TrimEnd(';')));

                setResult(result);
            }
            catch (Exception exception)
            {
                if (Config.Instance.ShowExceptionInMessageBox && !forceErrorInOutput)
                {
                    MessageBox.Show(exception.Message);
                }

                if (Config.Instance.ShowExceptionInOutput || forceErrorInOutput)
                {
                    setResult(exception);
                }

                if (!string.IsNullOrEmpty(CustomEvaluations.Print))
                    setResult(CustomEvaluations.Print);
            }
            finally
            {
                evaluator.EvaluateFunction -= CustomEvaluations.Evaluator_EvaluateFunction;
                evaluator.EvaluateVariable -= CustomEvaluations.Evaluator_EvaluateVariable;
                LastVariables = evaluator.Variables;
            }
        }
    }
}
