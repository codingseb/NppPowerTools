using CodingSeb.ExpressionEvaluator;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace NppPowerTools.Utils.Evaluations
{
    public class LoopEvaluation : IFunctionEvaluation
    {
        private static readonly Regex loopVariableEvalRegex = new Regex(@"^(lp|loop)(f(?<from>\d+)|(t()?<to>\d+)|[nc]?(?<count>\d+))*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public bool TryEvaluate(object sender, FunctionEvaluationEventArg e)
        {
            Match loopVariableEvalMatch = loopVariableEvalRegex.Match(e.Name);

            if (loopVariableEvalMatch.Success)
            {

                List<object> results = new List<object>();

                int from = loopVariableEvalMatch.Groups["from"].Success ? int.Parse(loopVariableEvalMatch.Groups["from"].Value, CultureInfo.InvariantCulture) : 1;

                if (loopVariableEvalMatch.Groups["to"].Success)
                {
                    for (int i = from; i <= int.Parse(loopVariableEvalMatch.Groups["to"].Value, CultureInfo.InvariantCulture); i++)
                    {
                        e.Evaluator.Variables[e.Args.Count > 1 ? e.Args[1].Trim() : "i"] = i;
                        results.Add(e.Evaluator.Evaluate(e.Args[0]).ToString());
                    }
                }
                else
                {
                    int count = loopVariableEvalMatch.Groups["count"].Success ? int.Parse(loopVariableEvalMatch.Groups["count"].Value, CultureInfo.InvariantCulture) : 10;

                    for (int i = 0; i < count; i++)
                    {
                        e.Evaluator.Variables[e.Args.Count > 1 ? e.Args[1].Trim() : "i"] = i + from;
                        results.Add(e.Evaluator.Evaluate(e.Args[0]));
                    }
                }

                e.Value = results;

                return true;
            }
            else
                return false;
        }
    }
}
