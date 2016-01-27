using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using James.HelperClasses;
using MahApps.Metro.Controls;

namespace James.Windows
{
    /// <summary>
    ///     Interaction logic for OptionWindow.xaml
    /// </summary>
    public partial class OptionWindow : MetroWindow
    {
        public OptionWindow()
        {
            InitializeComponent();
            Thread.CurrentThread.Priority = ThreadPriority.Highest;
        }

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            AboutFlyout.IsOpen = !AboutFlyout.IsOpen;
        }

        private void TabChanged(object sender, SelectionChangedEventArgs e) => Config.Instance.Persist();

        private void MetroWindow_Closing(object sender, CancelEventArgs e)
        {
            Config.Instance.Persist();
            ShortcutManager.Instance.Reload();
        }
    }
}