using MahApps.Metro;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace WinFred
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void SetStyleAccents()
        {
            String accentColor = "pack://application:,,,/MahApps.Metro;component/Styles/Accents/"+ Config.GetInstance().WindowAccentColor +".xaml";
            (((App)Application.Current).Resources).MergedDictionaries[5].Source = new Uri(accentColor);
            String baseColor = Config.GetInstance().IsBaseLight ? "BaseLight" : "BaseDark";
            baseColor = "pack://application:,,,/MahApps.Metro;component/Styles/Accents/" + baseColor + ".xaml";
            (((App)Application.Current).Resources).MergedDictionaries[4].Source = new Uri(baseColor);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            Config.GetInstance().WindowChangedAccentColor += App_WindowChangedAccentColor;
            SetStyleAccents();
            base.OnStartup(e);
        }

        private void App_WindowChangedAccentColor(object sender, EventArgs e)
        {
            SetStyleAccents();
        }
    }
}