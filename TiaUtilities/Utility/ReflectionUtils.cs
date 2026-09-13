using System.Reflection;

namespace TiaUtilities.Utility
{
    public static class ReflectionUtils
    {
        public static bool IsUnsignedNumber(Type type) => type == typeof(byte) || type == typeof(ushort) || type == typeof(uint) || type == typeof(ulong);

        public static bool IsSignedNumber(Type type) => type == typeof(sbyte) || type == typeof(short) || type == typeof(int) || type == typeof(long);


        public static bool IsFloatingNumber(Type type) => type == typeof(float) || type == typeof(double);

        public static bool SetPropertyCastedAsSigned(PropertyInfo propertyInfo, object instance, long value)
        {
            var propertyType = propertyInfo.PropertyType;
            if (propertyType == typeof(sbyte))
            {
                propertyInfo.SetValue(instance, (sbyte)value);
            }
            else if (propertyType == typeof(short))
            {
                propertyInfo.SetValue(instance, (short)value);
            }
            else if (propertyType == typeof(int))
            {
                propertyInfo.SetValue(instance, (int)value);
            }
            else if (propertyType == typeof(long))
            {
                propertyInfo.SetValue(instance, (long)value);
            }
            else
            {
                return false;
            }

            return true;
        }

        public static bool SetPropertyCastedAsUnsigned(PropertyInfo propertyInfo, object instance, ulong value)
        {
            var propertyType = propertyInfo.PropertyType;
            if (propertyType == typeof(byte))
            {
                propertyInfo.SetValue(instance, (byte)value);
            }
            else if (propertyType == typeof(ushort))
            {
                propertyInfo.SetValue(instance, (ushort)value);
            }
            else if (propertyType == typeof(uint))
            {
                propertyInfo.SetValue(instance, (uint)value);
            }
            else if (propertyType == typeof(ulong))
            {
                propertyInfo.SetValue(instance, (ulong)value);
            }
            else
            {
                return false;
            }

            return true;
        }

        public static bool SetPropertyCastedAsFloating(PropertyInfo propertyInfo, object instance, double value)
        {
            var propertyType = propertyInfo.PropertyType;
            if (propertyType == typeof(float))
            {
                propertyInfo.SetValue(instance, (float)value);
            }
            else if (propertyType == typeof(double))
            {
                propertyInfo.SetValue(instance, value);
            }
            else
            {
                return false;
            }

            return true;
        }


    }
}
