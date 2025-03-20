
using Kbg.NppPluginNET;
using System;
using System.Collections;
using System.Linq;

namespace NppPowerTools.Utils.Evaluations
{
    public class StringEvaluation : IVariableEvaluation, IFunctionEvaluation
    {
        public bool TryEvaluate(object sender, VariableEvaluationEventArg e)
        {
            if ((e.Name.Equals("stringjoin", StringComparison.OrdinalIgnoreCase) || e.Name.Equals("j", StringComparison.OrdinalIgnoreCase) || e.Name.Equals("join", StringComparison.OrdinalIgnoreCase)) && e.This is IEnumerable enumerableForJoin)
            {
                e.Value = string.Join("", enumerableForJoin.Cast<object>());

                return true;
            }
            if ((e.Name.Equals("spacejoin", StringComparison.OrdinalIgnoreCase) || e.Name.Equals("sjoin", StringComparison.OrdinalIgnoreCase) || e.Name.Equals("sj", StringComparison.OrdinalIgnoreCase)) && e.This is IEnumerable enumerableForSJoin)
            {
                e.Value = string.Join(" ", enumerableForSJoin.Cast<object>());

                return true;
            }
            if ((e.Name.Equals("linejoin", StringComparison.OrdinalIgnoreCase) || e.Name.Equals("ljoin", StringComparison.OrdinalIgnoreCase) || e.Name.Equals("lj", StringComparison.OrdinalIgnoreCase)) && e.This is IEnumerable enumerableForLJoin)
            {
                e.Value = string.Join(Npp.CurrentEOL, enumerableForLJoin.Cast<object>());

                return true;
            }
            else
            {
                return false;
            }
        }

        public bool TryEvaluate(object sender, FunctionEvaluationEventArg e)
        {
            if ((e.Name.Equals("stringjoin", StringComparison.OrdinalIgnoreCase) || e.Name.Equals("sjoin", StringComparison.OrdinalIgnoreCase) || e.Name.Equals("sj", StringComparison.OrdinalIgnoreCase) || e.Name.Equals("j", StringComparison.OrdinalIgnoreCase)) && (e.This is IEnumerable || (e.Args.Count > 1 && e.EvaluateArg(1) is IEnumerable)))
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
            else if (e.Name.Equals("format", StringComparison.OrdinalIgnoreCase) && e.This is string format)
            {
                e.Value = string.Format(format, e.EvaluateArgs());
            }

            return e.FunctionReturnedValue;
        }
    }
}
