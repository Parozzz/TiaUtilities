using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiaUtilities.Languages;

namespace TiaUtilities.Generation.SettingsNew
{
    public static class SettingsUtils
    {
        public static void AddContextualMenu(Control control, SettingsValue settingsValue)
        {
            ToolStripMenuItem saveItem = new(Locale.CONFIG_LINE_SAVE_DEFAULT_TOOLTIP) { Image = Image.FromFile("Resources/Images/noun-save-2433498.png") };
            saveItem.Click += (sender, args) =>
            {
                settingsValue.ValueBinding.SettingsBindings.SaveToPresetConfiguration();
            };

            ToolStripMenuItem setToOther = new(Locale.CONFIG_LINE_TRANSFER_TO_OTHERS) { Image = Image.FromFile("Resources/Images/noun-transfer-7710063.png") };
            setToOther.Click += (sender, args) =>
            {
                var settingsBindings = settingsValue.ValueBinding.SettingsBindings;

                var otherConfigurationsFunc = settingsBindings.OtherConfigurationsFunc;
                if (otherConfigurationsFunc == null)
                {
                    return;
                }

                var otherConfigurationsEnumerable = otherConfigurationsFunc.Invoke();
                if (!otherConfigurationsEnumerable.Any())
                {
                    return;
                }

                var mainConfigurationValue = settingsValue.GetConfigurationValue();
                if (mainConfigurationValue == null)
                {
                    return;
                }

                foreach (var otherConfiguration in otherConfigurationsEnumerable)
                {
                    if (otherConfiguration != settingsBindings.ConfigurationObject)
                    {
                        settingsValue.SetConfigurationValue(otherConfiguration, mainConfigurationValue);
                    }
                }
            };

            control.ContextMenuStrip = new() { Items = { saveItem, setToOther } };
        }
    }
}
