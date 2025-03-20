using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NppPowerTools.Utils.Evaluations
{
    public sealed class DBEvaluation : IFunctionEvaluation, IVariableEvaluation
    {
        private static readonly Regex sqlRegex = new(@"sql((?<full>f(ull)?)|(?<number>\d+))?_(?<connection>[a-zA-Z0-9]+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex valuePropRegex = new("^(to_?)?v(al(ue)?)?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex csvRegex = new("^(to_?)?csv(?<noheader>_?no?(h(ead(er)?)?))?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex excelRegex = new("^(to_?)?((xl(sx?)?)|(ex(cel)?))((?<noheader>_?no?(h(ead(er)?)?))|_(?<filter>f(ilter)?))?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public bool TryEvaluate(object sender, FunctionEvaluationEventArg e)
        {
            Match match = sqlRegex.Match(e.Name);

            if(match.Success)
            {
                DBConfig dBConfig = Config.Instance.DBConfigs.FirstOrDefault(dBConfig => dBConfig.Id.Equals(match.Groups["connection"].Value, StringComparison.OrdinalIgnoreCase));

                if (dBConfig != null)
                {
                    using var dbConnection = dBConfig.GetConnection();
                    dbConnection.Open();

                    if (!string.IsNullOrWhiteSpace(dBConfig.InitCommands))
                    {
                        using var initCommand = dbConnection.CreateCommand();
                        initCommand.CommandText = dBConfig.InitCommands;
                        initCommand.ExecuteNonQuery();
                    }

                    using var command = dbConnection.CreateCommand();
                    command.CommandText = e.EvaluateArg<string>(0);
                    Func<int, bool> numberLimit;

                    if(match.Groups["full"].Success)
                    {
                        numberLimit = _ => true;
                    }
                    else if(match.Groups["number"].Success)
                    {
                        int number = int.Parse(match.Groups["number"].Value);

                        numberLimit = i => i < number;
                    }
                    else if(Config.Instance.DBAutoLimitRequests)
                    {
                        numberLimit = i => i < Config.Instance.DBAutoLimitRequestsValue;
                    }
                    else
                    {
                        numberLimit = _ => true;
                    }

                    if (e.Args.Count > 1)
                    {
                        Delegate lambda = e.EvaluateArg<Delegate>(1);
                    }
                    else
                    {
                        var reader = command.ExecuteReader();

                        List<dynamic> result = new();

                        for (int i = 0; reader.Read() && numberLimit(i); i++)
                        {
                            result.Add(ToExpando(reader));
                        }

                        e.Value = new DBResultViewModel()
                        {
                            ColumnsNames = Enumerable.Range(0, reader.FieldCount).Select(n => reader.GetName(n)).ToList(),
                            Results = result
                        };
                    }
                }
                else
                {
                    throw new ExpressionEvaluatorSyntaxErrorException($"No DB Connection with ID = {match.Groups["connection"].Value}");
                }
            }
            else if (e.This is DBResultViewModel dBResultViewModel)
            {
                Match csvMatch;
                Match excelMatch;

                if (valuePropRegex.IsMatch(e.Name))
                {
                    if (dBResultViewModel.Results.Count == 1 && dBResultViewModel.Results[0] is IDictionary<string, object> dict)
                    {
                        if (dict.Count == 1)
                            e.Value = dict[dict.Keys.First()];
                        else
                            e.Value = dBResultViewModel.Results[0];
                    }
                    else
                    {
                        e.Value = dBResultViewModel.Results;
                    }
                }
                else if ((csvMatch = csvRegex.Match(e.Name)).Success)
                {
                    StringBuilder stringBuilder = new();

                    object[] args = e.EvaluateArgs();

                    string separator = args.OfType<string>().FirstOrDefault() ?? ";";

                    if (!csvMatch.Groups["noheader"].Success)
                    {
                        stringBuilder.AppendLine(string.Join(separator, dBResultViewModel.ColumnsNames.Select(n => "\"" + n + "\"")));
                    }

                    foreach (IDictionary<string, object> result in dBResultViewModel.Results)
                    {
                        stringBuilder.AppendLine(string.Join(separator, dBResultViewModel.ColumnsNames.Select(n =>
                        {
                            if (result[n] is string s)
                                return "\"" + s.Replace("\"", "\"\"");
                            else
                                return result[n]?.ToString() ?? string.Empty;
                        })));
                    }

                    e.Value = stringBuilder.ToString();
                }
                else if((excelMatch = excelRegex.Match(e.Name)).Success)
                {
                    var args = e.EvaluateArgs();
                    var options = args.OfType<IDictionary<string, object>>().FirstOrDefault();
                    string fileName = args.OfType<string>().FirstOrDefault();

                    e.Value = CreateExcel(dBResultViewModel, excelMatch, options, fileName);
                }
            }

            return e.FunctionReturnedValue;
        }

        public bool TryEvaluate(object sender, VariableEvaluationEventArg e)
        {
            if (e.Name.Equals("test"))
            {
                e.Value = DateTime.Now.ToString("dd.MM.yyyy");
                return true;
            }

            if (e.This is DBResultViewModel dBResultViewModel)
            {
                Match csvMatch;
                Match excelMatch;

                if (valuePropRegex.IsMatch(e.Name))
                {
                    if (dBResultViewModel.Results.Count == 1 && dBResultViewModel.Results[0] is IDictionary<string, object> dict)
                    {
                        if (dict.Count == 1)
                            e.Value = dict[dict.Keys.First()];
                        else
                            e.Value = dBResultViewModel.Results[0];
                    }
                    else
                    {
                        e.Value = dBResultViewModel.Results;
                    }
                }
                else if ((csvMatch = csvRegex.Match(e.Name)).Success)
                {
                    StringBuilder stringBuilder = new();

                    if(!csvMatch.Groups["noheader"].Success)
                    {
                        stringBuilder.AppendLine(string.Join(";", dBResultViewModel.ColumnsNames.Select(n => "\"" + n + "\"")));
                    }

                    foreach(IDictionary<string, object> result in dBResultViewModel.Results)
                    {
                        stringBuilder.AppendLine(string.Join(";", dBResultViewModel.ColumnsNames.Select(n => result[n].ToString().Replace("\"", "\"\""))));
                    }

                    e.Value = stringBuilder.ToString();
                }
                else if ((excelMatch = excelRegex.Match(e.Name)).Success)
                {
                    e.Value = CreateExcel(dBResultViewModel, excelMatch);
                }
            }

            return e.HasValue;
        }

        private static dynamic ToExpando(IDataRecord record)
        {
            var expandoObject = new ExpandoObject() as IDictionary<string, object>;

            for (var i = 0; i < record.FieldCount; i++)
                expandoObject.Add(record.GetName(i), record[i]);

            return expandoObject;
        }

        private static ExcelPackage CreateExcel(DBResultViewModel dBResultViewModel, Match excelMatch, IDictionary<string, object> options = null, string fileName = null)
        {
            var package = new ExcelPackage();

            if(options != null)
                options = new Dictionary<string, object>(options, StringComparer.OrdinalIgnoreCase);

            fileName ??= options?.GetFirstValueOfKeys<string>("f", "fn", "file", "filename");

            if (!string.IsNullOrWhiteSpace(fileName))
                package.File = new FileInfo(fileName);

            string sheetName = options?.GetFirstValueOfKeys<string>("sn","sname", "sheetname") ?? "data";

            var sheet = package.Workbook.Worksheets.Add(sheetName);

            int row = 1;

            if (!excelMatch.Groups["noheader"].Success)
            {
                for (int col = 1; col <= dBResultViewModel.ColumnsNames.Count; col++)
                {
                    sheet.Cells[row, col].Value = dBResultViewModel.ColumnsNames[col - 1];
                }
                row++;
            }

            foreach (IDictionary<string, object> result in dBResultViewModel.Results)
            {
                for (int col = 1; col <= dBResultViewModel.ColumnsNames.Count; col++)
                {
                    sheet.Cells[row, col].Value = result[dBResultViewModel.ColumnsNames[col - 1]];
                }

                row++;
            }

            for (int col = 1; col <= dBResultViewModel.ColumnsNames.Count; col++)
            {
                sheet.Column(col).AutoFit();
            }

            return package;
        }

        #region Singleton          

        private static DBEvaluation instance;

        public static DBEvaluation Instance => instance ??= new DBEvaluation();

        private DBEvaluation()
        { }

        #endregion

    }
}
