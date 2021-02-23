using System.Windows;
using System.Windows.Controls;

namespace NppPowerTools.Behaviors
{
    public static class TextBoxExtensions
    {
        public static int GetSelectionStart(this DependencyObject obj)
        {
            return (int)obj.GetValue(SelectionStartProperty);
        }

        public static void SetSelectionStart(this DependencyObject obj, int value)
        {
            obj.SetValue(SelectionStartProperty, value);
        }

        // Using a DependencyProperty as the backing store for SelectionStart.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectionStartProperty =
            DependencyProperty.RegisterAttached("SelectionStart", typeof(int), typeof(TextBoxExtensions), new PropertyMetadata(0, OnSelectionStartChanged));

        private static void OnSelectionStartChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox Target)
            {
                Target.SelectionStart = (int)e.NewValue;
            }
        }

        public static int GetSelectionLength(this DependencyObject obj)
        {
            return (int)obj.GetValue(SelectionLengthProperty);
        }

        public static void SetSelectionLength(this DependencyObject obj, int value)
        {
            obj.SetValue(SelectionLengthProperty, value);
        }

        // Using a DependencyProperty as the backing store for SelectionLength.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectionLengthProperty =
            DependencyProperty.RegisterAttached("SelectionLength", typeof(int), typeof(TextBoxExtensions), new PropertyMetadata(0, OnSelectionLengthChanged));

        private static void OnSelectionLengthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox Target)
            {
                Target.SelectionLength = (int)e.NewValue;
            }
        }
    }
}
