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
        public static string DEFAULT_FILE_PATH = Directory.GetCurrentDirectory() + @"\tempIOSave." + EXTENSION;

        [JsonProperty] public Dictionary<int, IOData> RowDict { get; set; } = new Dictionary<int, IOData>();

        public IOGenerationProjectSave()
        {

        }

        public static IOGenerationProjectSave Load(ref string filePath)
        {
            return GenerationUtils.Load(ref filePath, EXTENSION, out IOGenerationProjectSave projectSave) ? projectSave : new IOGenerationProjectSave();
        }

        public bool Save(ref string filePath, bool showFileDialog = false)
        {
            return GenerationUtils.Save(this, ref filePath, EXTENSION, showFileDialog);
        }
    }
}
