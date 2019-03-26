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
            PluginBase.SetCommand(0, "Execute", Process, new ShortcutKey(true, false, false, Keys.E));
            RefreshCommands();
        }

        private static void RefreshCommands()
        {
            for (int i = 0; i < Config.Instance.ResultOuts.Count; i++)
            {
                int value = i;
                PluginBase.SetCommand(i + 1, Config.Instance.ResultOuts[i].Name, () =>
                {
                    try
                    {
                        Config.Instance.CurrentResultOutIndex = value;
                        Config.Instance.Save();

                        MessageBox.Show($"Result output : \"{Config.Instance.ResultOuts[value].Name }\" selected.", "Info", MessageBoxButtons.OK);
                        RefreshCommands();
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show($"{exception.Message}\r\n{exception.StackTrace}{(exception.InnerException == null ? string.Empty : $"\r\nInner Exception :\r\n{exception.InnerException.Message}\r\n{exception.InnerException.StackTrace}")}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }, new ShortcutKey(false, true, false, Keys.NumPad1 + i), i == Config.Instance.CurrentResultOutIndex);
            }
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

                Config.Instance.CurrentResultOut.SetResult(evaluator.Evaluate(BNpp.SelectedText).ToString());
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