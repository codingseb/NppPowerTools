namespace NppPowerTools.Utils.Evaluations
{
    public interface IVariableEvaluation
    {
        bool TryEvaluate(object sender, VariableEvaluationEventArg e);
    }
}
