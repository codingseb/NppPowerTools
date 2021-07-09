using CodingSeb.ExpressionEvaluator;
using HtmlAgilityPack;
using System;

namespace NppPowerTools.Utils.Evaluations
{
    public class HTMLEvaluation : IFunctionEvaluation, IVariableEvaluation
    {
        public bool TryEvaluate(object sender, VariableEvaluationEventArg e)
        {
            if(e.Name.Equals("html", StringComparison.OrdinalIgnoreCase))
            {
                HtmlDocument htmlDocument = new HtmlDocument();

                if(e.This is string sHTML)
                    htmlDocument.LoadHtml(sHTML);

                e.Value = htmlDocument.DocumentNode;
            }

            return e.HasValue;
        }

        public bool TryEvaluate(object sender, FunctionEvaluationEventArg e)
        {
            if (e.Name.Equals("html", StringComparison.OrdinalIgnoreCase))
            {
                HtmlDocument htmlDocument = new HtmlDocument();

                if (e.This is string sHTML || e.Args.Count > 0 && !string.IsNullOrEmpty(sHTML = e.EvaluateArg<string>(0)))
                    htmlDocument.LoadHtml(sHTML);

                e.Value = htmlDocument.DocumentNode;
            }

            return e.FunctionReturnedValue;
        }

        #region Singleton          

        private static HTMLEvaluation instance;

        public static HTMLEvaluation Instance => instance ??= new HTMLEvaluation();

        private HTMLEvaluation()
        { }

        #endregion

    }
}
