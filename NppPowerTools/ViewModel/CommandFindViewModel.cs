using NppPowerTools.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace NppPowerTools
{
    public class CommandFindViewModel : ViewModelBase
    {
        private static readonly Regex expressionEvalRegex = new Regex(@"^[\<\>:?](?<expression>.*)$", RegexOptions.Compiled);
        private static readonly Regex charRegex = new Regex(".", RegexOptions.Compiled);

        CommandFindWindow commandFindWindow = null;

        public string Find { get; set; } = string.Empty;

        public List<NPTCommand> CommandsList => GetFilteredList(Find);

        public int SelectionIndex { get; set; }

        public List<NPTCommand> GetFilteredList(string filter)
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

                        expressionCommand.CommandAction = () => BNpp.SelectedText = expressionCommand.ResultOrInfoSup.ToStringOutput();

                        SelectionIndex = 0;

                        return new List<NPTCommand>
                        {
                            expressionCommand
                        };
                    }
                }
                else
                {
                    Regex findRegex = new Regex(".*" + charRegex.Replace(filter, match => Regex.Escape(match.Value) + ".*"), RegexOptions.IgnoreCase);
                    SelectionIndex = 0;
                    return NPTCommands.Commands.FindAll(command => findRegex.IsMatch(command.Name));
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
