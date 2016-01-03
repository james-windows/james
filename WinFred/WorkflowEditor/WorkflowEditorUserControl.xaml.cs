using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using James.Workflows;
using James.Workflows.Actions;
using James.Workflows.Outputs;
using James.Workflows.Triggers;

namespace James.WorkflowEditor
{
    /// <summary>
    ///     Interaction logic for WorkflowEditorUserControl.xaml
    /// </summary>
    public partial class WorkflowEditorUserControl : UserControl
    {
        private const int ComponentHeight = 70;
        private const int ComponentWidth = 120;
        private const int ComponentPadding = 10;
        private const int CircleRadius = 5;
        private readonly List<CustomLine> _myLines;
        private CustomLine _currLine;

        public WorkflowEditorUserControl()
        {
            InitializeComponent();
            _myLines = new List<CustomLine>();
            DataContextChanged += WorkflowEditorUserControl_DataContextChanged;
        }

        public Type[] Types { get; set; }

        private void Item_OnUpdate(object sender) => DrawCanvas(sender, null);

        private void DeleteConnection(object sender, MouseButtonEventArgs e)
        {
            var myLine = _myLines.First(line => ReferenceEquals(line.Line, (Line) sender));
            myLine.DeleteConnection();
            DrawCanvas(this, null);
        }

        #region creating connection

        private static FrameworkElement CurrentHoveredWorkflowComponent
        {
            get
            {
                var curr = ((FrameworkElement) Mouse.DirectlyOver);
                while (!(curr is WorkflowComponentUserControl) && curr != null)
                {
                    curr = (FrameworkElement) curr.Parent;
                }
                return curr;
            }
        }

        private void StartDragging(object sender, MouseButtonEventArgs e)
        {
            var tmp = (WorkflowComponentUserControl) sender;
            if (tmp.DataContext is BasicOutput)
            {
                return;
            }
            try
            {
                var position = tmp.grid.Children[0].TransformToAncestor(editorCanvas).Transform(new Point(5, 5));
                _currLine = new CustomLine((WorkflowComponent) tmp.DataContext, position);
                _currLine.ChangeDestination(position);
                MouseMove += MoveWhileDragging;
                editorCanvas.Children.Add(_currLine.Line);
                Mouse.OverrideCursor = Cursors.None;
                editorCanvas.Children.OfType<WorkflowComponentUserControl>().ForEach(component => component.NewSource(tmp.DataContext as WorkflowComponent));
            }
            catch (Exception)
            {
                DrawCanvas(this, null);
            }
        }

        private void MoveWhileDragging(object sender, MouseEventArgs e)
        {
            if (_currLine != null)
            {
                Mouse.OverrideCursor = null;
                if (CurrentHoveredWorkflowComponent != null)
                {
                    var data = CurrentHoveredWorkflowComponent.DataContext;
                    var destination = data as WorkflowComponent;
                    if (destination != null && !destination.IsAllowed(_currLine.Source))
                    {
                        Mouse.OverrideCursor = Cursors.No;
                    }
                }
                //TODO cleaner effect by using the pythagoras
                var point = e.GetPosition(editorCanvas);
                point.X -= 5;
                point.Y -= 5;
                if (_currLine.Line.X1 > point.X)
                {
                    point.X += 10;
                }
                if (_currLine.Line.Y1 > point.Y)
                {
                    point.Y += 10;
                }
                _currLine.ChangeDestination(point);
            }
        }

        private void FinishDragging(object sender = null, MouseButtonEventArgs e = null)
        {
            if (_currLine != null)
            {
                MouseMove -= MoveWhileDragging;
                Mouse.OverrideCursor = null;
                var curr = CurrentHoveredWorkflowComponent;
                var context = curr?.DataContext as WorkflowComponent;
                if (context != null && context.IsAllowed(_currLine.Source))
                {
                    AddConnection(_currLine.Source, context);
                }
                editorCanvas.Children.Remove(_currLine.Line);
                _currLine = null;
                DrawCanvas(this, null);
            }
        }

        private void UIElement_OnMouseLeave(object sender, MouseEventArgs e) => FinishDragging();

        private static void AddConnection(WorkflowComponent source, WorkflowComponent destination)
        {
            if (source is BasicTrigger &&
                ((BasicTrigger) source).Runnables.IndexOf((RunnableWorkflowComponent) destination) == -1)
            {
                ((BasicTrigger) source).Runnables.Add((RunnableWorkflowComponent) destination);
            }
            if (source is BasicAction && ((BasicAction) source).Displayables.IndexOf((BasicOutput) destination) == -1)
            {
                ((BasicAction) source).Displayables.Add((BasicOutput) destination);
            }
        }

