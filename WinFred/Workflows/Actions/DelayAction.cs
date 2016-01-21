using James.Workflows.Outputs;
using James.Workflows.Triggers;

namespace James.Workflows.Actions
{
    public class DelayAction : BasicAction
    {
        public DelayAction()
        {
        }

        [ComponentField("Sets the delay [ms]")]
        public int Delay { get; set; } = 1000;

        public override string GetSummary() => $"Timeouts for {Delay} ms";

        public override void Run(string[] arguments)
        {
            System.Threading.Thread.Sleep(Delay);
            CallNext(arguments);
        }

        public override bool IsAllowed(WorkflowComponent source)
        {
            return (source is BasicTrigger || source is BasicAction || source is MagicOutput) && source != this;
        }
    }
}
