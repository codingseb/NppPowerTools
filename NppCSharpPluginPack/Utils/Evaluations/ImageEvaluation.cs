using ImageProcessor;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text.RegularExpressions;

namespace NppPowerTools.Utils.Evaluations
{
    public class ImageEvaluation : IFunctionEvaluation, IVariableEvaluation, IEvaluatorInitializator
    {
        private static readonly Regex convertToImageFactoryRegex = new(@"^(proc(ess)?|(to_?)?fac(tory)?)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex convertToBitmapRegex = new(@"^(to_?)?(bitmap|bmp)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public bool TryEvaluate(object sender, FunctionEvaluationEventArg e)
        {
            if (convertToImageFactoryRegex.IsMatch(e.Name) && e.This is Image image)
            {
                e.Value = new ImageFactory().Load(image);
            }
            else if (e.This is ImageFactory factory)
            {
                if (convertToBitmapRegex.IsMatch(e.Name))
                {
                    using MemoryStream ms = new();
                    factory.Save(ms);
                    e.Value = new Bitmap(ms);
                }
                else if (e.Name.Equals("Resize", Config.Instance.CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase)
                    && e.Args.Count == 2
                    && e.EvaluateArg(0) is int x
                    && e.EvaluateArg(1) is int y)
                {
                    e.Value = factory.Resize(new Size(x, y));
                }
            }
            else if (e.This is Bitmap bitmap && (e.Name.Equals("save", StringComparison.OrdinalIgnoreCase) || e.Name.Equals("saveandopen", StringComparison.OrdinalIgnoreCase) || e.Name.Equals("saveopen", StringComparison.OrdinalIgnoreCase)))
            {
                SaveFileDialog saveFileDialog = new()
                {
                    Filter = "PNG Picture|*.png",
                    AddExtension = true
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    bitmap.Save(saveFileDialog.FileName, ImageFormat.Png);

                    if (e.Name.Equals("saveandopen", StringComparison.OrdinalIgnoreCase) || e.Name.Equals("saveopen", StringComparison.OrdinalIgnoreCase))
                    {
                        Process.Start(saveFileDialog.FileName);
                    }
                }

                e.Value = bitmap;
            }

            return e.FunctionReturnedValue;
        }

        public bool TryEvaluate(object sender, VariableEvaluationEventArg e)
        {
            if (convertToImageFactoryRegex.IsMatch(e.Name)
                && e.This is Image image)
            {
                e.Value = new ImageFactory().Load(image);
            }
            else if (convertToBitmapRegex.IsMatch(e.Name)
                && e.This is ImageFactory factory)
            {
                using MemoryStream ms = new();
                factory.Save(ms);
                e.Value = new Bitmap(ms);
            }
            else if (e.This is Bitmap bitmap && (e.Name.Equals("save", StringComparison.OrdinalIgnoreCase) || e.Name.Equals("saveandopen", StringComparison.OrdinalIgnoreCase) || e.Name.Equals("saveopen", StringComparison.OrdinalIgnoreCase)))
            {
                SaveFileDialog saveFileDialog = new()
                {
                    Filter = "PNG Picture|*.png",
                    AddExtension = true
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    bitmap.Save(saveFileDialog.FileName, ImageFormat.Png);

                    if (e.Name.Equals("saveandopen", StringComparison.OrdinalIgnoreCase) || e.Name.Equals("saveopen", StringComparison.OrdinalIgnoreCase))
                    {
                        Process.Start(saveFileDialog.FileName);
                    }
                }

                e.Value = bitmap;
            }

            return e.HasValue;
        }

        public void Init(ExpressionEvaluator evaluator)
        {
            evaluator.Namespaces.Add("System.Drawing");
            evaluator.Namespaces.Add("ImageProcessor.Imaging.Filters.Photo");
            evaluator.Namespaces.Add("ImageProcessor.Imaging.Filters.EdgeDetection");
            evaluator.Namespaces.Add("ImageProcessor.Imaging");
            evaluator.Namespaces.Add("ImageProcessor");
        }

        #region Singleton          

        private static ImageEvaluation instance;

        public static ImageEvaluation Instance => instance ??= new ImageEvaluation();

        private ImageEvaluation()
        { }

        #endregion

    }
}
