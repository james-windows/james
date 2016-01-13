using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using James.HelperClasses;

namespace James.Workflows.Outputs
{
    public class ClipboardOutput:BasicOutput
    {
        [STAThread]
        public override void Run(string[] input)
        {
            Thread thread = new Thread(() => Clipboard.SetText(string.Join(" ", input)));
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
        }
    }
}