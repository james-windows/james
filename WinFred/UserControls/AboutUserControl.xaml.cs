using System;
using System.Windows.Controls;
using Squirrel;

namespace WinFred.UserControls
{
    /// <summary>
    /// Interaction logic for AboutUserControl.xaml
    /// </summary>
    public partial class AboutUserControl : UserControl
    {
        public string Version { get; private set; }
        public AboutUserControl()
        {
            InitializeComponent();
            #if DEBUG
                Version = "v0.1.1";
            #else
                UpdateManager updateManager = new UpdateManager(@"D:\WinFred\Project\winfred\Releases");
                Version version = updateManager.CurrentlyInstalledVersion();
                Version = string.Format("v{0}.{1}.{2}", version.Major, version.Revision, version.Minor); //Todo isn't working; has to be fixed in the future
            #endif
            versionLabel.Content = Version;
        }
    }
}
