namespace SimaticML.Enums.Utility
{
    public static class SimaticEnumUtils
    {
        public static T? FindByString<T>(string simaticMLString) where T : Enum
        {
            var type = typeof(T);
            foreach (T loopEnumValue in Enum.GetValues(type))
            {
                var anyFound = loopEnumValue.GetSimaticMLAlias()
                    .Where(s => string.Equals(s, simaticMLString, StringComparison.OrdinalIgnoreCase))
                    .Any();
                if (anyFound)
                {
                    return loopEnumValue;
                }
            }

            return default;
        }

    }
}
