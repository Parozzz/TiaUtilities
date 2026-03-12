using TiaUtilities.Configuration;
using TiaUtilities.SettingsNew.Bindings;

namespace TiaUtilities.SettingsNew.FormHelpers
{
    public class SettingsFormMacroSection
    {
        public SettingsMacroSectionBinding<ObservableConfiguration> Binding { get; init; }
        public Guid Guid { get => this.Binding.Guid; }
        public string Name { get => this.Binding.Name; }
        public bool Visible { get => this.Binding.Visible; }
        public List<SettingsFormSection> Sections { get; init; } = [];
        public List<Label> ValueDescriptionLabelList { get; init; } = [];

        public ListViewItem ListItem { get; init; }

        public Label? Label { get; set; } = null;

        public SettingsFormMacroSection(SettingsMacroSectionBinding<ObservableConfiguration> binding)
        {
            this.Binding = binding;
            this.ListItem = new()
            {
                Text = this.Name,
                Tag = new SettingsFormSectionListView.ItemMacroSectionTag(this)
            };
        }

        public void SetDescriptionLabelVisibility(bool visibile)
        {
            foreach (var label in ValueDescriptionLabelList)
            {
                label.Visible = visibile;
            }
        }

        public override string ToString() => $"{Binding}";

    }
}
