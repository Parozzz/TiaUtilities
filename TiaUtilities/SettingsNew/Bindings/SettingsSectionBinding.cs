using TiaUtilities.Configuration;

namespace TiaUtilities.SettingsNew.Bindings
{
    public class SettingsSectionBinding(SettingsMacroSectionBinding<ObservableConfiguration> macroSectionBinding,
        string name, string toolTip, Func<bool> enabledFunc)
    {
        public SettingsMacroSectionBinding<ObservableConfiguration> MacroSectionBinding { get; init; } = macroSectionBinding;
        public string Name { get; init; } = name;
        public string ToolTip { get; init; } = toolTip;
        public Func<bool> EnabledFunc { get; init; } = enabledFunc;
        public List<SettingsValueBinding> ValueList { get; init; } = [];
    }
}
