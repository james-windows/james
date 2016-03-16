using System;
using System.Windows;
using James.HelperClasses;
using James.Windows;

namespace James.Workflows.Outputs
{
    public class SearchBoxOutput: BasicOutput
    {
        public override void Run(string[] input)
        {
            string output = FormatString.InsertArguments(input);
            Application.Current.Dispatcher.Invoke(() =>
            {
                var main = MainWindow.GetInstance();
                main.OnHotKeyHandler(new Shortcut.Shortcut() {HotKey = null});
                main.SearchTextBox.Text = output;
                main.SearchTextBox.SelectionStart = output.Length;
            });          
        }
    }
}
