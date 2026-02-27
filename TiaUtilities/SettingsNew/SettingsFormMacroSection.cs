using TiaUtilities.Configuration;
using TiaUtilities.SettingsNew.Bindings;

namespace TiaUtilities.SettingsNew
{
    public class SettingsFormMacroSection(SettingsMacroSectionBinding<ObservableConfiguration> binding)
    {
        public SettingsMacroSectionBinding<ObservableConfiguration> Binding { get; init; } = binding;
        public Guid Guid { get => this.Binding.Guid; }
        public string Name { get => this.Binding.Name; }
        public List<SettingsFormSection> Sections { get; init; } = [];
        public List<Label> ValueDescriptionLabelList { get; init; } = [];

        public Label? Label { get; set; } = null;

        public void SetDescriptionLabelVisibility(bool visibile)
        {
            foreach (var label in ValueDescriptionLabelList)
            {
                label.Visible = visibile;
            }
        }

        public override string ToString() => Name;

    }
}
