using System;
using System.Windows.Controls;
using WinFred.Enumerations;

namespace WinFred.UserControls
{
    /// <summary>
    ///     Interaction logic for GeneralUserControl.xaml
    /// </summary>
    public partial class GeneralUserControl : UserControl
    {
        public GeneralUserControl()
        {
            InitializeComponent();
            DataContext = Config.GetInstance();
            AccentColorComboBox.ItemsSource = Enum.GetNames(typeof (AccentColorTypes));
        }
    }
}