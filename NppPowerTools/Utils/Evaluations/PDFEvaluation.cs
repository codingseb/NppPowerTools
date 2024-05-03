using QuestPDF.Fluent;
using System;
using System.Collections.Generic;
using System.Drawing;
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
                if (e.This == null)
                {
                    e.Value = new PDFFile();
                }
                else if (e.This is Bitmap bitmap)
                {
                    e.Value = new PDFFile()
                    {
                        FirstAction = container => container.Page(pageDescriptor => pageDescriptor.Content().Image(bitmap.ToByteArray()))
                    };
                }
                else if (e.This is string text)
                {
                e.Value = new PDFFile()
                {
                    FirstAction = container => container.Page(pageDescriptor => pageDescriptor.Content().Text(text))
                };
                }
            }
            else if (e.This is PDFFile pDFFile && (e.Name.Equals("saveandopen", StringComparison.OrdinalIgnoreCase) || e.Name.Equals("saveopen", StringComparison.OrdinalIgnoreCase)))
            {
                pDFFile.SaveOpen(Config.Instance.PDFDefaultFileName);
                e.Value = pDFFile;
            }

            return e.HasValue;
        }

        public bool TryEvaluate(object sender, FunctionEvaluationEventArg e)
        {
            if (e.Name.Equals("pdf", StringComparison.OrdinalIgnoreCase))
            {
                PDFFile pdf = null;

                if (e.Args.Count > 0 && e.EvaluateArg(0) is Delegate del)
                {
                    pdf = new PDFFile()
                    {
                        ComposeAction = del
                    };
                }
                else
                {
                    pdf = new PDFFile();
                }

                if (e.This is Bitmap bitmap)
                {
                    pdf.FirstAction = container => container.Page(pageDescriptor => pageDescriptor.Content().Image(bitmap.ToByteArray()));
                }
                else if (e.This is string text)
                {
                    pdf.FirstAction = container => container.Page(pageDescriptor => pageDescriptor.Content().Text(text));
                }

                e.Value = pdf;
            }
            else if (e.This is PDFFile pDFFile && (e.Name.Equals("saveandopen", StringComparison.OrdinalIgnoreCase) || e.Name.Equals("saveopen", StringComparison.OrdinalIgnoreCase)))
            {
                if (e.Args.Count > 0 && e.EvaluateArg(0) is string fileName)
                    pDFFile.SaveOpen(fileName);
                else
                    pDFFile.SaveOpen(Config.Instance.PDFDefaultFileName);

                e.Value = pDFFile;
            }

            return e.FunctionReturnedValue;
        }

        public void Init(ExpressionEvaluator evaluator)
        {
            if (extentionTypes == null)
            {
                extentionTypes = typeof(QuestPDF.Fluent.GenerateExtensions).Assembly.GetTypes()
                    .Where(t => t.Namespace?.StartsWith("QuestPDF.Fluent") == true && t.Name.EndsWith("Extensions")).ToList();
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
