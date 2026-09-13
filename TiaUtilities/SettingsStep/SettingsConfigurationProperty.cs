using System.ComponentModel;
using System.Configuration;
using System.Reflection;
using TiaUtilities.Configuration;
using TiaUtilities.Utility;

namespace TiaUtilities.SettingsStep
{
    public class SettingsConfigurationProperty(PropertyInfo property)
    {

        public Type PropertyType { get => property.PropertyType; }
        public bool IsPropertyChanged(PropertyChangedEventArgs args) => args.PropertyName == property.Name;

        public object? GetFrom(ObservableConfiguration configuration)
        {
            return this.VerifyOwner(configuration) ? property.GetValue(configuration) : null;
        }

        public void SetTo(ObservableConfiguration configuration, object setValue)
        {
            if (!this.VerifyOwner(configuration))
            {
                return;
            }

            try
            {
                var propertyType = property.PropertyType;
                if (propertyType == setValue.GetType())
                {
                    property.SetValue(configuration, setValue);
                }
                else if (ReflectionUtils.IsSignedNumber(propertyType) && setValue is long signedSetValue) //When parsed, always use maximun size!
                {
                    ReflectionUtils.SetPropertyCastedAsSigned(property, configuration, signedSetValue);
                }
                else if (ReflectionUtils.IsUnsignedNumber(propertyType) && setValue is ulong unsignedSetValue) //When parsed, always use maximun size!
                {
                    ReflectionUtils.SetPropertyCastedAsUnsigned(property, configuration, unsignedSetValue);
                }
                else if (ReflectionUtils.IsFloatingNumber(propertyType) && setValue is double float64Value)
                {
                    ReflectionUtils.SetPropertyCastedAsFloating(property, configuration, float64Value);
                }
            }
            catch (Exception ex)
            {
                Utils.ShowExceptionMessage(ex);
            }
        }

        private bool VerifyOwner(object owner)
        {
            var propertyOwnerType = property.DeclaringType;
            var ownerType = owner.GetType();
            return propertyOwnerType != null && ownerType != null && propertyOwnerType.IsAssignableFrom(ownerType);
        }

        public bool TryParseSigned(string text, out long value)
        {
            value = 0;

            var propertyType = property.PropertyType;
            if (ReflectionUtils.IsSignedNumber(propertyType)) //When parsed, always use maximun size!
            {
                if (propertyType == typeof(sbyte) && sbyte.TryParse(text, out sbyte sbyteValue))
                {
                    value = sbyteValue;
                    return true;
                }
                else if (propertyType == typeof(short) && short.TryParse(text, out short shortVaue))
                {
                    value = shortVaue;
                    return true;
                }
                else if(propertyType == typeof(int) && int.TryParse(text, out int intValue))
                {
                    value = intValue;
                    return true;
                }
                else if( propertyType == typeof(long) && long.TryParse(text, out long longValue))
                {
                    value = longValue;
                    return true;
                }
            }

            return false;
        }

        public bool TryParseUnsigned(string text, out ulong value)
        {
            value = 0;

            var propertyType = property.PropertyType;
            if (ReflectionUtils.IsUnsignedNumber(propertyType))
            {
                if (propertyType == typeof(byte) && byte.TryParse(text, out byte byteValue))
                {
                    value = byteValue;
                    return true;
                }
                else if (propertyType == typeof(ushort) && ushort.TryParse(text, out ushort ushortValue))
                {
                    value = ushortValue;
                    return true;
                }
                else if (propertyType == typeof(uint) && uint.TryParse(text, out uint uintValue))
                {
                    value = uintValue;
                    return true;
                }
                else if (propertyType == typeof(ulong) && ulong.TryParse(text, out ulong ulongValue))
                {
                    value = ulongValue;
                    return true;
                }
            }

            return false;
        }

        public bool TryParseFloating(string text, out double value)
        {
            value = 0.0;

            var propertyType = property.PropertyType;
            if (ReflectionUtils.IsFloatingNumber(propertyType))
            {
                if (propertyType == typeof(float) && float.TryParse(text, out float floatValue))
                {
                    value = floatValue;
                    return true;
                }
                else if (propertyType == typeof(double) && double.TryParse(text, out double doubleValue))
                {
                    value = doubleValue;
                    return true;
                }
            }

            return false;
        }

        public override string ToString()
        {
            return $"Property:{property.Name}, Type: {property.PropertyType.Name}.";
        }
    }
}
