using FastColoredTextBoxNS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiaUtilities.Editors;

namespace TiaUtilities.Generation.SettingsNew.Editors
{
    public class SettingsJSONEditor : SettingsEditor
    {
        private readonly JsonEditor editor;
        private FastColoredTextBox Control { get => editor.GetTextBox(); }

        public SettingsJSONEditor(SettingsValue value) : base(value)
        {
            this.editor = new JsonEditor();
            this.editor.InitControl();

            this.Control.Dock = DockStyle.Fill;
            this.Control.MinimumSize = new Size(0, 500);
            this.Control.Font = SettingsConstants.SETTINGS_VALUE_TEXTBOX_FONT;
            this.Control.Text = "" + value.GetConfigurationValue();

            this.Control.TextChanged += TextChangedEventHandler;
        }
        private void TextChangedEventHandler(object? sender, EventArgs args)
        {
            var text = this.Control.Text;
            this.Value.SetConfigurationValue(text);
        }

        public override Control GetControl()
        {
            return this.Control;
        }
    }
}
