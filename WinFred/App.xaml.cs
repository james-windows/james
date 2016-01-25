using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Windows;
using James.Search;
using James.Workflows;
using Microsoft.Win32.TaskScheduler;
using Squirrel;
using Task = System.Threading.Tasks.Task;

namespace James
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static bool _showTheWelcomeWizard;

        private static Mutex _mutex;

        private static void SetStyleAccents()
        {
            var accentColor = "pack://application:,,,/MahApps.Metro;component/Styles/Accents/" +
                              Config.Instance.WindowAccentColor + ".xaml";
            (((App) Current).Resources).MergedDictionaries[5].Source = new Uri(accentColor);
            var baseColor = Config.Instance.IsBaseLight ? "BaseLight" : "BaseDark";
            baseColor = "pack://application:,,,/MahApps.Metro;component/Styles/Accents/" + baseColor + ".xaml";
            (((App) Current).Resources).MergedDictionaries[4].Source = new Uri(baseColor);
        }

        [STAThread]
        protected override void OnStartup(StartupEventArgs e)
        {
            bool createdNew;
            _mutex = new Mutex(true, "James", out createdNew);
            if (createdNew && e.Args.Length == 0)
            {
                StartProgram();
                base.OnStartup(e);
                var instance = ApiListener.Instance;
            }
            else
            {
                using (NamedPipeClientStream client = new NamedPipeClientStream("james"))
                {
                    client.Connect(100);
                    if (client.IsConnected)
                    {
                        using (StreamWriter writer = new StreamWriter(client))
                        {
                            writer.WriteLine(string.Join(" ", e.Args).Substring(6));
                            writer.Flush();
                        }
                    }
                }
                Environment.Exit(1);
            }
        }

        private static void StartProgram()
        {
            Config.Instance.WindowChangedAccentColor += App_WindowChangedAccentColor;
            SetStyleAccents();
            SquirrelAwareApp.HandleEvents(onFirstRun: OnFirstRun, onAppUninstall: OnAppUninstall);
            if (_showTheWelcomeWizard)
            {
                new WelcomeWindow().Show();
            }
            else
            {
                James.MainWindow.GetInstance().Show();
                Task.Run(() => InitializeSingeltons());
            }
        }

        private static void InitializeSingeltons()
        {
            var instance = SearchEngine.Instance;
            var watcher = MyFileWatcher.Instance;
            var workflowManager = WorkflowManager.Instance;
        }

        private static void OnAppUninstall(Version version)
        {
            using (var ts = new TaskService())
            {
                ts.RootFolder.DeleteTask("James");
            }
            using (var updateManager = new UpdateManager(Config.Instance.ReleaseUrl))
            {
                updateManager.RemoveShortcutsForExecutable("James.exe", ShortcutLocation.Desktop);
                updateManager.RemoveShortcutsForExecutable("James.exe", ShortcutLocation.StartMenu);
            }
        }

        private static void AddAutoUpdateTask()
        {
            using (var ts = new TaskService())
            {
                var td = ts.NewTask();
                td.RegistrationInfo.Description = "Calls the updater for James once a day";
                td.Triggers.Add(new DailyTrigger {DaysInterval = 1});
                td.Actions.Add(new ExecAction(Config.Instance.ConfigFolderLocation + "\\Update.exe",
                    "--update " + Config.Instance.ReleaseUrl, null));
                ts.RootFolder.RegisterTaskDefinition(@"James", td);
            }
        }

        private static void OnFirstRun()
        {
            _showTheWelcomeWizard = true;
            using (var updateManager = new UpdateManager(Config.Instance.ReleaseUrl))
            {
                updateManager.CreateShortcutsForExecutable("James.exe", ShortcutLocation.Desktop, true);
                updateManager.CreateShortcutsForExecutable("James.exe", ShortcutLocation.StartMenu, true);
            }
            AddAutoUpdateTask();
        }

        private static void App_WindowChangedAccentColor(object sender, EventArgs e) => SetStyleAccents();
    }
}