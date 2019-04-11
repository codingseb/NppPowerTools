using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace NppPowerTools
{
    public class ResultTemplateSelector : DataTemplateSelector
    {
        public List<DataTemplate> Templates { get; set; } = new List<DataTemplate>();

        public DataTemplate DefaultTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return Templates.Find(t => (t.DataType as Type).IsAssignableFrom(item.GetType())) ?? DefaultTemplate;
        }
    }
}
