using System;
using System.Diagnostics;
using James.HelperClasses;
using James.Windows;

namespace James.Workflows.Outputs
{
    class OpenOutput: BasicOutput
    {
        public override void Run(string[] input)
        {
            try
            {
                MainWindow.GetInstance().HideWindow();
                Process.Start(FormatString.InsertArguments(input));
            }
            catch (Exception e)
            {
                Console.WriteLine("Error when starting " + input[0]);
            }
        }

        public override string GetSummary() => "opens url or path";
    }
}
