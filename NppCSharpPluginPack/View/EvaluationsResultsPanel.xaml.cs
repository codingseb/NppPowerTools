using Microsoft.Win32;
using Newtonsoft.Json;
using NppPowerTools.Utils;
using OfficeOpenXml;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace NppPowerTools
{
    /// <summary>
    /// Logique d'interaction pour EvaluationsResultsPanel.xaml
    /// </summary>
    public partial class EvaluationsResultsPanel : UserControl
    {
        public EvaluationsResultsPanel()
        {
            InitializeComponent();
        }

        private void Reset_Variables_Button_Click(object sender, RoutedEventArgs e)
        {
            Evaluation.ResetVariables();
        }

        private void ListBox_Copy_CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = sender is ListBox listBox && listBox.SelectedValue is EvaluationResult;
            e.Handled = true;
        }

        private void ListBox_Copy_CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (sender is ListBox listBox)
                Copy(listBox);
        }

        private void Copy(ListBox listBox)
        {
            if (listBox?.SelectedValue is EvaluationResult evaluationResult)
            {
                if (evaluationResult.Result is Bitmap bitmap)
                {
                    Clipboard.SetDataObject(bitmap);
                }
                else
                {
                    Clipboard.SetText(evaluationResult.Result?.ToString() ?? string.Empty);
                }
            }
        }

        private void Show_Properties_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if(sender is FrameworkElement fe)
                ShowPropertiesViewModel.Instance.ShowPropertiesWindow(fe.DataContext);
        }

        private void ListBox_Delete_CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = sender is ListBox listBox && listBox.SelectedIndex >= 0;
            e.Handled = true;
        }

        private void ListBox_Delete_CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (sender is ListBox listBox)
                Delete(listBox);
        }

        private void Delete(ListBox listBox)
        {
            if(listBox?.SelectedIndex >= 0)
                EvaluationsResultPanelViewModel.Instance.Results.RemoveAt(listBox.SelectedIndex);
        }

        private void Save_As_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement fe && fe.DataContext is EvaluationResult evaluationResult)
            {
                var saveFileDialog = new SaveFileDialog();
                string filter = "ToString() in text file|*.txt|Json file|*.json";

                int filterOffset = 0;

                if (evaluationResult.Result is Bitmap)
                {
                    filter = "PNG Picture|*.png|" + filter;
                    filterOffset++;
                }
                //else if(evaluationResult.Result is PDFFile)
                //{
                //    filter = "PDF Document|*.pdf|" + filter;
                //    filterOffset++;
                //}
                else if(evaluationResult.Result is ExcelPackage)
                {
                    filter = "Excel file|*.xlsx|" + filter;
                    filterOffset++;
                }

                saveFileDialog.Filter = filter;
                saveFileDialog.AddExtension = true;

                if (saveFileDialog.ShowDialog() == true)
                {
                    try
                    {
                        if (saveFileDialog.FilterIndex == 1 && evaluationResult.Result is Bitmap bitmap)
                        {
                            bitmap.Save(saveFileDialog.FileName, ImageFormat.Png);
                        }
                        //else if (saveFileDialog.FilterIndex == 1 && evaluationResult.Result is PDFFile pdfFile)
                        //{
                        //    pdfFile.Save(saveFileDialog.FileName);
                        //}
                        else if (saveFileDialog.FilterIndex == 1 && evaluationResult.Result is ExcelPackage excelFile)
                        {
                            excelFile.File = new FileInfo(saveFileDialog.FileName);

                            if (excelFile.Workbook.Worksheets.Count == 0)
                                excelFile.Workbook.Worksheets.Add(Config.Instance.ExcelDefaultSheetName);

                            excelFile.Save();
                        }
                        else if (saveFileDialog.FilterIndex == 1 + filterOffset)
                        {
                            File.WriteAllText(saveFileDialog.FileName, evaluationResult.Result?.ToString() ?? string.Empty);
                        }
                        else if (saveFileDialog.FilterIndex == 2 + filterOffset)
                        {
                            File.WriteAllText(saveFileDialog.FileName, JsonConvert.SerializeObject(evaluationResult.Result, Formatting.Indented));
                        }

                        if(MessageBox.Show("Do you want to open the resulting file ?", "Open file", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            Process.Start(saveFileDialog.FileName);
                        }
                    }
                    catch(Exception exception)
                    {
                        MessageBox.Show(exception.Message, "Error while saving", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void ListBox_Control_GotFocus(object sender, RoutedEventArgs e)
        {
            if(sender is FrameworkElement frameworkElement && frameworkElement.DataContext is EvaluationResult evaluationResult)
            {
                frameworkElement.FindVisualParent<ListBox>().SelectedValue = evaluationResult;
            }
        }

        private void ListBox_Control_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is FrameworkElement frameworkElement
                && frameworkElement.FindVisualParent<ListBox>() is ListBox listBox)
            {
                if (e.Key == Key.Delete
                    && e.KeyboardDevice.Modifiers == ModifierKeys.None)
                {
                    Delete(listBox);
                    e.Handled = true;
                }
                else if (e.Key == Key.C
                    && e.KeyboardDevice.Modifiers == ModifierKeys.Control)
                {
                    if (frameworkElement is TextBox textBox && textBox.SelectionLength > 0)
                        return;
                    Copy(listBox);
                    e.Handled = true;
                }
            }
        }

        private void DataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is DataGrid dataGrid && dataGrid.DataContext is EvaluationResult evaluationResult && evaluationResult.Result is DBResultViewModel dBResultViewModel)
            {
                foreach (string text in dBResultViewModel.ColumnsNames)
                {
                    // now set up a column and binding for each property
                    var column = new DataGridTextColumn
                    {
                        Header = new TextBlock() { Text = text },
                        Binding = new Binding(text)
                    };

                    dataGrid.Columns.Add(column);
                }
            }
        }
    }
}
