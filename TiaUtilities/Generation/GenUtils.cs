using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;
using System.Reflection;
using TiaUtilities.Languages;
using TiaUtilities.Utility;

namespace TiaUtilities.Generation
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
