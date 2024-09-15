using System.Reflection;

namespace SimaticML.Enums
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Enum)]
    public class SimaticEnumAttribute(string simaticMLString, params string[] alias) : Attribute
    {
        public string SimaticMLString { get; init; } = simaticMLString;
        public string[] Alias { get; init; } = alias;
    }

    public static class SimaticEnumExtension
    {
        public static string GetSimaticMLString(this Enum enumValue)
        {
            var memberInfo = enumValue.GetType().GetMember(enumValue.ToString()).FirstOrDefault();
            if (memberInfo == null)
            {
                throw new Exception("SimaticML string not set for enum type= " + enumValue.GetType() + ", value= " + enumValue);
            }

            var simaticEnumAttribute = memberInfo.GetCustomAttribute<SimaticEnumAttribute>();
            if (simaticEnumAttribute == null)
            {
                throw new Exception("SimaticML string not set for enum type= " + enumValue.GetType() + ", value= " + enumValue);
            }

            return simaticEnumAttribute.SimaticMLString;
        }

        public static IEnumerable<string> GetSimaticMLAlias(this Enum enumValue)
        {
            var memberInfo = enumValue.GetType().GetMember(enumValue.ToString()).FirstOrDefault();
            if (memberInfo == null)
            {
                throw new Exception("SimaticML string not set for enum type= " + enumValue.GetType() + ", value= " + enumValue);
            }

            var simaticEnumAttribute = memberInfo.GetCustomAttribute<SimaticEnumAttribute>();
            if (simaticEnumAttribute == null)
            {
                throw new Exception("SimaticML string not set for enum type= " + enumValue.GetType() + ", value= " + enumValue);
            }

            return new HashSet<string>(simaticEnumAttribute.Alias)
            {
                simaticEnumAttribute.SimaticMLString,
            };
        }
    }
}