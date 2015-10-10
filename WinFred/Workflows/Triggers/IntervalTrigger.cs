using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Timers;
using James.Workflows.Interfaces;

namespace James.Workflows.Triggers
{
    [DataContract]
    public class IntervalTrigger: BasicTrigger, ISurviveable
    {
        [DataMember]
        public int Interval { get; set; }

        public IntervalTrigger(Workflow parent) : base(parent)
        {
        }

        public IntervalTrigger():base()
        {
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            TriggerRunables();
        }

        private Timer _timer;

        public override void Run()
        {
            if (_timer == null)
            {
                _timer = new Timer { AutoReset = true };
                _timer.Elapsed += Timer_Elapsed;
            }
            TriggerRunables();
            _timer.Interval = Interval;
            _timer.Start();
        }

        public void Cancel()
        {
            _timer?.Stop();
        }
    }
}
