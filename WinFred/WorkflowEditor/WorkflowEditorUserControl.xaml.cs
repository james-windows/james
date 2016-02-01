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
        private const int ComponentPadding = 10;
        private readonly List<CustomPath> _myLines;
        
        public WorkflowEditorUserControl()
        {
            InitializeComponent();
            _myLines = new List<CustomPath>();
            DataContextChanged += WorkflowEditorUserControl_DataContextChanged;
            SizeChanged += (sender, args) => MouseHelper.ClearMouseTrap();
        }

        private void Item_OnUpdate(object sender) => DrawCanvas(sender, null);

        private List<WorkflowComponentUserControl> WorkflowComponentUserControls => editorCanvas.Children.OfType<WorkflowComponentUserControl>().ToList();

        private Workflow Workflow => DataContext as Workflow;

        private void DeleteConnection(object sender, MouseButtonEventArgs e)
        {
            var myLine = _myLines.First(line => ReferenceEquals(line.Path, (Path) sender));
            myLine.DeleteConnection();
            DrawCanvas(this, null);
        }

        #region edit workflow

        private static FrameworkElement CurrentHoveredComponent
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

        private Point _startPoint;
        private WorkflowComponentUserControl _selectedComponent;
        private CustomPath _customPath;

        private void StartDragging(object sender, MouseButtonEventArgs e)
        {
            MouseHelper.TrapMouseInsideControl(editorBorder);
            _selectedComponent = (WorkflowComponentUserControl) sender;
            if (e.OriginalSource is Ellipse)
            {
                var component = _selectedComponent.Component;
                _customPath = new CustomPath(component, new Point(component.X + 95, component.Y + ComponentHeight / 2 - ComponentPadding));
                editorCanvas.Children.Add(_customPath.Path);
                MouseMove += MovePath;
                WorkflowComponentUserControls.ForEach(comp => comp.NewSource(_selectedComponent.Component));
            }
            else
            {
                _startPoint = e.GetPosition(_selectedComponent);
                MouseMove += MoveComponent;
            }
        }
        
        private void MoveComponent(object sender, MouseEventArgs e)
        {
            if (_selectedComponent == null) return;
            var component = _selectedComponent.Component;
            var point = e.GetPosition(editorCanvas);
            double newX = point.X - _startPoint.X;
            if (newX < 0)
            {
                foreach (var comp in Workflow.Components.Where(item => item != component))
                {
                    comp.X -= newX;
                }
                component.X = 0;
            }
            else
            {
                component.X = newX;
            }
            double newY = point.Y - _startPoint.Y;
            if (newY < 0)
            {
                foreach (var comp in Workflow.Components.Where(item => item != component))
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

                double minY = components.Min(component => component.Y) - 5;
                components.ForEach(component => component.Y -= minY);
            }
        } 

        private void MovePath(object sender, MouseEventArgs e)
        {
            _customPath?.ChangeDestination(e.GetPosition(editorCanvas));
        }

        private void FinishMoving(object sender, MouseButtonEventArgs e)
        {
            MouseHelper.ClearMouseTrap();
            if (_selectedComponent != null)
            {
                _startPoint = new Point();
                MouseMove -= MoveComponent;
                _selectedComponent = null;
            }
            if (_customPath != null) 
            {
                MouseMove -= MovePath;
                editorCanvas.Children.Remove(_customPath.Path);
                if (CurrentHoveredComponent != null)
                {
                    AddConnection(_customPath.Source, (CurrentHoveredComponent as WorkflowComponentUserControl).Component);
                }
                _customPath = null;
                DrawCanvas(this, null);
            }
        }

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
            
            DrawComponents(Workflow.Components);
            DrawConnections();
        }

        private void DrawComponents(List<WorkflowComponent> components)
        {
            if (components.Count > 0)
            {
                CorrectLeftAndTop(components);
                editorCanvas.Height = components.Max(component => component.Y) + 60;
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

        private void DrawConnections()
        {
            foreach (var item in Workflow.Components)
            {
                var source = new Point(item.X + 95, item.Y + ComponentHeight / 2 - ComponentPadding);

                foreach (var id in item.ConnectedTo)
                {
                    WorkflowComponent nextComponent = Workflow.GetNext(id);
                    Point destination = new Point(nextComponent.X + ComponentPadding - 5, nextComponent.Y + ComponentHeight / 2 - ComponentPadding);

                    var path = new CustomPath(item, source, destination) { Destination = nextComponent };
                    path.Path.MouseLeftButtonDown += DeleteConnection;
                    editorCanvas.Children.Add(path.Path);
                    _myLines.Add(path);
                }
            }
        }

        #endregion

        private void FillContextMenu(object sender, RoutedEventArgs e)
        {
            TriggerContextMenu.ItemsSource = GetAllChildTypesInAssembly(typeof(BasicTrigger));
            ActionContextMenu.ItemsSource = GetAllChildTypesInAssembly(typeof(BasicAction));
            OutputContextMenu.ItemsSource = GetAllChildTypesInAssembly(typeof(BasicOutput));
        }

        /// <summary>
        /// Returnes all types in the assembly which inherits from the given type.
        /// Also excludes all abstract types.
        /// </summary>
        /// <param name="type">The type from which to get the child classes</param>
        /// <returns>All classes which inherits from the given type, excluding abstract classes</returns>
        private IEnumerable<Type> GetAllChildTypesInAssembly(Type type)
        {
            return (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                    from assemblyType in domainAssembly.GetTypes()
                    where (type.IsAssignableFrom(assemblyType) && !assemblyType.IsAbstract)
                    select assemblyType);
        }

        #region Add Component

        private void AddComponent(object sender, RoutedEventArgs e)
        {
            var component = (Type)((MenuItem)e.OriginalSource).DataContext;
            var instance = (WorkflowComponent)Activator.CreateInstance(component);
            instance.X = point.X;
            instance.Y = point.Y;
            Workflow.AddComponent(instance);
            DrawCanvas(this, null);
        }

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

        #endregion

        private void UIElement_OnMouseLeave(object sender, MouseEventArgs e)
        {
            FinishMoving(this, null);
        }
    }
}