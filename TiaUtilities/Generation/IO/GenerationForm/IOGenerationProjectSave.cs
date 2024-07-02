using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TiaXmlReader.Generation;
using TiaXmlReader.Generation.IO;
using static TiaXmlReader.Generation.IO.GenerationForm.IOGenerationProjectSave;
using TiaXmlReader.Utility;
using TiaXmlReader.Generation.IO.GenerationForm;

namespace TiaXmlReader.Generation.IO.GenerationForm
{
    public class IOGenerationProjectSave
    {
        public const string EXTENSION = "json";

        [JsonProperty] public IOConfiguration IOConfiguration { get; set; } = new IOConfiguration();
        [JsonProperty] public Dictionary<int, IOData> RowDict { get; set; } = [];
        [JsonProperty] public Dictionary<int, IOSuggestionData> SuggestionRowDict = [];

        public IOGenerationProjectSave()
        {

        }

        public static IOGenerationProjectSave Load(ref string? filePath)
        {
            return GenerationUtils.Load<IOGenerationProjectSave>(ref filePath, EXTENSION) ?? new IOGenerationProjectSave();
        }

        public bool Save(ref string? filePath, bool showFileDialog = false)
        {
            return GenerationUtils.Save(this, ref filePath, EXTENSION, showFileDialog);
        }

        public override bool Equals(object? obj)
        {
            return obj is IOGenerationProjectSave compare && 
                this.IOConfiguration.Equals(compare.IOConfiguration) &&
                this.RowDict.SequenceEqual(compare.RowDict) &&
                this.SuggestionRowDict.SequenceEqual(compare.SuggestionRowDict);
        }
    }
}
