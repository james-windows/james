namespace James.Workflows.Outputs
{
    public abstract class BasicOutput : WorkflowComponent
    { 
        [ComponentField("Format String")]
        public string FormatString { get; set; } = "{0}";

        public override string GetSummary() => "output";
    }
}