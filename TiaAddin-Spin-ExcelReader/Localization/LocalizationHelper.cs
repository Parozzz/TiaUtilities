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
        public static string GetDescription(this Enum enumValue)
        {
            return enumValue.GetType().GetMember(enumValue.ToString())
                .FirstOrDefault()?
                .GetCustomAttribute<DisplayAttribute>()
                .GetDescription() ?? "unknown";
        }

        public static bool TryGetEnumByDescription<T>(string displayString, out T enumValue) where T : Enum
        {
            enumValue = default(T);
            foreach (T loopEnumValue in Enum.GetValues(typeof(T)))
            {
                var description = loopEnumValue.GetDescription();
                if(displayString.ToLower() == description.ToLower())
                {
                    enumValue = loopEnumValue;
                    return true;
                }
            }

            return false;
        }
    }
}
