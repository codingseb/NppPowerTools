using CodingSeb.ExpressionEvaluator;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace NppPowerTools.Utils.Evaluations
{
    public sealed class LoopEvaluation : IFunctionEvaluation, IVariableEvaluation
    {
        private static readonly Regex loopVariableEvalRegex = new Regex(@"^(lp|loop|r|range)(f(?<from>\d+)|(t(?<to>\d+))|[nc]?(?<count>\d+))*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public bool TryEvaluate(object sender, FunctionEvaluationEventArg e)
        {
            object value = null;

            if(GenericTryToEvaluate(loopVariableEvalRegex.Match(e.Name), e.Evaluator, e.Args, ref value))
            {
                e.Value = value;
                return true;
            }

            return false;
        }

        public bool TryEvaluate(object sender, VariableEvaluationEventArg e)
        {
            object value = null;

            if(GenericTryToEvaluate(loopVariableEvalRegex.Match(e.Name), e.Evaluator, new List<string>(), ref value))
            {
                e.Value = value;
                return true;
            }

            return false;
        }

        private bool GenericTryToEvaluate(Match loopVariableEvalMatch,ExpressionEvaluator evaluator, List<string> args, ref object value)
        {
            if (loopVariableEvalMatch.Success)
            {
                List<object> results = new List<object>();

                int from = loopVariableEvalMatch.Groups["from"].Success
                    ? int.Parse(loopVariableEvalMatch.Groups["from"].Value, CultureInfo.InvariantCulture)
                    : 1;

                if (loopVariableEvalMatch.Groups["to"].Success)
                {
                    for (int i = from; i <= int.Parse(loopVariableEvalMatch.Groups["to"].Value, CultureInfo.InvariantCulture); i++)
                    {
                        evaluator.Variables[args.Count > 1 ? args[1].Trim() : "i"] = i;
                        results.Add(args.Count > 0 ? evaluator.Evaluate(args[0]) : i);
                    }
                }
                else
                {
                    int count = loopVariableEvalMatch.Groups["count"].Success
                        ? int.Parse(loopVariableEvalMatch.Groups["count"].Value, CultureInfo.InvariantCulture)
                        : 10;

                    for (int i = 0; i < count; i++)
                    {
                        evaluator.Variables[args.Count > 1 ? args[1].Trim() : "i"] = i + from;
                        results.Add(args.Count > 0 ? evaluator.Evaluate(args[0]) : i + from);
                    }
                }

                value = results;

                return true;
            }
            else
            {
                return false;
            }
        }

        #region Singleton          

        private static LoopEvaluation instance;

        public static LoopEvaluation Instance => instance ??= new LoopEvaluation();

        private LoopEvaluation()
        { }

        #endregion Singleton

    }
}
