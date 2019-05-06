using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NppPowerTools
{
    /// <summary>
    /// Logique d'interaction pour CommandFindWindow.xaml
    /// </summary>
    public partial class CommandFindWindow : Window
    {
        [DllImport("user32.dll")]
        private static extern int GetWindowRect(IntPtr hwnd, out Rectangle rect);

        public CommandFindWindow()
        {
            InitializeComponent();
        }

        private void FindTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            FindTextBox.SelectAll();
        }

        private void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.KeyboardDevice.Modifiers == ModifierKeys.None && e.Key == Key.Escape)
            {
                try
                {
                    this.Close();
                    e.Handled = true;
                }
                catch { }
            }
        }

        private void Window_Deactivated(object sender, System.EventArgs e)
        {
            try
            {
                this.Close();
            }
            catch { }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                IntPtr windowHandle = Process.GetCurrentProcess().MainWindowHandle;
                GetWindowRect(windowHandle, out Rectangle rect);

                Left = ((rect.Left + rect.Width) / 2) - (Width / 2);
                Top = rect.Top + 100;
            }
            catch { }
        }

        private void FindTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyboardDevice.Modifiers == ModifierKeys.None && e.Key == Key.Down)
            {
                try
                {
                    CommandsListBox.SelectedIndex++;
                    CommandsListBox.ScrollIntoView(CommandsListBox.SelectedValue);
                    e.Handled = true;
                }
                catch { }
            }
            if (e.KeyboardDevice.Modifiers == ModifierKeys.None && e.Key == Key.Up)
            {
                try
                {
                    CommandsListBox.SelectedIndex--;
                    CommandsListBox.ScrollIntoView(CommandsListBox.SelectedValue);
                    e.Handled = true;
                }
                catch { }
            }
            else if (e.KeyboardDevice.Modifiers == ModifierKeys.None && e.Key == Key.Enter && CommandsListBox.SelectedValue is NPTCommand command)
            {
                command.CommandAction?.Invoke();
                Close();
                e.Handled = true;
            }
        }

        private void Item_StackPanel_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.OriginalSource is FrameworkElement frameworkElement && frameworkElement.DataContext is NPTCommand command)
            {
                command.CommandAction?.Invoke();
                Close();
                e.Handled = true;
            }
        }
    }
}
