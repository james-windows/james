using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using James.Workflows.Interfaces;

namespace James.Workflows.Triggers
{
    [DataContract]
    public class TimeoutTrigger : BasicTrigger
    {
        public TimeoutTrigger(Workflow parent) : base(parent)
        {
        }

        public TimeoutTrigger()
        {
        }

        [DataMember]
        [ComponentField("Sets the timeout [ms]")]
        public int Timeout { get; set; } = 1000;

        public override string GetSummary() => $"Timeouts for {Timeout} ms";

        public override void Run(string argument = "")
        {
            System.Threading.Thread.Sleep(Timeout);
            TriggerRunables();
        }
    }
}
