using CodingSeb.ExpressionEvaluator;
using System;

namespace NppPowerTools.Utils.Evaluations
{
    public class PDFEvaluation : IVariableEvaluation, IFunctionEvaluation, IEvaluatorInitializator
    {
        public bool TryEvaluate(object sender, VariableEvaluationEventArg e)
        {
            if(e.Name.Equals("pdf", StringComparison.OrdinalIgnoreCase))
            {
                e.Value = new PDFFile();
            }

            return e.HasValue;
        }

        public bool TryEvaluate(object sender, FunctionEvaluationEventArg e)
        {
            if (e.Name.Equals("pdf", StringComparison.OrdinalIgnoreCase))
            {
                if (e.Args.Count > 0 && e.EvaluateArg(0) is Delegate del)
                {
                    e.Value = new PDFFile()
                    {
                        ComposeAction = del
                    };
                }
                else
                {
                    e.Value = new PDFFile();
                }
            }

            return e.FunctionReturnedValue;
        }

        public void Init(ExpressionEvaluator evaluator)
        {
            evaluator.StaticTypesForExtensionsMethods.Add(typeof(QuestPDF.Fluent.AlignmentExtensions));
            evaluator.StaticTypesForExtensionsMethods.Add(typeof(QuestPDF.Fluent.BorderExtensions));
            evaluator.StaticTypesForExtensionsMethods.Add(typeof(QuestPDF.Fluent.ComponentExtensions));
            evaluator.StaticTypesForExtensionsMethods.Add(typeof(QuestPDF.Fluent.ConstrainedExtensions));
            evaluator.StaticTypesForExtensionsMethods.Add(typeof(QuestPDF.Fluent.ElementExtensions));
            evaluator.StaticTypesForExtensionsMethods.Add(typeof(QuestPDF.Fluent.ExtendExtensions));
            evaluator.StaticTypesForExtensionsMethods.Add(typeof(QuestPDF.Fluent.GenerateExtensions));
            evaluator.StaticTypesForExtensionsMethods.Add(typeof(QuestPDF.Fluent.PaddingExtensions));
            evaluator.StaticTypesForExtensionsMethods.Add(typeof(QuestPDF.Fluent.PageExtensions));
            evaluator.StaticTypesForExtensionsMethods.Add(typeof(QuestPDF.Fluent.RowExtensions));
            evaluator.StaticTypesForExtensionsMethods.Add(typeof(QuestPDF.Fluent.SectionExtensions));
            evaluator.StaticTypesForExtensionsMethods.Add(typeof(QuestPDF.Fluent.StackExtensions));
            evaluator.StaticTypesForExtensionsMethods.Add(typeof(QuestPDF.Fluent.TextStyleExtensions));
        }

        #region Singleton          

        private static PDFEvaluation instance;

        public static PDFEvaluation Instance => instance ??= new PDFEvaluation();

        private PDFEvaluation()
        { }

        #endregion

    }
}
