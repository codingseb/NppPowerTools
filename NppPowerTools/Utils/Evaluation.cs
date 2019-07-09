using CodingSeb.ExpressionEvaluator;
using NppPowerTools.PluginInfrastructure;
using System;
using System.Collections.Generic;
using System.Windows;

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
                LastVariables = new Dictionary<string, object>();

            ExpressionEvaluator evaluator = new ExpressionEvaluator(LastVariables)
            {
                OptionForceIntegerNumbersEvaluationsAsDoubleByDefault = Config.Instance.OptionForceIntegerNumbersEvaluationsAsDoubleByDefault,
                OptionCaseSensitiveEvaluationActive = Config.Instance.CaseSensitive,
            };

            evaluator.Namespaces.Add("NppPowerTools");
            evaluator.Namespaces.Add("System.Windows");

            CustomEvaluations.EvaluatorInit(evaluator);

            CustomEvaluations.Print = string.Empty;

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

                setResult = setResult ?? Config.Instance.CurrentResultOut.SetResult;

                script = script ?? BNpp.SelectedText;

                Config.Instance.LastScript = script;

                object result = isScript ? evaluator.ScriptEvaluate(evaluator.RemoveComments(script.TrimEnd(';', ' ', '\t', '\r', '\n') + ";")) : evaluator.Evaluate(script.TrimEnd(';'));

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
