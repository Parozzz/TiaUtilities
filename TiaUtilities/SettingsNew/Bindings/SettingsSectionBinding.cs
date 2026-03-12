using TiaUtilities.Configuration;

namespace TiaUtilities.SettingsNew.Bindings
{
    public class SettingsSectionBinding(SettingsMacroSectionBinding<ObservableConfiguration> macroSectionBinding,
        string name, string toolTip)
    {
        public SettingsMacroSectionBinding<ObservableConfiguration> MacroSectionBinding { get; init; } = macroSectionBinding;
        public string Name { get; init; } = name;
        public string ToolTip { get; init; } = toolTip;
        public List<SettingsValueBinding> ValueList { get; init; } = [];
    }
}
