using CodingSeb.ExpressionEvaluator;

namespace NppPowerTools.Utils.Evaluations
{
    interface IVariableEvaluation
    {
        bool CanEvaluate(object sender, VariableEvaluationEventArg e);

        void Evaluate(object sender, VariableEvaluationEventArg e);
    }
}
