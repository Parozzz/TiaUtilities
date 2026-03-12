using System.Reflection;
using TiaUtilities.Configuration;
using TiaUtilities.SettingsNew.Bindings;
using TiaUtilities.SettingsNew.Editors;

namespace TiaUtilities.SettingsNew.FormHelpers
{
    public abstract class SettingsFormValue
    {
        public static SettingsFormValue FromBinding(SettingsValueBinding binding, ObservableConfiguration configurationObject, Form form)
        {
            if(string.IsNullOrEmpty(binding.PropertyName) || binding.EditorType == SettingsEditorTypeEnum.NONE)
            {
                return new EmptySettingsFormValueImpl(binding, configurationObject);
            }

            var propertyInfo = configurationObject.GetType().GetProperty(binding.PropertyName, BindingFlags.Public | BindingFlags.Instance);
            if (propertyInfo == null || !propertyInfo.CanRead || !propertyInfo.CanWrite)
            {
                throw new InvalidOperationException($"No valid PropertyInfo has been found named \"{binding.PropertyName}\" for Object: {configurationObject.GetType().FullName}");
            }

            SettingsFormValueImpl valueImpl = new(binding, configurationObject, propertyInfo);
            valueImpl.Editor?.AddFormCallbacks(form);
            return valueImpl;
        }

        public SettingsValueBinding Binding { get; init; }
        public ObservableConfiguration ConfigurationObject { get; private set; }
        public string Name { get => this.Binding.Name; }
        public string Description { get => this.Binding.Description; }

        public SettingsMacroSectionBinding<ObservableConfiguration> MacroSectionBinding { get => this.Binding.SectionBinding.MacroSectionBinding; }
        public SettingsSectionBinding SectionBinding { get => this.Binding.SectionBinding; }

        public abstract SettingsEditor? Editor { get; init; } //if Binding.Type is SettingsEditorTypeEnum.NONE, returns null.

        public SettingsFormValue(SettingsValueBinding binding, ObservableConfiguration configurationObject)
        {
            this.Binding = binding;
            this.ConfigurationObject = configurationObject;
        }

        public void UpdateConfigurationObject(ObservableConfiguration newConfigurationObject)
        {
            if (newConfigurationObject.GetType() != this.ConfigurationObject.GetType())
            {
                throw new ArgumentException($"New Configuration Object type is different from already present configuration. New: {newConfigurationObject.GetType().FullName}, Old: {this.ConfigurationObject.GetType().FullName}");
            }

            this.ConfigurationObject = newConfigurationObject;
        }

        public void SetConfigurationValue(object setValue) => this.SetConfigurationValue(this.ConfigurationObject, setValue);

        public abstract void SetConfigurationValue(ObservableConfiguration configuration, object setValue);

        public abstract object? GetConfigurationValue(ObservableConfiguration configuration);

        public T? GetConfigurationValue<T>() => this.GetConfigurationValue() is T t ? t : default;

        public object? GetConfigurationValue() => this.GetConfigurationValue(this.ConfigurationObject);
    }

    public class EmptySettingsFormValueImpl(SettingsValueBinding binding, ObservableConfiguration configurationObject) 
        : SettingsFormValue(binding, configurationObject)
    {
        public override SettingsEditor? Editor { get; init; } = null;

        public override object? GetConfigurationValue(ObservableConfiguration configuration) => null;

        public override void SetConfigurationValue(ObservableConfiguration configuration, object setValue) { }
    }

    public class SettingsFormValueImpl : SettingsFormValue
    {
        public override SettingsEditor? Editor { get; init; }
        public PropertyInfo PropertyInfo { get; init; }

        public bool SetInProgress { get; private set; } = false;

        public SettingsFormValueImpl(SettingsValueBinding binding, ObservableConfiguration configurationObject, PropertyInfo propertyInfo) : base(binding, configurationObject)
        {
            this.PropertyInfo = propertyInfo;
            this.Editor = SettingsEditor.ObtainFromValue(this);
        }

        public override void SetConfigurationValue(ObservableConfiguration configuration, object setValue)
        {
            this.SetInProgress = true;

            if (configuration.GetType() == this.ConfigurationObject.GetType())
            {
                var propertyType = this.PropertyInfo.PropertyType;
                if (propertyType == setValue.GetType())
                {
                    this.PropertyInfo.SetValue(configuration, setValue);
                }
                else if (SettingsFormValueImpl.IsSignedInt(propertyType) && setValue is long signedSetValue) //When parsed, always use maximun size!
                {
                    SettingsFormValueImpl.SetCastedAsSignedInt(this.PropertyInfo, configuration, signedSetValue);
                }
                else if (SettingsFormValueImpl.IsUnsignedInt(propertyType) && setValue is ulong unsignedSetValue) //When parsed, always use maximun size!
                {
                    SettingsFormValueImpl.SetCastedAsUnsignedInt(this.PropertyInfo, configuration, unsignedSetValue);
                }
            }

            this.SetInProgress = false;
        }

        public override object? GetConfigurationValue(ObservableConfiguration configuration)
        {
            return this.PropertyInfo.GetValue(configuration);
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

        public override string ToString() => $"{Name};{Binding.EditorType};{Binding.SectionBinding.Name}";
    }
}
