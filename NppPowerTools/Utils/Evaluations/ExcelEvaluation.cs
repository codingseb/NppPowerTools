
using Microsoft.Win32;
using OfficeOpenXml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace NppPowerTools.Utils.Evaluations
{
    public sealed class ExcelEvaluation : IFunctionEvaluation, IVariableEvaluation, IEvaluatorInitializator
    {
        private static readonly Regex baseExcelRegex = new("(?<new>n(ew)?_?)?(xls?|excel)(?<default>_?d(ef(ault)?)?)?$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex excelSheetVariableRegex = new(@"^(sheet[s]?|sh)(?<index>\d+)?$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex toExcelRegex = new(@"^to_?(xl[s]?|excel)((?<noheader>_?no?_?h(ead(er)?)?)|(?<fatheader>_?(f(at)?|b(old)?)_?h(ead(er)?)?)|(?<autofilter>_?(a(uto)?)_?f(ilter)?)|(?<freeze>_?fr(eeze)?_?r?(?<freezerow>\d+)(_c|_|c)(?<freezecolumn>\d+))|(?<columnautosize>_?(c(ol(umn)?)?)_?(a(uto)?)_?s(ize)?))*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public bool TryEvaluate(object sender, FunctionEvaluationEventArg e)
        {
            Match excelSheetVariableMatch;
            Match toExcelMatch;
            Match excelMatch;

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
                if (e.Name.Equals("save", StringComparison.OrdinalIgnoreCase) || e.Name.Equals("saveandopen", StringComparison.OrdinalIgnoreCase) || e.Name.Equals("saveopen", StringComparison.OrdinalIgnoreCase))
                {
                    pack4Book.File = e.EvaluateArgs().OfType<string>().Select(fileName => new FileInfo(fileName)).FirstOrDefault();

                    SaveFileDialog saveFileDialog = new()
                    {
                        Filter = "Excel file|*.xlsx",
                        AddExtension = true
                    };

                    if (pack4Book.File != null || saveFileDialog.ShowDialog() == true)
                    {
                        if (pack4Book.Workbook.Worksheets.Count == 0)
                            pack4Book.Workbook.Worksheets.Add(string.Format(Config.Instance.ExcelDefaultSheetName, 1));

                        pack4Book.File ??= new FileInfo(saveFileDialog.FileName);

                        pack4Book.Save();

                        if (!e.Name.Equals("save", StringComparison.OrdinalIgnoreCase))
                            Process.Start(pack4Book.File.FullName);
                    }

                    e.Value = pack4Book;
                }
            }
            else if ((excelMatch = baseExcelRegex.Match(e.Name)).Success && e.This == null)
            {
                var fileName = e.EvaluateArgs().OfType<string>().FirstOrDefault();

                if (!excelMatch.Groups["default"].Success)
                {
                    SaveFileDialog saveFileDialog = new()
                    {
                        Filter = "Excel file|*.xlsx",
                        AddExtension = true
                    };

                    if (fileName == null && saveFileDialog.ShowDialog() == true)
                        fileName = saveFileDialog.FileName;
                }

                fileName ??= Config.Instance.ExcelDefaultSheetName;

                if (excelMatch.Groups["new"].Success)
                    File.Delete(fileName);

                e.Value = string.IsNullOrEmpty(fileName) ? new ExcelPackage() : new ExcelPackage(new FileInfo(fileName));
            }
            else if (e.This is IEnumerable rowEnumerable && (toExcelMatch = toExcelRegex.Match(e.Name)).Success)
            {
                e.Value = IEnumerableToExcel(rowEnumerable, toExcelMatch, e.EvaluateArgs().ToList());
            }

            return e.FunctionReturnedValue;
        }

        public bool TryEvaluate(object sender, VariableEvaluationEventArg e)
        {
            Match excelSheetVariableMatch;
            Match toExcelMatch;
            Match excelMatch;

            if (e.This is ExcelPackage pack4Book)
            {
                if (e.Name.Equals("book", StringComparison.OrdinalIgnoreCase) || e.Name.Equals("b", StringComparison.OrdinalIgnoreCase))
                {
                    e.Value = pack4Book.Workbook;
                }
                else if (e.Name.Equals("save", StringComparison.OrdinalIgnoreCase) | e.Name.Equals("saveandopen", StringComparison.OrdinalIgnoreCase) || e.Name.Equals("saveopen", StringComparison.OrdinalIgnoreCase))
                {
                    SaveFileDialog saveFileDialog = new()
                    {
                        Filter = "Excel file|*.xlsx",
                        AddExtension = true
                    };

                    if (pack4Book.File != null || saveFileDialog.ShowDialog() == true)
                    {
                        if (pack4Book.Workbook.Worksheets.Count == 0)
                            pack4Book.Workbook.Worksheets.Add(string.Format(Config.Instance.ExcelDefaultSheetName, 1));

                        pack4Book.File ??= new FileInfo(saveFileDialog.FileName);

                        pack4Book.Save();

                        if (!e.Name.Equals("save", StringComparison.OrdinalIgnoreCase))
                            Process.Start(pack4Book.File.FullName);
                    }

                    e.Value = pack4Book;
                }
            }
            else if ((excelMatch = baseExcelRegex.Match(e.Name)).Success && e.This == null)
            {
                string fileName = null;

                if (!excelMatch.Groups["default"].Success)
                {
                    SaveFileDialog saveFileDialog = new()
                    {
                        Filter = "Excel file|*.xlsx",
                        AddExtension = true
                    };

                    if (fileName == null && saveFileDialog.ShowDialog() == true)
                        fileName = saveFileDialog.FileName;
                }

                fileName ??= Config.Instance.ExcelDefaultSheetName;

                if (excelMatch.Groups["new"].Success)
                    File.Delete(fileName);

                e.Value = string.IsNullOrEmpty(fileName)? new ExcelPackage() : new ExcelPackage(new FileInfo(fileName));
            }
            else if ((excelSheetVariableMatch = excelSheetVariableRegex.Match(e.Name)).Success && (e.This is ExcelPackage || e.This is ExcelWorkbook))
            {
                ExcelWorkbook excelWorkbook = (e.This as ExcelWorkbook) ?? (e.This as ExcelPackage)?.Workbook;

                e.Value = excelSheetVariableMatch.Groups["index"].Success
                    ? excelWorkbook.Worksheets[int.Parse(excelSheetVariableMatch.Groups["index"].Value)]
                    : (object)excelWorkbook.Worksheets;
            }
            else if (e.This is IEnumerable rowEnumerable && (toExcelMatch = toExcelRegex.Match(e.Name)).Success)
            {
                e.Value = IEnumerableToExcel(rowEnumerable, toExcelMatch);
            }

            return e.HasValue;
        }

        private object IEnumerableToExcel(IEnumerable rowEnumerable, Match toExcelMatch, List<object> args = null)
        {
            ExcelPackage pack4Book = new();

            IDictionary<string, object> config = args?.OfType<IDictionary<string, object>>().FirstOrDefault();

            string sheetName = config?.FirstOrDefault(kvp => Regex.IsMatch(kvp.Key, "^s(heet)?_?n(ame)?$", RegexOptions.IgnoreCase)).Value?.ToString();

            ExcelWorksheet sheet = pack4Book.Workbook.Worksheets.Add(string.Format(sheetName ?? Config.Instance.ExcelDefaultSheetName, pack4Book.Workbook.Worksheets.Count + 1));

            dynamic preDo = config?.FirstOrDefault(kvp => Regex.IsMatch(kvp.Key, "^((pre_?((do)|(execute)))|(before_?((do)|(execute)))|(((do)|(execute))_?((before)|(at_?(the_?)?((start)|(begining))))))$", RegexOptions.IgnoreCase)).Value;

            if (preDo != null)
            {
                MethodInfo method = preDo.Method as MethodInfo;
                method?.Invoke(preDo.Target, new object[] { new object[] { sheet } });
            }

            List<object> rowList = rowEnumerable.Cast<object>().ToList();

            Dictionary<string, int> headersColumns = new();

            var headersConfig = (config?.FirstOrDefault(kvp => Regex.IsMatch(kvp.Key, "^(headers?(_?names?)?)|(col(umns?)?(_?names?)?)$", RegexOptions.IgnoreCase) && kvp.Value is not string && kvp.Value is IEnumerable).Value as IEnumerable)?.OfType<string>()?.ToList();

            List<string> headers = headersConfig
                ?? args?
                    .Where(a => a is not string && a is not IDictionary<string, object>)
                    .OfType<IEnumerable>()
                    .FirstOrDefault()?
                    .OfType<string>()?
                    .ToList()
                ?? new List<string>();

            for (int r = 0; r < rowList.Count; r++)
            {
                if (rowList[r] is IDictionary<string, object> dict)
                {
                    dict.Keys.ToList().ForEach(name =>
                    {
                        if (!headersColumns.ContainsKey(name))
                        {
                            headers.Add(name);
                            headersColumns[name] = headers.Count;
                        }

                        if (dict[name] is DateTime dateTime)
                        {
                            sheet.SetValue(r + 1, headersColumns[name], dateTime);
                            sheet.Cells[r + 1, headersColumns[name]].Style.Numberformat.Format = Config.Instance.ExcelDateTimeDefaultFormat;
                        }
                        else if (dict[name].GetType().IsValueType)
                        {
                            sheet.SetValue(r + 1, headersColumns[name], dict[name]);
                        }
                        else
                        {
                            sheet.SetValue(r + 1, headersColumns[name], dict[name].ToString());
                        }
                    });
                }
                else if (rowList[r] is IEnumerable columnEnumerable)
                {
                    List<object> columnList = columnEnumerable.Cast<object>().ToList();

                    for (int c = 0; c < columnList.Count; c++)
                    {
                        sheet.SetValue(r + 1, c + 1, columnList[c].ToString());
                    }
                }
                else if (rowList[r] is DateTime dateTime)
                {
                    sheet.SetValue(r + 1, 1, dateTime);
                    sheet.Cells[r + 1, 1].Style.Numberformat.Format = Config.Instance.ExcelDateTimeDefaultFormat;
                }
                else if (rowList[r].GetType().IsValueType)
                {
                    sheet.SetValue(r + 1, 1, rowList[r]);
                }
            }

            if(headers.Count > 0
                && !toExcelMatch.Groups["noheader"].Success
                && !(config?.FirstOrDefault(kvp => Regex.IsMatch(kvp.Key, "^no?_?h(ead(er)?)?$", RegexOptions.IgnoreCase)).Value as bool?).GetValueOrDefault())
            {
                sheet.InsertRow(1, 1);

                for(int c = 0; c < headers.Count; c++)
                {
                    sheet.SetValue(1, c + 1, headers[c]);
                }
            }

            if (toExcelMatch.Groups["fatheader"].Success
                || (config?.FirstOrDefault(kvp => Regex.IsMatch(kvp.Key, "^(f(at)?|b(old)?)_?h(ead(er)?)?$", RegexOptions.IgnoreCase)).Value as bool?).GetValueOrDefault())
            {
                sheet.Cells[1, 1, 1, sheet.Dimension.Columns].Style.Font.Bold = true;
            }

            if (toExcelMatch.Groups["autofilter"].Success
                || (config?.FirstOrDefault(kvp => Regex.IsMatch(kvp.Key, "^(a(uto)?)_?f(ilter)?$", RegexOptions.IgnoreCase)).Value as bool?).GetValueOrDefault())
            {
                sheet.Cells[1, 1, sheet.Dimension.Rows, sheet.Dimension.Columns].AutoFilter = true;
            }

            if (toExcelMatch.Groups["freeze"].Success)
            {
                int row = int.Parse(toExcelMatch.Groups["freezerow"].Value);
                int column = int.Parse(toExcelMatch.Groups["freezecolumn"].Value);

                sheet.View.FreezePanes(row, column);
            }
            else if(config?.FirstOrDefault(kvp => Regex.IsMatch(kvp.Key, "^fr(eeze)?$", RegexOptions.IgnoreCase)).Value is object freezeObject)
            {
                int row = 1;
                int column = 1;

                if (freezeObject is int freezeInt)
                {
                    row = freezeInt;
                }
                else if (freezeObject is Dictionary<string, object> freezeDictionary)
                {
                    if (freezeDictionary.FirstOrDefault(kvp => Regex.IsMatch(kvp.Key, "^(r(ows?)?)|y$", RegexOptions.IgnoreCase)).Value is int freezeRow)
                        row = freezeRow;
                    if (freezeDictionary.FirstOrDefault(kvp => Regex.IsMatch(kvp.Key, "^(c(ol(umn)?s?)?)|y$", RegexOptions.IgnoreCase)).Value is int freezeColumn)
                        column = freezeColumn;
                }
                else if(freezeObject is IEnumerable freezeEnum)
                {
                    var values = freezeEnum.OfType<int>().ToList();
                    if (values.Count > 0)
                        row = values[0];
                    if (values.Count > 1)
                        column = values[1];
                }

                sheet.View.FreezePanes(row, column);
            }
            if (toExcelMatch.Groups["columnautosize"].Success
                || (config?.FirstOrDefault(kvp => Regex.IsMatch(kvp.Key, "^(c(ol(umn)?)?)_?(a(uto)?)_?s(ize)?$", RegexOptions.IgnoreCase)).Value as bool?).GetValueOrDefault())
            {
                sheet.Cells[sheet.Dimension.Address].AutoFitColumns();
            }

            dynamic postDo = config?.FirstOrDefault(kvp => Regex.IsMatch(kvp.Key, "^((post_?((do)|(execute)))|(after_?((do)|(execute)))|(((do)|(execute))_?((after)|(at_?(the_?)?end))))$", RegexOptions.IgnoreCase)).Value;

            if (postDo != null)
            {
                MethodInfo method = postDo.Method as MethodInfo;
                method?.Invoke(postDo.Target, new object[] { new object[] { sheet } });
            }

            return pack4Book;
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
