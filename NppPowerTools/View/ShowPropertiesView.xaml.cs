using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;

namespace NppPowerTools
{
    /// <summary>
    /// Logique d'interaction pour ShowPropertiesView.xaml
    /// </summary>
    public partial class ShowPropertiesView : UserControl
    {
        public ShowPropertiesView()
        {
            InitializeComponent();
        }

        private void PropertyGrid_PreparePropertyItem(object sender, PropertyItemEventArgs e)
        {
            if(e.PropertyItem is PropertyItem propItem && propItem.IsEnabled && propItem.IsReadOnly)
            {
                
            }
        }
    }
}
