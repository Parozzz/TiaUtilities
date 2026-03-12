using TiaUtilities.SettingsNew.Bindings;

namespace TiaUtilities.SettingsNew.FormHelpers
{
    public class SettingsFormSection
    {
        public SettingsSectionBinding Binding { get; init; }
        public SettingsFormMacroSection MacroSection { get; init; }
        public string Name { get => this.Binding.Name; }
        public string ToolTip { get => this.Binding.ToolTip; }
        public List<SettingsFormValue> FormValueList { get; init; } = [];

        public ListViewItem ListItem { get; init; }

        public TableLayoutPanel? Panel { get; set; } = null;
        public float VisiblePercentage { get; set; } = 0f;

        public SettingsFormSection(SettingsSectionBinding binding, SettingsFormMacroSection macroSection)
        {
            this.Binding = binding;
            this.MacroSection = macroSection;
            this.ListItem = new()
            {
                Text = this.Name,
                ToolTipText = this.ToolTip,
                Tag = new SettingsFormSectionListView.ItemSectionTag(this)
            };
        }

        public override string ToString() => $"{Name};{VisiblePercentage};{MacroSection.Name}";
    }
}
