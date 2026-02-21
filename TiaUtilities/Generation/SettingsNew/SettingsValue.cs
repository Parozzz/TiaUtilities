using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TiaUtilities.Configuration;
using TiaUtilities.Generation.SettingsNew.Bindings;

namespace TiaUtilities.Generation.SettingsNew
{
    public class SettingsValue(SettingsValueBinding binding, ObservableConfiguration ConfigurationObject)
    {
        public SettingsValueBinding ValueBinding { get; init; } = binding;
        public ObservableConfiguration ConfigurationObject { get; init; } = ConfigurationObject;

        public string Name { get => this.ValueBinding.Name; }
        public string Description { get => this.ValueBinding.Description; }
        public PropertyInfo PropertyInfo { get => this.ValueBinding.PropertyInfo; }
        public SettingsMacroSectionBinding<ObservableConfiguration> MacroSectionBinding { get => this.ValueBinding.SectionBinding.MacroSectionBinding; }
        public SettingsSectionBinding SectionBinding { get => this.ValueBinding.SectionBinding; }

        public bool SetInProgress { get; private set; } = false;

        public void SetConfigurationValue(object setValue)
        {
            this.SetConfigurationValue(this.ConfigurationObject, setValue);
        }

        public void SetConfigurationValue(ObservableConfiguration configuration, object setValue)
        {
            this.SetInProgress = true;

            if (configuration.GetType() == this.ConfigurationObject.GetType())
            {
                var propertyType = this.PropertyInfo.PropertyType;
                if (propertyType == setValue.GetType())
                {
                    this.PropertyInfo.SetValue(configuration, setValue);
                }
                else if (SettingsValue.IsSignedInt(propertyType) && setValue is long signedSetValue) //When parsed, always use maximun size!
                {
                    SettingsValue.SetCastedAsSignedInt(this.PropertyInfo, configuration, signedSetValue);
                }
                else if (SettingsValue.IsUnsignedInt(propertyType) && setValue is ulong unsignedSetValue) //When parsed, always use maximun size!
                {
                    SettingsValue.SetCastedAsUnsignedInt(this.PropertyInfo, configuration, unsignedSetValue);
                }
            }

            this.SetInProgress = false;
        }

        public object? GetConfigurationValue()
        {
            return this.PropertyInfo.GetValue(this.ConfigurationObject);
        }

        public T? GetConfigurationValue<T>()
        {
            var value = this.PropertyInfo.GetValue(this.ConfigurationObject);
            if (value is T t)
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
                propertyInfo.SetValue(instance, (sbyte)value);
            }
            else if (type == typeof(short))
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
