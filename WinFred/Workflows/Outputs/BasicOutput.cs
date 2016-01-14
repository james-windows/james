using System;
using James.Workflows.Actions;
using James.Workflows.Triggers;

namespace James.Workflows.Outputs
{
    public abstract class BasicOutput : WorkflowComponent
    {
        public override bool IsAllowed(WorkflowComponent source) => source is BasicAction;

        [ComponentField("Format String")]
        public string FormatString { get; set; } = "";

        public override string GetSummary() => "output";

        public override int GetColumn() => 2;

        public override int GetRow() => ParentWorkflow.Outputs.FindIndex(outputs => outputs.Id == Id);

        protected string FormatStringToText(string[] arguments)
        {
            return string.Format(FormatString, arguments);
        }
    }
}