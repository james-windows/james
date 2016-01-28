using James.Workflows.Actions;
using James.Workflows.Interfaces;
using James.Workflows.Outputs;

namespace James.Workflows.Triggers
{
    public abstract class BasicTrigger : WorkflowComponent, ICrossPlatform
    {
        protected BasicTrigger()
        {
        }

        protected BasicTrigger(Workflow parent) : base(parent)
        {
        }

        public override bool IsAllowed(WorkflowComponent source) => (source is BasicAction || source is MagicOutput) && source != this;
    }
}