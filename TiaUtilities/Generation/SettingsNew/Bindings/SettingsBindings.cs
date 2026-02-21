using System.Reflection;
using TiaUtilities.Configuration;

namespace TiaUtilities.Generation.SettingsNew.Bindings
{


    public class SettingsBindings
    {
        public List<SettingsMacroSectionBinding<ObservableConfiguration>> MacroSectionList { get; init; } = [];

        private SettingsMacroSectionBinding<ObservableConfiguration>? lastMacroSection;
        private SettingsSectionBinding? lastSection;

        public SettingsBindings() { }

        public void Clear()
        {
            this.MacroSectionList.Clear();

            this.lastMacroSection = null;
            this.lastSection = null;
        }

        public SettingsBindings MacroSection<T>(Func<string> getNameFunc, Func<T?> GetConfigurationFunc,
            T? PresetConfigurationObject = null, Func<IEnumerable<T>>? otherConfigurationsFunc = null) where T : ObservableConfiguration
        {
            lastMacroSection = new(getNameFunc, typeof(T), GetConfigurationFunc, PresetConfigurationObject, otherConfigurationsFunc);

            this.MacroSectionList.Add(lastMacroSection);
            return this;
        }

        public SettingsBindings Section(string name, string description = "")
        {
            var macroSection = lastMacroSection ?? throw new InvalidOperationException("No MacroSection has been defined before");

            lastSection = new SettingsSectionBinding(name, description, macroSection, []);
            macroSection.SectionsList.Add(lastSection);

            return this;
        }

        public SettingsBindings AddString(string propertyName, string name, string description = "") => Add(propertyName, name, description, SettingsEditorTypeEnum.STRING);
        public SettingsBindings AddInt(string propertyName, string name, string description = "") => Add(propertyName, name, description, SettingsEditorTypeEnum.INT);
        public SettingsBindings AddUInt(string propertyName, string name, string description = "") => Add(propertyName, name, description, SettingsEditorTypeEnum.UINT);
        public SettingsBindings AddBool(string propertyName, string name, string description = "") => Add(propertyName, name, description, SettingsEditorTypeEnum.BOOLEAN);
        public SettingsBindings AddJSON(string propertyName, string name, string description = "") => Add(propertyName, name, description, SettingsEditorTypeEnum.JSON);
        public SettingsBindings AddJavascript(string propertyName, string name, string description = "") => Add(propertyName, name, description, SettingsEditorTypeEnum.JAVASCRIPT);
        public SettingsBindings AddColor(string propertyName, string name, string description = "") => Add(propertyName, name, description, SettingsEditorTypeEnum.COLOR);
        public SettingsBindings AddEnum(string propertyName, string name, string description = "") => Add(propertyName, name, description, SettingsEditorTypeEnum.ENUM);

        public SettingsBindings Add(string propertyName, string name, string description, SettingsEditorTypeEnum editorType)
        {
            var macroSection = lastMacroSection ?? throw new InvalidOperationException("No MacroSection has been defined before");
            var section = lastSection ?? throw new InvalidOperationException("No Section has been defined before");

            var propertyInfo = macroSection.GetConfigurationType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            if (propertyInfo == null || !propertyInfo.CanRead || !propertyInfo.CanWrite)
            {
                throw new InvalidOperationException($"No valid PropertyInfo has been found for {propertyName} for Object: {macroSection.GetConfigurationType().FullName}");
            }

            SettingsValueBinding valueBinding = new(propertyInfo, name, description, editorType, section);
            this.lastSection.ValueList.Add(valueBinding);

            return this;
        }
    }
}
