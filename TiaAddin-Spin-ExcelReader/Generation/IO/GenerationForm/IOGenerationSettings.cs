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
using TiaXmlReader.AutoSave;

namespace TiaXmlReader.Generation.IO.GenerationForm
{
    public class IOGenerationSettings : ISettingsAutoSave
    {
        [JsonProperty] public IOConfiguration IOConfiguration { get; set; } = new IOConfiguration();
        [JsonProperty] public IOGenerationExcelImportSettings ExcelImportConfiguration { get; set; } = new IOGenerationExcelImportSettings();

        public IOGenerationSettings() { }
    }
}
