using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiaUtilities.Languages;
using TiaUtilities.SettingsNew.Bindings;
using TiaUtilities.SettingsNew.FormHelpers;
using TiaUtilities.Utility.Extensions;

namespace TiaUtilities.SettingsNew
{
    public static class SettingsUtils
    {
        public static ContextMenuStrip AddContextualMenu(Control control, SettingsFormValue formValue)
        {
            ToolStripMenuItem setToOther = new()
            {
                Text = SettingsUtils.GetSetToOtherText(formValue),
                Image = Image.FromFile("Resources/Images/noun-transfer-7710063.png")
            };
            setToOther.Click += (sender, args) =>
            {
                //This only transfers ONE value to the other configuration, the one contextMenu is applied
                var configurationObject = formValue.ConfigurationObject;

                var mainConfigurationValue = formValue.GetConfigurationValue();
                var otherConfigurationsEnumerable = formValue.MacroSectionBinding.OtherConfigurations;
                if (mainConfigurationValue == null || otherConfigurationsEnumerable == null || !otherConfigurationsEnumerable.Any())
                {
                    return;
                }

                otherConfigurationsEnumerable
                    .Where(otherConf => otherConf.GetType() == configurationObject.GetType())
                    .Where(otherConf => otherConf != configurationObject)
                    .ForEach(otherConf => formValue.SetConfigurationValue(otherConf, mainConfigurationValue));
            };

            DataGridView dataGridView = new()
            {
                AutoSize = true,
                Dock = DockStyle.Fill,
                Margin = new Padding(10),
                //MinimumSize = new(0, 300),
                MaximumSize = new(0, 300),
                AllowUserToResizeColumns = false,
                AllowUserToResizeRows = false,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing,
            };

            var column1Name = new DataGridViewTextBoxColumn()
            {
                Name = "Name",
                HeaderText = "Name",
                ReadOnly = true,
            };
            var column2Value = new DataGridViewTextBoxColumn()
            {
                HeaderText = "Value",
                ReadOnly = false,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
            };

            dataGridView.Columns.AddRange([column1Name, column2Value]);
            ToolStripControlHost panel = new(dataGridView)
            {
            };

            ContextMenuStrip menuStrip = new()
            {
                AutoSize = true,
                MaximumSize = new(300, 0),
                Items = { setToOther, panel },
            };
            menuStrip.VisibleChanged += (sender, args) =>
            {
                if (setToOther.Visible)
                {
                    var otherConfigurationsEnumerable = formValue.MacroSectionBinding.OtherConfigurations;
                    setToOther.Enabled = otherConfigurationsEnumerable != null && otherConfigurationsEnumerable.Any(otherConf => otherConf != formValue.ConfigurationObject);

                    if(otherConfigurationsEnumerable != null)
                    {
                        dataGridView.RowCount = otherConfigurationsEnumerable.Count();

                        int rowCount = 0;
                        foreach (var otherConf in otherConfigurationsEnumerable)
                        {
                            dataGridView.Rows[rowCount].Cells[0].Value = formValue.MacroSectionBinding.Name.ToString();
                            dataGridView.Rows[rowCount].Cells[1].Value = formValue.GetConfigurationValue(otherConf)?.ToString();
                            rowCount++;
                        }

                        dataGridView.Refresh();
                        dataGridView.PerformLayout();
                    }

                    setToOther.Text = SettingsUtils.GetSetToOtherText(formValue);
                }
            };
            control.ContextMenuStrip = menuStrip;

            return menuStrip;
        }

        private static string GetSetToOtherText(SettingsFormValue formValue)
        {
            var count = formValue.MacroSectionBinding.OtherConfigurations?.Count() ?? 0;
            return $"{Locale.CONFIG_LINE_TRANSFER_TO_OTHERS} ({count})";
        }
    }
}
