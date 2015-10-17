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

        private LargeType()
        {
            InitializeComponent();
            Width = SystemParameters.PrimaryScreenWidth;
            Height = SystemParameters.PrimaryScreenHeight;
            DataContext = Config.Instance;
            KeyUp += LargeType_KeyUp;
        }

        public string Message
        {
            get { return TextBlock.Text; }
            private set { TextBlock.Text = value; }
        }

        public static LargeType Instance
        {
            get
            {
                lock (SingeltonLock)
                {
                    return _largeType ?? (_largeType = new LargeType());
                }
            }
        }

        private void LargeType_KeyUp(object sender, KeyEventArgs e)
        {
            KeyDown += Window_KeyDown;
        }

        public void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e == null || e.KeyboardDevice.IsKeyDown(Key.Escape) ||
                (Keyboard.IsKeyDown(Key.L) && Keyboard.IsKeyDown(Key.LeftAlt)))
            {
                Message = "";
                Hide();
                KeyDown -= Window_KeyDown;
            }
        }

        public void DisplayMessage(string message)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke(new Action(() => DisplayMessage(message)));
                return;
            }
            Message = message;
            KeyDown -= Window_KeyDown;
            Hide();
            KeyDown -= Window_KeyDown;
            Show();
            Activate();
        }

        public void ChangeMessage(string message)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke(new Action(() => ChangeMessage(message)));
                return;
            }
            Message = message;
        }
    }
}