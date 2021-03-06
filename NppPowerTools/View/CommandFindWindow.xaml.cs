﻿using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;

namespace NppPowerTools
{
    /// <summary>
    /// Logique d'interaction pour CommandFindWindow.xaml
    /// </summary>
    public partial class CommandFindWindow : Window
    {
        private const int DWMWA_EXTENDED_FRAME_BOUNDS = 9;
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;        // x position of upper-left corner
            public int Top;         // y position of upper-left corner
            public int Right;       // x position of lower-right corner
            public int Bottom;      // y position of lower-right corner
        }

        [DllImport("dwmapi.dll")]
        private static extern int DwmGetWindowAttribute(IntPtr hwnd, int dwAttribute, out RECT pvAttribute, int cbAttribute);

        public CommandFindWindow()
        {
            InitializeComponent();
        }

        private void FindTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            FindTextBox.SelectAll();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyboardDevice.Modifiers == ModifierKeys.None && e.Key == Key.Escape)
            {
                try
                {
                    Close();
                    e.Handled = true;
                }
                catch { }
            }
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            try
            {
                Close();
            }
            catch { }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                IntPtr windowHandle = Process.GetCurrentProcess().MainWindowHandle;
                DwmGetWindowAttribute(windowHandle, DWMWA_EXTENDED_FRAME_BOUNDS, out RECT rect, Marshal.SizeOf(typeof(RECT)));

                Left = (rect.Left + rect.Right) / 2 - Width / 2;
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
                    if (CommandsListBox.SelectedIndex >= CommandsListBox.Items.Count - 1)
                        CommandsListBox.SelectedIndex = 0;
                    else
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
                    if (CommandsListBox.SelectedIndex <= 0)
                        CommandsListBox.SelectedIndex = CommandsListBox.Items.Count - 1;
                    else
                        CommandsListBox.SelectedIndex--;
                    CommandsListBox.ScrollIntoView(CommandsListBox.SelectedValue);
                    e.Handled = true;
                }
                catch { }
            }
            else if (e.KeyboardDevice.Modifiers == ModifierKeys.None && e.Key == Key.Enter && CommandsListBox.SelectedValue is NPTCommand command)
            {
                command.Execute(this);
                e.Handled = true;
            }
        }

        private void Item_StackPanel_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is FrameworkElement fe && fe.DataContext is NPTCommand command)
            {
                command.Execute(this);
                e.Handled = true;
            }
        }
    }
}
