using System.Runtime.Serialization;
using System.Timers;
using James.Workflows.Interfaces;

namespace James.Workflows.Triggers
{
    [DataContract]
    public class IntervalTrigger : BasicTrigger, ISurviveable
    {
        private Timer _timer;

        public IntervalTrigger(Workflow parent) : base(parent)
        {
        }

        public IntervalTrigger()
        {
        }

        [DataMember]
        public int Interval { get; set; }

        public void Cancel()
        {
            _timer?.Stop();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            TriggerRunables();
        }

        public override void Run()
        {
            if (_timer == null)
            {
                _timer = new Timer {AutoReset = true};
                _timer.Elapsed += Timer_Elapsed;
            }
            TriggerRunables();
            _timer.Interval = Interval;
            _timer.Start();
        }
    }
}