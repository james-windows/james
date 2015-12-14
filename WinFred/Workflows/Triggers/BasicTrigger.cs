using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using James.Properties;
using James.Workflows.Interfaces;

namespace James.Workflows.Triggers
{
    [DataContract, KnownType(typeof (KeywordTrigger)), KnownType(typeof (IntervalTrigger)), KnownType(typeof (TimeoutTrigger))]
    public abstract class BasicTrigger : RunnableWorkflowComponent, ICrossPlatform
    {
        protected BasicTrigger()
        {
        }

        protected BasicTrigger(Workflow parent) : base(parent)
        {
        }

        [DataMember]
        public List<RunnableWorkflowComponent> Runnables { get; set; } = new List<RunnableWorkflowComponent>();

        public void TriggerRunables(string arguments = "")
        {
            Console.WriteLine(ParentWorkflow.Title + Resources.BasicTrigger_EventGotTriggered_Notification);
            Runnables.ForEach(component => component.Run(arguments));
        }
    }
}