using System.Windows;
using James.HelperClasses;
using James.MyWindows;

namespace James.Workflows.Outputs
{
    public class SearchBoxOutput: BasicOutput
    {
        /// <summary>
        /// Inserts the generated string (input[] and FormatString) into the SearchBox of the search window
        /// </summary>
        /// <param name="input"></param>
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
