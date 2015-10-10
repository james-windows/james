using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Xml.Serialization;
using James.Workflows.Interfaces;

namespace James.Workflows.Outputs
{
    [DataContract]
    public class LargeTypeOutput: BasicOutput
    {
        public override void Display(string output)
        {
            //WorkflowManager.GetInstance(null).ParentSearchResultUserControl.GetParentWindow().DisplayLargeTypeOutput(output);
        }
    }
}
