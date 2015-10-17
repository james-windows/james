using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using James.WorkflowEditor;
using MahApps.Metro.Controls;

namespace James.Workflows
{
    [System.AttributeUsage(System.AttributeTargets.Property)
]
    class ComponentFieldAttribute: Attribute
    {
        public string Description { get; private set; }
        public bool IsFile { get; private set; }
        public ComponentFieldAttribute(string description, bool isFile = false)
        {
            this.Description = description;
            IsFile = isFile;
        }

        public FrameworkElement GetElement(PropertyInfo prop, WorkflowComponent component)
        {
            string description = Description;
            FrameworkElement tmp = null;
            switch (prop.PropertyType.Name)
            {
                case "Int32":
                    tmp = new NumericUpDown() { Value = (int)prop.GetValue(component) };
                    break;
                case "String":
                    tmp = new TextBox() { Text = prop.GetValue(component).ToString() };
                    break;
            }
            return tmp != null ? new ComponentPropertyUserControl(description, tmp, prop, component) : null;
        }
    }
}
