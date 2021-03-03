using CodingSeb.ExpressionEvaluator;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms.DataVisualization.Charting;

namespace NppPowerTools.Utils.Evaluations
{
    public sealed class ChartPlotEvaluation : IVariableEvaluation, IFunctionEvaluation
    {
        private static readonly Regex plotRegex = new Regex($@"plot(?<type>{string.Join("|", Enum.GetNames(typeof(SeriesChartType)))})((?<size>\d+)|w(?<width>\d+)h(?<height>\d+))?", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex labelVariableRegex = new Regex("[%](?<variable>[pv])({(?<format>[^}]+)})?", RegexOptions.Compiled);

        private Chart GetChart(Match plotMatch, object data, string[] labels = null)
        {
            Chart chart = new Chart();
            chart.ChartAreas.Add("A1");

            if (data is IEnumerable<double> enumerable)
            {
                Series serie = chart.Series.Add("S1");
                serie.ChartType = (SeriesChartType)Enum.Parse(typeof(SeriesChartType), plotMatch.Groups["type"].Value, true);

                double total = enumerable.Sum();

                foreach (double point in enumerable)
                {
                    int index = serie.Points.AddY(point);

                    if(labels != null && labels.Length > index)
                    {
                        string label = labelVariableRegex.Replace(labels[index], match =>
                        {
                            if (match.Groups["variable"].Value.Equals("v"))
                                return point.ToString(match.Groups["format"].Success ? match.Groups["format"].Value : "0.##");
                            else
                                return (point / total * 100d).ToString(match.Groups["format"].Success ? match.Groups["format"].Value : "0.##");
                        });

                        if (new SeriesChartType[] { SeriesChartType.Pie, SeriesChartType.Doughnut }.Contains(serie.ChartType))
                            serie.Points[index].Label = label;
                        else
                            serie.Points[index].AxisLabel = label;
                    }
                }
            }

            if (plotMatch.Groups["size"].Success)
            {
                chart.Width = int.Parse(plotMatch.Groups["size"].Value);
                chart.Height = int.Parse(plotMatch.Groups["size"].Value);
            }
            else if (plotMatch.Groups["width"].Success)
            {
                chart.Width = int.Parse(plotMatch.Groups["width"].Value);
                chart.Height = int.Parse(plotMatch.Groups["height"].Value);
            }

            return chart;
        }

        public bool TryEvaluate(object sender, VariableEvaluationEventArg e)
        {
            Match plotMatch = plotRegex.Match(e.Name);

            if (plotMatch.Success)
            {
                e.Value = GetChart(plotMatch, e.This);
            }
            else if (new string[] { "chartlist", "charttypes", "chartslist", "chartstypes", "plotlist", "plottypes" }.Any(name => e.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                e.Value = string.Join("\r\n", Enum.GetNames(typeof(SeriesChartType)));
            }
            else if(new string[] { "topng", "topic", "topicture", "toimage" }.Any(name => e.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                && e.This is Chart chart)
            {
                using MemoryStream ms = new MemoryStream();
                chart.SaveImage(ms, ChartImageFormat.Png);
                e.Value = new Bitmap(ms);
            }

            return e.HasValue;
        }

        public bool TryEvaluate(object sender, FunctionEvaluationEventArg e)
        {
            Match plotMatch = plotRegex.Match(e.Name);

            if (plotMatch.Success)
            {
                object[] args = e.EvaluateArgs();

                e.Value = GetChart(plotMatch,
                    e.This ?? args[0],
                    (Array.Find(args, a => a is IEnumerable<string>) as IEnumerable<string> ?? args.OfType<string>())?.ToArray());
            }
            else if (new string[] { "chartlist", "charttypes", "chartslist", "chartstypes", "plotlist", "plottypes" }.Any(name => e.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                e.Value = string.Join("\r\n", Enum.GetNames(typeof(SeriesChartType)));
            }
            else if (new string[] { "topng", "topic", "topicture", "toimage" }.Any(name => e.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                && e.This is Chart chart)
            {
                using MemoryStream ms = new MemoryStream();
                chart.SaveImage(ms, ChartImageFormat.Png);
                e.Value = new Bitmap(ms);
            }

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
