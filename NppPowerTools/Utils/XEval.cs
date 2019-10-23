using CodingSeb.ExpressionEvaluator;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace NppPowerTools.Utils
{
    /// <summary>
    /// -- Describe here to what is this class used for. (What is it's purpose) --
    /// </summary>
    public class XEval : ExpressionEvaluator
    {
        public XEval() : base()
        {}

        public XEval(IDictionary<string, object> variables) : base(variables)
        {}

        protected override void Init()
        {
            ParsingMethods.Add(EvaluateDollarAsCmdOperator);
        }

        protected virtual bool EvaluateDollarAsCmdOperator(string expression, Stack<object> stack, ref int i)
        {
            Match match = Regex.Match(expression.Substring(i), @"^[$]\s*(?<cmd>.*)");

            if(match.Success)
            {
                i+= match.Length - 1;

                stack.Push(ExecuteCommand(match.Groups["cmd"].Value));

                return true;
            }

            return false;
        }

        private string ExecuteCommand(string command)
        {
            var procStartInfo = new ProcessStartInfo("cmd", "/c " + command)
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,

                // Set encoding if command is window command
                StandardOutputEncoding = Encoding.GetEncoding(850)
            };

            Process proc = new Process
            {
                StartInfo = procStartInfo
            };

            proc.Start();

            return proc.StandardOutput.ReadToEnd() + proc.StandardError.ReadToEnd();
        }
    }
}
