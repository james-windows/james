using System;
using System.Windows;
using System.Windows.Controls;
using Squirrel;

namespace WinFred.UserControls
{
    /// <summary>
    /// Interaction logic for AboutUserControl.xaml
    /// </summary>
    public partial class AboutUserControl : UserControl
    {
        public string Version { get; private set; } = "v0.1.1";
        public AboutUserControl()
        {
            InitializeComponent();
            #if !DEBUG 
                if(AppDomain.CurrentDomain.BaseDirectory.Split('-').Length == 2)
                    Version = "v" + AppDomain.CurrentDomain.BaseDirectory.Split('-')[1];
            #endif
            versionLabel.Content = Version;
        }

        private void DisplayChangelog(object sender, RoutedEventArgs e)
        {
            ChangelogWindow changelogWindow = new ChangelogWindow();
            changelogWindow.ShowDialog();
        }
    }
}
