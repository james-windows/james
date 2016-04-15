using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using James.HelperClasses;
using James.Properties;
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

        /// <summary>
        /// Every component needs to provide an individuel summary describing itself
        /// </summary>
        public string Summary => GetSummary();
        public string Description => GetDescription();

        public Workflow ParentWorkflow { get; set; }

        public int Id { get; set; }

        public double X { get; set; }

        public double Y { get; set; }

        public abstract void Run(string[] input);

        public abstract string GetSummary();


        /// <summary>
        /// Every component has to determine if a connection from the providen type is accepted
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public virtual bool IsAllowed(WorkflowComponent source) => source != this && !source.ConnectedTo.Contains(Id);

        public List<int> ConnectedTo { get; set; } = new List<int>();

        /// <summary>
        /// Calls the next to this workflow connected components
        /// </summary>
        /// <param name="arguments"></param>
        public virtual void CallNext(string[] arguments)
        {
            foreach (var id in ConnectedTo)
            {
                Task.Run(() => this.GetNext(id).Run(arguments));
            }
        }

        public virtual string GetDescription() => Resources.General_WorkflowComponent_Description;

        /// <summary>
        /// Converts the current component to an JObject
        /// </summary>
        /// <returns></returns>
        public JObject Persist()
        {
            dynamic component = new JObject();
            component.id = Id;
            component.type = Name.Replace(" ", "").ToLower();
            component.connectedTo = new JArray(ConnectedTo);
            component.y = Y;
            component.x = X;

            foreach (var prop in GetType().GetProperties())
            {
                var attrs = prop.GetCustomAttributes(true);
                if (attrs.OfType<ComponentFieldAttribute>().Count() != 0)
                {
                    component.Add(prop.Name.ToLower(), JToken.FromObject(prop.GetValue(this)));
                }
            }
            component.ToString();
            return component;
        }

        /// <summary>
        /// Loads all components and sets the attributes of the workflow
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public static WorkflowComponent LoadComponent(dynamic component)
        {
            var type = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                            from assemblyType in domainAssembly.GetTypes()
                            where (typeof(WorkflowComponent).IsAssignableFrom(assemblyType) && !assemblyType.IsAbstract && assemblyType.Name.ToLower().StartsWith(GetPropertyValue(component, "type").Replace(" ", "")))
                            select assemblyType).First();

            WorkflowComponent item = (WorkflowComponent) Activator.CreateInstance(type);
            item.Id = component.id;
            item.X = component.x;
            item.Y = component.y;
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
                    else if (prop.PropertyType == typeof(bool))
                    {
                        prop.SetValue(item, bool.Parse(GetPropertyValue(component, prop.Name)));
                    }
                }
            }
            return item;
        }

        /// <summary>
        /// Small helper for the LoadWorkflow() method. 
        /// It fetches the string value to an providen key.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        private static string GetPropertyValue(dynamic item, string property)
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

        public override string ToString() => $"{GetType().Name} id: {Id}";
    }
}