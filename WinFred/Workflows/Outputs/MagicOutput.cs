using System.Collections.Generic;
using James.ResultItems;

namespace James.Workflows.Outputs
{
    public class MagicOutput : BasicOutput
    {
        const char HORIZONTALSPLIT = '|';

        [ComponentField("Subtitle Format")]
        public string SubtitleFormat { get; set; } = "{0}";

        public override void Run(string[] output)
        {
            var outputResults = new List<ResultItem>();
            foreach (string text in output)
            {
                if (text.Trim().Length > 0)
                {
                    string[] splits = text.Split(HORIZONTALSPLIT);
                    outputResults.Add(new MagicResultItem()
                    {
                        Title = FormatStringToText(FormatString, splits),
                        Subtitle = FormatStringToText(SubtitleFormat, splits),
                        WorkflowComponent = this,
                        WorkflowArguments = splits
                    });
                }
            }
            
            MainWindow.GetInstance().searchResultControl.WorkflowOutput(outputResults);
        }

        public override string GetSummary() => $"Displays the response in SearchResultList";
    }
}