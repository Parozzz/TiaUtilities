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
using TiaXmlReader.Generation.IO.GenerationForm;

namespace TiaXmlReader.Generation.Alarms.GenerationForm
{
    public class AlarmGenerationSettings
    {
        public const string EXTENSION = "json";
        public const string FILE_NAME = "settings/AlarmGenerationPreferences.json";

        [JsonProperty] public AlarmConfiguration Configuration {  get; set; } = new AlarmConfiguration();
        [JsonProperty] public GridSettings GridSettings { get; set; } = new GridSettings();

        public AlarmGenerationSettings() { }

        private static string GetFilePath()
        {
            return Directory.GetCurrentDirectory() + @"\" + FILE_NAME;
        }

        public static AlarmGenerationSettings Load()
        {
            var filePath = AlarmGenerationSettings.GetFilePath();
            return GenerationUtils.Load(ref filePath, EXTENSION, out AlarmGenerationSettings settings, showFileDialog: false) ? settings : new AlarmGenerationSettings();
        }

        public void Save()
        {
            var filePath = AlarmGenerationSettings.GetFilePath();
            GenerationUtils.Save(this, ref filePath, EXTENSION, showFileDialog: false);
        }
    }
}
