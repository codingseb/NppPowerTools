using CodingSeb.ExpressionEvaluator;
using OfficeOpenXml;
using System;
using System.IO;

namespace NppPowerTools.Utils.Evaluations
{
    public class ExcelEvaluation : IFunctionEvaluation
    {
        public bool TryEvaluate(object sender, FunctionEvaluationEventArg e)
        {
            if(e.Name.Equals("xl", StringComparison.OrdinalIgnoreCase) && e.Args.Count > 0)
            {
                string filName = e.EvaluateArg(0).ToString();
                e.Value = new ExcelPackage(new FileInfo(filName));

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
