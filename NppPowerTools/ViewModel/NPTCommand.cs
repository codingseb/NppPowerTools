using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;

namespace NppPowerTools
{
    public class NPTCommand : ViewModelBase
    {
        private IEnumerable<Inline> inlines;

        public string Name { get; set; } = string.Empty;

        public int CommandId { get; set; }

        public object ResultOrInfoSup { get; set; }

        public IEnumerable<Inline> Inlines
        {
            get
            {
                return inlines ?? new List<Inline> { new Run(Name) };
            }

            set { inlines = value; }
        }

        public Action<Window> CommandAction { get; set; } = win => win.Close();
    }
}
