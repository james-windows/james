using System;
using System.IO;
using System.IO.Compression;
using System.IO.Pipes;
using System.Threading;
using System.Windows;
using James.HelperClasses;
using James.Search;
using James.Shortcut;
using James.Workflows;

namespace James
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static Mutex _mutex;

        private static void SetStyleAccents()
        {
            var accentColor = "pack://application:,,,/MahApps.Metro;component/Styles/Accents/" +
                              Config.Instance.WindowAccentColor + ".xaml";
            ((App) Current).Resources.MergedDictionaries[5].Source = new Uri(accentColor);
            var baseColor = Config.Instance.IsBaseLight ? "BaseLight" : "BaseDark";
            baseColor = "pack://application:,,,/MahApps.Metro;component/Styles/Accents/" + baseColor + ".xaml";
            ((App) Current).Resources.MergedDictionaries[4].Source = new Uri(baseColor);
        }
        
        /// <summary>
        /// Entry point of the application, here we check if it's the first running instance of James
        /// </summary>
        /// <param name="e"></param>
        [STAThread]
        protected override void OnStartup(StartupEventArgs e)
        {
            bool createdNew;
            _mutex = new Mutex(true, "James", out createdNew);
            if (createdNew)
            {
                StartProgram();
                base.OnStartup(e);
            }
            else if (e.Args.Length != 0)
            {
                Config.Instance.FirstInstance = false;
                AlternativeRun(e);
            }
        }

        /// <summary>
        /// If an instance of James is already running, its opened to import a workflow or triggers the Api
        /// </summary>
        /// <param name="e"></param>
        private static void AlternativeRun(StartupEventArgs e)
        {
            using (NamedPipeClientStream namedPipeClient = new NamedPipeClientStream("james"))
            {
                namedPipeClient.Connect(100);
                using (StreamWriter writer = new StreamWriter(namedPipeClient))
                {                    
                    if (e.Args.Length == 1 && File.Exists(e.Args[0]) && e.Args[0].EndsWith(".james"))
                    {
                        ImportWorkflow(e.Args[0], namedPipeClient, writer);
                    }
                    else if (namedPipeClient.IsConnected)
                    {
                        writer.WriteLine(string.Join(" ", e.Args).Substring(6));
                    }
                    writer.Flush();
                }
            }
            Environment.Exit(0);
        }

        /// <summary>
        /// Tries to import the workflow with the providen path
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="client"></param>
        /// <param name="writer"></param>
        private static void ImportWorkflow(string filePath, NamedPipeClientStream client, StreamWriter writer)
        {
            string workflowFolder = Config.WorkflowFolderLocation + "\\" + PathHelper.GetFilename(filePath).Replace(".james", "");
            if (!Directory.Exists(workflowFolder))
            {
                ZipFile.ExtractToDirectory(filePath, workflowFolder);
                if (client.IsConnected)
                {
                    writer.WriteLine("workflow/" + PathHelper.GetFilename(filePath).Replace(".james", ""));
                }
            }
            else
            {
                MessageBox.Show($"An Workflow with the name '{PathHelper.GetFilename(filePath).Replace(".james", "")}' is already importet!", "Workflow already imported!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static void StartProgram()
        {
            Config.Instance.WindowChangedAccentColor += App_WindowChangedAccentColor;
            SetStyleAccents();
            InitializeSingeltons();
            if (Config.Instance.FirstStart)
            {
                Config.Instance.FirstStart = false;
                new Windows.WelcomeWindow().Show();
            }
            else
            {
                James.Windows.MainWindow.GetInstance().Show();
            }
        }

        private static void InitializeSingeltons()
        {
            var searchEngine = SearchEngine.Instance;
            var workflowManager = WorkflowManager.Instance;
            var watcher = MyFileWatcher.Instance;
            var shortcutManager = ShortcutManager.Instance;
            var apiListener = ApiListener.Instance;
            var fileIconCache = FileIconCache.Instance;
        }

        private static void App_WindowChangedAccentColor(object sender, EventArgs e) => SetStyleAccents();
    }
}