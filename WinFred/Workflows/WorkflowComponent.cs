using System.Runtime.Serialization;
using James.Workflows.Outputs;

namespace James.Workflows
{
    [DataContract(IsReference = true), KnownType(typeof (RunnableWorkflowComponent)), KnownType(typeof (BasicOutput))]
    public class WorkflowComponent
    {
        public WorkflowComponent()
        {
        }

        public WorkflowComponent(Workflow parent)
        {
            ParentWorkflow = parent;
        }

        [DataMember]
        public Workflow ParentWorkflow { get; set; }
    }
}