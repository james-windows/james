using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Linq;
using James.Workflows;
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;

namespace James.HelperClasses
{
    public static class SerializationHelper
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
    }
}