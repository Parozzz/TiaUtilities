using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiaUtilities.Languages;

namespace TiaUtilities.SettingsNew
{
    public static class SettingsUtils
    {
        public static ContextMenuStrip AddContextualMenu(Control control, SettingsFormValue settingsValue)
        {
            /*
            ToolStripMenuItem saveItem = new(Locale.CONFIG_LINE_SAVE_DEFAULT_TOOLTIP) { Image = Image.FromFile("Resources/Images/noun-save-2433498.png") };
            saveItem.Click += (sender, args) =>
            {
                settingsValue.MacroSectionBinding.SaveToPresetConfiguration();
            };
            */

            var count = settingsValue.MacroSectionBinding.OtherConfigurations?.Count() ?? 0;

            ToolStripMenuItem setToOther = new($"{Locale.CONFIG_LINE_TRANSFER_TO_OTHERS} ({count})") { Image = Image.FromFile("Resources/Images/noun-transfer-7710063.png") };
            setToOther.Click += (sender, args) =>
            {
                //This only transfers ONE value to the other configuration, the one contextMenu is applied
                var configurationObject = settingsValue.ConfigurationObject;

                var mainConfigurationValue = settingsValue.GetConfigurationValue();
                if (mainConfigurationValue == null)
                {
                    return;
                }

                var otherConfigurationsEnumerable = settingsValue.MacroSectionBinding.OtherConfigurations;
                if (otherConfigurationsEnumerable == null || !otherConfigurationsEnumerable.Any())
                {
                    return;
                }

                foreach (var otherConfiguration in otherConfigurationsEnumerable)
                {
                    if (otherConfiguration.GetType() == configurationObject.GetType() && 
                        otherConfiguration != configurationObject)
                    {
                        settingsValue.SetConfigurationValue(otherConfiguration, mainConfigurationValue);
                    }
                }
            };

            ContextMenuStrip menuStrip = new()
            {
                Items = { setToOther }
            };
            control.ContextMenuStrip = menuStrip;

            return menuStrip;
        }
    }
}
