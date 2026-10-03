using System.Reflection;

namespace TiaUtilities.Utility
{
    public static class ReflectionUtils
    {
        private readonly static NullabilityInfoContext NULLABILITY_STATE_CONTEXT = new();

        public static bool IsNullable(PropertyInfo propertyInfo)
        {
            var nullabilityInfo = ReflectionUtils.NULLABILITY_STATE_CONTEXT.Create(propertyInfo);
            return nullabilityInfo.ReadState == NullabilityState.Nullable;
        }

        public static bool IsUnsignedNumber(Type type) => ReflectionUtils.IsUnsignedNumber(Type.GetTypeCode(type));

        public static bool IsUnsignedNumber(TypeCode typeCode) =>
            typeCode switch
            {
                TypeCode.Byte => true,
                TypeCode.UInt16 => true,
                TypeCode.UInt32 => true,
                TypeCode.UInt64 => true,
                _ => false
            };

        public static bool IsSignedNumber(Type type) => ReflectionUtils.IsSignedNumber(Type.GetTypeCode(type));

        public static bool IsSignedNumber(TypeCode typeCode) =>
            typeCode switch
            {
                TypeCode.SByte => true,
                TypeCode.Int16 => true,
                TypeCode.Int32 => true,
                TypeCode.Int64 => true,
                _ => false
            };

        public static bool IsNumber(Type type) => ReflectionUtils.IsNumber(Type.GetTypeCode(type));

        public static bool IsNumber(TypeCode typeCode) => ReflectionUtils.IsUnsignedNumber(typeCode) || ReflectionUtils.IsSignedNumber(typeCode);


        public static bool IsFloating(Type type) => ReflectionUtils.IsFloating(Type.GetTypeCode(type));

        public static bool IsFloating(TypeCode typeCode) =>
            typeCode switch
            {
                TypeCode.Single => true,
                TypeCode.Double => true,
                TypeCode.Decimal => true,
                _ => false
            };

        public static bool SetPropertyIfNumber(PropertyInfo propertyInfo, object instance, object value)
        {
            return ReflectionUtils.SetPropertyIfNumber(propertyInfo, Type.GetTypeCode(propertyInfo.PropertyType), instance, value);
        }

        public static bool SetPropertyIfNumber(PropertyInfo propertyInfo, TypeCode typeCode, object instance, object value)
        {
            switch (typeCode)
            {
                case TypeCode.SByte:
                    propertyInfo.SetValue(instance, Convert.ToSByte(value));
                    break;
                case TypeCode.Int16:
                    propertyInfo.SetValue(instance, Convert.ToInt16(value));
                    break;
                case TypeCode.Int32:
                    propertyInfo.SetValue(instance, Convert.ToInt32(value));
                    break;
                case TypeCode.Int64:
                    propertyInfo.SetValue(instance, Convert.ToInt64(value));
                    break;
                case TypeCode.Byte:
                    propertyInfo.SetValue(instance, Convert.ToByte(value));
                    break;
                case TypeCode.UInt16:
                    propertyInfo.SetValue(instance, Convert.ToUInt16(value));
                    break;
                case TypeCode.UInt32:
                    propertyInfo.SetValue(instance, Convert.ToUInt32(value));
                    break;
                case TypeCode.UInt64:
                    propertyInfo.SetValue(instance, Convert.ToUInt64(value));
                    break;
                case TypeCode.Single:
                    propertyInfo.SetValue(instance, Convert.ToSingle(value));
                    break;
                case TypeCode.Double:
                    propertyInfo.SetValue(instance, Convert.ToDouble(value));
                    break;
                case TypeCode.Decimal:
                    propertyInfo.SetValue(instance, Convert.ToDecimal(value));
                    break;
                default:
                    return false;
            }

            return true;
        }

    }
}
