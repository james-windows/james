using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Controls;
using James.Workflows;
using MahApps.Metro.Controls;
using Microsoft.Win32.TaskScheduler;

namespace James.WorkflowEditor
{
    /// <summary>
    /// Interaction logic for ComponentWindow.xaml
    /// </summary>
    public partial class ComponentWindow : MetroWindow
    {
        private WorkflowComponent component;
        public ComponentWindow(WorkflowComponent component)
        {
            InitializeComponent();
            this.component = component;
            DataContext = component;
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Type type = component.GetType();
            PropertyInfo[] propertyInfos = type.GetProperties();
            foreach (PropertyInfo prop in propertyInfos)
            {
                object[] attrs = prop.GetCustomAttributes(true);
                foreach (object attr in attrs)
                {
                    ComponentFieldAttribute componentFieldAttribute = attr as ComponentFieldAttribute;
                    if (componentFieldAttribute != null)
                    {
                        propertyPanel.Children.Add(componentFieldAttribute.GetElement(prop, component));   
                    }
                }
            }
        }

        private void DiscardChanges(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OpenWorkflowDirectory(object sender, RoutedEventArgs e)
        {
            component.ParentWorkflow.OpenFolder();
        }

        private void SaveChanges(object sender, RoutedEventArgs e)
        {
            foreach (ComponentPropertyUserControl componentPropertyUserControl in propertyPanel.Children.Cast<ComponentPropertyUserControl>())
            {
                componentPropertyUserControl.WriteToWorkflowComponent();
            }
            Close();
        }
    }
}
