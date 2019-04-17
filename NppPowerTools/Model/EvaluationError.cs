using System;

namespace NppPowerTools
{
    public class EvaluationError
    {
        public Exception Exception { get; set; }

        public override string ToString()
        {
            return Exception.Message;
        }
    }
}
