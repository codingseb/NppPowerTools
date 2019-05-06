using System;
using System.Text.RegularExpressions;
using System.Windows.Documents;
using System.Windows.Input;

namespace NppPowerTools
{
    public class NPTCommand : ViewModelBase
    {
        public string Name { get; set; } = string.Empty;

        public object ResultOrInfoSup { get; set; }

        public InlineCollection Inlines { get; set; }

        public bool FilterWith(Regex regex)
        {
            return regex.IsMatch(Name);
        }

        public Action CommandAction { get; set; } = () => { };
    }
}
