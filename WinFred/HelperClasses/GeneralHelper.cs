using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using James.Workflows;
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;

namespace James.HelperClasses
{
    public static class GeneralHelper
    {
        public static string Serialize<T>(this T value)
        {
            return JsonConvert.SerializeObject(value, Formatting.Indented, new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects
            });
        }

        public static string SerializeWorkflow(Workflow workflow)
        {
            var ms = new MemoryStream();
            var ser = new DataContractSerializer(typeof (Workflow));
            ms.Position = 0;
            ser.WriteObject(ms, workflow);
            ms.Position = 0;
            var output = new StreamReader(ms).ReadToEnd();
            return XDocument.Parse(output).ToString();
        }

        public static T Deserialize<T>(string path)
        {
            return JsonConvert.DeserializeObject<T>(File.ReadAllText(path), new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All,
                PreserveReferencesHandling = PreserveReferencesHandling.All
            });
        }

        public static Workflow DeserializeWorkflow(string path)
        {
            using (Stream stream = new MemoryStream())
            {
                var data = Encoding.UTF8.GetBytes(File.ReadAllText(path));
                stream.Write(data, 0, data.Length);
                stream.Position = 0;
                var deserializer = new DataContractSerializer(typeof (Workflow));
                return (Workflow) deserializer.ReadObject(stream);
            }
        }

        public static ImageSource ToImageSource(this Icon icon)
        {
            var tmp = DateTime.Now;
            ImageSource imageSource = Imaging.CreateBitmapSourceFromHIcon(
                icon.Handle,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
            Console.WriteLine((DateTime.Now - tmp).TotalMilliseconds);
            return imageSource;
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


        public const uint SHGFI_ICON = 0x100;
        public const uint SHGFI_LARGEICON = 0x0; // 'Large icon
        public const uint SHGFI_SMALLICON = 0x1; // 'Small icon

        [DllImport("shell32.dll")]
        public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);

        public static ImageSource GetIcon(string strPath)
        {
            SHFILEINFO shinfo = new SHFILEINFO();

            SHGetFileInfo(strPath, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), SHGFI_ICON | SHGFI_LARGEICON);

            if (shinfo.hIcon.ToInt32() != 0)
            {
                Icon myIcon = Icon.FromHandle(shinfo.hIcon);
                return ToImageSource(myIcon);
            }
            return null;
        }
    }
}