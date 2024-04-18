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
using static TiaXmlReader.GenerationForms.IO.IOGenerationProjectSave;
using TiaXmlReader.Utility;

namespace TiaXmlReader.GenerationForms.IO
{
    public class IOGenerationProjectSave
    {
        public const string EXTENSION = "json";
        public static string DEFAULT_FILE_PATH = Directory.GetCurrentDirectory() + @"\tempIOSave." + EXTENSION;

        public class SaveData
        {
            [JsonProperty] public string Address { get; set; }
            [JsonProperty] public string IOName { get; set; }
            [JsonProperty] public string DBName { get; set; }
            [JsonProperty] public string Variable { get; set; }
            [JsonProperty] public string Comment { get; set; }
            [JsonProperty] public int RowIndex { get; set; }

            public void CopyFrom(IOData ioData)
            {
                this.Address = ioData.Address;
                this.IOName = ioData.IOName;
                this.DBName = ioData.DBName;
                this.Variable = ioData.Variable;
                this.Comment = ioData.Comment;
            }

            public void SaveTo(IOData ioData)
            {
                ioData.Address = this.Address;
                ioData.IOName = this.IOName;
                ioData.DBName = this.DBName;
                ioData.Variable = this.Variable;
                ioData.Comment = this.Comment;
            }
        }

        [JsonProperty] public List<SaveData> SaveDataList { get; set; }

        public IOGenerationProjectSave()
        {

        }

        public static bool Exists(string path)
        {
            return !string.IsNullOrEmpty(path) && File.Exists(path);
        }

        public void AddIOData(IOData data, int rowIndex)
        {
            if (SaveDataList == null)
            {
                SaveDataList = new List<SaveData>();
            }

            var saveData = new SaveData();
            saveData.CopyFrom(data);
            saveData.RowIndex = rowIndex;
            SaveDataList.Add(saveData);
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
