using MahApps.Metro.Controls;

namespace WinFred
{
    /// <summary>
    /// Interaction logic for OptionWindow.xaml
    /// </summary>
    public partial class OptionWindow : MetroWindow
    {
        public OptionWindow()
        {
            InitializeComponent();
            System.Threading.Thread.CurrentThread.Priority = System.Threading.ThreadPriority.Highest;
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Config.GetInstance().Persist();
        }

        private void AboutButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            AboutFlyout.IsOpen = !AboutFlyout.IsOpen;
        }
    }
}
