using CodingSeb.ExpressionEvaluator;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms.DataVisualization.Charting;

namespace NppPowerTools.Utils.Evaluations
{
    public class ChartPlotEvaluation : IVariableEvaluation, IFunctionEvaluation
    {
        private static readonly Regex plotRegex = new Regex(@"plot(?<type>pie)(?<size>\d+)?", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public bool TryEvaluate(object sender, VariableEvaluationEventArg e)
        {
            Match plotMatch = plotRegex.Match(e.Name);

            if (plotMatch.Success)
            {
                Chart chart = new Chart();
                chart.ChartAreas.Add("A1");

                if (plotMatch.Groups["type"].Value.Equals("pie", StringComparison.OrdinalIgnoreCase)
                    && e.This is IEnumerable<double> enumerable)
                {
                    Series serie = chart.Series.Add("serie");
                    serie.ChartType = SeriesChartType.Pie;

                    foreach (double point in enumerable)
                    {
                        serie.Points.AddY(point);
                    }
                }

                if(plotMatch.Groups["size"].Success)
                {
                    chart.Width = int.Parse(plotMatch.Groups["size"].Value);
                    chart.Height = int.Parse(plotMatch.Groups["size"].Value);
                }

                using (MemoryStream ms = new MemoryStream())
                {
                    chart.SaveImage(ms, ChartImageFormat.Png);
                    e.Value = new Bitmap(ms);
                }
            }

            return e.HasValue;
        }

        public bool TryEvaluate(object sender, FunctionEvaluationEventArg e)
        {

            return e.FunctionReturnedValue;
        }

        #region Singleton          

        private static ChartPlotEvaluation instance;

        public static ChartPlotEvaluation Instance => instance ??= new ChartPlotEvaluation();

        private ChartPlotEvaluation()
        { }

        #endregion

    }
}
