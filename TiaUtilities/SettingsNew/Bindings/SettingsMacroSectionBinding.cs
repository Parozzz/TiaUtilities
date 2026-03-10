using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TiaUtilities.Configuration;
using TiaUtilities.Generation;
using TiaUtilities.SettingsNew.Editors;

namespace TiaUtilities.SettingsNew.Bindings
{
    public class SettingsMacroSectionBinding<T>(Func<string> getNameFunc, Func<bool> isVisibleFunc,
        Type configurationType,
        Func<T?> getConfigurationFunc, T? presetConfigurationObject, Func<IEnumerable<T>>? getOtherConfigurationsFunc) 
        where T : ObservableConfiguration
    {
        public Guid Guid { get; init; } = Guid.NewGuid();

        public string Name { get => getNameFunc(); }
        public bool Visible { get => isVisibleFunc(); }
        public T? PresetConfigurationObject { get; init; } = presetConfigurationObject;
        public IEnumerable<T>? OtherConfigurations { get => this.getOtherConfigurationsFunc?.Invoke(); }

        public List<SettingsSectionBinding> SectionsList { get; init; } = [];

        private readonly Func<string> getNameFunc = getNameFunc;
        private readonly Func<bool> isVisibleFunc = isVisibleFunc;
        private readonly Type configurationType = configurationType;
        private readonly Func<T?> getConfigurationObject = getConfigurationFunc;
        private readonly Func<IEnumerable<T>>? getOtherConfigurationsFunc = getOtherConfigurationsFunc;

        public T? GetConfigurationObject()
        {
            return getConfigurationObject.Invoke();
        }

        public Type GetConfigurationType()
        {
            return configurationType;
        }

        public void SaveToPresetConfiguration()
        {
            var configurationObject = this.getConfigurationObject();
            if (configurationObject != null && this.PresetConfigurationObject != null)
            {
                GenUtils.CopySamePublicFieldsAndProperties(configurationObject, this.PresetConfigurationObject);
            }
        }

        public override string ToString() => $"{Guid};{Name};{Visible}";
    }

    public record SettingsSectionBinding(SettingsMacroSectionBinding<ObservableConfiguration> MacroSectionBinding,
        string Name, string ToolTip,
        List<SettingsValueBinding> ValueList
    );

    public record SettingsValueBinding(SettingsSectionBinding SectionBinding, 
        string PropertyName, string Name, string Description,
        SettingsEditorTypeEnum EditorType,
        Object? Tag = null
    );

    public record SettingsValueListStringTag(List<string> List);
    public record SettingsValueListSignedTag(List<long> List);
    public record SettingsValueListUnsignedTag(List<ulong> List);
}
