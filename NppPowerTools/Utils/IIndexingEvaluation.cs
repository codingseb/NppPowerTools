using CodingSeb.ExpressionEvaluator;

namespace NppPowerTools.Utils.Evaluations
{
    public interface IIndexingEvaluation
    {
        bool TryEvaluate(object sender, IndexingPreEvaluationEventArg e);
    }
}