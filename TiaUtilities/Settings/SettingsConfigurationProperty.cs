using System.ComponentModel;
using System.Configuration;
using System.Reflection;
using TiaUtilities.Configuration;
using TiaUtilities.Utility;

namespace TiaUtilities.SettingsStep
{
    public class SettingsConfigurationProperty
    {

        public Type PropertyType { get => targetProperty.PropertyType; }
        public bool IsThisPropertyChanged(PropertyChangedEventArgs args) => args.PropertyName == targetProperty.Name;

        private readonly IReadOnlyList<PropertyInfo> propertiesChain;
        private readonly PropertyInfo targetProperty;

        public SettingsConfigurationProperty(IReadOnlyList<PropertyInfo> propertiesChain)
        {
            Validate.NotNull(propertiesChain);
            Validate.IsTrue(propertiesChain.Count > 0, "Properties chain cannot be empty");

            this.propertiesChain = propertiesChain;
            this.targetProperty = propertiesChain[^1];
        }

        public object? GetFrom(ObservableConfiguration configuration)
        {
            if (!this.IsAssignableToOwner(configuration))
            {
                return null;
            }

            object? currentObj = configuration;

            // Navigate whole tree to get the last value
            foreach (var prop in this.propertiesChain)
            {
                if (currentObj == null)
                {
                    return null;
                }

                currentObj = prop.GetValue(currentObj);
            }

            return currentObj;
        }

        public void SetTo(ObservableConfiguration configuration, object setValue)
        {
            if (!this.IsAssignableToOwner(configuration))
            {
                return;
            }

            object? targetOwner = configuration;
            for (int i = 0; i < this.propertiesChain.Count - 1; i++)
            {
                if (targetOwner == null)
                {
                    return;
                }

                targetOwner = this.propertiesChain[i].GetValue(targetOwner);
            }

            if (targetOwner == null)
            {
                return;
            }

            try
            {
                var propertyType = this.PropertyType;
                var setValueType = setValue.GetType();

                if (propertyType == setValue.GetType())
                {
                    this.targetProperty.SetValue(targetOwner, setValue);
                }
                else
                {
                    var result = ReflectionUtils.SetPropertyIfNumber(this.targetProperty, targetOwner, setValue);
                }

            }
            catch (Exception ex)
            {
                Utils.ShowExceptionMessage(ex);
            }
        }

        private bool IsAssignableToOwner(object owner)
        {
            if (owner == null)
            {
                return false;
            }

            // Verifica che l'oggetto radice sia compatibile con la prima proprietà della catena
            var rootPropertyOwnerType = this.propertiesChain[0].DeclaringType;
            var ownerType = owner.GetType();

            return rootPropertyOwnerType != null && rootPropertyOwnerType.IsAssignableFrom(ownerType);
        }

        public bool TryParseSigned(string text, out long value)
        {
            value = 0;

            var propertyType = this.PropertyType;
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
                else if (propertyType == typeof(int) && int.TryParse(text, out int intValue))
                {
                    value = intValue;
                    return true;
                }
                else if (propertyType == typeof(long) && long.TryParse(text, out long longValue))
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

            var propertyType = this.PropertyType;
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

            var propertyType = this.PropertyType;
            if (ReflectionUtils.IsFloating(propertyType))
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
            return $"Property:{this.targetProperty.Name}, Type: {this.PropertyType.Name}.";
        }
    }
}
