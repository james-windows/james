using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using James.HelperClasses;
using James.Workflows;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using DragDropEffects = System.Windows.DragDropEffects;
using DragEventArgs = System.Windows.DragEventArgs;

namespace James.WorkflowEditor
{
    /// <summary>
    /// Interaction logic for WorkflowSettingsDialog.xaml
    /// </summary>
    public partial class WorkflowSettingsDialog : BaseMetroDialog
    {
        private Workflow Workflow => DataContext as Workflow;
        private readonly string _workflowName;
        public WorkflowSettingsDialog(Workflow workflow)
        {
            InitializeComponent();
            _workflowName = workflow.Name;
            DataContext = workflow;
            WorkflowImage.Source = workflow.Icon;
        }

        private void FinishedWorkflow(object sender, RoutedEventArgs e)
        {
            if (Workflow.Name != _workflowName)
            {
                WorkflowImage.Source = null;
                try
                {
                    Directory.Move(Workflow.Path.Replace(Workflow.Name, _workflowName), Workflow.Path);
                }
                catch (IOException)
                {
                    Workflow.Name = _workflowName;
                }
                
            }
            Workflow.Persist();
            Close();
        }

        private void OpenWorkflowFolder(object sender, RoutedEventArgs e)
        {
            Workflow.OpenFolder();
        }

        private async void DeleteWorkflow(object sender, RoutedEventArgs e)
        {
            var parentWindow = (MetroWindow)Window.GetWindow(this);
            var setting = new MetroDialogSettings
            {
                NegativeButtonText = "Cancel",
                AffirmativeButtonText = "Yes, I'm sure!"
            };
            var result =
                await
                    parentWindow.ShowMessageAsync("Delete Workflow", "Are you sure?",
                        MessageDialogStyle.AffirmativeAndNegative, setting);
            if (MessageDialogResult.Affirmative == result)
            {
                WorkflowManager.Instance.Remove(Workflow);
            }
            Close();
        }

        private void ExportWorkflow(object sender, RoutedEventArgs e)
        {
            Workflow.Export();
        }

        private void Close()
        {
            var window = (MetroWindow)Window.GetWindow(this);
            window.HideMetroDialogAsync(this);
        }

        private void ImportWorkflowIcon(object sender, RoutedEventArgs e)
        {
            FileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Image Files (*.bmp;*.jpg;*.jpeg,*.png)|*.BMP;*.JPG;*.JPEG;*.PNG";
            if (dialog.ShowDialog() == DialogResult.OK && File.Exists(dialog.FileName))
            {
                LoadIcon(dialog.FileName);
            }
        }

        private void DropFilePath(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
            {
                string[] paths = (string[]) e.Data.GetData(System.Windows.DataFormats.FileDrop);
                if (paths.Length > 0)
                {
                    LoadIcon(paths[0]);
                }
            }
        }

        /// <summary>
        /// Tests to load the icon correctly and saves it into the workflow folder on success
        /// Also updates the Icon in the Workflow object
        /// </summary>
        /// <param name="path"></param>
        private async void LoadIcon(string path)
        {
            try
            {
                Bitmap bitmap = new Bitmap(path);
                bitmap.Save(Workflow.Path + "\\icon.png");
                Workflow.LoadWorkflowIcon();
                WorkflowImage.Source = null;
                WorkflowImage.Source = Workflow.Icon;
            }
            catch (Exception e) when (e is ExternalException || e is ArgumentException)
            {
                var parentWindow = (MetroWindow)Window.GetWindow(this);
                await parentWindow.ShowMessageAsync("Icon Error",
                    "Icon couldn't be importet! Make sure it has the correct file format");
            }
        }
    }
}
