using System.Windows;
using System.Windows.Input;

namespace WinFred
{
    /// <summary>
    ///     Interaction logic for LargeType.xaml
    /// </summary>
    public partial class LargeType : Window
    {
        public LargeType(string message)
        {
            InitializeComponent();
            WindowState = WindowState.Maximized;
            Width = SystemParameters.PrimaryScreenWidth;
            Height = SystemParameters.PrimaryScreenHeight;
            TextBlock.Text = message;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyboardDevice.IsKeyDown(Key.Escape) ||
                (e.KeyboardDevice.IsKeyDown(Key.L) && e.KeyboardDevice.IsKeyDown(Key.LeftAlt)))
            {
                Close();
            }
        }
    }
}