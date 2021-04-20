using CodingSeb.ExpressionEvaluator;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Text;
using System.Text.RegularExpressions;

namespace NppPowerTools.Utils
{
    /// <summary>
    /// -- Describe here to what is this class used for. (What is it's purpose) --
    /// </summary>
    public class XEval : ExpressionEvaluator
    {
        private static readonly Regex cmdRegex = new Regex(@"^[$]\s*(?<cmd>.*)", RegexOptions.Compiled);

        public XEval()
        {}

        public XEval(IDictionary<string, object> variables) : base(variables)
        {}

        protected override void Init()
        {
            ParsingMethods.Add(EvaluateSimplifiedSyntaxForExpandoObject);
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

        protected virtual bool EvaluateSimplifiedSyntaxForExpandoObject(string expression, Stack<object> stack, ref int i)
        {
            if(expression[i] == '{')
            {
                i++;

                object element = new ExpandoObject();

                List<string> initArgs = GetExpressionsBetweenParenthesesOrOtherImbricableBrackets(expression, ref i, true, OptionInitializersSeparator, "{", "}");

                InitSimpleObjet(element, initArgs);

                stack.Push(element);

                return true;
            }

            return false;
        }
        protected virtual bool EvaluateDollarAsCmdOperator(string expression, Stack<object> stack, ref int i)
        {
            Match match = cmdRegex.Match(expression.Substring(i));

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
            ProcessStartInfo procStartInfo = new ProcessStartInfo("cmd", "/c " + command)
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,

                // Set encoding if command is window command
                StandardOutputEncoding = Encoding.GetEncoding(850)
            };

            using Process proc = new Process
            {
                StartInfo = procStartInfo
            };

            proc.Start();

            return proc.StandardOutput.ReadToEnd() + proc.StandardError.ReadToEnd();
        }
    }
}
