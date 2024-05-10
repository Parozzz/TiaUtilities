using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TiaXmlReader.Localization
{
    public static class LocalizationHelper
    {
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
            var memberInfo = enumValue.GetType().GetMember(enumValue.ToString()).FirstOrDefault();
            if(memberInfo == null)
            {
                return "unknown";
            }

            return memberInfo.GetTranslation();
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
