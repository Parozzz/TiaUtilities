using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiaXmlReader.Utility;
using TiaXmlReader.Generation.Alarms.GenerationForm;
using System.Reflection;
using Newtonsoft.Json.Serialization;

namespace TiaXmlReader.Generation
{
    public static class GenerationUtils
    {
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

        public static C? Load<C>(ref string? filePath, string extension, bool showFileDialog = true)
        {
            try
            {
                if (showFileDialog)
                {
                    var fileDialog = CreateFileDialog(true, filePath, extension);
                    if (fileDialog.ShowDialog() != CommonFileDialogResult.Ok)
                    {
                        return default;
                    }

                    filePath = fileDialog.FileName;
                }

                if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                {
                    return default;
                }

                FixExtesion(ref filePath, extension);

                var serializer = new JsonSerializer();
                serializer.Converters.Add(new JavaScriptDateTimeConverter());
                serializer.NullValueHandling = NullValueHandling.Ignore;
                serializer.DefaultValueHandling = DefaultValueHandling.Include;
                serializer.Formatting = Formatting.Indented;

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

        public static void CopyJsonFieldsAndProperties<T>(T copyFrom, T saveTo)
        {
            var type = typeof(T);

            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
            foreach (var field in fields)
            {
                var attribute = field.GetCustomAttribute<JsonPropertyAttribute>();
                if(attribute == null)
                {
                    continue;
                }

                var obj = field.GetValue(copyFrom);
                field.SetValue(saveTo, obj);
            }

            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(p => p.CanRead && p.CanWrite);
            foreach(var property in properties)
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
    }
}
