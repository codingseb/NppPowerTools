using CodingSeb.ExpressionEvaluator;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NppPowerTools.Utils.Evaluations
{
    public sealed class JsonEvaluation : IVariableEvaluation, IFunctionEvaluation, IIndexingEvaluation
    {
        public bool TryEvaluate(object sender, VariableEvaluationEventArg e)
        {
            if (e.Name.Equals("json", System.StringComparison.OrdinalIgnoreCase) && e.This != null)
            {
                e.Value = JsonConvert.SerializeObject(e.This);
            }
            else if (e.Name.Equals("jsoni", System.StringComparison.OrdinalIgnoreCase) && e.This != null)
            {
                e.Value = JsonConvert.SerializeObject(e.This, Formatting.Indented);
            }
            else if ((e.Name.Equals("ujson", System.StringComparison.OrdinalIgnoreCase)
                || e.Name.Equals("unjson", System.StringComparison.OrdinalIgnoreCase)
                || e.Name.Equals("dejson", System.StringComparison.OrdinalIgnoreCase)
                || e.Name.Equals("djson", System.StringComparison.OrdinalIgnoreCase)) && e.This is string)
            {
                e.Value = JsonConvert.DeserializeObject(e.This.ToString());
            }
            else if(e.This is JToken jToken)
            {
                e.Value = jToken.SelectToken(e.Name, true);
            }

            return e.HasValue;
        }

        public bool TryEvaluate(object sender, FunctionEvaluationEventArg e)
        {
            if ((e.Name.Equals("ujson", System.StringComparison.OrdinalIgnoreCase)
                || e.Name.Equals("unjson", System.StringComparison.OrdinalIgnoreCase)
                || e.Name.Equals("dejson", System.StringComparison.OrdinalIgnoreCase)
                || e.Name.Equals("djson", System.StringComparison.OrdinalIgnoreCase)) && e.Args.Count > 0)
            {
                e.Value = JsonConvert.DeserializeObject(e.EvaluateArg(0).ToString());
            }

            return e.FunctionReturnedValue;
        }

        public bool TryEvaluate(object sender, IndexingPreEvaluationEventArg e)
        {
            if (e.This is JToken jToken && e.Args.Count == 1)
            {
                e.Value = jToken.SelectToken(e.EvaluateArg<string>(0), true);
            }

            return e.HasValue;
        }

        #region Singleton          

        private static JsonEvaluation instance;

        public static JsonEvaluation Instance => instance ??= new JsonEvaluation();

        private JsonEvaluation()
        { }

        #endregion
    }
}
