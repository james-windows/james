using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using James.Properties;
using James.Workflows;
using James.Workflows.Actions;
using James.Workflows.Outputs;
using James.Workflows.Triggers;

namespace James.WorkflowEditor
{
    public class CustomLine
    {
        public CustomLine(WorkflowComponent sourceComponent, Point source)
        {
            Line = new Line
            {
                X1 = source.X,
                Y1 = source.Y,
                Stroke = Brushes.White,
                StrokeThickness = 10,
                ToolTip = Resources.CustomLine_RightClickToRemoveLine,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeEndLineCap = PenLineCap.Round
            };
            Source = sourceComponent;
        }

        public CustomLine()
        {
        }

        public WorkflowComponent Source { get; set; }
        public WorkflowComponent Destination { get; set; }
        public Line Line { get; set; }

        public void ChangeDestination(Point destination)
        {
            Line.X2 = destination.X;
            Line.Y2 = destination.Y;
        }

        public void DeleteConnection()
        {
            var trigger = Source as BasicTrigger;
            trigger?.Runnables.Remove((RunnableWorkflowComponent) Destination);
            var action = Source as BasicAction;
            action?.Displayables.Remove((BasicOutput) Destination);
        }
    }
}