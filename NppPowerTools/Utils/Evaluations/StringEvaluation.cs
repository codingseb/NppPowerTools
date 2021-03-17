using CodingSeb.ExpressionEvaluator;
using System.Collections;
using System.Linq;

namespace NppPowerTools.Utils.Evaluations
{
    public class StringEvaluation : IVariableEvaluation, IFunctionEvaluation
    {
        public bool TryEvaluate(object sender, VariableEvaluationEventArg e)
        {
            if ((e.Name.Equals("stringjoin", System.StringComparison.OrdinalIgnoreCase) || e.Name.Equals("sjoin", System.StringComparison.OrdinalIgnoreCase) || e.Name.Equals("sj", System.StringComparison.OrdinalIgnoreCase) || e.Name.Equals("j", System.StringComparison.OrdinalIgnoreCase)) && e.This is IEnumerable enumerableForJoin)
            {
                e.Value = string.Join("", enumerableForJoin.Cast<object>());

                return true;
            }
            if ((e.Name.Equals("linejoin", System.StringComparison.OrdinalIgnoreCase) || e.Name.Equals("ljoin", System.StringComparison.OrdinalIgnoreCase) || e.Name.Equals("lj", System.StringComparison.OrdinalIgnoreCase)) && e.This is IEnumerable enumerableForLJoin)
            {
                e.Value = string.Join(BNpp.CurrentEOL, enumerableForLJoin.Cast<object>());

                return true;
            }
            else
            {
                return false;
            }
        }

        public bool TryEvaluate(object sender, FunctionEvaluationEventArg e)
        {
            if ((e.Name.Equals("stringjoin", System.StringComparison.OrdinalIgnoreCase) ||e.Name.Equals("sjoin", System.StringComparison.OrdinalIgnoreCase) || e.Name.Equals("sj", System.StringComparison.OrdinalIgnoreCase) || e.Name.Equals("j", System.StringComparison.OrdinalIgnoreCase)) && (e.This is IEnumerable || (e.Args.Count > 1 && e.EvaluateArg(1) is IEnumerable)))
            {
                if (e.This is IEnumerable enumerable)
                {
                    e.Value = string.Join(e.Args.Count > 0 ? e.EvaluateArg<string>(0) : "", enumerable.Cast<object>());
                }
                else if (e.Args.Count > 1 && e.EvaluateArg(1) is IEnumerable enumerable2)
                {
                    e.Value = string.Join(e.EvaluateArg<string>(0), enumerable2.Cast<object>());
                }
            }
            else if (e.Name.Equals("format", System.StringComparison.OrdinalIgnoreCase) && e.This is string format)
            {
                e.Value = string.Format(format, e.EvaluateArgs());
            }

            return e.FunctionReturnedValue;
        }
    }
}
