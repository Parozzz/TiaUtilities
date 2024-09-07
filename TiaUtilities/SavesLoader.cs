using ClosedXML.Excel.CalcEngine;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System.Linq;
using TiaXmlReader;
using TiaXmlReader.Utility;
using TiaUtilities.Generation.IO.Module;
using TiaUtilities.Generation.Alarms.Module;

namespace TiaUtilities
{
    public static class SavesLoader
    {
        private class SaveFile
        {
            [JsonProperty] public string? Type { get; set; }
            [JsonProperty] public object? @Object { get; set; }
        }

        private static readonly Dictionary<Type, string> TYPE_TO_ID_DICT = [];
        private static readonly Dictionary<string, Type> ID_TO_TYPE_DICT = [];
        private static void RegisterType(Type type, string ID)
        {
            if (ID_TO_TYPE_DICT.ContainsKey(ID) || TYPE_TO_ID_DICT.ContainsKey(type))
            {
                throw new InvalidOperationException($"Trying to register the same type {ID}_{type.Name} twice for SaveLoader.");
            }

            TYPE_TO_ID_DICT.Add(type, ID);
            ID_TO_TYPE_DICT.Add(ID, type);
        }

        static SavesLoader()
        {
            SavesLoader.RegisterType(typeof(ProgramSettings), "ProgramSettings");
            SavesLoader.RegisterType(typeof(AlarmGenSave), "AlarmGeneration");
            SavesLoader.RegisterType(typeof(IOGenSave), "IOGeneration");
        }

        public static CommonOpenFileDialog CreateFileDialog(bool ensureFileExists, string? filePath, string extension)
        {
            return new CommonOpenFileDialog
            {
                EnsurePathExists = true,
                EnsureFileExists = ensureFileExists,
                DefaultExtension = "." + extension,
                Filters = { new CommonFileDialogFilter(extension + " Files", "*." + extension) },
                InitialDirectory = !string.IsNullOrEmpty(filePath) && File.Exists(filePath) ? Path.GetDirectoryName(filePath) : null,// : Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };
        }

        private static JsonSerializer CreateJSONSerializer()
        {
            JsonSerializer serializer = new();
            serializer.Converters.Add(new JavaScriptDateTimeConverter());
            serializer.NullValueHandling = NullValueHandling.Ignore;
            serializer.DefaultValueHandling = DefaultValueHandling.Ignore;
            serializer.Formatting = Formatting.Indented;
            return serializer;
        }

        private static void FixExtesion(ref string filePath, string extension)
        {
            var filePathExtension = Path.GetExtension(filePath);
            if (string.IsNullOrEmpty(filePathExtension))
            {
                filePath += "." + extension;
            }
        }

        public static bool Save(object obj, string? filePath, string extension, bool showFileDialog = false)
        {
            var localFilePath = filePath;
            return Save(obj, ref localFilePath, extension, showFileDialog);
        }

        public static bool Save(object obj, ref string? filePath, string extension, bool showFileDialog = false)
        {
            try
            {
                var type = obj.GetType();
                if (!TYPE_TO_ID_DICT.TryGetValue(type, out string? typeID) || typeID == null)
                {
                    throw new InvalidOperationException($"Trying to save the type {type} that has not been registered inside SaveLoader.");
                }

                if (!File.Exists(filePath) || showFileDialog)
                {
                    if (!showFileDialog)
                    {
                        throw new ArgumentException("File path invalid while saving without opening file dialog. Path: " + filePath);
                    }

                    var fileDialog = CreateFileDialog(false, filePath, extension);
                    if (fileDialog.ShowDialog() != CommonFileDialogResult.Ok)
                    {
                        return false;
                    }

                    filePath = fileDialog.FileName;
                }

                if (string.IsNullOrEmpty(filePath))
                {
                    return false;
                }

                FixExtesion(ref filePath, extension);

                var directoryName = Path.GetDirectoryName(filePath);
                if (string.IsNullOrEmpty(directoryName))
                {
                    return false;
                }

                Directory.CreateDirectory(directoryName);
                File.Create(filePath).Close(); //This is just to throw an exception in case the path is wrong!

                SaveFile saveFile = new() { Type = typeID, Object = obj, };

                var jObject = JObject.FromObject(saveFile, SavesLoader.CreateJSONSerializer());
                File.WriteAllText(filePath, jObject.ToString());

                return true;
            }
            catch (Exception ex)
            {
                Utils.ShowExceptionMessage(ex);
            }

            return false;
        }

        private static bool LoadSetup(ref string? filePath, string extension, bool showFileDialog = true)
        {
            if (showFileDialog)
            {
                var fileDialog = CreateFileDialog(true, filePath, extension);
                if (fileDialog.ShowDialog() != CommonFileDialogResult.Ok)
                {
                    return false;
                }

                filePath = fileDialog.FileName;
            }

            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                return false;
            }

            FixExtesion(ref filePath, extension);

            return true;
        }

        public static object? LoadWithDialog(ref string? filePath, string extension)
        {
            try
            {
                var loadOK = LoadSetup(ref filePath, extension, true);
                if (!loadOK || string.IsNullOrEmpty(filePath))
                {
                    return default;
                }

                return Load(filePath);
            }
            catch (Exception ex)
            {
                Utils.ShowExceptionMessage(ex);
            }

            return default;
        }

        public static object? LoadWithoutDialog(string filePath, string extension)
        {
            try
            {
                var localFilePath = filePath;

                var loadOK = LoadSetup(ref localFilePath, extension, showFileDialog: false);
                if (!loadOK || string.IsNullOrEmpty(filePath))
                {
                    return default;
                }

                return Load(filePath);
            }
            catch (Exception ex)
            {
                Utils.ShowExceptionMessage(ex);
            }

            return default;
        }

        private static object? Load(string filePath)
        {
            using StreamReader sr = new(filePath);
            using JsonTextReader jReader = new(sr);

            var saveFile = SavesLoader.CreateJSONSerializer().Deserialize<SaveFile>(jReader);
            if (saveFile == null || saveFile.Type == null || saveFile.Object == null)
            {
                return default;
            }

            if (!ID_TO_TYPE_DICT.TryGetValue(saveFile.Type, out Type? type) || type == null)
            {
                throw new InvalidOperationException("Trying to Load a Save that has not been registered");
            }

            return ((JObject)saveFile.Object).ToObject(type);
        }
    }
}
