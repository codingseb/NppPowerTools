using NppPowerTools.PluginInfrastructure;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;

namespace NppPowerTools
{
    public class NPTCommand : ViewModelBase
    {
        private IEnumerable<Inline> inlines;

        public string FindId { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string DisplayName
        {
            get
            {
                int index = NPTCommands.LastCommands.FindIndex(c => c.Name.Equals(Name));

                return (index >-1 ? $"{NPTCommands.LastCommands.Count - index - 1}. " : string.Empty) + Name;
            }
        }

        public int CommandId { get; set; }

        public object ResultOrInfoSup { get; set; }

        public IEnumerable<Inline> Inlines
        {
            get
            {
                return inlines ?? new List<Inline> { new Run(DisplayName) };
            }

            set { inlines = value; }
        }

        public Action<Window> CommandAction { get; set; } = win => win.Close();
    }
}
