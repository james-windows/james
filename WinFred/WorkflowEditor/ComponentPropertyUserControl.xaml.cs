using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using James.Workflows;
using MahApps.Metro.Controls;
using Action = Microsoft.Win32.TaskScheduler.Action;

namespace James.WorkflowEditor
{
    /// <summary>
    /// Interaction logic for ComponentPropertyUserControl.xaml
    /// </summary>
    public partial class ComponentPropertyUserControl : UserControl
    {
        private readonly PropertyInfo _prop;
        private readonly WorkflowComponent _component;
        private readonly FrameworkElement _element;
        public ComponentPropertyUserControl(string description, FrameworkElement element, PropertyInfo prop, WorkflowComponent component)
        {
            InitializeComponent();
            descriptionBox.Text = description + " :";
            Grid.SetColumn(element,1);
            Grid.Children.Add(element);
            _element = element;
            _prop = prop;
            _component = component;
        }

        public void WriteToWorkflowComponent()
        {
            switch (_prop.PropertyType.Name)
            {
                case "Int32":
                    var value = ((NumericUpDown)_element).Value;
                    if (value != null)
                        _prop.SetValue(_component, (int)value);
                    break;
                case "String":
                    _prop.SetValue(_component, ((TextBox)_element).Text);
                    break;
            }
        }
    }
}
