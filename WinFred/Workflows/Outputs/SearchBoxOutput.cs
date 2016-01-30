using System;
using System.Net.Mime;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using James.HelperClasses;
using James.Windows;
using MahApps.Metro.Controls;

namespace James.Workflows.Outputs
{
    public class SearchBoxOutput: BasicOutput
    {
        public override void Run(string[] input)
        {
            string output = FormatString.InsertArguments(input);
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                var main = MainWindow.GetInstance();
                main.OnHotKeyHandler(new Shortcut() {HotKey = null});
                main.SearchTextBox.Text = output;
                main.SearchTextBox.SelectionStart = output.Length;
            }));          
        }
    }
}
