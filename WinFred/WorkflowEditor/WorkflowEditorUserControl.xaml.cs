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
        private readonly List<CustomPath> _myLines;

        public WorkflowEditorUserControl()
        {
            InitializeComponent();
            _myLines = new List<CustomPath>();
            DataContextChanged += WorkflowEditorUserControl_DataContextChanged;
        }

        private void Item_OnUpdate(object sender) => DrawCanvas(sender, null);

        private List<WorkflowComponentUserControl> WorkflowComponentUserControls
            => editorCanvas.Children.OfType<WorkflowComponentUserControl>().ToList();

        private Workflow CurrentWorkflow => DataContext as Workflow;

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

        private Point startPoint;
        private WorkflowComponentUserControl selectedComponentUserControl;
        private CustomPath customPath;

        private void StartDragging(object sender, MouseButtonEventArgs e)
        {
            selectedComponentUserControl = (WorkflowComponentUserControl) sender;
            if (e.OriginalSource is Ellipse)
            {
                var component = selectedComponentUserControl.DataContext as WorkflowComponent;
                customPath = new CustomPath(component, new Point(component.X + 95, component.Y + ComponentHeight / 2 - ComponentPadding));
                editorCanvas.Children.Add(customPath.Path);
                MouseMove += MovePath;
                WorkflowComponentUserControls.ForEach(comp => comp.NewSource(selectedComponentUserControl.DataContext as WorkflowComponent));
            }
            else
            {
                startPoint = e.GetPosition(selectedComponentUserControl);
                MouseMove += MoveComponent;
            }
        }

        private void MoveComponent(object sender, MouseEventArgs e)
        {
            if (selectedComponentUserControl == null) return;
            var component = selectedComponentUserControl.DataContext as WorkflowComponent;
            var point = e.GetPosition(editorCanvas);
            double newX = point.X - startPoint.X;
            if (newX < 0)
            {
                foreach (var comp in CurrentWorkflow.Components.Where(workflowComponent => workflowComponent != component))
                {
                    comp.X -= newX;
                }
                component.X = 0;
            }
            else
            {
                component.X = newX;
            }
            double newY = point.Y - startPoint.Y;
            if (newY < 0)
            {
                foreach (var comp in CurrentWorkflow.Components.Where(workflowComponent => workflowComponent != component))
                {
                    comp.Y -= newY;
                }
                component.Y = 0;
            }
            else
            {
                component.Y = newY;
            }
            DrawCanvas(this, null);
        }

        private void CorrectLeftAndTop(List<WorkflowComponent> components)
        {
            if (components.Count > 0)
            {
                double minX = components.Min(component => component.X);
                components.ForEach(component => component.X -= minX);

                double minY = components.Min(component => component.Y);
                components.ForEach(component => component.Y -= minY);
            }
        } 

        private void MovePath(object sender, MouseEventArgs e)
        {
            customPath?.ChangeDestination(e.GetPosition(editorCanvas));
        }

        private void FinishMoving(object sender, MouseButtonEventArgs e)
        {
            if (selectedComponentUserControl != null)
            {
                startPoint = new Point();
                MouseMove -= MoveComponent;
                selectedComponentUserControl = null;
            }
            if (customPath != null) 
            {
                MouseMove -= MovePath;
                editorCanvas.Children.Remove(customPath.Path);
                if (CurrentHoveredWorkflowComponent != null)
                {
                    AddConnection(customPath.Source, (CurrentHoveredWorkflowComponent as WorkflowComponentUserControl).DataContext as WorkflowComponent);
                }
                customPath = null;
                DrawCanvas(this, null);
            }
        }

        private void UIElement_OnMouseLeave(object sender, MouseEventArgs e) => FinishMoving(this, null);

        /// <summary>
        /// Adds the connection if it's not already existing
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        private static void AddConnection(WorkflowComponent source, WorkflowComponent destination)
        {
            if (!source.ConnectedTo.Contains(destination.Id) && source != destination && destination.IsAllowed(source))
            {
                source.ConnectedTo.Add(destination.Id);
            }
        }

        #endregion

        #region drawing Workflow

        private void WorkflowEditorUserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            (e.OldValue as Workflow)?.Persist();
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
            DrawComponents(workflow.Components);

            DrawConnections(workflow);
        }

        private void DrawComponents(List<WorkflowComponent> components)
        {
            CorrectLeftAndTop(CurrentWorkflow.Components);
            if (components.Count > 0)
            {
                editorCanvas.Height = components.Max(component => component.Y) + 50;
                editorCanvas.Width = components.Max(component => component.X) + 100;
            }
            foreach (var component in components)
            {
                var item = new WorkflowComponentUserControl(component);
                item.MouseLeftButtonDown += StartDragging;
                item.MouseLeftButtonUp += FinishMoving;
                item.OnUpdate += Item_OnUpdate;
                editorCanvas.Children.Add(item);
            }
        }

        private void DrawConnections(Workflow workflow)
        {
            foreach (var item in workflow.Components)
            {
                var source = new Point(item.X + 95, item.Y + ComponentHeight / 2 - ComponentPadding);

                foreach (var id in item.ConnectedTo)
                {
                    WorkflowComponent nextComponent = workflow.GetNext(id);
                    Point destination = new Point(nextComponent.X + ComponentPadding - 5, nextComponent.Y + ComponentHeight / 2 - ComponentPadding);

                    var path = new CustomPath(item, source, destination) { Destination = nextComponent };
                    path.Path.MouseLeftButtonDown += DeleteConnection;
                    editorCanvas.Children.Add(path.Path);
                    _myLines.Add(path);
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
            instance.X = point.X;
            instance.Y = point.Y;
            var workflow = (Workflow)DataContext;
            workflow.AddComponent(instance);
            DrawCanvas(this, null);
        }

        #endregion

        private Point point = new Point();

        private void Border_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            double offset = (border.ActualWidth - viewBox.ActualWidth);
            if (e.CursorLeft > (sender as FrameworkElement).ActualWidth / 2)
            {
                offset *= -1;
            }
            point.X = e.CursorLeft / border.ActualWidth * editorCanvas.ActualWidth;
            offset = (border.ActualHeight - viewBox.ActualHeight);
            if (e.CursorTop > (sender as FrameworkElement).ActualHeight / 2)
            {
                offset *= -1;
            }
            point.Y = e.CursorTop / viewBox.ActualHeight * editorCanvas.ActualHeight;
        }
    }
}