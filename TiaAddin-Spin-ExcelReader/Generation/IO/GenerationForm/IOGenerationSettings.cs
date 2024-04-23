using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiaXmlReader.Generation.IO;
using TiaXmlReader.Generation.GridHandler;
using TiaXmlReader.Generation.IO.GenerationForm;
using TiaXmlReader.Generation.IO.GenerationForm.ExcelImporter;

namespace TiaXmlReader.Generation.IO.GenerationForm
{
    public class IOGenerationSettings
    {
        public const string EXTENSION = "json";
        public const string FILE_NAME = "settings/IOGenerationPreferences." + EXTENSION;

        [JsonProperty] public IOConfiguration IOConfiguration { get; set; } = new IOConfiguration();
        [JsonProperty] public IOGenerationExcelImportConfiguration ExcelImportConfiguration { get; set; } = new IOGenerationExcelImportConfiguration();
        [JsonProperty] public GridSettings GridSettings { get; set; } = new GridSettings();

        public IOGenerationSettings() { }


        private static string GetFilePath()
        {
            return Directory.GetCurrentDirectory() + @"\" + FILE_NAME;
        }

        public static IOGenerationSettings Load()
        {
            var filePath = IOGenerationSettings.GetFilePath();
            return GenerationUtils.Load(ref filePath, EXTENSION, out IOGenerationSettings settings, showFileDialog: false) ? settings : new IOGenerationSettings();
        }

        public void Save()
        {
            var filePath = IOGenerationSettings.GetFilePath();
            GenerationUtils.Save(this, ref filePath, EXTENSION, showFileDialog: false);
        }
    }
}
