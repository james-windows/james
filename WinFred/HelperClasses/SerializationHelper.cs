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

        public static T Deserialize<T>(string path)
        {
            return JsonConvert.DeserializeObject<T>(File.ReadAllText(path), new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All,
                PreserveReferencesHandling = PreserveReferencesHandling.All
            });
        }
    }
}