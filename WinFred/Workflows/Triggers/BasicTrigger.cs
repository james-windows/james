using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using James.Workflows.Interfaces;

namespace James.Workflows.Triggers
{
    [DataContract, KnownType(typeof (KeywordTrigger)), KnownType(typeof (IntervalTrigger))]
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

        public void TriggerRunables()
        {
            Console.WriteLine($"{ParentWorkflow.Title} - Event got triggerd");
            foreach (var item in Runnables)
            {
                item.Run();
            }
        }
    }
}