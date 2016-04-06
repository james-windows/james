using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using James.Shortcut;
using James.Workflows;
using MahApps.Metro.Controls;

namespace James.Windows
{
    /// <summary>
    ///     Interaction logic for OptionWindow.xaml
    /// </summary>
    public partial class OptionWindow : MetroWindow
    {
        //Todo make singelton
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

        /// <summary>
        /// Closes the window and persists all changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MetroWindow_Closing(object sender, CancelEventArgs e)
        {
            Config.Instance.Persist();
            ShortcutManager.Instance.Reload();
            WorkflowManager.Instance.PersistWorkflows();
        }
    }
}