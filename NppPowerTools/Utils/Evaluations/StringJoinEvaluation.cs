using CodingSeb.ExpressionEvaluator;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NppPowerTools.Utils.Evaluations
{
    public class StringJoinEvaluation : IVariableEvaluation, IFunctionEvaluation
    {
        public bool TryEvaluate(object sender, VariableEvaluationEventArg e)
        {
            if ((e.Name.Equals("stringjoin", System.StringComparison.OrdinalIgnoreCase) || e.Name.Equals("sjoin", System.StringComparison.OrdinalIgnoreCase) || e.Name.Equals("sj", System.StringComparison.OrdinalIgnoreCase) || e.Name.Equals("j", System.StringComparison.OrdinalIgnoreCase)) && e.This is IEnumerable enumerableForJoin)
            {
                e.Value = string.Join(BNpp.CurrentEOL, enumerableForJoin.Cast<object>());

                return true;
            }
            else
            {
                return false;
            }
        }

        public bool TryEvaluate(object sender, FunctionEvaluationEventArg e)
        {
            if ((e.Name.Equals("stringjoin", System.StringComparison.OrdinalIgnoreCase) ||e.Name.Equals("sjoin", System.StringComparison.OrdinalIgnoreCase) || e.Name.Equals("sj", System.StringComparison.OrdinalIgnoreCase) || e.Name.Equals("j", System.StringComparison.OrdinalIgnoreCase)) && (e.This is IEnumerable<object> || (e.Args.Count > 1 && e.EvaluateArg(1) is List<object>)))
            {
                if (e.This is List<object> list)
                {
                    e.Value = string.Join(e.Args.Count > 0 ? e.EvaluateArg<string>(0) : "\r\n", list);
                }
                else if (e.Args.Count > 1 && e.EvaluateArg(1) is List<object> list2)
                {
                    e.Value = string.Join(e.EvaluateArg<string>(0), list2);
                }

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
