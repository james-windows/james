using System.Runtime.Serialization;
using James.Workflows.Actions;

namespace James.Workflows.Outputs
{
    [DataContract, KnownType(typeof (LargeTypeOutput)), KnownType(typeof (BasicResultOutput)),
     KnownType(typeof (NotificationOutput))]
    public abstract class BasicOutput : WorkflowComponent
    {
        public abstract void Display(string output);

        public override bool IsAllowed(WorkflowComponent source) => source is BasicAction;
    }
}