using CodingSeb.ExpressionEvaluator;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text.RegularExpressions;

namespace NppPowerTools.Utils.Evaluations
{
    public class DBEvaluation : IFunctionEvaluation, IVariableEvaluation
    {
        private static readonly Regex sqlRegex = new Regex("sql_(?<connection>[a-zA-Z0-9]+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex valuePropRegex = new Regex("^(to_?)?v(al(ue)?)?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

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
                        using (var initCommand = dbConnection.CreateCommand())
                        {
                            initCommand.CommandText = dBConfig.InitCommands;
                            initCommand.ExecuteNonQuery();
                        }
                    }

                    using var command = dbConnection.CreateCommand();
                    command.CommandText = e.EvaluateArg<string>(0);

                    if (e.Args.Count > 1)
                    {
                        Delegate lambda = e.EvaluateArg<Delegate>(1);
                    }
                    else
                    {
                        var reader = command.ExecuteReader();

                        List<dynamic> result = new List<dynamic>();

                        int i = 0;

                        while(i < 100 && reader.Read())
                        {
                            result.Add(ToExpando(reader));
                            i++;
                        }

                        e.Value = new DBResultViewModel()
                        {
                            Results = result
                        };
                    }
                }
                else
                {
                    throw new ExpressionEvaluatorSyntaxErrorException($"No DB Connection with ID = {match.Groups["connection"].Value}");
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
            if(e.This is DBResultViewModel dBResultViewModel && valuePropRegex.IsMatch(e.Name))
            {
                if(dBResultViewModel.Results.Count == 1 && dBResultViewModel.Results[0] is IDictionary<string,object> dict)
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

            return e.HasValue;
        }

        #region Singleton          

        private static DBEvaluation instance;

        public static DBEvaluation Instance => instance ?? (instance = new DBEvaluation());

        private DBEvaluation()
        { }

        #endregion

    }
}
