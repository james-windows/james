using System;
using System.Net.Mime;
using System.Windows;
using System.Windows.Threading;
using James.Windows;

namespace James.Workflows.Outputs
{
    public class SearchBoxOutput: BasicOutput
    {
        public override void Run(string[] input)
        {
            string output = FormatStringToText(FormatString, input);
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                var box = MainWindow.GetInstance().SearchTextBox;
                box.Text = output;
                box.SelectionStart = output.Length;
            }));          
        }
    }
}
