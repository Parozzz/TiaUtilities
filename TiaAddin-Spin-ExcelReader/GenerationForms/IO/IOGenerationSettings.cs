using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiaXmlReader.Generation.IO;
using TiaXmlReader.GenerationForms.GridHandler;
using TiaXmlReader.GenerationForms.IO.ExcelImporter;

namespace TiaXmlReader.GenerationForms.IO
{
    public class IOGenerationSettings
    {
        public const string FILE_NAME = "IOGenerationPreferences.json";

        [JsonProperty] public IOConfiguration IOConfiguration { get; set; } = new IOConfiguration();
        [JsonProperty] public IOGenerationExcelImportConfiguration ExcelImportConfiguration { get; set; } = new IOGenerationExcelImportConfiguration();
        [JsonProperty] public IOGenerationPreferences Preferences { get; set; } = new IOGenerationPreferences();
        [JsonProperty] public GridSettings GridSettings { get; set; } = new GridSettings();

        public IOGenerationSettings() { }


        private static string GetFilePath()
        {
            return Directory.GetCurrentDirectory() + @"\" + FILE_NAME;
        }

        public static bool Exists()
        {
            return File.Exists(GetFilePath());
        }

        public static IOGenerationSettings Load()
        {
            if (!Exists())
            {
                return new IOGenerationSettings();
            }

            JsonSerializer serializer = new JsonSerializer();
            serializer.Converters.Add(new JavaScriptDateTimeConverter());
            serializer.NullValueHandling = NullValueHandling.Ignore;
            serializer.DefaultValueHandling = DefaultValueHandling.Ignore;
            serializer.Formatting = Formatting.Indented;

            using (var sr = new StreamReader(GetFilePath()))
            {
                using (var reader = new JsonTextReader(sr))
                {
                    return serializer.Deserialize<IOGenerationSettings>(reader);
                }
            }
        }

        public void Save()
        {
            JsonSerializer serializer = new JsonSerializer();
            serializer.Converters.Add(new JavaScriptDateTimeConverter());
            serializer.NullValueHandling = NullValueHandling.Ignore;
            serializer.DefaultValueHandling = DefaultValueHandling.Ignore;
            serializer.Formatting = Formatting.Indented;

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
