using CodingSeb.ExpressionEvaluator;

namespace NppPowerTools.Utils.Evaluations
{
    interface IVariableEvaluation
    {
        bool TryEvaluate(object sender, VariableEvaluationEventArg e);
    }
}
