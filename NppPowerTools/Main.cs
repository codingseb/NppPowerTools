using CodingSeb.ExpressionEvaluator;
using NppPowerTools.PluginInfrastructure;
using NppPowerTools.Utils;
using System;
using System.Windows.Forms;

namespace NppPowerTools
{
    class Main
    {
        internal const string PluginName = "Npp Power Tools";

        public static void OnNotification(ScNotification notification)
        { }

        internal static void CommandMenuInit()
        {
            PluginBase.SetCommand(0, "Process", Process, new ShortcutKey(true, false, true, Keys.E));
        }

        internal static void Process()
        {
            ExpressionEvaluator evaluator = new ExpressionEvaluator();

            evaluator.EvaluateFunction += CustomEvaluations.Evaluator_EvaluateFunction;
            evaluator.EvaluateVariable += CustomEvaluations.Evaluator_EvaluateVariable;

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
                evaluator.EvaluateFunction -= CustomEvaluations.Evaluator_EvaluateFunction;
                evaluator.EvaluateVariable -= CustomEvaluations.Evaluator_EvaluateVariable;
            }
        }
    }
}