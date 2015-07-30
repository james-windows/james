using System.Windows.Controls;

namespace WinFred.OptionControls
{
    /// <summary>
    /// Interaction logic for GeneralUserControl.xaml
    /// </summary>
    public partial class GeneralUserControl : UserControl
    {
        public GeneralUserControl()
        {
            InitializeComponent();
            this.DataContext = Config.GetInstance();
        }
    }
}
