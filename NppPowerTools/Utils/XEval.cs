using CodingSeb.ExpressionEvaluator;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace NppPowerTools.Utils
{
    public class XEval : ExpressionEvaluator
    {
        private static readonly Regex cmdRegex = new Regex(@"^[$]((?<wait>w[ait]?)?(?<process>p)?)*\s*(?<cmd>.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public XEval()
        {}

        public XEval(IDictionary<string, object> variables) : base(variables)
        {}

        protected override void Init()
        {
            ParsingMethods.Add(EvaluateDollarAsCmdOperator);
        }

        protected void InitSimpleObjet(object element, List<string> initArgs)
        {
            string variable = "V" + Guid.NewGuid().ToString().Replace("-", "");

            Variables[variable] = element;

            initArgs.ForEach(subExpr =>
            {
                if (subExpr.Contains("="))
                {
                    string trimmedSubExpr = subExpr.TrimStart();

                    Evaluate($"{variable}{(trimmedSubExpr.StartsWith("[") ? string.Empty : ".")}{trimmedSubExpr}");
                }
                else
                {
                    throw new ExpressionEvaluatorSyntaxErrorException($"A '=' char is missing in [{subExpr}]. It is in a object initializer. It must contains one.");
                }
            });

            Variables.Remove(variable);
        }

        protected virtual bool EvaluateDollarAsCmdOperator(string expression, Stack<object> stack, ref int i)
        {
            Match match = cmdRegex.Match(expression.Substring(i));

            if(match.Success)
            {
                i+= match.Length - 1;

                stack.Push(ExecuteCommand(match.Groups["cmd"].Value, match.Groups["wait"].Success, match.Groups["process"].Success));

                return true;
            }

            return false;
        }

        private object ExecuteCommand(string command, bool wait, bool returnProcess)
        {
            string fileName = "cmd";
            string args = "/c " + command;

            if(returnProcess)
            {
                var fileNameMatch = Regex.Match(command, @"^((?<fileName1>\S+)|(""(?<fileName2>[^""]+)""))(\s+(?<args>.*))?");

                if(fileNameMatch.Success && fileNameMatch.Groups["fileName1"].Success)
                {
                    fileName = fileNameMatch.Groups["fileName1"].Value;
                    args = fileNameMatch.Groups["args"].Value;
                }
                else if(fileNameMatch.Success && fileNameMatch.Groups["fileName2"].Success)
                {
                    fileName = fileNameMatch.Groups["fileName2"].Value;
                    args = fileNameMatch.Groups["args"].Value;
                }
            }
            
            ProcessStartInfo procStartInfo = new ProcessStartInfo(fileName, args)
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

            if (wait)
                proc.WaitForExit();

            if (returnProcess)
                return proc;
            else
                return proc.StandardOutput.ReadToEnd() + proc.StandardError.ReadToEnd();
        }
    }
}
