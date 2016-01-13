using System;
using System.Diagnostics;

namespace James.Workflows.Outputs
{
    class OpenOutput: BasicOutput
    {
        public override void Run(string[] input)
        {
            try
            {
                Process.Start(input[0]);
            }
            catch (Exception)
            {
                Console.WriteLine("Error when starting " + input[0]);
            }
        }

        public override string GetSummary() => "opens url or path";
    }
}
