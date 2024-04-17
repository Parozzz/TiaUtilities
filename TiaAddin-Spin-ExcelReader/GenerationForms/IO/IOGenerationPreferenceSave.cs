using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiaXmlReader.Generation.IO;
using TiaXmlReader.GenerationForms.IO.ExcelImporter;

namespace TiaXmlReader.GenerationForms.IO
{
    public class IOGenerationPreferenceSave
    {
        public const string FILE_NAME = "IOGenerationPreferences.json";

        [JsonProperty] public IOConfiguration Configuration { get; set; }
        [JsonProperty] public IOGenerationExcelImportConfiguration ExcelImportConfiguration { get; set; }


        public IOGenerationPreferenceSave() { }


        private static string GetFilePath()
        {
            return Directory.GetCurrentDirectory() + @"\" + FILE_NAME;
        }

        public static bool Exists()
        {
            return File.Exists(GetFilePath());
        }

        public static IOGenerationPreferenceSave Load()
        {
            if (!Exists())
            {
                return new IOGenerationPreferenceSave();
            }

            JsonSerializer serializer = new JsonSerializer();
            serializer.Converters.Add(new JavaScriptDateTimeConverter());
            serializer.NullValueHandling = NullValueHandling.Ignore;

            using (var sr = new StreamReader(GetFilePath()))
            {
                using (var reader = new JsonTextReader(sr))
                {
                    return serializer.Deserialize<IOGenerationPreferenceSave>(reader);
                }
            }
        }

        public void Save()
        {
            JsonSerializer serializer = new JsonSerializer();
            serializer.Converters.Add(new JavaScriptDateTimeConverter());
            serializer.NullValueHandling = NullValueHandling.Ignore;

            using (StreamWriter sw = new StreamWriter(GetFilePath()))
            {
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    serializer.Serialize(writer, this);
                    writer.Flush();
                }
            }
        }
    }
}
