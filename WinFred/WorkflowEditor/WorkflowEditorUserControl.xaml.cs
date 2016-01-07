using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using James.HelperClasses;
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
        private readonly List<CustomPath> _myLines;
        private CustomPath _currPath;

        public WorkflowEditorUserControl()
        {
            InitializeComponent();
            _myLines = new List<CustomPath>();
            DataContextChanged += WorkflowEditorUserControl_DataContextChanged;
        }

        private void Item_OnUpdate(object sender) => DrawCanvas(sender, null);

        private void DeleteConnection(object sender, MouseButtonEventArgs e)
        {
            var myLine = _myLines.First(line => ReferenceEquals(line.Path, (Path) sender));
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
                var position = tmp.grid.Children[0].TransformToAncestor(editorCanvas).Transform(new Point(7, 5));
                _currPath = new CustomPath((WorkflowComponent) tmp.DataContext, position);
                _currPath.ChangeDestination(position);
                MouseMove += MoveWhileDragging;
                editorCanvas.Children.Add(_currPath.Path);
                editorCanvas.Children.OfType<WorkflowComponentUserControl>().ForEach(component => component.NewSource(tmp.DataContext as WorkflowComponent));
                Mouse.OverrideCursor = Cursors.None;
            }
            catch (Exception)
            {
                DrawCanvas(this, null);
            }
        }

        private void MoveWhileDragging(object sender, MouseEventArgs e)
        {
            if (_currPath != null)
            {
                Mouse.OverrideCursor = Cursors.None;
                if (CurrentHoveredWorkflowComponent != null)
                {
                    var data = CurrentHoveredWorkflowComponent.DataContext;
                    var destination = data as WorkflowComponent;
                    if (destination != null && !destination.IsAllowed(_currPath.Source))
                    {
                        Mouse.OverrideCursor = Cursors.No;
                    }
                }
                var point = e.GetPosition(editorCanvas);
                _currPath.ChangeDestination(point);
            }
        }

        private void FinishDragging(object sender = null, MouseButtonEventArgs e = null)
        {
            if (_currPath != null)
            {
                MouseMove -= MoveWhileDragging;
                Mouse.OverrideCursor = null;
                var curr = CurrentHoveredWorkflowComponent;
                var context = curr?.DataContext as WorkflowComponent;
                if (context != null && context.IsAllowed(_currPath.Source))
                {
                    AddConnection(_currPath.Source, context);
                }
                editorCanvas.Children.Remove(_currPath.Path);
                _currPath = null;
                DrawCanvas(this, null);
            }
        }

        private void UIElement_OnMouseLeave(object sender, MouseEventArgs e) => FinishDragging();

        /// <summary>
        /// Adds the connection if it's not already existing
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
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
            maxHeight = Math.Max(maxHeight, workflow.Triggers.Count() * ComponentHeight);
            DrawComponents(workflow.Triggers.Cast<WorkflowComponent>().ToList(), 0);

            maxHeight = Math.Max(maxHeight, workflow.Actions.Count * ComponentHeight);
            DrawComponents(workflow.Actions.Cast<WorkflowComponent>().ToList(), 1);

            maxHeight = Math.Max(maxHeight, workflow.Outputs.Count * ComponentHeight);
            DrawComponents(workflow.Outputs.Cast<WorkflowComponent>().ToList(), 2);
            return maxHeight;
        }

        private void DrawConnections(Workflow workflow)
        {
            for (var i = 0; i < workflow.Triggers.Count; i++)
            {
                var source = new Point(ComponentWidth - ComponentPadding - CircleRadius,
                    ComponentHeight / 2 + ComponentHeight * i);
                foreach (var runnable in workflow.Triggers[i].Runnables)
                {
                    Point destination;
                    if (runnable is BasicTrigger)
                    {
                        destination = new Point(ComponentPadding + CircleRadius,
                            ComponentHeight / 2 + ComponentHeight * workflow.Triggers.IndexOf(runnable as BasicTrigger));
                    }
                    else
                    {
                        destination = new Point(ComponentWidth + ComponentPadding + CircleRadius,
                            ComponentHeight / 2 + ComponentHeight*workflow.Actions.IndexOf(runnable as BasicAction));
                    }
                    var path = new CustomPath(workflow.Triggers[i], source, destination) {Destination = runnable};
                    path.Path.MouseRightButtonDown += DeleteConnection;
                    editorCanvas.Children.Add(path.Path);
                    _myLines.Add(path);
                }
            }
            for (var i = 0; i < workflow.Actions.Count; i++)
            {
                var source = new Point(2 * ComponentWidth - ComponentPadding - CircleRadius,
                    ComponentHeight / 2 + ComponentHeight*i);
                foreach (var displayer in workflow.Actions[i].Displayables)
                {
                    var destination = new Point(2 * ComponentWidth + ComponentPadding + CircleRadius,
                        ComponentHeight / 2 + ComponentHeight*workflow.Outputs.IndexOf(displayer));
                    var line = new CustomPath(workflow.Actions[i], source, destination) {Destination = displayer};

                    line.Path.MouseRightButtonDown += DeleteConnection;
                    editorCanvas.Children.Add(line.Path);
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
            var triggers = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                from assemblyType in domainAssembly.GetTypes()
                where (typeof (BasicTrigger).IsAssignableFrom(assemblyType) && !assemblyType.IsAbstract)
                select assemblyType).ToArray();
            TriggerContextMenu.ItemsSource = triggers;

            var actions = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                from assemblyType in domainAssembly.GetTypes()
                where (typeof(BasicAction).IsAssignableFrom(assemblyType) && !assemblyType.IsAbstract)
                select assemblyType).ToArray();
            ActionContextMenu.ItemsSource = actions;

            var outputs = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                from assemblyType in domainAssembly.GetTypes()
                where (typeof(BasicOutput).IsAssignableFrom(assemblyType) && !assemblyType.IsAbstract)
                select assemblyType).ToArray();
            OutputContextMenu.ItemsSource = outputs;
        }

        private void AddComponent(object sender, RoutedEventArgs e)
        {
            var component = (Type)((MenuItem)e.OriginalSource).DataContext;
            var instance = (WorkflowComponent)Activator.CreateInstance(component);
            var workflow = (Workflow)DataContext;
            workflow.AddComponent(instance);
            DrawCanvas(this, null);
        }

        #endregion
    }
}