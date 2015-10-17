using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using James.Workflows;
using James.Workflows.Actions;
using James.Workflows.Outputs;
using James.Workflows.Triggers;
using Brushes = System.Windows.Media.Brushes;

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
                ToolTip = "right click to remove",
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
            if (Source is BasicTrigger)
            {
                ((BasicTrigger)Source).Runnables.Remove(
                    (RunnableWorkflowComponent)Destination);
            }
            if (Source is BasicAction)
            {
                ((BasicAction)Source).Displayables.Remove((BasicOutput)Destination);
            }
        }
    }
}
