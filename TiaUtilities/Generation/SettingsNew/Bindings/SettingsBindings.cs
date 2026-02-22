using System.Reflection;
using TiaUtilities.Configuration;
using TiaUtilities.Generation.SettingsNew.Editors;

namespace TiaUtilities.Generation.SettingsNew.Bindings
{

    public record SettingsBindingsUpdateRequestEventArgs();

    public delegate void SettingsBindingsUpdateRequestEvent(object? sender, SettingsBindingsUpdateRequestEventArgs e);

    public class SettingsBindings
    {
        
        public event SettingsBindingsUpdateRequestEvent UpdateRequest = delegate { };

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

            lastSection = new SettingsSectionBinding(macroSection, name, description, []);
            macroSection.SectionsList.Add(lastSection);

            return this;
        }

        public SettingsBindings AddString(string propertyName, string name = "", string description = "") => Add(propertyName, name, description, SettingsEditorTypeEnum.STRING);
        public SettingsBindings AddInt(string propertyName, string name = "", string description = "") => Add(propertyName, name, description, SettingsEditorTypeEnum.INT);
        public SettingsBindings AddUInt(string propertyName, string name = "", string description = "") => Add(propertyName, name, description, SettingsEditorTypeEnum.UINT);
        public SettingsBindings AddBool(string propertyName, string name = "", string description = "") => Add(propertyName, name, description, SettingsEditorTypeEnum.BOOLEAN);
        public SettingsBindings AddJSON(string propertyName, string name = "", string description = "") => Add(propertyName, name, description, SettingsEditorTypeEnum.JSON);
        public SettingsBindings AddJavascript(string propertyName, string name = "", string description = "") => Add(propertyName, name, description, SettingsEditorTypeEnum.JAVASCRIPT);
        public SettingsBindings AddColor(string propertyName, string name = "", string description = "") => Add(propertyName, name, description, SettingsEditorTypeEnum.COLOR);
        public SettingsBindings AddEnum(string propertyName, string name = "", string description = "") => Add(propertyName, name, description, SettingsEditorTypeEnum.ENUM);
        public SettingsBindings AddList(string propertyName, List<string> list, string name = "", string description = "") => Add(propertyName, name, description, SettingsEditorTypeEnum.LIST, new SettingsValueListTag(list));
        public SettingsBindings AddLabel(string name = "", string description = "") => Add("", name, description, SettingsEditorTypeEnum.NONE);

        public SettingsBindings Add(string propertyName, string name, string description, SettingsEditorTypeEnum editorType, Object? tag = null)
        {
            var macroSection = lastMacroSection ?? throw new InvalidOperationException("No MacroSection has been defined before");
            var section = lastSection ?? throw new InvalidOperationException("No Section has been defined before");

            SettingsValueBinding valueBinding = new(section, propertyName, name, description, editorType, tag);
            this.lastSection.ValueList.Add(valueBinding);

            return this;
        }

        public void RequestUpdate()
        {
            UpdateRequest.Invoke(this, new());
        }
    }
}
