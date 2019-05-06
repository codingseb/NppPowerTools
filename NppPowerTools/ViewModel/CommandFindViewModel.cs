using NppPowerTools.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

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
                    Regex findRegex = new Regex("(?<start>.*)" + charRegex.Replace(filter, match => "(?<match>" + Regex.Escape(match.Value) + ")(?<between>.*)")  , RegexOptions.IgnoreCase);
                    SelectionIndex = 0;
                    return NPTCommands.Commands.FindAll(command =>
                    {
                        Match match = findRegex.Match(command.Name);

                        if (match.Success)
                        {
                            List<Inline> inlines = new List<Inline>();

                            string start = match.Groups["start"].Value;

                            if (!string.IsNullOrEmpty(start))
                                inlines.Add(new Run(start));

                            CaptureCollection mcaptures = match.Groups["match"].Captures;
                            CaptureCollection bcaptures = match.Groups["between"].Captures;

                            for(int i = 0; i < mcaptures.Count; i++)
                            {
                                inlines.Add(new Span(new Run(mcaptures[i].Value))
                                {
                                    FontWeight = FontWeights.Bold,
                                    Background = Brushes.Yellow,
                                });
                                inlines.Add(new Run(bcaptures[i].Value));
                            }

                            command.Inlines = inlines;

                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    });
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
