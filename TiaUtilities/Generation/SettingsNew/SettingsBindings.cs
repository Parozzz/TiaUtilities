using System.ComponentModel;
using System.Reflection;
using TiaUtilities.Configuration;

namespace TiaUtilities.Generation.SettingsNew
{
    //A Sections contains Groups that contains Values.
    public record SettingsSectionBinding(string Name, string Tooltip);
    public record SettingsGroupBinding(string Name, string Description, string Section);
    public record SettingsValueBinding(string PropertyName, string Name, string Description, SettingsEditorTypeEnum EditorType, string Section, string Group);

    public class SettingsBindings
    {
        public List<SettingsSectionBinding> SectionBindings { get; init; } = [];
        public List<SettingsGroupBinding> GroupBindings { get; init; } = [];
        public List<SettingsValue> ValueList { get; init; } = [];

        private readonly ObservableConfiguration configurationObject;

        private SettingsSectionBinding? lastSection;
        private SettingsGroupBinding? lastGroup;

        public SettingsBindings(ObservableConfiguration configurationObject)
        {
            this.configurationObject = configurationObject;
        }

        public SettingsBindings AddSection(string name, string tooltip = "")
        {
            var sectionBinding = new SettingsSectionBinding(name, tooltip);

            lastSection = sectionBinding;
            lastGroup = null;

            SectionBindings.Add(sectionBinding);
            return this;
        }

        public SettingsBindings AddGroup(string name, string description = "")
        {
            var section = lastSection ?? throw new InvalidOperationException("No Sections has been defined before");

            var groupBinding = new SettingsGroupBinding(name, description, section.Name);

            lastGroup = groupBinding;

            GroupBindings.Add(groupBinding);
            return this;
        }

        public SettingsBindings AddValue(string propertyName, string name, string description, SettingsEditorTypeEnum editorType)
        {
            var section = lastSection ?? throw new InvalidOperationException("No Section has been defined before");
            var group = lastGroup ?? throw new InvalidOperationException("No Group has been defined before");

            var propertyInfo = configurationObject.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            if (propertyInfo == null || !propertyInfo.CanRead || !propertyInfo.CanWrite)
            {
                throw new InvalidOperationException($"No valid PropertyInfo has been found at Name: {propertyName} for Object: {configurationObject.GetType().FullName}");
            }

            SettingsValueBinding valueBinding = new(propertyName, name, description, editorType, section.Name, group.Name);

            SettingsValue settingsValue = new(this.configurationObject, valueBinding, propertyInfo);
            this.ValueList.Add(settingsValue);

            return this;
        }
    }
}
