using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using James.Workflows.Interfaces;

namespace James.Workflows.Triggers
{
    [DataContract]
    public class KeywordTrigger: BasicTrigger
    {
        [DataMember]
        public string Keyword { get; set; }

        public override void Run()
        {
            TriggerRunables();
        }

        public KeywordTrigger():base()
        {
            
        }

        public KeywordTrigger(Workflow parent) : base(parent)
        {
        }
    }
}
