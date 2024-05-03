namespace NppPowerTools.Utils.Evaluations
{
    public interface IFunctionEvaluation
    {
        bool TryEvaluate(object sender, FunctionEvaluationEventArg e);
    }
}
