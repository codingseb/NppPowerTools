namespace NppPowerTools.Utils
{
    public static class ResultExtention
    {
        public static string ToStringOutput(this object result)
        {
            string sResult = result?.ToString() ?? Config.Instance.TextWhenResultIsNull;
            return CustomEvaluations.Print + sResult;
        }
    }
}
