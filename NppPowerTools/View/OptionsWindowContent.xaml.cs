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

        private void Color_Label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Label label && ColorPickerWindow.ShowDialog(out Color color))
            {
                label.Background = new SolidColorBrush(color);
            }
        }

        private void ClearHistory_Button_Click(object sender, RoutedEventArgs e)
        {
            Config.Instance.LastScripts = new List<string>();
            MessageBox.Show("History Cleared !!!");
        }

        private void UserControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                this.FindVisualParent<Window>().Close();
        }
    }
}
