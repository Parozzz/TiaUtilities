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
using TiaUtilities.Languages;
using System.Reflection;

namespace TiaXmlReader.Languages
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Enum)]
    public class LocaleAttribute(string translationStringName, string append = "") : Attribute
    {
        private readonly string translationStringName = translationStringName;
        private readonly string append = append;

        public string? GetTranslation()
        {
            var translation = Locale.ResourceManager.GetString(translationStringName, Thread.CurrentThread.CurrentUICulture);
            if(translation == null)
            {
                return null;
            }

            return $"{translation}{append}";
        }
    }

    public static class LocaleAttributeExpansion
    {
        public static string GetTranslation(this MemberInfo memberInfo)
        {
            var localizationAttribute = memberInfo.GetCustomAttribute<LocaleAttribute>();
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
