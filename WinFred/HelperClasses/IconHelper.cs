using System;
using System.Windows.Media.Imaging;
using Alphaleonis.Win32.Filesystem;

namespace James.HelperClasses
{
    public static class IconHellper
    {
        public static BitmapImage GetIcon(string path)
        {
            BitmapImage tmp = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\Resources\\" + path));
            tmp.Freeze();
            return tmp;
        }
    }
}
