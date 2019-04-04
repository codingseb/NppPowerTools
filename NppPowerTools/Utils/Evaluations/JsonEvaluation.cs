using CodingSeb.ExpressionEvaluator;
using Newtonsoft.Json;
using System;

namespace NppPowerTools.Utils.Evaluations
{
    public class JsonEvaluation : IVariableEvaluation, IFunctionEvaluation
    {
        public bool CanEvaluate(object sender, VariableEvaluationEventArg e) => e.Name.ToLower().Equals("json") && e.This != null;

        public bool CanEvaluate(object sender, FunctionEvaluationEventArg e)
        {
            return false;
        }

        public void Evaluate(object sender, VariableEvaluationEventArg e) => e.Value = JsonConvert.SerializeObject(e.This, Formatting.Indented);

        public void Evaluate(object sender, FunctionEvaluationEventArg e)
        {
            throw new NotImplementedException();
        }
    }
}
