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

        /// <summary>
        /// Cancels the timer of the action
        /// </summary>
        public override void Cancel()
        {
            if (!Background)
            {
                timer?.Stop();
                timer?.Close();
            }
        }

        /// <summary>
        /// Starts a timeout for a specific amount of time (arguments + FormatString)
        /// </summary>
        /// <param name="arguments"></param>
        public override void Run(string[] arguments)
        {
            if (Background == false && ParentWorkflow.IsCanceled)
            {
                return;
            }
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
                if (!ParentWorkflow.IsCanceled || Background)
                {
                    CallNext(arguments);
                }
            };
            timer.Start();
        }

        public override bool IsAllowed(WorkflowComponent source)
        {
            return (source is BasicTrigger || source is BasicAction || source is MagicOutput) && source != this;
        }

        public override string GetDescription() => "This component provides the possibility to delay an action for a specific amount of time";
    }
}
