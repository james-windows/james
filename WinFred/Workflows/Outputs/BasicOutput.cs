using System.Runtime.Serialization;

namespace James.Workflows.Outputs
{
    [DataContract, KnownType(typeof (LargeTypeOutput)), KnownType(typeof (SearchResultOutput)),
     KnownType(typeof (NotificationOutput))]
    public abstract class BasicOutput : WorkflowComponent
    {
        public abstract void Display(string output);
    }
}