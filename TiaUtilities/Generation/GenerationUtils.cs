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

namespace TiaXmlReader.Generation
{
    public static class GenerationUtils
    {
        private static CommonOpenFileDialog CreateFileDialog(bool ensureFileExists, string filePath, string extension)
        {
            return new CommonOpenFileDialog
            {
                EnsurePathExists = true,
                EnsureFileExists = ensureFileExists,
                DefaultExtension = "." + extension,
                Filters = { new CommonFileDialogFilter(extension + " Files", "*." + extension) },
                InitialDirectory = File.Exists(filePath) ? Path.GetDirectoryName(filePath) : Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
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

        public static void Save(object obj, ref string filePath, string extension, bool showFileDialog = false)
        {
            try
            {
                if(!showFileDialog)
                {
                    if (string.IsNullOrEmpty(filePath))
                    {
                        throw new ArgumentException("File path invalid while saving without opening file dialog.");
                    }

                    FixExtesion(ref filePath, extension);
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                    File.Create(filePath).Close(); //This is just to throw an exception in case the path is wrong!
                }
                else
                {
                    if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                    {
                        var fileDialog = CreateFileDialog(false, filePath, extension);
                        if (fileDialog.ShowDialog() == CommonFileDialogResult.Ok)
                        {
                            filePath = fileDialog.FileName;
                        }
                    }

                    FixExtesion(ref filePath, extension);
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                }

                JsonSerializer serializer = new JsonSerializer();
                serializer.Converters.Add(new JavaScriptDateTimeConverter());
                serializer.NullValueHandling = NullValueHandling.Ignore;
                serializer.DefaultValueHandling = DefaultValueHandling.Include;
                serializer.Formatting = Formatting.Indented;

                using (var sw = new StreamWriter(filePath))
                {
                    using (var writer = new JsonTextWriter(sw))
                    {
                        serializer.Serialize(writer, obj);
                        writer.Flush();
                    }
                }
            }
            catch (Exception ex)
            {
                Utils.ShowExceptionMessage(ex);
            }
        }

        public static bool Load<C>(ref string filePath, string extension, out C loaded, bool showFileDialog = true)
        {
            loaded = default;

            try
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
                else if(!File.Exists(filePath))
                {
                    return false;
                }

                FixExtesion(ref filePath, extension);

                JsonSerializer serializer = new JsonSerializer();
                serializer.Converters.Add(new JavaScriptDateTimeConverter());
                serializer.NullValueHandling = NullValueHandling.Ignore;
                serializer.DefaultValueHandling = DefaultValueHandling.Include;
                serializer.Formatting = Formatting.Indented;

                using (var sr = new StreamReader(filePath))
                {
                    using (var reader = new JsonTextReader(sr))
                    {
                        loaded = serializer.Deserialize<C>(reader);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Utils.ShowExceptionMessage(ex);
            }

            return false;
        }

    }
}
