using CodingSeb.ExpressionEvaluator;

namespace NppPowerTools.Utils.Evaluations
{
    interface IFunctionEvaluation
    {
        bool TryEvaluate(object sender, FunctionEvaluationEventArg e);
    }
}
