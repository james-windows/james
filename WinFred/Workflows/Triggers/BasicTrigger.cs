using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using James.Workflows.Actions;
using James.Workflows.Interfaces;

namespace James.Workflows.Triggers
{
    [DataContract, KnownType(typeof(KeywordTrigger)), KnownType(typeof(IntervalTrigger))]
    public abstract class BasicTrigger : RunnableWorkflowComponent, ICrossPlatform
    {
        [DataMember]
        public List<RunnableWorkflowComponent> Runnables { get; set; } = new List<RunnableWorkflowComponent>();

        protected BasicTrigger(): base()
        {
            
        }

        protected BasicTrigger(Workflow parent):base(parent)
        {
            
        }

        public void TriggerRunables()
        {
            Console.WriteLine($"{ParentWorkflow.Title} - Event got triggerd");
            foreach (RunnableWorkflowComponent item in Runnables)
            {
                item.Run();
            }
        }
    }
}
