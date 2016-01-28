namespace James.Workflows.Triggers
{
    class ApiTrigger: BasicTrigger
    {
        public override void Run(string[] arguments)
        {
            ParentWorkflow.canceld = false;
            CallNext(arguments);
        }

        [ComponentField("The name of the action to listen")]
        public string Action { get; set; } = "";

        public override string GetSummary() => $"Listens for '{Action}' in the api";
    }
}
