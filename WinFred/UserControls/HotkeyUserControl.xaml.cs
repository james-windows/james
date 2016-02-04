using System.Windows.Controls;

namespace James.UserControls
{
    /// <summary>
    /// Interaction logic for HotkeyUserControl.xaml
    /// </summary>
    public partial class HotkeyUserControl : UserControl
    {
        public HotkeyUserControl()
        {
            InitializeComponent();
            DataContext = Config.Instance.ShortcutManagerSettings;
        }
    }
}
