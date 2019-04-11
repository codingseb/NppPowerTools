using ColorPickerWPF;
using System.Windows.Controls;
using System.Windows.Media;

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
    }
}
