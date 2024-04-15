using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SpinXmlReader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TiaXmlReader.Generation;
using TiaXmlReader.Generation.IO;
using static TiaXmlReader.GenerationForms.IO.IOGenerationSave;

namespace TiaXmlReader.GenerationForms.IO
{
    public class IOGenerationSave
    {
        public static string DEFAULT_FILE_PATH = Directory.GetCurrentDirectory() + @"\tempIOSave.json";

        public class SaveData
        {
            [JsonProperty] public string Address { get; set; }
            [JsonProperty] public string IOName { get; set; }
            [JsonProperty] public string DBName { get; set; }
            [JsonProperty] public string Variable { get; set; }
            [JsonProperty] public string Comment { get; set; }
            [JsonProperty] public int RowIndex { get; set; }
        }

        [JsonProperty] public List<SaveData> SaveDataList { get; set; }

        public string SaveDataPath { get; set; }

        public IOGenerationSave()
        {

        }

        public static bool Exists(string path)
        {
            return !string.IsNullOrEmpty(path) && File.Exists(path);
        }

        public void AddIOData(IOData data, int rowIndex)
        {
            var saveData = new SaveData()
            {
                Address = data.Address,
                IOName = data.IOName,
                DBName = data.DBName,
                Variable = data.Variable,
                Comment = data.Comment,
                RowIndex = rowIndex
            };

            if (SaveDataList == null)
            {
                SaveDataList = new List<SaveData>();
            }
            SaveDataList.Add(saveData);
        }

        public static IOGenerationSave Load()
        {
            try
            {
                var fileDialog = new CommonOpenFileDialog
                {
                    IsFolderPicker = false,
                    EnsurePathExists = true,
                    EnsureFileExists = true,
                    DefaultExtension = ".json"
                };
                fileDialog.Filters.Add(new CommonFileDialogFilter("Json Files", "*.json"));
                fileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                if (fileDialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    var filePath = fileDialog.FileName;

                    var extension = Path.GetExtension(filePath);
                    if(string.IsNullOrEmpty(extension))
                    {
                        filePath += ".json";
                    }

                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Converters.Add(new JavaScriptDateTimeConverter());
                    serializer.NullValueHandling = NullValueHandling.Ignore;

                    using (var sr = new StreamReader(filePath))
                    {
                        using (var reader = new JsonTextReader(sr))
                        {
                            return serializer.Deserialize<IOGenerationSave>(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Utils.ShowExceptionMessage(ex);
                Console.WriteLine("Exception: {0}", ex.ToString());
            }

            return null;
        }

        public void Save()
        {
            try
            {
                var filePath = SaveDataPath;
                if(string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                {
                    var fileDialog = new CommonOpenFileDialog
                    {
                        IsFolderPicker = false,
                        EnsurePathExists = true,
                        EnsureFileExists = false,
                        DefaultExtension = ".json"
                    };
                    fileDialog.Filters.Add(new CommonFileDialogFilter("Json Files", "*.json"));
                    fileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                    if (fileDialog.ShowDialog() == CommonFileDialogResult.Ok)
                    {
                        filePath = fileDialog.FileName;

                        var extension = Path.GetExtension(filePath);
                        if (string.IsNullOrEmpty(extension))
                        {
                            filePath += ".json";
                        }

                        SaveDataPath = filePath;
                    }
                }

                JsonSerializer serializer = new JsonSerializer();
                serializer.Converters.Add(new JavaScriptDateTimeConverter());
                serializer.NullValueHandling = NullValueHandling.Ignore;

                using (StreamWriter sw = new StreamWriter(filePath))
                {
                    using (JsonWriter writer = new JsonTextWriter(sw))
                    {
                        serializer.Serialize(writer, this);
                        writer.Flush();
                    }
                }
            }
            catch (Exception ex)
            {
                Utils.ShowExceptionMessage(ex);
                Console.WriteLine("Exception: {0}", ex.ToString());
            }


        }
    }
}
