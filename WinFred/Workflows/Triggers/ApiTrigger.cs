using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace James.Workflows.Triggers
{
    class ApiTrigger: BasicTrigger
    {
        public override void Run(string[] input) => CallNext(input);

        [ComponentField("The name of the action to listen")]
        public string Action { get; set; } = "";

        public override string GetSummary() => $"Listens for {Action} in the james: protocol";
    }
}
