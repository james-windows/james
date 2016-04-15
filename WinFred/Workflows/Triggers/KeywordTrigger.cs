using System.Threading.Tasks;
using James.HelperClasses;
using James.Workflows.Outputs;

namespace James.Workflows.Triggers
{
    public class KeywordTrigger : BasicTrigger
    {
        public KeywordTrigger()
        {
        }

        public KeywordTrigger(Workflow parent) : base(parent)
        {
        }

        [ComponentField("Title: Gets displayed with the SearchResults")]
        public string Title { get; set; } = "";

        [ComponentField("Subtitle: Gets displayed with the SearchResults")]
        public string Subtitle { get; set; } = "";

        [ComponentField("Listens for this keyword to trigger")]
        public string Keyword { get; set; } = "";

        [ComponentField("Auto runs the trigger")]
        public bool Autorun { get; set; } = false;

        public override string GetSummary() => $"Triggers for \"{Keyword}\"";

        public override void Run(string[] arguments)
        {
            ParentWorkflow.IsCanceled = false;
            CallNext(arguments);
        }

        /// <summary>
        /// Calls the next Componenten which are connected to the current one
        /// </summary>
        /// <param name="arguments"></param>
        public override void CallNext(string[] arguments)
        {
            foreach (var id in ConnectedTo)
            {
                var next = this.GetNext(id);
                if (next is MagicOutput)
                {
                    Task.Run(() => this.GetNext(id).Run(new [] { string.Join("|", arguments), }));
                }
                else
                {
                    Task.Run(() => this.GetNext(id).Run(arguments));
                }
            }
        }

        public override string GetDescription() => "Triggers once the given keyword is entered into the SearchBox";
    }
}