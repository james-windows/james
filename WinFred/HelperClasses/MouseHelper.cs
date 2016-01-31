using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace James.HelperClasses
{
    public static class MouseHelper
    {
        [DllImport("user32.dll")]
        static extern void ClipCursor(ref System.Drawing.Rectangle rect);
        [DllImport("user32.dll")]
        static extern void ClipCursor(IntPtr rect);

        public static void TrapMouseInsideControl(FrameworkElement element)
        {
            Point a = element.PointToScreen(new Point(0, 0));
            System.Drawing.Rectangle r = new System.Drawing.Rectangle((int)a.X + 1, (int)a.Y + 1,
                (int)(a.X - 1 + element.ActualWidth), (int)(a.Y - 1 + element.ActualHeight));
            ClipCursor(ref r);
        }

        public static void ClearMouseTrap()
        {
            ClipCursor(IntPtr.Zero);
        }
    }
}
