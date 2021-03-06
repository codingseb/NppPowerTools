﻿using ColorPickerWPF;
using NppPowerTools.Utils;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using System;

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

        private void AddConnection_Button_Click(object sender, RoutedEventArgs e)
        {
            var dbConfig = new DBConfig()
            {
                Id = "mydb",
                Name = "New Connection"
            };

            Config.Instance.DBConfigs.Add(dbConfig);
            DBConfigsListBox.SelectedValue = dbConfig;
        }

        private void DBCheck_Button_Click(object sender, RoutedEventArgs e)
        {
            if(sender is FrameworkElement frameworkElement
                && frameworkElement.DataContext is DBConfig dBConfig)
            {
                try
                {
                    using var connection = dBConfig.GetConnection();
                    connection.Open();

                    Config.Instance.Save();

                    MessageBox.Show("Connection OK", "Success");
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message, "Connection Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void DBListBox_Delete_CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = sender is ListBox listBox && listBox.SelectedIndex >= 0;
            e.Handled = true;
        }

        private void DBListBox_Delete_CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (sender is ListBox listBox)
                DBDelete(listBox);
        }

        private void DBDelete(ListBox listBox)
        {
            if (listBox?.SelectedIndex >= 0 && MessageBox.Show("This will delete the selected DB connection.\r\nDo you want to continue", "Delete DB connection", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                Config.Instance.DBConfigs.RemoveAt(listBox.SelectedIndex);
        }
    }
}
