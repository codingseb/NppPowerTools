using CodingSeb.ExpressionEvaluator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace NppPowerTools.Utils.Evaluations
{
    public class DBEvaluation : IFunctionEvaluation
    {
        private static readonly Regex sqlRegex = new Regex("(?<value>v)?sql_(?<connection>[a-zA-Z0-9]+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

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

                        List<List<object>> result = new List<List<object>>();

                        while(reader.Read())
                        {
                            object[] row = new object[reader.FieldCount];

                            reader.GetValues(row);

                            result.Add(row.ToList());
                        }

                        if (match.Groups["value"].Success)
                        {
                            if (result.Count == 0)
                            {
                                e.Value = null;
                            }
                            else if (result.Count == 1)
                            {
                                if (result[0].Count == 1)
                                    e.Value = result[0][0];
                                else
                                    e.Value = result[0];
                            }
                        }

                        if (!e.FunctionReturnedValue)
                            e.Value = result;
                    }
                }
                else
                {
                    throw new ExpressionEvaluatorSyntaxErrorException($"No DB Connection with ID = {match.Groups["connection"].Value}");
                }
            }

            return e.FunctionReturnedValue;
        }
    }
}
