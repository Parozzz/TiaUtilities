using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiaXmlReader.Generation.Alarms;
using TiaXmlReader.Generation.Alarms.GenerationForm;
using TiaXmlReader.Generation.GridHandler;

namespace TiaXmlReader.Generation.Alarms.GenerationForm
{
    public class AlarmGenerationSettings
    {
        public const string FILE_NAME = "AlarmGenerationPreferences.json";

        [JsonProperty] public AlarmConfiguration Configuration {  get; set; } = new AlarmConfiguration();
        [JsonProperty] public GridSettings GridSettings { get; set; } = new GridSettings();

        public AlarmGenerationSettings() { }

        private static string GetFilePath()
        {
            return Directory.GetCurrentDirectory() + @"\" + FILE_NAME;
        }

        public static bool Exists()
        {
            return File.Exists(GetFilePath());
        }

        public static AlarmGenerationSettings Load()
        {
            if (!Exists())
            {
                return new AlarmGenerationSettings();
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
                    return serializer.Deserialize<AlarmGenerationSettings>(reader);
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
