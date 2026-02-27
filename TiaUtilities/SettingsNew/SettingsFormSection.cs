using TiaUtilities.SettingsNew.Bindings;

namespace TiaUtilities.SettingsNew
{
    public class SettingsFormSection(SettingsSectionBinding binding, SettingsFormMacroSection macroSection)
    {
        public SettingsSectionBinding Binding { get; init; } = binding;
        public SettingsFormMacroSection MacroSection { get; init; } = macroSection;
        public string Name { get => this.Binding.Name; }
        public string ToolTip { get => this.Binding.ToolTip; }
        public List<SettingsFormValue> FormValueList { get; init; } = [];

        public ListViewItem? ListItem { get; set; } = null;
        public TableLayoutPanel? Panel { get; set; } = null;
        public float VisiblePercentage { get; set; } = 0f;

        public override string ToString() => $"{Name}, {VisiblePercentage}";
    }
}
