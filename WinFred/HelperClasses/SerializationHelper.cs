using System.IO;
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;

namespace James.HelperClasses
{
    public static class SerializationHelper
    {
        /// <summary>
        /// Serialize an given object to json and returns it as string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Serialize<T>(this T value)
        {
            return JsonConvert.SerializeObject(value, Formatting.Indented, new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects
            });
        }

        /// <summary>
        /// Opens an providen json file and converts it to the providen type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
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