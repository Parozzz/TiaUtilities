using TiaUtilities.Languages;
using TiaUtilities.SettingsNew.FormHelpers;
using TiaUtilities.Utility.Extensions;

namespace TiaUtilities.SettingsNew
{
    public static class SettingsFormUtils
    {
        public static ContextMenuStrip AddContextualMenu(Control control, SettingsFormValue formValue)
        {
            ToolStripMenuItem setToOther = new()
            {
                Text = SettingsFormUtils.GetSetToOtherText(formValue),
                Image = Image.FromFile("Resources/Images/noun-transfer-7710063.png")
            };
            setToOther.Click += (sender, args) =>
            {
                //This only transfers ONE value to the other configuration, the one contextMenu is applied
                var configurationObject = formValue.ConfigurationObject;

                var mainConfigurationValue = formValue.GetConfigurationValue();
                var otherConfigurationDict = formValue.MacroSectionBinding.OtherConfigurationDict;
                if (mainConfigurationValue == null || otherConfigurationDict == null || otherConfigurationDict.Count == 0)
                {
                    return;
                }

                otherConfigurationDict
                    .Values
                    .Where(otherConf => otherConf.GetType() == configurationObject.GetType())
                    .Where(otherConf => otherConf != configurationObject)
                    .ForEach(otherConf => formValue.SetConfigurationValue(otherConf, mainConfigurationValue));
            };

            ToolStripMenuItem showOtherConf = new()
            {
                Text = Locale.SETTINGS_FORM_CONTEXT_MENU_OPEN_OTHER_CONF_FAST_EDIT,
                Image = Image.FromFile("Resources/Images/edit-5260657.png")
            };
            showOtherConf.Click += (sender, args) => 
            {
                if (showOtherConf.Owner is ContextMenuStrip contextMenuStrip)
                {
                    var form = contextMenuStrip.SourceControl?.FindForm(); 
                    if(form != null)
                    {
                        OpenOtherConfigurationForm(formValue, form);
                    }
                }
            };

            ContextMenuStrip menuStrip = new()
            {
                AutoSize = true,
                Items = { setToOther, showOtherConf },
            };
            menuStrip.VisibleChanged += (sender, args) =>
            {

                if (setToOther.Visible)
                {
                    var otherConfigurationDict = formValue.MacroSectionBinding.OtherConfigurationDict;
                    setToOther.Enabled = otherConfigurationDict != null && otherConfigurationDict
                                                                                .Select(pair => pair.Value)
                                                                                .Any(otherConf => otherConf != formValue.ConfigurationObject);

                    setToOther.Text = SettingsFormUtils.GetSetToOtherText(formValue);
                }
            };
            control.ContextMenuStrip = menuStrip;

            return menuStrip;
        }

        private static string GetSetToOtherText(SettingsFormValue formValue)
        {
            var count = formValue.MacroSectionBinding.OtherConfigurationDict?.Count ?? 0;
            return $"{Locale.SETTINGS_FORM_CONTEXT_MENU_SET_TO_OTHERS} ({count})";
        }

        private static void OpenOtherConfigurationForm(SettingsFormValue formValue, Form parentForm)
        {
            var dict = formValue.MacroSectionBinding.OtherConfigurationDict;
            if (dict == null)
            {
                return;
            }

            TableLayoutPanel tableLayoutPanel = new()
            {
                AutoSize = true,
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top,
                BorderStyle = BorderStyle.None,
                ColumnStyles = { new(SizeType.AutoSize), new(SizeType.Percent, 100f) },
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                BackColor = Color.Transparent,
            };

            tableLayoutPanel.SuspendLayout();
            tableLayoutPanel.Controls.Clear();

            int rowCount = 0;
            foreach (var pair in dict)
            {
                var name = pair.Key;
                var otherConf = pair.Value;

                var otherFormValue = formValue.Clone();
                otherFormValue.UpdateConfigurationObject(otherConf);

                var editor = otherFormValue.Editor;
                if (editor == null)
                {
                    continue;
                }

                Label nameLabel = new()
                {
                    Text = name,
                    Anchor = AnchorStyles.Left | AnchorStyles.Right,
                    BorderStyle = BorderStyle.None,
                    AutoSize = true,
                    TextAlign = ContentAlignment.MiddleCenter,
                    BackColor = Form.DefaultBackColor,
                    Padding = Padding.Empty,
                    Margin = Padding.Empty,
                };

                tableLayoutPanel.RowStyles.Add(new(SizeType.AutoSize, 30f));

                tableLayoutPanel.Controls.Add(nameLabel, 0, rowCount);
                tableLayoutPanel.Controls.Add(editor.GetControl(), 1, rowCount);
                rowCount++;
            }

            tableLayoutPanel.RowStyles.Add(new(SizeType.Absolute, 30f));

            var endLabel = new Label()
            {
                Text = "",
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                Padding = Padding.Empty,
                Margin = new Padding(2),
            };

            tableLayoutPanel.Controls.Add(endLabel, 0, rowCount);
            tableLayoutPanel.SetColumnSpan(endLabel, 2);
            rowCount++;

            tableLayoutPanel.Refresh();
            tableLayoutPanel.ResumeLayout(true);


            ScrollableControl scrollableControl = new()
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                AutoScrollMinSize = new Size(100, 100),
                Controls = { tableLayoutPanel }
            };

            var location = Cursor.Position;
            location.Offset(-20, -20);
            OtherConfigurationForm form = new()
            {
                Text = $"{formValue.SectionBinding.Name} - {formValue.Name}",
                Size = new(300, 300),
                Controls = { scrollableControl },
                TopLevel = true,
                ControlBox = false,
                FormBorderStyle = FormBorderStyle.FixedToolWindow,
                StartPosition = FormStartPosition.Manual,
                Location = location,
                ShowInTaskbar = false,
            };
            form.Show(parentForm);
        }
    }
}
