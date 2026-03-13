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
                var otherConfigurationDict = formValue.MacroSectionBinding.OtherConfigurationDict;
                if (mainConfigurationValue == null || otherConfigurationDict == null || !otherConfigurationDict.Any())
                {
                    return;
                }

                otherConfigurationDict
                    .Values
                    .Where(otherConf => otherConf.GetType() == configurationObject.GetType())
                    .Where(otherConf => otherConf != configurationObject)
                    .ForEach(otherConf => formValue.SetConfigurationValue(otherConf, mainConfigurationValue));
            };

            DataGridView dataGridView = new()
            {
                AutoSize = true,
                Dock = DockStyle.Fill,
                Margin = new Padding(10),
                MinimumSize = new(0, 150),
                MaximumSize = new(0, 300),
                AllowUserToResizeColumns = false,
                AllowUserToResizeRows = false,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing,
                BorderStyle = BorderStyle.None,
            };

            var column1Name = new DataGridViewTextBoxColumn()
            {
                Name = "Name",
                HeaderText = "Name",
                ReadOnly = true,
                SortMode = DataGridViewColumnSortMode.NotSortable
            };
            var column2Value = new DataGridViewTextBoxColumn()
            {
                HeaderText = "Value",
                ReadOnly = false,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                SortMode = DataGridViewColumnSortMode.NotSortable,
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
                    var otherConfigurationDict = formValue.MacroSectionBinding.OtherConfigurationDict;
                    setToOther.Enabled = otherConfigurationDict != null && otherConfigurationDict
                                                                                .Select(pair => pair.Value)
                                                                                .Any(otherConf => otherConf != formValue.ConfigurationObject);

                    if (otherConfigurationDict != null)
                    {
                        dataGridView.RowCount = otherConfigurationDict.Count;

                        if (dataGridView.RowCount < 5)
                        {
                            dataGridView.MinimumSize = new(0, 80);
                            menuStrip.MinimumSize = new(0, 100);
                        }
                        else if (dataGridView.RowCount < 12)
                        {
                            dataGridView.MinimumSize = new(0, 170);
                            menuStrip.MinimumSize = new(0, 190);
                        }
                        else
                        {
                            dataGridView.MinimumSize = new(0, 240);
                            menuStrip.MinimumSize = new(0, 260);
                        }

                        int rowCount = 0;
                        foreach (var pair in otherConfigurationDict)
                        {
                            var name = pair.Key;
                            var otherConf = pair.Value;

                            dataGridView.Rows[rowCount].Cells[0].Value = name;
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
            var count = formValue.MacroSectionBinding.OtherConfigurationDict?.Count() ?? 0;
            return $"{Locale.CONFIG_LINE_TRANSFER_TO_OTHERS} ({count})";
        }
    }
}
