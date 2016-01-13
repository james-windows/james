namespace James.Workflows.Triggers
{
    public class KeywordTrigger : BasicTrigger
    {
        public KeywordTrigger()
        {
        }

        public KeywordTrigger(Workflow parent) : base(parent)
        {
        }

        [ComponentField("Gets displayed with the SearchResults")]
        public string Title { get; set; } = "";

        [ComponentField("Listens for this keyword to trigger")]
        public string Keyword { get; set; } = "";

        public override string GetSummary() => $"Triggers for \"{Keyword}\"";

        public override void Run(string[] arguments) => CallNext(arguments);

        public override bool IsAllowed(WorkflowComponent source) => false;
    }
}