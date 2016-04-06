using James.Workflows.Interfaces;

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

        public override bool IsAllowed(WorkflowComponent source) => false;
    }
}