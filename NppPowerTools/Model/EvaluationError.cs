using System;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace NppPowerTools
{
    public class EvaluationError
    {
        [ExpandableObject]
        public Exception Exception { get; set; }

        public override string ToString()
        {
            return Exception.Message;
        }
    }
}
