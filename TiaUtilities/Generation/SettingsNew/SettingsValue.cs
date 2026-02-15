using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TiaUtilities.Configuration;

namespace TiaUtilities.Generation.SettingsNew
{
    public class SettingsValue
    {
        public SettingsValueBinding Binding { get; init; }
        public PropertyInfo PropertyInfo { get; init; }

        private readonly ObservableConfiguration configurationObject;

        public SettingsValue(ObservableConfiguration configurationObject, SettingsValueBinding binding, PropertyInfo propertyInfo)
        {
            this.configurationObject = configurationObject;

            this.Binding = binding;
            this.PropertyInfo = propertyInfo;
        }

        public void SetConfigurationValue(object setValue)
        {
            var propertyType = this.PropertyInfo.PropertyType;
            if (propertyType == setValue.GetType())
            {
                this.PropertyInfo.SetValue(this.configurationObject, setValue);
            }
            else if(SettingsValue.IsSignedInt(propertyType) && setValue is long signedSetValue) //When parsed, always use maximun size!
            {
                SettingsValue.SetCastedAsSignedInt(this.PropertyInfo, this.configurationObject, signedSetValue);
            }
            else if (SettingsValue.IsUnsignedInt(propertyType) && setValue is ulong unsignedSetValue) //When parsed, always use maximun size!
            {
                SettingsValue.SetCastedAsUnsignedInt(this.PropertyInfo, this.configurationObject, unsignedSetValue);
            }
        }

        public object? GetConfigurationValue()
        {
            return this.PropertyInfo.GetValue(this.configurationObject);
        }

        private static bool IsSignedInt(Type type)
        {
            return type == typeof(sbyte) || type == typeof(short) || type == typeof(int) || type == typeof(long);
        }

        private static bool SetCastedAsSignedInt(PropertyInfo propertyInfo, object instance, long value)
        {
            var type = propertyInfo.PropertyType;
            if (type == typeof(sbyte))
            {
                propertyInfo.SetValue(instance, (sbyte) value);
            }
            else if(type == typeof(short))
            {
                propertyInfo.SetValue(instance, (short)value);
            }
            else if (type == typeof(int))
            {
                propertyInfo.SetValue(instance, (int)value);
            }
            else if (type == typeof(long))
            {
                propertyInfo.SetValue(instance, (long)value);
            }
            else
            {
                return false;
            }

            return true;
        }

        private static bool IsUnsignedInt(Type type)
        {
            return type == typeof(byte) || type == typeof(ushort) || type == typeof(uint) || type == typeof(ulong);
        }

        private static bool SetCastedAsUnsignedInt(PropertyInfo propertyInfo, object instance, ulong value)
        {
            var type = propertyInfo.PropertyType;
            if (type == typeof(byte))
            {
                propertyInfo.SetValue(instance, (byte)value);
            }
            else if (type == typeof(ushort))
            {
                propertyInfo.SetValue(instance, (ushort)value);
            }
            else if (type == typeof(uint))
            {
                propertyInfo.SetValue(instance, (uint)value);
            }
            else if (type == typeof(ulong))
            {
                propertyInfo.SetValue(instance, (ulong)value);
            }
            else
            {
                return false;
            }

            return true;
        }
    }
}
