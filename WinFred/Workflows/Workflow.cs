using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using James.HelperClasses;
using James.Workflows.Actions;
using James.Workflows.Interfaces;
using James.Workflows.Outputs;
using James.Workflows.Triggers;
using Newtonsoft.Json.Linq;

namespace James.Workflows
{
    public class Workflow
    {
        public Workflow(dynamic item, string path)
        {
            Name = path.Split('\\').Last();
            Author = item.author;
            IsEnabled = item.enabled;

            foreach (var component in item.components)
            {
                Components.Add(WorkflowComponent.LoadComponent(component));
                Components.Last().ParentWorkflow = this;
            }
            _lastId = Components.Count > 0 ? Components.Last().Id + 1: 0;
            LoadWorkflowIcon();
        }

        public Workflow(string name)
        {
            Name = name;
            Directory.CreateDirectory(Path);
            Persist();
            LoadWorkflowIcon();
        }

        /// <summary>
        /// Loads icon of the workflow if no icon is found it takes the james icon
        /// </summary>
        public void LoadWorkflowIcon()
        {
            string iconPath = Path + "\\icon.png";
            if (File.Exists(iconPath))
            {
                Icon = new BitmapImage();
                Icon.BeginInit();
                Icon.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                Icon.CacheOption = BitmapCacheOption.OnLoad;
                Icon.UriSource = new Uri(iconPath);
                Icon.EndInit();
                Icon.Freeze();
            }
            else
            {
                Icon = Config.Instance.Icon;
                Icon.Freeze();
            }
        }

        public string Name { get; set; }

        public BitmapImage Icon { get; set; }

        [DataMember(Order = 3)]
        public string Author { get; set; } = System.Security.Principal.WindowsIdentity.GetCurrent()?.Name.Split('\\').Last();

        [DataMember(Order = 6)]
        public bool IsEnabled { get; set; } = true;

        [DataMember(Order = 7)]
        public List<WorkflowComponent> Components { get; set; } = new List<WorkflowComponent>();

        public List<BasicTrigger> Triggers => Components.OfType<BasicTrigger>().ToList();
        public List<BasicAction> Actions => Components.OfType<BasicAction>().ToList();
        public List<BasicOutput> Outputs => Components.OfType<BasicOutput>().ToList();

        private int _lastId = 0;

        public string Path => Config.WorkflowFolderLocation + "\\" + Name;

        public bool IsCanceled { get; set; } = false;

        /// <summary>
        /// Cancels all running components
        /// </summary>
        public void Cancel()
        {
            IsCanceled = true;
            Components.OfType<ISurviveable>().ForEach(surviveable => surviveable.Cancel());
        }

        /// <summary>
        /// Saves the workflow in form of a .json file to the disc
        /// </summary>
        public void Persist()
        {
            dynamic workflow = new JObject();
            workflow.author = Author;
            workflow.enabled = IsEnabled;
            workflow.components = new JArray(Components.Select(component => component.Persist()).ToArray());
            File.WriteAllText(Path + "\\config.json", workflow.ToString());
        }

        /// <summary>
        /// Adds a new WorkflowComponent to the Workflow
        /// </summary>
        /// <param name="instance"></param>
        public void AddComponent(WorkflowComponent instance)
        {
            instance.Id = _lastId;
            _lastId++;
            instance.ParentWorkflow = this;
            Components.Add(instance);
            WorkflowManager.Instance.LoadKeywordTriggers();
        }

        /// <summary>
        /// Removes a WorkflowComponent and it's connections from the workflow
        /// </summary>
        /// <param name="component"></param>
        public void RemoveComponent(WorkflowComponent component)
        {
            Triggers.ForEach(trigger => trigger.ConnectedTo.Remove(component.Id));
            Actions.ForEach(action => action.ConnectedTo.Remove(component.Id));

            Components.Remove(component);
        }

        public void OpenFolder() => Process.Start(Path);

        /// <summary>
        /// Tries to delete the folder of the workflow
        /// notifies the user if an error occured
        /// </summary>
        public void Remove()
        {
            try
            {
                Directory.Delete(Path, true);
            }
            catch (Exception)
            {
                MessageBox.Show("An error occured while deleting the folder of the workflow: " + Name, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        /// <summary>
        /// archives the workflow folder and renames it to a .james file for export
        /// </summary>
        public void Export()
        {
            FileDialog dialog = new SaveFileDialog();
            dialog.FileName = Name + ".james";
            dialog.Filter = "james workflows|*.james";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                ZipFile.CreateFromDirectory(Path, dialog.FileName);
            }
        }
    }
}