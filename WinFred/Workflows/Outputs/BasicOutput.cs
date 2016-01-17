using System;
using James.Workflows.Actions;
using James.Workflows.Triggers;

namespace James.Workflows.Outputs
{
    public abstract class BasicOutput : WorkflowComponent
    { 
        [ComponentField("Format String")]
        public string FormatString { get; set; } = "{0}";

        public override string GetSummary() => "output";

        protected string FormatStringToText(string[] arguments)
        {
            return string.Format(FormatString, arguments);
        }
    }
}