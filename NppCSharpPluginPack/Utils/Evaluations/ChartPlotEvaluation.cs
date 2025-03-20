using System;
using System.Collections;
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
        private static readonly Regex plotRegex = new($@"plot(?<type>{string.Join("|", Enum.GetNames(typeof(SeriesChartType)))})((?<size>\d+)|w(?<width>\d+)h(?<height>\d+))?", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex labelVariableRegex = new("[%](?<variable>[pv])({(?<format>[^}]+)})?", RegexOptions.Compiled);
        private static readonly string[] toPicKeywords = new string[] { "topng", "topic", "topicture", "toimage", "to_png", "to_pic", "to_picture", "to_image" };
        private static readonly string[] plotListKeywords = new string[] { "chartlist", "charttypes", "chartslist", "chartstypes", "plotlist", "plottypes" };

    private Chart GetChart(Match plotMatch, object data, string[] labels = null)
        {
            Chart chart = new();
            chart.ChartAreas.Add("A1");

            IEnumerable<double> doubleEnumerable = data as IEnumerable<double>;

            if (doubleEnumerable == null && data is IEnumerable enumerable && enumerable.Cast<object>().All(v => v is double || v is int))
                doubleEnumerable = enumerable.Cast<object>().Select(v => v as double? ?? (double)(v as int?)).ToList();

            if (doubleEnumerable != null)
            {
                Series serie = chart.Series.Add("S1");
                serie.ChartType = (SeriesChartType)Enum.Parse(typeof(SeriesChartType), plotMatch.Groups["type"].Value, true);

                double total = doubleEnumerable.Sum();

                foreach (double point in doubleEnumerable)
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
            else if (plotListKeywords.Any(name => e.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                e.Value = string.Join("\r\n", Enum.GetNames(typeof(SeriesChartType)));
            }
            else if (toPicKeywords.Any(name => e.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                && e.This is Chart chart)
            {
                using MemoryStream ms = new();
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
            else if (plotListKeywords.Any(name => e.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                e.Value = string.Join("\r\n", Enum.GetNames(typeof(SeriesChartType)));
            }
            else if (toPicKeywords.Any(name => e.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                && e.This is Chart chart)
            {
                using MemoryStream ms = new();
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
