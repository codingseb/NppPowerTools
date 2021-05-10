using CodingSeb.ExpressionEvaluator;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NppPowerTools.Utils.Evaluations
{
    public sealed class PDFEvaluation : IVariableEvaluation, IFunctionEvaluation, IEvaluatorInitializator
    {
        private static List<Type> extentionTypes;

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
            if(extentionTypes == null)
            {
                extentionTypes = typeof(QuestPDF.Fluent.GenerateExtensions).Assembly.GetTypes()
                    .Where(t => t.Namespace.StartsWith("QuestPDF.Fluent") && t.Name.EndsWith("Extensions")).ToList();
            }

            evaluator.StaticTypesForExtensionsMethods = evaluator.StaticTypesForExtensionsMethods.Concat(extentionTypes).ToList(); 
        }

        #region Singleton          

        private static PDFEvaluation instance;

        public static PDFEvaluation Instance => instance ??= new PDFEvaluation();

        private PDFEvaluation()
        { }

        #endregion

    }
}
