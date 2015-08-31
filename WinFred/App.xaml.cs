using Microsoft.Win32.TaskScheduler;
using Squirrel;
using System;
using System.Windows;

namespace WinFred
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static void SetStyleAccents()
        {
            string accentColor = "pack://application:,,,/MahApps.Metro;component/Styles/Accents/"+ Config.GetInstance().WindowAccentColor +".xaml";
            (((App)Application.Current).Resources).MergedDictionaries[5].Source = new Uri(accentColor);
            string baseColor = Config.GetInstance().IsBaseLight ? "BaseLight" : "BaseDark";
            baseColor = "pack://application:,,,/MahApps.Metro;component/Styles/Accents/" + baseColor + ".xaml";
            (((App)Application.Current).Resources).MergedDictionaries[4].Source = new Uri(baseColor);
        }

        private static bool _showTheWelcomeWizard;
        protected override void OnStartup(StartupEventArgs e)
        {
            Config.GetInstance().WindowChangedAccentColor += App_WindowChangedAccentColor;            
            SetStyleAccents();
            base.OnStartup(e);
            SquirrelAwareApp.HandleEvents(onFirstRun: OnFirstRun, onInitialInstall: OnInitialInstall, onAppUninstall: OnAppUninstall);
            if (_showTheWelcomeWizard)
            {
                new WelcomeWindow().Show();
            }
            else
            {
                new MainWindow().Show();
                //MyFileWatcher.GetInstance();
            }
        }

        private static void OnAppUninstall(Version version)
        {
            using (TaskService ts = new TaskService())
            {
                ts.RootFolder.DeleteTask("James");
            }
        }

        private static void OnInitialInstall(Version version)
        {
            using (TaskService ts = new TaskService())
            {
                TaskDefinition td = ts.NewTask();
                td.RegistrationInfo.Description = "Calls the updater for James once a day";
                td.Triggers.Add(new DailyTrigger { DaysInterval = 1 });
                td.Actions.Add(new ExecAction("Update.exe", "--update http://moserm.tk/Releases", Config.GetInstance().ConfigFolderLocation));
                ts.RootFolder.RegisterTaskDefinition(@"James", td);
            }
        }

        private static void OnFirstRun() => _showTheWelcomeWizard = true;

        private static void App_WindowChangedAccentColor(object sender, EventArgs e) => SetStyleAccents();
    }
}