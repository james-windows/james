using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using James.WorkflowEditor;
using MahApps.Metro.Controls;

namespace James.Workflows
{
    [AttributeUsage(AttributeTargets.Property)
    ]
    internal class ComponentFieldAttribute : Attribute
    {
        public ComponentFieldAttribute(string description, bool isFile = false)
        {
            Description = description;
            IsFile = isFile;
        }

        public string Description { get; }
        public bool IsFile { get; private set; }

        public FrameworkElement GetElement(PropertyInfo prop, WorkflowComponent component)
        {
            var description = Description;
            FrameworkElement tmp = null;
            switch (prop.PropertyType.Name)
            {
                case "Int32":
                    tmp = new NumericUpDown {Value = (int) prop.GetValue(component), SelectAllOnFocus = true};
                    break;
                case "String":
                    tmp = new TextBox {Text = prop.GetValue(component).ToString()};
                    break;
            }
            return tmp != null ? new ComponentPropertyUserControl(description, tmp, prop, component) : null;
        }
    }
}