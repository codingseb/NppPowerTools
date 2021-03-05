using CodingSeb.ExpressionEvaluator;
using System;
using System.Data;
using System.Text.RegularExpressions;

namespace NppPowerTools.Utils.Evaluations
{
    public class DBEvaluation : IFunctionEvaluation
    {
        private Regex sqlRegex = new Regex("sql_(?<connection>[a-zA-Z0-9])_(?<command>s)");

        public bool TryEvaluate(object sender, FunctionEvaluationEventArg e)
        {
            if(e.Name.Equals("sql", StringComparison.OrdinalIgnoreCase))
            {
                IDbConnection dbConnection = new MySqlConnector.MySqlConnection("Server=localhost;User ID=root;Password=PTM2000;Database=mmdb");
                var command = dbConnection.CreateCommand();

                command.CommandText = e.EvaluateArg<string>(0);

                e.Value = command;

                //while(reader)
            }

            return e.FunctionReturnedValue;
        }
    }
}
