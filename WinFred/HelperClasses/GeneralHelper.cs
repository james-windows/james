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
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using James.Workflows;
using James.Workflows.Actions;
using James.Workflows.Interfaces;
using James.Workflows.Outputs;
using James.Workflows.Triggers;

namespace James.HelperClasses
{
    public static class GeneralHelper
    {
        public static string Serialize<T>(this T value)
        {
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
        public static string SerializeWorkflow(Workflow workflow)
        {
            MemoryStream ms = new MemoryStream();
            DataContractSerializer ser = new DataContractSerializer(typeof(Workflow));
            ms.Position = 0;
            ser.WriteObject(ms, workflow);
            ms.Position = 0;
            string output =  new StreamReader(ms).ReadToEnd();
            return XDocument.Parse(output).ToString();
        }

        public static T Deserialize<T>(string path)
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

        public static Workflow DeserializeWorkflow(string path)
        {
            using (Stream stream = new MemoryStream())
            {
                byte[] data = Encoding.UTF8.GetBytes(File.ReadAllText(path));
                stream.Write(data, 0, data.Length);
                stream.Position = 0;
                DataContractSerializer deserializer = new DataContractSerializer(typeof(Workflow));
                return (Workflow)deserializer.ReadObject(stream);
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