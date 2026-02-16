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
        public SettingsValueBinding ValueBinding { get; init; }
        public string Name { get => this.ValueBinding.Name; }
        public string Description { get => this.ValueBinding.Description; }
        public SettingsSectionBinding SectionBinding { get => this.ValueBinding.SectionBinding; }
        public SettingsGroupBinding? GroupBinding { get => this.ValueBinding.GroupBinding; }

        public PropertyInfo PropertyInfo { get; init; }


        private readonly ObservableConfiguration configurationObject;

        public SettingsValue(ObservableConfiguration configurationObject, SettingsValueBinding binding, PropertyInfo propertyInfo)
        {
            this.configurationObject = configurationObject;

            this.ValueBinding = binding;
            this.PropertyInfo = propertyInfo;
        }

        public void SetConfigurationValue(object setValue)
        {
            this.SetConfigurationValue(this.configurationObject, setValue);
        }

        public void SetConfigurationValue(ObservableConfiguration configuration, object setValue)
        {
            if (configuration.GetType() != this.configurationObject.GetType()) {
                return;
            }

            var propertyType = this.PropertyInfo.PropertyType;
            if (propertyType == setValue.GetType())
            {
                this.PropertyInfo.SetValue(configuration, setValue);
            }
            else if(SettingsValue.IsSignedInt(propertyType) && setValue is long signedSetValue) //When parsed, always use maximun size!
            {
                SettingsValue.SetCastedAsSignedInt(this.PropertyInfo, configuration, signedSetValue);
            }
            else if (SettingsValue.IsUnsignedInt(propertyType) && setValue is ulong unsignedSetValue) //When parsed, always use maximun size!
            {
                SettingsValue.SetCastedAsUnsignedInt(this.PropertyInfo, configuration, unsignedSetValue);
            }
        }

        public object? GetConfigurationValue()
        {
            return this.PropertyInfo.GetValue(this.configurationObject);
        }

        public T? GetConfigurationValue<T>()
        {
            var value = this.PropertyInfo.GetValue(this.configurationObject); 
            if(value is T t)
            {
                return t;
            }

            return default;
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
