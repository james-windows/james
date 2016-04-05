using System;
using System.Windows;
using System.Windows.Input;
using James.HelperClasses;

namespace James.Windows
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
            Loaded +=(sender, args) => WindowHelper.HideWindowFromTaskList(this);
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

        private void LargeType_KeyUp(object sender, KeyEventArgs e) => KeyDown += Window_KeyDown;


        /// <summary>
        /// Closes LargeType on the specific key events
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Displays the providen message in the LargeType window
        /// </summary>
        /// <param name="message"></param>
        public void DisplayMessage(string message)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke(new Action(() => DisplayMessage(message)));
                return;
            }
            Message = message;
            TextBlock.MaxWidth = Math.Sqrt(message.Length*200);
            KeyDown -= Window_KeyDown;
            Hide();
            KeyDown -= Window_KeyDown;
            Show();
            Activate();
        }

        /// <summary>
        /// Changes the message of the LargeType without bringing it to the front
        /// </summary>
        /// <param name="message"></param>
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