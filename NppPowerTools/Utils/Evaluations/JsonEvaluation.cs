using CodingSeb.ExpressionEvaluator;
using Newtonsoft.Json;

namespace NppPowerTools.Utils.Evaluations
{
    public class JsonEvaluation : IVariableEvaluation, IFunctionEvaluation
    {
        public bool TryEvaluate(object sender, VariableEvaluationEventArg e)
        {
            if (e.Name.ToLower().Equals("json") && e.This != null)
            {
                e.Value = JsonConvert.SerializeObject(e.This, Formatting.Indented);

                return true;
            }
            else
                return false;
        }

        public bool TryEvaluate(object sender, FunctionEvaluationEventArg e)
        {
            return false;
        }
    }
}
