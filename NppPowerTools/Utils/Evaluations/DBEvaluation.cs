using CodingSeb.ExpressionEvaluator;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NppPowerTools.Utils.Evaluations
{
    public sealed class DBEvaluation : IFunctionEvaluation, IVariableEvaluation
    {
        private static readonly Regex sqlRegex = new Regex(@"sql((?<full>f(ull)?)|(?<number>\d+))?_(?<connection>[a-zA-Z0-9]+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex valuePropRegex = new Regex("^(to_?)?v(al(ue)?)?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex csvRegex = new Regex("^(to_?)?csv(?<noheader>_?no?(h(ead(er)?)?))?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

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
                    else
                    {
                        numberLimit = i => i < 100;
                    }

                    if (e.Args.Count > 1)
                    {
                        Delegate lambda = e.EvaluateArg<Delegate>(1);
                    }
                    else
                    {
                        var reader = command.ExecuteReader();

                        List<dynamic> result = new List<dynamic>();

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
                    StringBuilder stringBuilder = new StringBuilder();

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
                                return result[n].ToString();
                        })));
                    }

                    e.Value = stringBuilder.ToString();
                }
            }

            return e.FunctionReturnedValue;
        }

        private static dynamic ToExpando(IDataRecord record)
        {
            var expandoObject = new ExpandoObject() as IDictionary<string, object>;

            for (var i = 0; i < record.FieldCount; i++)
                expandoObject.Add(record.GetName(i), record[i]);

            return expandoObject;
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
                    StringBuilder stringBuilder = new StringBuilder();

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
            }

            return e.HasValue;
        }

        #region Singleton          

        private static DBEvaluation instance;

        public static DBEvaluation Instance => instance ??= new DBEvaluation();

        private DBEvaluation()
        { }

        #endregion

    }
}
