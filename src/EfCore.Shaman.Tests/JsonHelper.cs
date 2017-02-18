#region using

using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

#endregion

namespace EfCore.Shaman.Tests
{
    internal static class JsonHelper
    {
        #region Static Methods

        private static JsonSerializer CreateJsonSerializer()
        {
            var serializer = new JsonSerializer();
            serializer.Converters.Add(new StringEnumConverter());
            serializer.NullValueHandling = NullValueHandling.Ignore;
            serializer.DefaultValueHandling = DefaultValueHandling.Ignore;
            return serializer;
        }


        public static string SerializeToTest(object data)
        {
            return (Serialize(data) ?? string.Empty).Replace("\"", "'");
        }

        private static string Serialize<T>(T o)
        {
            if (o == null)
                return null;
            using(var stringWriter = new StringWriter())
            {
                using(var jsonWriter = new JsonTextWriter(stringWriter))
                {
                    jsonWriter.Formatting = Formatting.None;
                    var serializer = CreateJsonSerializer();
                    serializer.Serialize(jsonWriter, o);
                }

                var serialized = stringWriter.ToString();
                return serialized;
            }
        }

        #endregion
    }
}