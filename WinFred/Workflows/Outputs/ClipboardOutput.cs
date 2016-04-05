using System;
using System.Threading;
using System.Windows;
using James.HelperClasses;
using James.Windows;

namespace James.Workflows.Outputs
{
    public class ClipboardOutput: BasicOutput
    {
        /// <summary>
        /// Copies the input to the clipboard
        /// </summary>
        /// <param name="input"></param>
        [STAThread]
        public override void Run(string[] input)
        {
            Thread thread = new Thread(() => Clipboard.SetText(FormatString.InsertArguments(input)));
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
            MainWindow.GetInstance().HideWindow();
        }
    }
}