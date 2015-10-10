using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace James.Workflows.Outputs
{
    [DataContract, KnownType(typeof(LargeTypeOutput)), KnownType(typeof(SearchResultOutput))]
    public abstract class BasicOutput : WorkflowComponent
    {
        public abstract void Display(string output);
    }
}
