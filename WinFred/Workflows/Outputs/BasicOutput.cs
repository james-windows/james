using System;
using System.Text.RegularExpressions;
using James.Workflows.Actions;
using James.Workflows.Triggers;

namespace James.Workflows.Outputs
{
    public abstract class BasicOutput : WorkflowComponent
    { 
        [ComponentField("Format String")]
        public string FormatString { get; set; } = "{0}";

        public override string GetSummary() => "output";

        protected string FormatStringToText(string format, string[] arguments)
        {
            string output = format;
            for (int i = 0; i < arguments.Length; i++)
            {
                output = output.Replace("{" + i + "}", arguments[i]);
            }
            return Regex.Replace(output, @"{[0-9]+}", string.Empty);
        }
    }
}