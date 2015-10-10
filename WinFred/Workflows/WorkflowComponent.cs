using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using James.Workflows.Outputs;

namespace James.Workflows
{
    [DataContract(IsReference = true), KnownType(typeof(RunnableWorkflowComponent)), KnownType(typeof(BasicOutput))]
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
