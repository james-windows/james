using James.Workflows.Outputs;
using James.Workflows.Triggers;
using System.Timers;
using James.HelperClasses;

namespace James.Workflows.Actions
{
    public class DelayAction : BasicAction
    {
        public DelayAction()
        {
        }
        
        public override string ExecutablePath { get; set; } = "";
        public override string ExecutableArguments { get; set; } = "";

        [ComponentField("Multiplier: ")]
        public int Multiplier { get; set; } = 1000;

        [ComponentField("Delay format string:")]
        public string DelayFormat { get; set; } = "{0}";

        public override string GetSummary() => $"Timeouts for a certain time";

        private Timer timer;

        public override void Cancel()
        {
            if (!Background)
            {
                timer?.Stop();
                timer?.Close();
            }
        }

        public override void Run(string[] arguments)
        {
            int delay;
            if (!int.TryParse(DelayFormat.InsertArguments(arguments), out delay) || delay * Multiplier == 0)
            {
                CallNext(arguments);
                return;
            }
            delay *= Multiplier;
            timer = new Timer {Interval = delay};
            timer.Elapsed += (sender, e) =>
            {
                timer.Stop();
                timer.Close();
                CallNext(arguments);
            };
            timer.Start();
        }

        public override bool IsAllowed(WorkflowComponent source)
        {
            return (source is BasicTrigger || source is BasicAction || source is MagicOutput) && source != this;
        }
    }
}
