using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Runtime.InteropServices;

namespace WinFred
{
    public static class HelperClass
    {
        public static string Serialize<T>(this T value)
        {
            if (value == null)
            {
                return string.Empty;
            }
            try
            {
                var xmlserializer = new XmlSerializer(typeof(T));
                var stringWriter = new StringWriter();
                using (var writer = XmlWriter.Create(stringWriter))
                {
                    xmlserializer.Serialize(writer, value);
                    XDocument doc = XDocument.Parse(stringWriter.ToString());
                    return doc.ToString();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred", ex);
            }
        }
        public static T Derialize<T>(string path)
        {
            try
            {
               XmlSerializer serializer = new XmlSerializer(typeof(T));
                T result = (T)serializer.Deserialize(new StreamReader(path));
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred", ex);
            }
        }
        public static ImageSource ToImageSource(this Icon icon)
        {
            ImageSource imageSource = Imaging.CreateBitmapSourceFromHIcon(
                icon.Handle,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            return imageSource;
        }
        public static String BuildHTML(string htmlFromWorkflow)
        {
            StringBuilder html = new StringBuilder(@"<!DOCTYPE html>
                    <html>
                    <head>
                        <meta charset=""utf-8"" />
                        <link rel=""stylesheet"" 
                          type=""text/css"" 
                          href=""http://holdirbootstrap.de/dist/css/bootstrap.min.css"" />
                    </head>
                    <body style=""-webkit-user-select: none;"">");

            html.Append(htmlFromWorkflow);
            html.Append("</body></html>");
            return html.ToString();
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct SHFILEINFO
        {
            public IntPtr hIcon;
            public IntPtr iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        };

        class Win32
        {
            public const uint SHGFI_ICON = 0x100;
            public const uint SHGFI_LARGEICON = 0x0; // 'Large icon
            public const uint SHGFI_SMALLICON = 0x1; // 'Small icon

            [DllImport("shell32.dll")]
            public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);
        }

        public static ImageSource GetIcon(string strPath)
        {
            SHFILEINFO shinfo = new SHFILEINFO();

            Win32.SHGetFileInfo(strPath, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), Win32.SHGFI_ICON | Win32.SHGFI_LARGEICON);

            if (shinfo.hIcon.ToInt32() != 0)
            {
                Icon myIcon = Icon.FromHandle(shinfo.hIcon);
                return ToImageSource(myIcon);
            }
            return null;
        }
    }
}
