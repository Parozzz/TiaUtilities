using System.Reflection;
using TiaUtilities.Configuration;

namespace TiaUtilities.Generation.SettingsNew
{
    //A Sections contains Groups that contains Values.
    public record SettingsMacroSectionBinding(SettingsBindings SettingsBindings, string Name);

    public record SettingsSectionBinding(SettingsBindings SettingsBindings, 
        string Name, string Description, 
        SettingsMacroSectionBinding MacroSectionBinding,
        List<SettingsValue> ValueList
        );

    public record SettingsValueBinding(SettingsBindings SettingsBindings,
        string PropertyName, string Name, string Description, SettingsEditorTypeEnum EditorType,
        SettingsMacroSectionBinding MacroSectionBinding,
        SettingsSectionBinding SectionBinding
    );

    public class SettingsBindings
    {
        public List<SettingsMacroSectionBinding> MacroSectionList { get; init; } = [];
        public List<SettingsSectionBinding> SectionList { get; init; } = [];

        public ObservableConfiguration ConfigurationObject { get; init; }
        public ObservableConfiguration? PresetConfigurationObject { get; private set; }
        public Func<IEnumerable<ObservableConfiguration>>? OtherConfigurationsFunc { get; private set; }

        private SettingsMacroSectionBinding? lastMacroSection;
        private SettingsSectionBinding? lastSection;

        public SettingsBindings(ObservableConfiguration configurationObject)
        {
            this.ConfigurationObject = configurationObject;
        }

        public SettingsBindings SetConfigureOtherConfigurations(Func<IEnumerable<ObservableConfiguration>> func)
        {
            this.OtherConfigurationsFunc = func;
            return this;
        }

        public SettingsBindings SetPresetConfiguration(ObservableConfiguration presetConfiguration)
        {
            if (this.ConfigurationObject.GetType() == presetConfiguration.GetType())
            {
                this.PresetConfigurationObject = presetConfiguration;
            }

            return this;
        }

        public void SaveToPresetConfiguration()
        {
            if (this.ConfigurationObject != null && this.PresetConfigurationObject != null)
            {
                GenUtils.CopySamePublicFieldsAndProperties(this.ConfigurationObject, this.PresetConfigurationObject);
            }
        }

        public SettingsBindings MacroSection(string name)
        {
            lastMacroSection = new(this, name);
            this.MacroSectionList.Add(lastMacroSection);
            return this;
        }

        public SettingsBindings Section(string name, string description = "")
        {
            var macroSection = lastMacroSection ?? throw new InvalidOperationException("No MacroSection has been defined before");
            lastSection = new SettingsSectionBinding(this, name, description, macroSection, []);

            this.SectionList.Add(lastSection);

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

            var propertyInfo = ConfigurationObject.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            if (propertyInfo == null || !propertyInfo.CanRead || !propertyInfo.CanWrite)
            {
                throw new InvalidOperationException($"No valid PropertyInfo has been found at Name: {propertyName} for Object: {ConfigurationObject.GetType().FullName}");
            }

            SettingsValueBinding valueBinding = new(this, propertyName, name, description, editorType, macroSection, section);

            SettingsValue settingsValue = new(this.ConfigurationObject, valueBinding, propertyInfo);
            this.lastSection.ValueList.Add(settingsValue);

            return this;
        }
    }
}
