using System;
using System.Threading;
using System.Windows;
using James.HelperClasses;

namespace James.Workflows.Outputs
{
    public class ClipboardOutput:BasicOutput
    {
        [STAThread]
        public override void Run(string[] input)
        {
            Thread thread = new Thread(() => Clipboard.SetText(FormatString.InsertArguments(input)));
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
        }
    }
}