        #endregion

        #region drawing Workflow

        private void WorkflowEditorUserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _myLines.Clear();
            editorCanvas.Children.Clear();
            if (DataContext != null)
            {
                DrawCanvas(this, null);
            }
        }

        private void DrawCanvas(object sender, RoutedEventArgs e)
        {
            _myLines.Clear();
            editorCanvas.Children.Clear();

            var workflow = (Workflow) DataContext;
            var maxHeight = DrawComponents(workflow);

            DrawConnections(workflow);
            editorCanvas.Height = maxHeight;
        }

        private int DrawComponents(Workflow workflow)
        {
            var maxHeight = 0;
            maxHeight = Math.Max(maxHeight, workflow.Triggers.Count*ComponentHeight);
            DrawComponents(workflow.Triggers.Cast<WorkflowComponent>().ToList(), 0);

            maxHeight = Math.Max(maxHeight, workflow.Actions.Count*ComponentHeight);
            DrawComponents(workflow.Actions.Cast<WorkflowComponent>().ToList(), 1);

            maxHeight = Math.Max(maxHeight, workflow.Outputs.Count*ComponentHeight);
            DrawComponents(workflow.Outputs.Cast<WorkflowComponent>().ToList(), 2);
            return maxHeight;
        }

        private void DrawConnections(Workflow workflow)
        {
            for (var i = 0; i < workflow.Triggers.Count; i++)
            {
                var source = new Point(ComponentWidth - ComponentPadding - CircleRadius,
                    ComponentHeight/2 + ComponentHeight*i);
                foreach (var runnable in workflow.Triggers[i].Runnables)
                {
                    Point destination;
                    if (runnable is BasicTrigger)
                    {
                        destination = new Point(ComponentPadding + CircleRadius,
                            ComponentHeight/2 + ComponentHeight*workflow.Triggers.IndexOf(runnable as BasicTrigger));
                    }
                    else
                    {
                        destination = new Point(ComponentWidth + ComponentPadding + CircleRadius,
                            ComponentHeight/2 + ComponentHeight*workflow.Actions.IndexOf(runnable as BasicAction));
                    }
                    var line = new CustomLine(workflow.Triggers[i], source) {Destination = runnable};
                    line.ChangeDestination(destination);
                    line.Line.MouseRightButtonDown += DeleteConnection;
                    editorCanvas.Children.Add(line.Line);
                    _myLines.Add(line);
                }
            }
            for (var i = 0; i < workflow.Actions.Count; i++)
            {
                var source = new Point(2*ComponentWidth - ComponentPadding - CircleRadius,
                    ComponentHeight/2 + ComponentHeight*i);
                foreach (var displayer in workflow.Actions[i].Displayables)
                {
                    var destination = new Point(2*ComponentWidth + ComponentPadding + CircleRadius,
                        ComponentHeight/2 + ComponentHeight*workflow.Outputs.IndexOf(displayer));
                    var line = new CustomLine(workflow.Actions[i], source) {Destination = displayer};
                    line.ChangeDestination(destination);
                    editorCanvas.Children.Add(line.Line);
                    line.Line.MouseRightButtonDown += DeleteConnection;
                    _myLines.Add(line);
                }
            }
        }

        public void DrawComponents(List<WorkflowComponent> components, int column)
        {
            for (var i = 0; i < components.Count; i++)
            {
                var item = new WorkflowComponentUserControl(components[i]);
                item.MouseLeftButtonDown += StartDragging;
                item.OnUpdate += Item_OnUpdate;
                Canvas.SetLeft(item, ComponentPadding + ComponentWidth*column);
                Canvas.SetTop(item, ComponentPadding + ComponentHeight*i);
                editorCanvas.Children.Add(item);
            }
        }

        #endregion

        #region Add Component

        private void OpenContextMenu(object sender, RoutedEventArgs e)
        {
            var tmp = (Button) sender;
            tmp.ContextMenu.IsOpen = !tmp.ContextMenu.IsOpen;
        }

        private void FillContextMenu(object sender, RoutedEventArgs e)
        {
            Types = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                from assemblyType in domainAssembly.GetTypes()
                where (typeof (WorkflowComponent).IsAssignableFrom(assemblyType) && !assemblyType.IsAbstract)
                select assemblyType).ToArray();
            AddButton.ItemsSource = Types;
        }

        private void AddComponent(object sender, RoutedEventArgs e)
        {
            var component = (Type) ((MenuItem) sender).DataContext;
            var instance = (WorkflowComponent) Activator.CreateInstance(component);
            var workflow = (Workflow) DataContext;
            workflow.AddComponent(instance);
            DrawCanvas(this, null);
        }

        #endregion
    }
}