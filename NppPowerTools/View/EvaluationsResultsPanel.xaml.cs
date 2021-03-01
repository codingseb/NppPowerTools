using Microsoft.Win32;
using Newtonsoft.Json;
using NppPowerTools.Utils;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Controls;
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
            e.CanExecute = sender is ListBox listBox && listBox.SelectedValue != null;
            e.Handled = true;
        }

        private void ListBox_Copy_CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if(sender is ListBox listBox && listBox.SelectedValue != null)
            {
                if(listBox.SelectedValue is Bitmap bitmap)
                {
                    Clipboard.SetDataObject(bitmap);
                }
                else
                {
                    Clipboard.SetText(listBox.SelectedValue.ToString());
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
            if(sender is ListBox listBox && listBox.SelectedIndex >= 0)
                EvaluationsResultPanelViewModel.Instance.Results.RemoveAt(listBox.SelectedIndex);
        }

        private void Save_As_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement fe && fe.DataContext != null)
            {
                var saveFileDialog = new SaveFileDialog();
                string filter = "ToString() in text file|*.txt|Json file|*.json";

                int filterOffset = 0;

                if (fe.DataContext is Bitmap)
                {
                    filter = "PNG Picture|*.png|" + filter;
                    filterOffset++;
                }

                saveFileDialog.Filter = filter;
                saveFileDialog.AddExtension = true;

                if (saveFileDialog.ShowDialog() == true)
                {
                    try
                    {
                        if (saveFileDialog.FilterIndex == 1 && fe.DataContext is Bitmap bitmap)
                        {
                            bitmap.Save(saveFileDialog.FileName, ImageFormat.Png);
                        }
                        else if (saveFileDialog.FilterIndex == 1 + filterOffset)
                        {
                            File.WriteAllText(saveFileDialog.FileName, fe.DataContext.ToString());
                        }
                        else if (saveFileDialog.FilterIndex == 2 + filterOffset)
                        {
                            File.WriteAllText(saveFileDialog.FileName, JsonConvert.SerializeObject(fe.DataContext, Formatting.Indented));
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
    }
}
