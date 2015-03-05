using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WinFred
{
    /// <summary>
    /// Interaction logic for LargeType.xaml
    /// </summary>
    public partial class LargeType : Window
    {
        public LargeType(string message)
        {
            InitializeComponent();
            WindowState = WindowState.Maximized;
            Width = System.Windows.SystemParameters.PrimaryScreenWidth;
            Height = System.Windows.SystemParameters.PrimaryScreenHeight;
            this.TextBlock.Text = message;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyboardDevice.IsKeyDown(Key.Escape) || (e.KeyboardDevice.IsKeyDown(Key.L) && e.KeyboardDevice.IsKeyDown(Key.LeftAlt)))
            {
                this.Close();
            }
        }
    }
}
