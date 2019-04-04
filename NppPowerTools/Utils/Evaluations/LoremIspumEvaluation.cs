using CodingSeb.ExpressionEvaluator;
using System.Globalization;
using System.Text.RegularExpressions;

namespace NppPowerTools.Utils.Evaluations
{
    public class LoremIspumEvaluation : IVariableEvaluation
    {
        private static readonly Regex loremIspumVariableEvalRegex = new Regex(@"^(li|loremipsum|lorem|ipsum)(w(?<words>\d+)|wl(?<wordsPerLine>\d+)|(?<language>fr|en|la)|l?(?<lines>\d+))*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private Match loremIspumVariableEvalMatch = null;

        public bool CanEvaluate(object sender, VariableEvaluationEventArg e)
        {
            loremIspumVariableEvalMatch = loremIspumVariableEvalRegex.Match(e.Name);

            return loremIspumVariableEvalMatch.Success;
        }

        public void Evaluate(object sender, VariableEvaluationEventArg e)
        {
            LoremIpsum li = new LoremIpsum();

            if (loremIspumVariableEvalMatch.Groups["language"].Success)
                li.CurrentLanguage = loremIspumVariableEvalMatch.Groups["language"].Value.ToLower();

            int wordPerLine = loremIspumVariableEvalMatch.Groups["wordsPerLine"].Success ? int.Parse(loremIspumVariableEvalMatch.Groups["wordsPerLine"].Value, CultureInfo.InvariantCulture) : 10;

            if (loremIspumVariableEvalMatch.Groups["words"].Success)
                e.Value = li.GetWords(int.Parse(loremIspumVariableEvalMatch.Groups["words"].Value, CultureInfo.InvariantCulture), wordPerLine);
            else if (loremIspumVariableEvalMatch.Groups["lines"].Success)
                e.Value = li.GetLines(int.Parse(loremIspumVariableEvalMatch.Groups["lines"].Value, CultureInfo.InvariantCulture), wordPerLine);
            else
                e.Value = li.GetWords(100, wordPerLine);
        }
    }
}
