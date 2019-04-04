using CodingSeb.ExpressionEvaluator;

namespace NppPowerTools.Utils.Evaluations
{
    interface IFunctionEvaluation
    {
        bool CanEvaluate(object sender, FunctionEvaluationEventArg e);
        void Evaluate(object sender, FunctionEvaluationEventArg e);
    }
}
