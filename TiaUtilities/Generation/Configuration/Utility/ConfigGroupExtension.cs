using TiaUtilities.Generation.Configuration.Lines;

namespace TiaUtilities.Generation.Configuration.Utility
{
    public static class ConfigGroupExtension
    {
        public static ConfigGroup AddGroup(this IConfigGroup group) => group.Add(new ConfigGroup(group.GetConfigForm()) { IsSubGroup = true });
        public static ConfigVerticalGroup AddVerticalGroup(this IConfigGroup group) => group.Add(new ConfigVerticalGroup(group.GetConfigForm()));

        public static ConfigLabelLine AddLabel(this IConfigGroup group) => group.Add(new ConfigLabelLine());
        public static ConfigSeparatorLine AddSeparator(this IConfigGroup group) => group.Add(new ConfigSeparatorLine());
        public static ConfigTextBoxLine AddTextBox(this IConfigGroup group) => group.Add(new ConfigTextBoxLine(group));
        public static ConfigCheckBoxLine AddCheckBox(this IConfigGroup group) => group.Add(new ConfigCheckBoxLine(group));
        public static ConfigComboBoxLine AddComboBox(this IConfigGroup group) => group.Add(new ConfigComboBoxLine(group));
        public static ConfigButtonPanelLine AddButtonPanel(this IConfigGroup group) => group.Add(new ConfigButtonPanelLine());
        public static ConfigColorPickerLine AddColorPicker(this IConfigGroup group) => group.Add(new ConfigColorPickerLine(group));
        public static ConfigJavascriptLine AddJavascript(this IConfigGroup group) => group.Add(new ConfigJavascriptLine(group));
        public static ConfigJSONLine AddJSON(this IConfigGroup group) => group.Add(new ConfigJSONLine(group));
        public static ConfigInteractableTabLine AddInteractableTab(this IConfigGroup group) => group.Add(new ConfigInteractableTabLine());
    }
}
