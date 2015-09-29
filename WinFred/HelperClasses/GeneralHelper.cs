using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace James.HelperClasses
{
    public static class GeneralHelper
    {
        public static string Serialize<T>(this T value)
        {
            if (value == null)
            {
                return string.Empty;
            }
            try
            {
                var xmlserializer = new XmlSerializer(typeof (T));
                var stringWriter = new StringWriter();
                using (var writer = XmlWriter.Create(stringWriter))
                {
                    xmlserializer.Serialize(writer, value);
                    var doc = XDocument.Parse(stringWriter.ToString());
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
                var serializer = new XmlSerializer(typeof (T));
                var result = (T) serializer.Deserialize(new StreamReader(path));
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred", ex);
            }
        }

        public static ImageSource ToImageSource(this Icon icon)
        {
            DateTime tmp = DateTime.Now;
            ImageSource imageSource = Imaging.CreateBitmapSourceFromHIcon(
                icon.Handle,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
            Console.WriteLine((DateTime.Now - tmp).TotalMilliseconds);
            return imageSource;
        }

        public static string BuildHtml(string htmlFromWorkflow)
        {
            var html = new StringBuilder(@"<!DOCTYPE html>
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

        [DllImport("shell32.dll")]
        static extern IntPtr ExtractIcon(IntPtr hInst, string lpszExeFileName, int nIconIndex);

        public static ImageSource GetIcon(string strPath)
        {
            
            IntPtr hIcon = IntPtr.Zero;
            hIcon = ExtractIcon(IntPtr.Zero, strPath, 0);
            if (hIcon.ToInt32() != 0)
            {
                var myIcon = Icon.FromHandle(hIcon);
                return ToImageSource(myIcon);
            }
            return null;
        }
    }
}