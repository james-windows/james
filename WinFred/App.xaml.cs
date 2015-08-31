using System;
using System.Windows;
using Microsoft.Win32.TaskScheduler;
using Squirrel;

namespace WinFred
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static bool _showTheWelcomeWizard;

        private static void SetStyleAccents()
        {
            var accentColor = "pack://application:,,,/MahApps.Metro;component/Styles/Accents/" +
                              Config.GetInstance().WindowAccentColor + ".xaml";
            (((App) Current).Resources).MergedDictionaries[5].Source = new Uri(accentColor);
            var baseColor = Config.GetInstance().IsBaseLight ? "BaseLight" : "BaseDark";
            baseColor = "pack://application:,,,/MahApps.Metro;component/Styles/Accents/" + baseColor + ".xaml";
            (((App) Current).Resources).MergedDictionaries[4].Source = new Uri(baseColor);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            Config.GetInstance().WindowChangedAccentColor += App_WindowChangedAccentColor;
            SetStyleAccents();
            base.OnStartup(e);
            SquirrelAwareApp.HandleEvents(onFirstRun: OnFirstRun, onInitialInstall: OnInitialInstall,
                onAppUninstall: OnAppUninstall);
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
            using (var ts = new TaskService())
            {
                ts.RootFolder.DeleteTask("James");
            }
        }

        private static void OnInitialInstall(Version version)
        {
            using (var ts = new TaskService())
            {
                var td = ts.NewTask();
                td.RegistrationInfo.Description = "Calls the updater for James once a day";
                td.Triggers.Add(new DailyTrigger {DaysInterval = 1});
                td.Actions.Add(new ExecAction("Update.exe", "--update " + Config.GetInstance().ReleaseUrl,
                    Config.GetInstance().ConfigFolderLocation));
                ts.RootFolder.RegisterTaskDefinition(@"James", td);
            }
        }

        private static void OnFirstRun() => _showTheWelcomeWizard = true;
        private static void App_WindowChangedAccentColor(object sender, EventArgs e) => SetStyleAccents();
    }
}