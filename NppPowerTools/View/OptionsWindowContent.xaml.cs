using ColorPickerWPF;
using NppPowerTools.Utils;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;

namespace NppPowerTools
{
    /// <summary>
    /// Logique d'interaction pour OptionsWindowContent.xaml
    /// </summary>
    public partial class OptionsWindowContent : UserControl
    {
        public OptionsWindowContent()
        {
            InitializeComponent();
        }

        private void Color_Label_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Color color;
            if(sender is Label label && ColorPickerWindow.ShowDialog(out color))
            {
                label.Background = new SolidColorBrush(color);
            }
        }

        private void ClearHistory_Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Config.Instance.LastScripts = new List<string>();
            MessageBox.Show("History Cleared !!!");
        }

        private void UserControl_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                this.FindVisualParent<Window>().Close();
        }
    }
}
