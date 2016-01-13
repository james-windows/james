using System;
using James.Workflows.Actions;

namespace James.Workflows.Outputs
{
    public abstract class BasicOutput : WorkflowComponent
    {
        public override bool IsAllowed(WorkflowComponent source) => source is BasicAction;

        public override string GetSummary() => "output";

        public override int GetColumn() => 2;

        public override int GetRow() => ParentWorkflow.Outputs.FindIndex(outputs => outputs.Id == Id);
    }
}