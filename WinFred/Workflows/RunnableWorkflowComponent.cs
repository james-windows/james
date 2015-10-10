using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using James.Workflows.Actions;
using James.Workflows.Interfaces;
using James.Workflows.Triggers;

namespace James.Workflows
{
    [DataContract, KnownType(typeof(BasicAction)), KnownType(typeof(BasicTrigger))]
    public abstract class RunnableWorkflowComponent: WorkflowComponent
    {
        protected RunnableWorkflowComponent(Workflow parent): base(parent)
        {
        }

        protected RunnableWorkflowComponent() : base()
        {
        }

        public abstract void Run();
    }
}
