using DocumentFormat.OpenXml.Vml.Office;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TiaXmlReader.Utility;
using TiaXmlReader.Utility.Extensions;

namespace TiaXmlReader.Languages
{
    public static class Localization
    {
        private static readonly Dictionary<string, Dictionary<string, string>> LANG_TEXT_DICTIONARY = [];
        static Localization()
        {
            var directory = Directory.GetCurrentDirectory() + "//" + nameof(Languages);
            if (!Directory.Exists(directory))
            {
                var ex = new Exception(nameof(Languages) + " folder not found");
                Utils.ShowExceptionMessage(ex);
                throw ex;
            }

            foreach (var filePath in Directory.GetFiles(directory))
            {
                try
                {
                    var extension = Path.GetExtension(filePath);
                    if (extension != ".json")
                    {
                        continue;
                    }

                    var fileLang = Path.GetFileNameWithoutExtension(filePath);
                    var langDictionary = new Dictionary<string, string>();

                    using var stream = File.OpenText(filePath);
                    using var reader = new JsonTextReader(stream);

                    var jsonObject = (JObject)JToken.ReadFrom(reader);
                    jsonObject.Cast<KeyValuePair<string, JToken>>()
                        .Where(e => e.Value != null)
                        .ForEach(e => langDictionary.Add(e.Key, e.Value.ToString()));
                    LANG_TEXT_DICTIONARY.Compute(fileLang, langDictionary);
                }
                catch { }
            }
        }

        public static string Get(string jsonKey, string append = "")
        {
            var langDictionary = LANG_TEXT_DICTIONARY.GetOrDefault(LocalizationVariables.LANG);
            if (langDictionary == null)
            {
                langDictionary = LANG_TEXT_DICTIONARY.GetOrDefault(LocalizationVariables.DEFAULT_LANG);
                if (langDictionary == null)
                {
                    var ex = new Exception("Cannot find text with key " + jsonKey + " for " + LocalizationVariables.LANG + " or " + LocalizationVariables.DEFAULT_LANG);
                    Utils.ShowExceptionMessage(ex);
                    throw ex;
                }
            }

            return (langDictionary.GetOrDefault(jsonKey) ?? "") + append;
        }

        public static string GetTranslation(this MemberInfo memberInfo)
        {
            var localizationAttribute = memberInfo.GetCustomAttribute<LocalizationAttribute>();
            if (localizationAttribute == null)
            {
                return memberInfo.Name;
            }

            return localizationAttribute.GetTranslation() ?? memberInfo.Name;
        }

        public static string GetTranslation(this Enum enumValue)
        {
            return enumValue.GetType().GetMember(enumValue.ToString())
                .Select(m => m.GetTranslation())
                .FirstOrElse(() => "unkown");
        }

        public static bool TryGetEnumByTranslation<T>(string displayString, out T enumValue) where T : Enum
        {
            enumValue = default;
            foreach (T loopEnumValue in Enum.GetValues(typeof(T)))
            {
                var description = loopEnumValue.GetTranslation();
                if (displayString.ToLower() == description.ToLower())
                {
                    enumValue = loopEnumValue;
                    return true;
                }
            }

            return false;
        }
    }
}
