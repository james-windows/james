using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using James.HelperClasses;
using James.Properties;
using James.Workflows.Actions;
using James.Workflows.Outputs;
using James.Workflows.Triggers;
using Microsoft.Win32.TaskScheduler;
using Newtonsoft.Json.Linq;
using Task = System.Threading.Tasks.Task;

namespace James.Workflows
{
    public abstract class WorkflowComponent
    {
        protected WorkflowComponent()
        {
        }

        protected WorkflowComponent(Workflow parent)
        {
            ParentWorkflow = parent;
        }

        public FrameworkElement UiElement { get; set; }
        public string Name
        {
            get
            {
                string name = GetType().Name.Trim();
                name = name.Replace("Trigger", "");
                name = name.Replace("Action", "");
                name = name.Replace("Output", "");
                name = Regex.Replace(name, "(?<=[a-z])([A-Z])", " $1", RegexOptions.Compiled);
                return name;
            }
        }

        public string Summary => GetSummary();
        public string Description => GetDescription();

        public Workflow ParentWorkflow { get; set; }

        public int Id { get; set; }

        public abstract void Run(string[] input);

        public abstract string GetSummary();

        public virtual bool IsAllowed(WorkflowComponent source) => false;

        public List<int> ConnectedTo { get; set; } = new List<int>();

        public void CallNext(string[] arguments)
        {
            foreach (var id in ConnectedTo)
            {
                Task.Run(() => this.GetNext(id).Run(arguments));
            }
        }

        public string GetDescription() => Resources.General_WorkflowComponent_Description;

        public JObject Persist()
        {
            dynamic component = new JObject();
            component.id = Id;
            component.type = Name.Replace(" ", "").ToLower();
            component.connectedTo = new JArray(ConnectedTo);
            component.y = GetRow() * 100 + 50;
            component.x = GetColumn() * 150 + 20;

            foreach (var prop in GetType().GetProperties())
            {
                var attrs = prop.GetCustomAttributes(true);
                if (attrs.OfType<ComponentFieldAttribute>().Count() != 0)
                {
                    component.Add(prop.Name.ToLower(), JToken.FromObject(prop.GetValue(this)));
                }
            }
            return component;
        }

        public static WorkflowComponent LoadComponent(dynamic component)
        {
            var type = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                            from assemblyType in domainAssembly.GetTypes()
                            where (typeof(WorkflowComponent).IsAssignableFrom(assemblyType) && assemblyType.Name.ToLower().StartsWith(GetPropertyValue(component, "type").Replace(" ", "")))
                            select assemblyType).First();

            WorkflowComponent item = (WorkflowComponent) Activator.CreateInstance(type);
            item.Id = component.id;
            foreach (var id in component.connectedTo)
            {
                item.ConnectedTo.Add(int.Parse(id.ToString()));   
            }
            foreach (var prop in type.GetProperties())
            {
                var attrs = prop.GetCustomAttributes(true);
                if (attrs.OfType<ComponentFieldAttribute>().Count() != 0)
                {
                    if (prop.PropertyType == typeof(string))
                    {
                        prop.SetValue(item, GetPropertyValue(component, prop.Name));
                    }
                    else if (prop.PropertyType == typeof(int))
                    {
                        prop.SetValue(item, int.Parse(GetPropertyValue(component, prop.Name)));
                    }
                }
            }
            return item;
        }

        public static string GetPropertyValue(dynamic item, string property)
        {
            foreach (var prop in item)
            {
                if (prop.Name.ToLower() == property.ToLower())
                {
                    return prop.Value.ToString();
                }
            }
            return "";
        }

        public override string ToString()
        {
            return GetType().Name + $" id: {Id}";
        }

        /// <summary>
        /// returns the column of the component
        /// </summary>
        /// <returns>value in [0,2]</returns>
        public abstract int GetColumn();

        /// <summary>
        /// Returns the index of the element in the column
        /// </summary>
        /// <returns></returns>
        public abstract int GetRow();
    }
}