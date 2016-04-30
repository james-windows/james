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
                BitmapImage tmp = new BitmapImage(new Uri(GetExecuteablePath() + "\\Resources\\" + path));
                tmp.Freeze();
                return tmp;
            }
            catch (Exception)
            {
                return new BitmapImage();
            }
        }

        private static string GetExecuteablePath()
        {
            var folders = Directory.GetDirectories(Config.ConfigFolderLocation).Where(s => PathHelper.GetFilename(s).StartsWith("app-")).Select(s => new  { path = s,version= new Version(PathHelper.GetFilename(s).Replace("app-", ""))});
            return folders.OrderByDescending(arg => arg.version).First().path;
        }
    }
}
