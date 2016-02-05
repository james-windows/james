using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Imaging;
using James.HelperClasses;
using James.ResultItems;

namespace James.Workflows.Outputs
{
    public class MagicOutput : BasicOutput
    {
        const char HORIZONTALSPLIT = '|';

        [ComponentField("Subtitle Format")]
        public string SubtitleFormat { get; set; } = "{0}";

        [ComponentField("Icon Format")]
        public string IconFormat { get; set; } = "{0}";

        [ComponentField("Auto close after execution")]
        public bool Hide { get; set; } = false;

        public override void Run(string[] output)
        {
            var outputResults = new List<MagicResultItem>();
            foreach (string text in output)
            {
                if (text.Trim().Length > 0)
                {
                    string[] splits = text.Split(HORIZONTALSPLIT);
                    outputResults.Add(new MagicResultItem()
                    {
                        Icon = GetIcon(IconFormat.InsertArguments(splits)),
                        Title = FormatString.InsertArguments(splits),
                        Subtitle = SubtitleFormat.InsertArguments(splits),
                        WorkflowComponent = this,
                        WorkflowArguments = splits
                    });
                }
            }
            if (outputResults.Count > 0)
            {
                Windows.MainWindow.GetInstance().searchResultControl.WorkflowOutput(outputResults);
            }
        }

        private BitmapImage GetIcon(string filePath)
        {
            if (!File.Exists(filePath))
            {
                filePath = ParentWorkflow.Path + "\\" + filePath;
            }
            if (File.Exists(filePath))
            {
                try
                {
                    BitmapImage image = new BitmapImage(new Uri(filePath));
                    return image;
                }
                catch (Exception)
                {
                    return ParentWorkflow.Icon;
                }
            }
            return ParentWorkflow.Icon;
        }

        public override string GetSummary() => $"Displays the response in SearchResultList";
    }
}