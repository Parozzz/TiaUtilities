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

        public static bool Exists(string path)
        {
            return !string.IsNullOrEmpty(path) && File.Exists(path);
        }

        public static IOGenerationProjectSave Load(ref string filePath)
        {
            try
            {
                var fileDialog = CreateFileDialog(true, filePath);
                if (fileDialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    filePath = fileDialog.FileName;
                    FixExtesion(ref filePath);

                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Converters.Add(new JavaScriptDateTimeConverter());
                    serializer.NullValueHandling = NullValueHandling.Ignore;
                    using (var sr = new StreamReader(filePath))
                    {
                        using (var reader = new JsonTextReader(sr))
                        {
                            return serializer.Deserialize<IOGenerationProjectSave>(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Utils.ShowExceptionMessage(ex);
            }

            return null;
        }

        public void Save(ref string filePath, bool saveAs = false)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath) || saveAs)
                {
                    var fileDialog = CreateFileDialog(false, filePath);
                    if (fileDialog.ShowDialog() == CommonFileDialogResult.Ok)
                    {
                        filePath = fileDialog.FileName;
                        FixExtesion(ref filePath);
                    }
                }

                JsonSerializer serializer = new JsonSerializer();
                serializer.Converters.Add(new JavaScriptDateTimeConverter());
                serializer.NullValueHandling = NullValueHandling.Ignore;
                using (var sw = new StreamWriter(filePath))
                {
                    using (var writer = new JsonTextWriter(sw))
                    {
                        serializer.Serialize(writer, this);
                        writer.Flush();
                    }
                }
            }
            catch (Exception ex)
            {
                Utils.ShowExceptionMessage(ex);
            }
        }

        private static CommonOpenFileDialog CreateFileDialog(bool ensureFileExists, string filePath)
        {
            return new CommonOpenFileDialog
            {
                EnsurePathExists = true,
                EnsureFileExists = ensureFileExists,
                DefaultExtension = "." + EXTENSION,
                Filters = { new CommonFileDialogFilter(EXTENSION + " Files", "*." + EXTENSION) },
                InitialDirectory = File.Exists(filePath) ? Path.GetDirectoryName(filePath) : Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };
        }

        private static void FixExtesion(ref string filePath)
        {
            var extension = Path.GetExtension(filePath);
            if (string.IsNullOrEmpty(extension))
            {
                filePath += "." + EXTENSION;
            }
        }
    }
}
