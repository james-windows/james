using System;
using System.Diagnostics;
using James.Workflows.Outputs;
using James.Workflows.Triggers;
using System.Timers;
using System.Windows.Controls;
using System.Windows.Media;

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
            timer = new Timer {Interval = Delay};
            timer.Elapsed += (sender, e) =>
            {
                timer.Close();
                CallNext(arguments);
            };
        }

        public override bool IsAllowed(WorkflowComponent source)
        {
            return (source is BasicTrigger || source is BasicAction || source is MagicOutput) && source != this;
        }
    }
}
