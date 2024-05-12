using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiaUtilities.Generation.Configuration.Lines;
using TiaXmlReader.Generation.Configuration;

namespace TiaUtilities.Generation.Configuration.Utility
{
    public static class ConfigGroupExtension
    {
        public static ConfigGroup AddGroup(this IConfigGroup group) => group.Add(new ConfigGroup(group.GetConfigForm()) { IsSubGroup = true });
        public static ConfigVerticalGroup AddVerticalGroup(this IConfigGroup group) => group.Add(new ConfigVerticalGroup(group.GetConfigForm()));

        public static ConfigLabelLine AddLabel(this IConfigGroup group) => group.Add(new ConfigLabelLine());
        public static ConfigTextBoxLine AddTextBox(this IConfigGroup group) => group.Add(new ConfigTextBoxLine());
        public static ConfigCheckBoxLine AddCheckBox(this IConfigGroup group) => group.Add(new ConfigCheckBoxLine());
        public static ConfigComboBoxLine AddComboBox(this IConfigGroup group) => group.Add(new ConfigComboBoxLine());
        public static ConfigButtonPanelLine AddButtonPanel(this IConfigGroup group) => group.Add(new ConfigButtonPanelLine());
        public static ConfigColorPickerLine AddColorPicker(this IConfigGroup group) => group.Add(new ConfigColorPickerLine());
        public static ConfigJavascriptLine AddJavascript(this IConfigGroup group) => group.Add(new ConfigJavascriptLine());
        public static ConfigJSONLine AddJSON(this IConfigGroup group) => group.Add(new ConfigJSONLine());
    }
}
