using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiaXmlReader.Utility;
using TiaXmlReader.Utility.Extensions;
using System.Windows.Forms;

namespace TiaXmlReader.Localization
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Enum)]
    public class LocalizationAttribute : Attribute
    {
        private static readonly Dictionary<string, Dictionary<string, string>> LANG_TEXT_DICTIONARY;
        static LocalizationAttribute()
        {
            LANG_TEXT_DICTIONARY = new Dictionary<string, Dictionary<string, string>>();

            var directory = Directory.GetCurrentDirectory() + "//Localization";
            if (!Directory.Exists(directory))
            {
                var ex = new Exception("Localization folder not found");
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

                    using (var stream = File.OpenText(filePath))
                    {
                        using (var reader = new JsonTextReader(stream))
                        {
                            var jsonObject = (JObject)JToken.ReadFrom(reader);
                            foreach (var entry in jsonObject)
                            {
                                langDictionary.Add(entry.Key, entry.Value.ToString());
                            }
                        }
                    }

                    LANG_TEXT_DICTIONARY.Compute(fileLang, langDictionary);
                }
                catch { }
            }
        }

        private readonly string jsonKey;
        private readonly string append;
        public LocalizationAttribute(string jsonKey, string append = "")
        {
            this.jsonKey = jsonKey;
            this.append = append;
        }

        public string GetTranslation() //Can be null!
        {
            var langDictionary = LANG_TEXT_DICTIONARY.GetOrDefault(LocalizationVariables.LANG);
            if (langDictionary == null)
            {
                langDictionary = LANG_TEXT_DICTIONARY.GetOrDefault(LocalizationVariables.DEFAULT_LANG);
                if (langDictionary == null)
                {
                    var ex = new Exception("Cannot find text list for " + LocalizationVariables.LANG + " or " + LocalizationVariables.DEFAULT_LANG);
                    Utils.ShowExceptionMessage(ex);
                    throw ex;
                }
            }

            return langDictionary.GetOrDefault(this.jsonKey) + append ?? "";
        }
    }
}
