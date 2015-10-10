using System;
using System.Windows;
using System.Windows.Input;

namespace James
{
    /// <summary>
    ///     Interaction logic for LargeType.xaml
    /// </summary>
    public partial class LargeType : Window
    {
        private static LargeType _largeType;
        private static readonly object SingeltonLock = new object();
        public static LargeType GetInstance()
        {
            lock (SingeltonLock)
            {
                return _largeType ?? (_largeType = new LargeType());
            }
        }

        public string Message
        {
            get { return TextBlock.Text; }
            set { TextBlock.Text = value; }
        }

        private LargeType()
        {
            InitializeComponent();
            WindowState = WindowState.Maximized;
            Width = SystemParameters.PrimaryScreenWidth;
            Height = SystemParameters.PrimaryScreenHeight;
            DataContext = Config.GetInstance();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyboardDevice.IsKeyDown(Key.Escape) /*||
                (e.KeyboardDevice.IsKeyDown(Key.L) && e.KeyboardDevice.IsKeyDown(Key.LeftAlt))*/)
            {
                Hide();
            }
        }
    }
}