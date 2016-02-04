using System;
using System.Diagnostics;
using James.HelperClasses;

namespace James.Workflows.Outputs
{
    class OpenOutput: BasicOutput
    {
        public override void Run(string[] input)
        {
            try
            {
                Process.Start(FormatString.InsertArguments(input));
            }
            catch (Exception)
            {
                Console.WriteLine("Error when starting " + input[0]);
            }
        }

        public override string GetSummary() => "opens url or path";
    }
}
