using System;
using System.Threading;
using System.Windows;
using James.Search;
using Microsoft.Win32.TaskScheduler;
using Squirrel;

namespace James
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static bool _showTheWelcomeWizard;

        private static Mutex mutex;

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
            var createdNew = true;
            mutex = new Mutex(true, "James", out createdNew);
            if (createdNew)
            {
                StartProgram();
                base.OnStartup(e);
            }
            else
            {
                Environment.Exit(1);
            }
        }

        private void StartProgram()
        {
            Config.Instance.WindowChangedAccentColor += App_WindowChangedAccentColor;
            SetStyleAccents();
            SquirrelAwareApp.HandleEvents(onFirstRun: OnFirstRun, onAppUninstall: OnAppUninstall);
            SearchEngine.GetInstance();
            if (_showTheWelcomeWizard)
            {
                new WelcomeWindow().Show();
            }
            else
            {
                James.MainWindow.GetInstance().Show();
                //MyFileWatcher.Instance();
            }
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