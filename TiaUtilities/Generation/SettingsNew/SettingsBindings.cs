using System.Reflection;
using TiaUtilities.Configuration;

namespace TiaUtilities.Generation.SettingsNew
{
    //A Sections contains Groups that contains Values.
    public record SettingsSectionBinding(SettingsBindings SettingsBindings, string Name, string Tooltip);
    public record SettingsGroupBinding(SettingsBindings SettingsBindings, string Name, string Description, string Section);
    public record SettingsValueBinding(SettingsBindings SettingsBindings, string PropertyName, string Name, string Description, SettingsEditorTypeEnum EditorType, SettingsSectionBinding SectionBinding, SettingsGroupBinding? GroupBinding);

    public class SettingsBindings
    {
        public List<SettingsSectionBinding> SectionBindings { get; init; } = [];
        public List<SettingsValue> ValueList { get; init; } = [];

        public ObservableConfiguration ConfigurationObject { get; init; }
        public ObservableConfiguration? PresetConfigurationObject { get; private set; }
        public Func<IEnumerable<ObservableConfiguration>>? OtherConfigurationsFunc { get; private set; }

        private SettingsSectionBinding? lastSection;
        private SettingsGroupBinding? lastGroup;

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
            if(this.ConfigurationObject.GetType() == presetConfiguration.GetType())
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

        public SettingsBindings Section(string name, string tooltip = "")
        {
            var sectionBinding = new SettingsSectionBinding(this, name, tooltip);

            lastSection = sectionBinding;
            lastGroup = null;

            SectionBindings.Add(sectionBinding);
            return this;
        }

        public SettingsBindings Group(string name, string description = "")
        {
            var section = lastSection ?? throw new InvalidOperationException("No Sections has been defined before");

            var groupBinding = new SettingsGroupBinding(this, name, description, section.Name);
            lastGroup = groupBinding;
            return this;
        }

        public SettingsBindings NoGroup()
        {
            lastGroup = null;
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
            var section = lastSection ?? throw new InvalidOperationException("No Section has been defined before");

            var propertyInfo = ConfigurationObject.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            if (propertyInfo == null || !propertyInfo.CanRead || !propertyInfo.CanWrite)
            {
                throw new InvalidOperationException($"No valid PropertyInfo has been found at Name: {propertyName} for Object: {ConfigurationObject.GetType().FullName}");
            }

            SettingsValueBinding valueBinding = new(this, propertyName, name, description, editorType, section, lastGroup);

            SettingsValue settingsValue = new(this.ConfigurationObject, valueBinding, propertyInfo);
            this.ValueList.Add(settingsValue);

            return this;
        }
    }
}
