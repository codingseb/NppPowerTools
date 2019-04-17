using CodingSeb.ExpressionEvaluator;
using Newtonsoft.Json;

namespace NppPowerTools.Utils.Evaluations
{
    public class JsonEvaluation : IVariableEvaluation, IFunctionEvaluation
    {
        public bool TryEvaluate(object sender, VariableEvaluationEventArg e)
        {
            if (e.Name.Equals("json", System.StringComparison.OrdinalIgnoreCase) && e.This != null)
            {
                e.Value = JsonConvert.SerializeObject(e.This, Formatting.Indented);
            }
            else if ((e.Name.Equals("ujson", System.StringComparison.OrdinalIgnoreCase)
                || e.Name.Equals("unjson", System.StringComparison.OrdinalIgnoreCase)) && e.This is string)
            {
                e.Value = JsonConvert.DeserializeObject(e.This.ToString());
            }

            return e.HasValue;
        }

        public bool TryEvaluate(object sender, FunctionEvaluationEventArg e)
        {
            return false;
        }
    }
}
