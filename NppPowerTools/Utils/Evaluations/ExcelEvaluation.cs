using CodingSeb.ExpressionEvaluator;
using OfficeOpenXml;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace NppPowerTools.Utils.Evaluations
{
    public class ExcelEvaluation : IFunctionEvaluation, IVariableEvaluation, IEvaluatorInitializator
    {
        private static Regex excelSheetVariableRegex = new Regex(@"^(sheet[s]?|sh)(?<index>\d+)?$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public bool TryEvaluate(object sender, FunctionEvaluationEventArg e)
        {
            if(e.Name.Equals("xl", StringComparison.OrdinalIgnoreCase) && e.Args.Count > 0)
            {
                string filName = e.EvaluateArg(0).ToString();
                e.Value = new ExcelPackage(new FileInfo(filName)); 
            }
            else
            {
                return false;
            }

            return true;
        }

        public bool TryEvaluate(object sender, VariableEvaluationEventArg e)
        {
            Match excelSheetVariableMatch;

            if ((e.Name.Equals("book", StringComparison.OrdinalIgnoreCase) || e.Name.Equals("b", StringComparison.OrdinalIgnoreCase)) && e.This is ExcelPackage pack4Book)
                e.Value = pack4Book.Workbook;
            else if((excelSheetVariableMatch = excelSheetVariableRegex.Match(e.Name)).Success && (e.This is ExcelPackage || e.This is ExcelWorkbook))
            {
                ExcelWorkbook excelWorkbook = (e.This as ExcelWorkbook) ?? (e.This as ExcelPackage)?.Workbook;

                if (excelSheetVariableMatch.Groups["index"].Success)
                    e.Value = excelWorkbook.Worksheets[int.Parse(excelSheetVariableMatch.Groups["index"].Value)];
                else
                    e.Value = excelWorkbook.Worksheets;
            }
            else
                return false;

            return true;
        }

        public void Init(ExpressionEvaluator evaluator)
        {
            evaluator.StaticTypesForExtensionsMethods.Add(typeof(CalculationExtension));
        }

        #region singleton
        private static ExcelEvaluation instance = null;

        public static ExcelEvaluation Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ExcelEvaluation();
                }

                return instance;
            }
        }

        private ExcelEvaluation()
        { }
        #endregion

    }
}
