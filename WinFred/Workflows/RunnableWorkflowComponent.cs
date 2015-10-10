using System.Runtime.Serialization;
using James.Workflows.Actions;
using James.Workflows.Triggers;

namespace James.Workflows
{
    [DataContract, KnownType(typeof (BasicAction)), KnownType(typeof (BasicTrigger))]
    public abstract class RunnableWorkflowComponent : WorkflowComponent
    {
        protected RunnableWorkflowComponent(Workflow parent) : base(parent)
        {
        }

        protected RunnableWorkflowComponent()
        {
        }

        public abstract void Run();
    }
}