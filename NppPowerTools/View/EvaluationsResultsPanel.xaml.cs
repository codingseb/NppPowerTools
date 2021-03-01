using Microsoft.Win32;
using NppPowerTools.Utils;
using System.Drawing;
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
            if (sender is ListBox listBox && listBox.SelectedValue != null)
            {
                var saveFileDialog = new SaveFileDialog();
                string filter = "ToString() in textFile|*.txt|Json file|*.json";

                if (listBox.SelectedValue is Bitmap)
                    filter = "PNG Picture|*.png|" + filter;

                saveFileDialog.Filter = filter;

                if(saveFileDialog.ShowDialog(this.GetTopFrameworkElement() as Window) == true)
                {

                }
            }
        }
    }
}
