using System;
using System.Linq;
using System.Windows.Media.Imaging;
using Alphaleonis.Win32.Filesystem;

namespace James.HelperClasses
{
    public static class IconHelper
    {
        /// <summary>
        /// Returns the icon from the james \Resources folder
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static BitmapImage GetIcon(string path)
        {
            try
            {
                BitmapImage tmp = new BitmapImage(new Uri(PathHelper.GetLocationOfJames() + "\\Resources\\" + path));
                tmp.Freeze();
                return tmp;
            }
            catch (Exception)
            {
                return new BitmapImage();
            }
        }
    }
}
