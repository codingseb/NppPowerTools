using NppPowerTools.Utils;
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

    }
}
