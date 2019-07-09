using NppPowerTools.PluginInfrastructure;
using NppPowerTools.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

namespace NppPowerTools
{
    public sealed class CommandFindViewModel : ViewModelBase
    {
        private static readonly Regex expressionEvalRegex = new Regex("^:(?<expression>.*)$", RegexOptions.Compiled);

        CommandFindWindow commandFindWindow;

        public string Find { get; set; } = string.Empty;

        public int FindSelectionStart { get; set; }

        public int FindSelectionLength { get; set; }

        public IEnumerable<NPTCommand> CommandsList => GetFilteredList(Find);

        public int SelectionIndex { get; set; }

        public IEnumerable<NPTCommand> GetFilteredList(string filter)
        {
            if (filter != null)
            {
                Match expressionEvalMatch = expressionEvalRegex.Match(filter);

                if (expressionEvalMatch.Success)
                {
                    string expression = expressionEvalMatch.Groups["expression"].Value.Trim();

                    if (!string.IsNullOrEmpty(expression))
                    {
                        NPTCommand expressionCommand = new NPTCommand
                        {
                            Name = expression,
                        };

                        Evaluation.Process(true, expressionCommand.Name, result => expressionCommand.ResultOrInfoSup = result, true);

                        expressionCommand.CommandAction = win =>
                        {
                            BNpp.SelectedText = expressionCommand.ResultOrInfoSup.ToStringOutput();
                            win.Close();
                        };

                        SelectionIndex = 0;

                        return new List<NPTCommand>
                        {
                            expressionCommand
                        };
                    }
                }
                else if(filter.StartsWith("@SetLanguage "))
                {
                    SelectionIndex = 0;
                    return NPTCommands.Languages.RegexFilterCommands(filter.Substring("@SetLanguage ".Length));
                }
                else
                {
                    SelectionIndex = 0;
                    return NPTCommands.LastCommands
                        .Cast<NPTCommand>()
                        .Reverse()
                        .Concat(NPTCommands.Commands)
                        .Append(new NPTCommand()
                        {
                            Name = "[@GetCurrentLanguage]",
                            ResultOrInfoSup = "[" + BNpp.NotepadPP.GetCurrentLanguage().ToString() + "] : " + NPTCommands.Languages.Find(c => c.ResultOrInfoSup is LangType tmplangType && tmplangType == BNpp.NotepadPP.GetCurrentLanguage())?.Name,
                            CommandAction = win =>
                            {
                                BNpp.Text = BNpp.NotepadPP.GetCurrentLanguage().ToString();
                                win.Close();
                            }
                        })
                        .Append(new NPTCommand()
                        {
                            Name = "[@ClearHistory] Clear the commands history",
                            CommandAction = _ =>
                            {
                                NPTCommands.LastCommands.Clear();
                                Find = string.Empty;
                            }
                        })
                        .RegexFilterCommands(filter);
                }
            }

            return new List<NPTCommand>();
        }



        #region WindowManagement

        public void Show()
        {
            if (commandFindWindow == null)
            {
                commandFindWindow = new CommandFindWindow();

                commandFindWindow.Closed += CommandFindWindow_Closed;
            }

            commandFindWindow.Show();
        }

        private void CommandFindWindow_Closed(object sender, System.EventArgs e)
        {
            if (commandFindWindow != null)
            {
                commandFindWindow.Closed -= CommandFindWindow_Closed;

                commandFindWindow = null;

                Find = string.Empty;
            }
        }

        #endregion

        #region singleton
        private static CommandFindViewModel instance = null;

        public static CommandFindViewModel Instance
        {
            get
            {
                return instance ?? (instance = new CommandFindViewModel());
            }
        }

        private CommandFindViewModel()
        {
        }
        #endregion

    }
}
