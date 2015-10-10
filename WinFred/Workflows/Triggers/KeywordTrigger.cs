using System.Runtime.Serialization;

namespace James.Workflows.Triggers
{
    [DataContract]
    public class KeywordTrigger : BasicTrigger
    {
        public KeywordTrigger()
        {
        }

        public KeywordTrigger(Workflow parent) : base(parent)
        {
        }

        [DataMember]
        public string Keyword { get; set; }

        public override void Run()
        {
            TriggerRunables();
        }
    }
}