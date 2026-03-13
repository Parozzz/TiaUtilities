using TiaUtilities.Configuration;
using TiaUtilities.SettingsNew.Editors;

namespace TiaUtilities.SettingsNew.Bindings
{

    public delegate void SettingsBindingsUpdateEvent(object? sender, EventArgs e);

    public delegate void SettingsBindingsReloadEvent(object? sender, EventArgs e);

    public class SettingsBindings
    {
        
        public event SettingsBindingsUpdateEvent UpdateEvent = delegate { };
        public event SettingsBindingsUpdateEvent ReloadEvent = delegate { };

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

        public SettingsBindings MacroSection<T>(Func<string> getNameFunc, Func<bool> isVisibleFunc,
            Func<T?> GetConfigurationFunc,  T? PresetConfigurationObject = null, Func<Dictionary<string, ObservableConfiguration>>? otherConfigurationsFunc = null) 
            where T : ObservableConfiguration
        {
            lastMacroSection = new(getNameFunc, isVisibleFunc, typeof(T), GetConfigurationFunc, PresetConfigurationObject, otherConfigurationsFunc);
            lastSection = null;

            this.MacroSectionList.Add(lastMacroSection);
            return this;
        }

        public SettingsBindings Section(string name, string description = "")
        {
            var macroSection = lastMacroSection ?? throw new InvalidOperationException("No MacroSection has been defined before");

            lastSection = new SettingsSectionBinding(macroSection, name, description);
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
        public SettingsBindings AddStringList(string propertyName, List<string> list, string name = "", string description = "") => Add(propertyName, name, description, SettingsEditorTypeEnum.STRING_LIST, new SettingsValueListStringTag(list));
        public SettingsBindings AddSignedNumberList(string propertyName, List<long> list, string name = "", string description = "") => Add(propertyName, name, description, SettingsEditorTypeEnum.SIGNED_LIST, new SettingsValueListSignedTag(list));
        public SettingsBindings AddUnsignedNumberList(string propertyName, List<ulong> list, string name = "", string description = "") => Add(propertyName, name, description, SettingsEditorTypeEnum.UNSIGNED_LIST, new SettingsValueListUnsignedTag(list));

        public SettingsBindings AddLabel(string name = "", string description = "") => Add("", name, description, SettingsEditorTypeEnum.NONE);

        public SettingsBindings Add(string propertyName, string name, string description, SettingsEditorTypeEnum editorType, Object? tag = null)
        {
            var _ = lastMacroSection ?? throw new InvalidOperationException("No MacroSection has been defined before");
            var section = lastSection ?? throw new InvalidOperationException("No Section has been defined before");

            SettingsValueBinding valueBinding = new(section, propertyName, name, description, editorType, tag);
            this.lastSection.ValueList.Add(valueBinding);

            return this;
        }

        public void Update()
        {
            UpdateEvent.Invoke(this, new());
        }

        public void Reload()
        {
            ReloadEvent.Invoke(this, new());
        }
    }
}
