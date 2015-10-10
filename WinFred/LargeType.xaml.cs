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
            DataContext = Config.GetInstance();
            KeyUp += LargeType_KeyUp;
        }

        private void LargeType_KeyUp(object sender, KeyEventArgs e)
        {
            KeyDown += Window_KeyDown;
        }

        public string Message
        {
            get { return TextBlock.Text; }
            set { TextBlock.Text = value; }
        }

        public static LargeType GetInstance()
        {
            lock (SingeltonLock)
            {
                return _largeType ?? (_largeType = new LargeType());
            }
        }

        public void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyboardDevice.IsKeyDown(Key.Escape) || (Keyboard.IsKeyDown(Key.L) && Keyboard.IsKeyDown(Key.LeftAlt)))
            {
                Message = "";
                Hide();
                KeyDown -= Window_KeyDown;
            }
        }
    }
}