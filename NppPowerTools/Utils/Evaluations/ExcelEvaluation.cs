using CodingSeb.ExpressionEvaluator;
using OfficeOpenXml;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace NppPowerTools.Utils.Evaluations
{
    public sealed class ExcelEvaluation : IFunctionEvaluation, IVariableEvaluation, IEvaluatorInitializator
    {
        private static Regex excelSheetVariableRegex = new Regex(@"^(sheet[s]?|sh)(?<index>\d+)?$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public bool TryEvaluate(object sender, FunctionEvaluationEventArg e)
        {
            Match excelSheetVariableMatch;

            if ((excelSheetVariableMatch = excelSheetVariableRegex.Match(e.Name)).Success && (e.This is ExcelPackage || e.This is ExcelWorkbook))
            {
                ExcelWorkbook excelWorkbook = (e.This as ExcelWorkbook) ?? (e.This as ExcelPackage)?.Workbook;

                string sheetName = e.EvaluateArgs().OfType<string>().FirstOrDefault();

                e.Value = excelSheetVariableMatch.Groups["index"].Success
                    ? excelWorkbook.Worksheets[int.Parse(excelSheetVariableMatch.Groups["index"].Value)]
                    : string.IsNullOrEmpty(sheetName) ? (object)excelWorkbook.Worksheets : excelWorkbook.Worksheets[sheetName];
            }
            else if (e.This is ExcelPackage pack4Book)
            {
                if (e.Name.Equals("saveandopen", StringComparison.OrdinalIgnoreCase) || e.Name.Equals("saveopen", StringComparison.OrdinalIgnoreCase))
                {
                    pack4Book.File = e.EvaluateArgs().OfType<string>().Select(fileName => new FileInfo(fileName)).FirstOrDefault();

                    pack4Book.Save();
                    Process.Start(pack4Book.File.FullName);
                    e.Value = pack4Book;
                }
            }
            else if (e.Name.Equals("xl", StringComparison.OrdinalIgnoreCase) && e.Args.Count > 0 && e.This == null)
            {
                string filName = e.EvaluateArg(0).ToString();
                e.Value = new ExcelPackage(new FileInfo(filName));
            }

            return e.FunctionReturnedValue;
        }

        public bool TryEvaluate(object sender, VariableEvaluationEventArg e)
        {
            Match excelSheetVariableMatch;

            if (e.This is ExcelPackage pack4Book)
            {
                if (e.Name.Equals("book", StringComparison.OrdinalIgnoreCase) || e.Name.Equals("b", StringComparison.OrdinalIgnoreCase))
                {
                    e.Value = pack4Book.Workbook;
                }
                else if(e.Name.Equals("saveandopen", StringComparison.OrdinalIgnoreCase) ||e.Name.Equals("saveopen", StringComparison.OrdinalIgnoreCase))
                {
                    pack4Book.Save();
                    Process.Start(pack4Book.File.FullName);
                    e.Value = pack4Book;
                }
            }
            else if ((excelSheetVariableMatch = excelSheetVariableRegex.Match(e.Name)).Success && (e.This is ExcelPackage || e.This is ExcelWorkbook))
            {
                ExcelWorkbook excelWorkbook = (e.This as ExcelWorkbook) ?? (e.This as ExcelPackage)?.Workbook;

                e.Value = excelSheetVariableMatch.Groups["index"].Success
                    ? excelWorkbook.Worksheets[int.Parse(excelSheetVariableMatch.Groups["index"].Value)]
                    : (object)excelWorkbook.Worksheets;
            }

            return e.HasValue;
        }

        public void Init(ExpressionEvaluator evaluator)
        {
            evaluator.StaticTypesForExtensionsMethods.Add(typeof(CalculationExtension));
        }

        #region singleton
        private static ExcelEvaluation instance;

        public static ExcelEvaluation Instance
        {
            get
            {
                return instance ??= new ExcelEvaluation();
            }
        }

        private ExcelEvaluation()
        { }
        #endregion

    }
}
