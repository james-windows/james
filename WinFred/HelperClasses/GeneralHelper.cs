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

        [DllImport("shell32.dll")]
        private static extern IntPtr ExtractIcon(IntPtr hInst, string lpszExeFileName, int nIconIndex);

        public static ImageSource GetIcon(string strPath)
        {
            var hIcon = IntPtr.Zero;
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