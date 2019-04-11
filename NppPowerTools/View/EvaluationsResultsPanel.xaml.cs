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

        private void ListBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Delete && sender is ListBox listBox && listBox.SelectedIndex >= 0)
            {
                EvaluationsResultPanelViewModel.Instance.Results.RemoveAt(listBox.SelectedIndex);
            }
        }

        private void Reset_Variables_Button_Click(object sender, System.Windows.RoutedEventArgs e)
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
    }
}
