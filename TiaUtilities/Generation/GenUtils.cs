using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Reflection;
using TiaXmlReader.Languages;
using TiaXmlReader.Utility;

namespace TiaXmlReader.Generation
{
    public static class GenUtils
    {
        public static readonly List<string?> DATA_INVALID_CHARS = ["\\", "/", "-", "_", ".", ","];
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
        private static void FixExtesion(ref string filePath, string extension)
        {
            var filePathExtension = Path.GetExtension(filePath);
            if (string.IsNullOrEmpty(filePathExtension))
            {
                filePath += "." + extension;
            }
        }
        /*
        public static bool Save(object obj, ref string? filePath, string extension, bool showFileDialog = false)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath) || showFileDialog)
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
                    if (string.IsNullOrEmpty(filePath))
                    {
                        return false;
                    }
                }

                FixExtesion(ref filePath, extension);

                var directoryName = Path.GetDirectoryName(filePath);
                if (string.IsNullOrEmpty(directoryName))
                {
                    return false;
                }

                Directory.CreateDirectory(directoryName);
                File.Create(filePath).Close(); //This is just to throw an exception in case the path is wrong!

                var serializer = new JsonSerializer();
                serializer.Converters.Add(new JavaScriptDateTimeConverter());
                serializer.NullValueHandling = NullValueHandling.Ignore;
                serializer.DefaultValueHandling = DefaultValueHandling.Include;
                serializer.Formatting = Formatting.Indented;

                using var sw = new StreamWriter(filePath);
                using var writer = new JsonTextWriter(sw);
                serializer.Serialize(writer, obj);
                writer.Flush();

                return true;
            }
            catch (Exception ex)
            {
                Utils.ShowExceptionMessage(ex);
            }

            return false;
        }
        */
        private static bool LoadSetup(out JsonSerializer? serializer, ref string? filePath, string extension, bool showFileDialog = true)
        {
            serializer = null;

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

            serializer = new JsonSerializer();
            serializer.Converters.Add(new JavaScriptDateTimeConverter());
            serializer.NullValueHandling = NullValueHandling.Ignore;
            serializer.DefaultValueHandling = DefaultValueHandling.Include;
            serializer.Formatting = Formatting.Indented;

            return true;
        }

        public static C? Deserialize<C>(ref string? filePath, string extension, bool showFileDialog = true)
        {
            try
            {
                var loadOK = LoadSetup(out JsonSerializer? serializer, ref filePath, extension, showFileDialog);
                if (!loadOK || serializer == null || string.IsNullOrEmpty(filePath))
                {
                    return default;
                }

                using var sr = new StreamReader(filePath);
                using var reader = new JsonTextReader(sr);
                var deserialized = serializer.Deserialize<C>(reader);

                return deserialized;
            }
            catch (Exception ex)
            {
                Utils.ShowExceptionMessage(ex);
            }

            return default;
        }

        public static bool Populate(object obj, ref string? filePath, string extension, bool showFileDialog = true)
        {
            try
            {
                var loadOK = LoadSetup(out JsonSerializer? serializer, ref filePath, extension, showFileDialog);
                if (!loadOK || serializer == null || string.IsNullOrEmpty(filePath) || obj == null)
                {
                    return false;
                }

                using var sr = new StreamReader(filePath);
                using var reader = new JsonTextReader(sr);
                serializer.Populate(reader, obj);

                return true;
            }
            catch (Exception ex)
            {
                Utils.ShowExceptionMessage(ex);
            }

            return false;
        }

        public static void CopyJsonFieldsAndProperties<T>(T copyFrom, T saveTo)
        {
            var type = typeof(T);

            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
            foreach (var field in fields)
            {
                var attribute = field.GetCustomAttribute<JsonPropertyAttribute>();
                if (attribute == null)
                {
                    continue;
                }

                var obj = field.GetValue(copyFrom);
                field.SetValue(saveTo, obj);
            }

            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(p => p.CanRead && p.CanWrite);
            foreach (var property in properties)
            {
                var attribute = property.GetCustomAttribute<JsonPropertyAttribute>();
                if (attribute == null)
                {
                    continue;
                }

                var obj = property.GetValue(copyFrom);
                property.SetValue(saveTo, obj);
            }
        }

        public static void CopySamePublicFieldsAndProperties(object copyFrom, object saveTo)
        {
            if(copyFrom.GetType() != saveTo.GetType())
            {
                return;
            }

            var type = copyFrom.GetType();

            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
            foreach (var field in fields)
            {
                var obj = field.GetValue(copyFrom);
                field.SetValue(saveTo, obj);
            }

            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(p => p.CanRead && p.CanWrite);
            foreach (var property in properties)
            {
                var obj = property.GetValue(copyFrom);
                property.SetValue(saveTo, obj);
            }
        }

        /***
         * RETURN TRUE IF ALL EQUALS
         * */
        public static bool CompareJsonFieldsAndProperties(object? first, object? second, out object? firstInvalidObj)
        {
            firstInvalidObj = null;

            if (first == null || second == null || first.GetType() != second.GetType())
            {
                return false;
            }

            var type = first.GetType();

            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
            foreach (var field in fields)
            {
                var attribute = field.GetCustomAttribute<JsonPropertyAttribute>();
                if (attribute == null)
                {
                    continue;
                }

                var firstFieldValue = field.GetValue(first);
                var secondFieldValue = field.GetValue(second);
                if (Utils.AreDifferentObject(firstFieldValue, secondFieldValue))
                {
                    firstInvalidObj = firstFieldValue;
                    return false;
                }
            }

            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(p => p.CanRead && p.CanWrite);
            foreach (var property in properties)
            {
                var attribute = property.GetCustomAttribute<JsonPropertyAttribute>();
                if (attribute == null)
                {
                    continue;
                }

                var firstAttributeValue = property.GetValue(first);
                var secondAttributeValue = property.GetValue(second);
                if (Utils.AreDifferentObject(firstAttributeValue, secondAttributeValue))
                {
                    firstInvalidObj = firstAttributeValue;
                    return false;
                }
            }

            return true;
        }

        public static void FillComboBoxWithEnumTranslation(ComboBox comboBox, Type enumType)
        {
            comboBox.DisplayMember = "Text";
            comboBox.ValueMember = "Value";

            var memoryTypeItems = new List<object>();
            foreach (Enum enumValue in Enum.GetValues(enumType))
            {
                memoryTypeItems.Add(new { Text = enumValue.GetTranslation(), Value = enumValue });
            }
            comboBox.DataSource = memoryTypeItems;
        }
    }
}
