using MahApps.Metro;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Squirrel;
using WinFred.Search;

namespace WinFred
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void SetStyleAccents()
        {
            string accentColor = "pack://application:,,,/MahApps.Metro;component/Styles/Accents/"+ Config.GetInstance().WindowAccentColor +".xaml";
            (((App)Application.Current).Resources).MergedDictionaries[5].Source = new Uri(accentColor);
            string baseColor = Config.GetInstance().IsBaseLight ? "BaseLight" : "BaseDark";
            baseColor = "pack://application:,,,/MahApps.Metro;component/Styles/Accents/" + baseColor + ".xaml";
            (((App)Application.Current).Resources).MergedDictionaries[4].Source = new Uri(baseColor);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            Config.GetInstance().WindowChangedAccentColor += App_WindowChangedAccentColor;
            MyFileWatcher.GetInstance();
            SetStyleAccents();
            new MainWindow().Show();
            base.OnStartup(e);
        }

        private static bool ShowTheWelcomeWizard;

        private void App_WindowChangedAccentColor(object sender, EventArgs e)
        {
            SetStyleAccents();
        }
    }
}