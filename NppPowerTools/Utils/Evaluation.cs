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

        internal static void Process(bool script)
        {
            if (!Config.Instance.KeepVariablesBetweenEvaluations)
                LastVariables = new Dictionary<string, object>();

            ExpressionEvaluator evaluator = new ExpressionEvaluator(LastVariables)
            {
                OptionForceIntegerNumbersEvaluationsAsDoubleByDefault = Config.Instance.OptionForceIntegerNumbersEvaluationsAsDoubleByDefault,
                OptionCaseSensitiveEvaluationActive = Config.Instance.CaseSensitive,
            };

            evaluator.Namespaces.Add("NppPowerTools");

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

                    if (script)
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

                    scintilla.SetSel(new Position(start), new Position(end));
                }

                object result = script ? evaluator.ScriptEvaluate(evaluator.RemoveComments(BNpp.SelectedText)) : evaluator.Evaluate(BNpp.SelectedText.TrimEnd(';'));

                Config.Instance.CurrentResultOut.SetResult(result);
            }
            catch (Exception exception)
            {
                if (Config.Instance.ShowExceptionInMessageBox)
                {
                    MessageBox.Show(exception.Message);
                }

                if (Config.Instance.ShowExceptionInOutput)
                {
                    Config.Instance.CurrentResultOut.SetResult(new EvaluationError { Exception = exception });
                }

                if (!string.IsNullOrEmpty(CustomEvaluations.Print))
                    Config.Instance.CurrentResultOut.SetResult(CustomEvaluations.Print);
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
